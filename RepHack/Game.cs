class Game
{
    Player player = new();
    Dungeon dungeon = new();
    Control control = new();
    List<Enemy> enemyList = [];
    List<Item> itemList = [];
    Random random = new();
    Dictionary<Control.Actions, Action> keyMap;
    Renderer renderer;
    public bool gameOver = false;
    int floor = 1;

    public Game()
    {
        renderer = new(dungeon, player, enemyList, itemList, floor);
        keyMap = new()
        {
            {Control.Actions.MoveUp, () => ProcessMove(0, -1)},
            {Control.Actions.MoveDown, () => ProcessMove(0, 1)},
            {Control.Actions.MoveLeft, () => ProcessMove(-1, 0)},
            {Control.Actions.MoveRight, () => ProcessMove(1, 0)},
            {Control.Actions.PickUp, () => ProcessPickUp(player.X, player.Y)},
            {Control.Actions.OpenInventory, () => renderer.DrawInventory()}
        };
    }

    public void Start()
    {
        dungeon.roomList.Clear();
        enemyList.Clear();
        itemList.Clear();
        dungeon.InitDungeon();
        player.Spawn(dungeon.roomList[0].CenterX, dungeon.roomList[0].CenterY);
        for(int i = 0; i < 2; i++)
        {
            Enemy slime = new Slime();
            Enemy goblin = new Goblin();
            int randomRoom = random.Next(0, dungeon.roomList.Count);
            int x = random.Next(dungeon.roomList[randomRoom].x, dungeon.roomList[randomRoom].x + dungeon.roomList[randomRoom].width);
            int y = random.Next(dungeon.roomList[randomRoom].y, dungeon.roomList[randomRoom].y + dungeon.roomList[randomRoom].length);
            slime.Spawn(x, y);
            x = random.Next(dungeon.roomList[randomRoom].x, dungeon.roomList[randomRoom].x + dungeon.roomList[randomRoom].width);
            y = random.Next(dungeon.roomList[randomRoom].y, dungeon.roomList[randomRoom].y + dungeon.roomList[randomRoom].length);
            goblin.Spawn(x, y);
            enemyList.Add(slime);
            enemyList.Add(goblin);
        }
        for(int i = 0; i < 2; i++)
        {
            Item potion = new PotionItem();
            int randomRoom = random.Next(0, dungeon.roomList.Count);
            int x = random.Next(dungeon.roomList[randomRoom].x, dungeon.roomList[randomRoom].x + dungeon.roomList[randomRoom].width);
            int y = random.Next(dungeon.roomList[randomRoom].y, dungeon.roomList[randomRoom].y + dungeon.roomList[randomRoom].length);
            potion.Spawn(x, y);
            itemList.Add(potion);
        }
    }
    public void Update()
    {
        control.GetInput();
        switch(control.action){
            case Control.Actions.MoveUp:{
                ProcessMove(0, -1);
                break;
            }
            case Control.Actions.MoveDown:{
                ProcessMove(0, 1);
                break;
            }
            case Control.Actions.MoveLeft:{
                ProcessMove(-1, 0);
                break;
            }
            case Control.Actions.MoveRight:{
                ProcessMove(1, 0);
                break;
            }
            case Control.Actions.PickUp:{
                ProcessPickUp(player.X, player.Y);
                break;
            }
            case Control.Actions.OpenInventory:{
                renderer.DrawInventory();
                return;
            }
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
            BFS(enemy, enemy.X, enemy.Y);
        }
    }

    public void BFS(Enemy enemy, int enemyX, int enemyY)
    {
        (int, int)[,] cameFrom = new (int, int)[dungeon.length, dungeon.width];
        bool[,] visited = new bool[dungeon.length, dungeon.width];
        Queue<(int x, int y)> queue = new();
        (int dx, int dy)[] dirs = {(0,1), (0,-1), (1,0), (-1,0)};
        bool flag = true;
        visited[enemyY, enemyX] = true;
        (int x, int y) pos = (enemyX, enemyY);
        while (flag)
        {
            foreach(var (dx, dy) in dirs)
            {
                if(!Control.IsCanMove(pos.x + dx, pos.y + dy, dungeon.map) || visited[pos.y + dy, pos.x + dx])
                {
                    continue;
                }
                else
                {
                    visited[pos.y + dy, pos.x + dx] = true;
                    cameFrom[pos.y + dy, pos.x + dx] = (pos.x, pos.y);
                    queue.Enqueue((pos.x + dx, pos.y + dy));
                }
            }
            if(queue.Count() == 0)
            {
                return;
            }
            pos = queue.Dequeue();
            if(pos.x == player.X && pos.y == player.Y)
            {
                flag = false;
            }
        }
        var lastPos = pos;
        while(pos != (enemyX, enemyY))
        {
            lastPos = pos;
            pos = cameFrom[lastPos.y, lastPos.x];
        }
        if(lastPos.x == player.X && lastPos.y == player.Y)
        {
            player.TakeDamage(enemy.Attack);
        }
        else
        {
            if(IsOccupied(lastPos.x, lastPos.y) == null)
            {
                enemy.Move(lastPos.x - enemy.X, lastPos.y - enemy.Y);
            }
        }
    }
}