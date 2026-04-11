class Game
{
    Player player = new();
    Dungeon dungeon = new();
    Control control = new();
    Pathfinding pathfinding;
    List<Enemy> enemyList = [];
    List<Item> itemList = [];
    Random random = new();
    Dictionary<Control.Actions, Action> keyMap;
    Renderer renderer;
    public bool gameOver = false;
    int floor = 1;

    public Game()
    {
        renderer = new(dungeon, player, enemyList, itemList);
        pathfinding = new(dungeon.width, dungeon.length);
        keyMap = new()
        {
            {Control.Actions.MoveUp, () => ProcessMove(0, -1)},
            {Control.Actions.MoveDown, () => ProcessMove(0, 1)},
            {Control.Actions.MoveLeft, () => ProcessMove(-1, 0)},
            {Control.Actions.MoveRight, () => ProcessMove(1, 0)},
            {Control.Actions.PickUp, () => ProcessPickUp(player.X, player.Y)},
            {Control.Actions.OpenInventory, () => ProcessInventory()}
        };
    }

    public void Start()
    {
        dungeon.roomList.Clear();
        enemyList.Clear();
        itemList.Clear();
        dungeon.InitDungeon();
        player.Spawn(dungeon.roomList[0].RoomCenterX, dungeon.roomList[0].RoomCenterY);
        var activeRooms = dungeon.roomList.Where(n=> n.isActive).ToList();
        for(int i = 0; i < 2; i++)
        {
            Enemy slime = new Slime();
            Enemy goblin = new Goblin();
            int randomRoom = random.Next(0, activeRooms.Count);
            int x = random.Next(activeRooms[randomRoom].RoomX, activeRooms[randomRoom].RoomX + activeRooms[randomRoom].RoomWidth);
            int y = random.Next(activeRooms[randomRoom].RoomY, activeRooms[randomRoom].RoomY + activeRooms[randomRoom].RoomLength);
            slime.Spawn(x, y);
            x = random.Next(activeRooms[randomRoom].RoomX, activeRooms[randomRoom].RoomX + activeRooms[randomRoom].RoomWidth);
            y = random.Next(activeRooms[randomRoom].RoomY, activeRooms[randomRoom].RoomY + activeRooms[randomRoom].RoomLength);
            goblin.Spawn(x, y);
            enemyList.Add(slime);
            enemyList.Add(goblin);
        }
        for(int i = 0; i < 2; i++)
        {
            Item potion = new PotionItem();
            int randomRoom = random.Next(0, dungeon.roomList.Count);
            int x = random.Next(activeRooms[randomRoom].RoomX, activeRooms[randomRoom].RoomX + activeRooms[randomRoom].RoomWidth);
            int y = random.Next(activeRooms[randomRoom].RoomY, activeRooms[randomRoom].RoomY + activeRooms[randomRoom].RoomLength);
            potion.Spawn(x, y);
            itemList.Add(potion);
        }
    }
    public void Update()
    {
        if(keyMap.TryGetValue(control.GetInput(), out Action? act))
        {
            act.Invoke();
        }
        enemyList.RemoveAll(e => e.Hp <= 0);
        itemList.RemoveAll(i => i.PickedUp == true);
        if(dungeon.map[player.Y, player.X] == '>')
        {
            floor++;
            Start();
        }
        EnemyTurn();
        if(player.Hp <= 0)
        {
            gameOver = true;
        }
    }

    public void ProcessMove(int dx, int dy)
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

    public void ProcessPickUp(int x, int y)
    {
        foreach (Item item in itemList){
            if(item.X == x && item.Y == y)
            {
                player.PickUp(item);
                item.PickedUp = true;
            }
        }
    }

    public void ProcessInventory()
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

    public Enemy? IsOccupied(int dx, int dy)
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
        char[,] buffer = renderer.DrawCall();
        Console.SetCursorPosition(0, 0);
        for(int i = 0; i < dungeon.length; i++)
        {
            for(int j = 0; j < dungeon.width; j++)
            {
                Console.Write(buffer[i, j]);
            }
            Console.Write('\n');
        }
        renderer.DrawUI(floor);
    }

    public void GameOver()
    {
        renderer.GameOver(floor);
    }

    public void EnemyTurn()
    {
        foreach(Enemy enemy in enemyList)
        {
            (int x, int y)? pos = pathfinding.BFS(enemy, player.X, player.Y, dungeon.map, (x, y) => IsOccupied(x, y));
            if(pos is (int x, int y))
            {
                if(x == player.X && y == player.Y)
                {
                    player.TakeDamage(enemy.Attack);
                }
                else
                {
                    enemy.Move(x - enemy.X, y - enemy.Y);
                }
            }
        }
        return;
    }

}