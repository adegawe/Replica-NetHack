namespace RepHack;

class Game
{
    Player player = new();
    Dungeon dungeon = new();
    Control control = new();
    FOV fov;
    Pathfinding pathfinding;
    List<Enemy> enemyList = new();
    List<Item> itemList = new();
    Random random = new();
    Dictionary<Control.Actions, (Action, bool)> actionMap;
    Renderer renderer;
    TurnContext ctx;
    public bool gameOver = false;
    int floor = 1;
    int minMonster = 3;

    public Game()
    {
        fov = new(dungeon.width, dungeon.length, dungeon.map);
        renderer = new(dungeon, player, fov, enemyList, itemList);
        pathfinding = new(dungeon.width, dungeon.length, dungeon.map);
        ctx = new TurnContext(player, pathfinding,
        (x, y) => IsOccupied(x, y));

        actionMap = new()
        {
            {Control.Actions.MoveUp, (() => ProcessMove(0, -1), true)},
            {Control.Actions.MoveDown, (() => ProcessMove(0, 1), true)},
            {Control.Actions.MoveLeft, (() => ProcessMove(-1, 0), true)},
            {Control.Actions.MoveRight, (() => ProcessMove(1, 0), true)},
            {Control.Actions.PickUp, (() => ProcessPickUp(player.X, player.Y), true)},
            {Control.Actions.OpenInventory, (() => ProcessInventory(), false)}
        };
    }

    public void Start()
    {
        dungeon.roomList.Clear();
        itemList.Clear();
        fov.ResetExplored();
        dungeon.InitDungeon();
        ctx.ReCompute();
        var activeRooms = dungeon.roomList.Where(n=> n.isActive).ToList();
        player.Spawn(activeRooms[0].RoomCenterX, activeRooms[0].RoomCenterY);
        var activeEnemy = EnemyLoader.Load().Where(data => data.MinFloor <= floor).ToList();
        enemyList.Clear();
        enemyList.AddRange(GetRandomEnemies(minMonster + floor, activeEnemy));
        SpawnEnemies(enemyList, activeRooms);
        for(int i = 0; i < 110; i++)
        {
            Item potion = new PotionItem();
            int randomRoom = random.Next(0, activeRooms.Count);
            int x = random.Next(activeRooms[randomRoom].RoomX, activeRooms[randomRoom].RoomX + activeRooms[randomRoom].RoomWidth);
            int y = random.Next(activeRooms[randomRoom].RoomY, activeRooms[randomRoom].RoomY + activeRooms[randomRoom].RoomLength);
            potion.Spawn(x, y);
            itemList.Add(potion);
        }
    }
    public void Update()
    {
        if(actionMap.TryGetValue(control.GetInput(), out (Action action, bool isTurnAction) entry))
        {
            entry.action.Invoke();
            if (!entry.isTurnAction)
            {
                return;
            }
        }
        else
        {
            return;
        }
        ctx.ReCompute();
        enemyList.RemoveAll(e => e.Hp <= 0);
        itemList.RemoveAll(i => i.PickedUp == true);
        if(dungeon.map[player.Y, player.X] == '>')
        {
            floor++;
            Start();
            return; //층을 넘길 시 적 턴 스킵
        }
        EnemyTurn();
        if(player.Hp <= 0)
        {
            gameOver = true;
        }
    }

    private void ProcessMove(int dx, int dy)
    {
        if(Control.IsCanMove(player.X + dx, player.Y + dy, dungeon.map))
        {
            Enemy? tempEnemy = IsOccupied(player.X + dx, player.Y + dy);
            if(tempEnemy != null)
            {
                tempEnemy.TakeDamage(player.Attack);
                return;
            }
            player.Move(dx, dy);
        }
    }

    private void ProcessPickUp(int x, int y)
    {
        foreach (Item item in itemList){
            if(item.X == x && item.Y == y)
            {
                player.PickUp(item);
                item.PickedUp = true;
            }
        }
    }

    private void ProcessInventory()
    {
        renderer.DrawInventory();
        while(true)
        {
            var key = Console.ReadKey().Key;
            if (key >= ConsoleKey.A && key <= ConsoleKey.Z && key - ConsoleKey.A < player.inventory.Count)
            {
                player.Use(key - ConsoleKey.A);
            }
            if(key == ConsoleKey.Escape) { break; }
            renderer.DrawInventory();
            Thread.Sleep(16);
        }
    }

    private Enemy? IsOccupied(int dx, int dy)
    {
        foreach(Enemy enemy in enemyList)
        {
            if(enemy.X == dx && enemy.Y == dy)
            {
                return enemy;
            }
        }
        return null;
    }

    public void Render()
    {
        renderer.Render(floor);
    }

    public void GameOver()
    {
        renderer.GameOver(floor);
    }

    private void EnemyTurn()
    {        
        foreach(Enemy enemy in enemyList)
        {
            enemy.Act(ctx);
        }
    }
    private List<Enemy> GetRandomEnemies(int requiredNum, List<EnemyData> enemyList)
    {
        int totalWeight = 0;
        List<Enemy> enemies = new();
        foreach(EnemyData enemyData in enemyList)
        {
            totalWeight += enemyData.Weight;
        }
        for(int i = 0; i < requiredNum; i++)
        {
            int cumulative = 0;
            int pick = random.Next(0, totalWeight);
            foreach (EnemyData enemyData in enemyList)
            {
                cumulative += enemyData.Weight;
                if(cumulative > pick)
                {
                    Enemy enemy = EnemyFactory.Create(enemyData); //밖에 빼둘까 했는데 가독성으로 이게 더 좋아서 유지
                    enemies.Add(enemy);
                    break;
                }
            }
        }
        return enemies;
    }

    private void SpawnEnemies(List<Enemy> enemyList, List<Node> roomList)
    {
        foreach(Enemy enemy in enemyList)
        {
            int randomRoom = random.Next(0, roomList.Count);
            int randomX = random.Next(roomList[randomRoom].RoomX, roomList[randomRoom].RoomX + roomList[randomRoom].RoomWidth);
            int randomY = random.Next(roomList[randomRoom].RoomY, roomList[randomRoom].RoomY + roomList[randomRoom].RoomLength);
            enemy.Spawn(randomX, randomY);
        }
    }
}

class TurnContext
{
    public Player player { get; }
    public Pathfinding pathfinding { get; }
    public Func<int, int, Enemy?> IsOccupied { get; }
    public Tile[,] distanceMap { get; private set; }
    private readonly Func<int,int,bool> isEnemyAtCached;
    public TurnContext(Player p, Pathfinding path, Func<int, int, Enemy?> i)
    {
        player = p;
        pathfinding = path;
        IsOccupied = i;
        isEnemyAtCached = (x, y) => IsOccupied(x, y) != null;
        ReCompute();
    }
    
    public void ReCompute()
    {
        distanceMap = pathfinding.Dijkstra(player.X, player.Y, 
        IsOccupied, isEnemyAtCached);
    }
}