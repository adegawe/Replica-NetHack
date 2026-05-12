namespace RepHack;
using System.Diagnostics;
class Game
{
    Player player = new();
    Dungeon dungeon = new();
    Control control = new();
    FOV fov;
    Pathfinding pathfinding;
    List<Item> itemList = new();
    Random random = new();
    EnemyRegistry enemyRegistry = new();
    Dictionary<Control.Actions, (Action, bool)> actionMap;
    Renderer renderer;
    TurnContext ctx;
    public bool gameOver = false;
    int floor = 1;
    int minMonster = 3;
    int minItem = 3;
    List<EnemyData> enemyData = EnemyLoader.Load();
    List<ItemData> itemData = ItemLoader.Load();

    public Game()
    {
        fov = new(dungeon.width, dungeon.length, dungeon.map);
        renderer = new(dungeon, player, fov, enemyRegistry, itemList);
        pathfinding = new(dungeon.width, dungeon.length, dungeon.map);
        ctx = new TurnContext(player, pathfinding,
        (x, y) => enemyRegistry.IsOccupied(x, y));
        renderer.RegisterEnemyColors(enemyData);

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
        var activeEnemy = enemyData.Where(data => data.MinFloor <= floor).ToList();
        enemyRegistry.Clear();
        var pickedEnemyData = PickWeighted(minMonster + floor, activeEnemy, d => d.Weight);
        var enemies = pickedEnemyData.Select(EnemyFactory.Create).ToList();
        SpawnEntities(enemies, activeRooms, (enemy, x, y) => enemy.Spawn(x, y));
        foreach (Enemy enemy in enemies){
            enemyRegistry.Add(enemy);
        }
        var activeItem = itemData.Where(data => data.MinFloor <= floor).ToList();
        var pickedItemData = PickWeighted(minItem + floor, activeItem, d => d.Weight);
        var items = pickedItemData.Select(ItemFactory.Create).ToList();
        SpawnEntities(items, activeRooms, (item, x, y) => item.Spawn(x, y));
        itemList.AddRange(items);
    }
    public void Update()
    {
        var sw = Stopwatch.StartNew();
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
        player.TickEffect();
        ctx.ReCompute();
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
        sw.Stop();
        if(sw.ElapsedTicks > 10000)
        {
            File.AppendAllText(
                Path.Combine(AppContext.BaseDirectory, "perf.log"),
                $"floor={floor},enemies={enemyRegistry.count},ticks={sw.ElapsedTicks}\n"
            );
        }
    }

    private void ProcessMove(int dx, int dy)
    {
        if(Control.IsCanMove(player.X + dx, player.Y + dy, dungeon.map))
        {
            Enemy? tempEnemy = enemyRegistry.IsOccupied(player.X + dx, player.Y + dy);
            if(tempEnemy != null)
            {
                tempEnemy.TakeDamage(player.stats[StatType.Attack].Value);
                if(tempEnemy.Hp <= 0)
                {
                    enemyRegistry.Remove(tempEnemy);
                }
                return;
            }
            player.Move(dx, dy);
        }
    }

    private void ProcessPickUp(int x, int y)
    {
        foreach (Item item in itemList.ToList()){
            if(item.X == x && item.Y == y)
            {
                if(item.category == Item.ItemType.Weapon)
                {
                    if(player.equippedWeapon != null)
                    {
                        itemList.Add(player.equippedWeapon);
                        player.equippedWeapon.Spawn(x, y);
                        player.equippedWeapon.PickedUp = false;
                        player.UnEquip(player.equippedWeapon);
                    }
                    player.Equip(item);
                }
                else if(item.category == Item.ItemType.Armor)
                {
                    if(player.equippedArmor != null)
                    {
                        itemList.Add(player.equippedArmor);
                        player.equippedArmor.Spawn(x, y);
                        player.equippedArmor.PickedUp = false;
                        player.UnEquip(player.equippedArmor);
                    }
                    player.Equip(item);
                }
                else
                {
                    player.PickUp(item);
                }
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
        foreach(Enemy enemy in enemyRegistry.enemyList.ToList())
        {
            enemy.Act(ctx);
        }
    }
    private List<T> PickWeighted<T>(int requiredNum, List<T> candidates, Func<T, int> weightSelector)
    {
        int totalWeight = 0;
        List<T> entities = new();
        foreach(T entityData in candidates)
        {
            totalWeight += weightSelector(entityData);
        }
        for(int i = 0; i < requiredNum; i++)
        {
            int cumulative = 0;
            int pick = random.Next(0, totalWeight);
            foreach (T entityData in candidates)
            {
                cumulative += weightSelector(entityData);
                if(cumulative > pick)
                {
                    entities.Add(entityData);
                    break;
                }
            }
        }
        return entities;
    }

    private void SpawnEntities<T>(List<T> enemiesToSpawn, List<Node> roomList, 
                                Action<T, int, int> spawnAction)
    {
        List<(int x, int y)> activeCells = new();
        foreach(Node node in roomList)
        {
            for(int i = node.RoomY; i < node.RoomY + node.RoomLength; i++)
            {
                for(int j = node.RoomX; j < node.RoomX + node.RoomWidth; j++)
                {
                    activeCells.Add((j, i));
                }
            }
        }
        foreach(T entity in enemiesToSpawn)
        {
            if(activeCells.Count <= 0) { break; }
            var randomPos = activeCells[random.Next(0, activeCells.Count)];
            spawnAction(entity, randomPos.x, randomPos.y);
            activeCells.Remove(randomPos);
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
        distanceMap = pathfinding.Dijkstra(player.X, player.Y, 
         isEnemyAtCached);
    }
    
    public void ReCompute()
    {
        distanceMap = pathfinding.Dijkstra(player.X, player.Y, 
         isEnemyAtCached);
    }
}