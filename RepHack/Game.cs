class Game
{
    Player player = new();
    Dungeon dungeon = new();
    Control control = new();
    List<Enemy> enemyList = [];
    Random random = new();
    public bool gameOver = false;
    int floor = 1;

    public void Start()
    {
        dungeon.roomList.Clear();
        enemyList.Clear();
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
        }
        enemyList.RemoveAll(e => e.Hp <= 0);
        if(dungeon.map[player.X, player.Y] == '>')
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

    public void Render()
    {
        Console.SetCursorPosition(0, 0);
        for(int i = 0; i < dungeon.length; i++)
        {
            for(int j = 0; j < dungeon.width; j++)
            {
                bool enemyFlag = false;
                char symbol = ' ';
                foreach(Enemy enemy in enemyList)
                {
                    if(j == enemy.X && i == enemy.Y)
                    {
                        enemyFlag = true;
                        symbol = enemy.Symbol;
                    }
                }
                if(j == player.X && i == player.Y)
                {
                    Console.Write('@');
                }
                else if (enemyFlag)
                {
                    Console.Write(symbol);
                }
                else
                {
                    Console.Write(dungeon.map[j, i]);
                }
            }
            Console.Write('\n');
        }
        DrawUI();
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

    public void EnemyTurn()
    {
        foreach(Enemy enemy in enemyList)
        {
            BFS(enemy, enemy.X, enemy.Y);
        }
    }

    public void BFS(Enemy enemy, int enemyX, int enemyY)
    {
        (int , int)[,] cameFrom = new (int, int)[dungeon.width, dungeon.length];
        bool[,] visited = new bool[dungeon.width, dungeon.length];
        Queue<(int x, int y)> queue = new();
        (int dx, int dy)[] dirs = {(0,1), (0, -1), (1,0), (-1, 0)};
        bool flag = true;
        visited[enemyX, enemyY] = true;
        (int x, int y)pos = (enemyX, enemyY);
        while (flag)
        {
            foreach(var (dx, dy) in dirs)
            {
                if(!Control.IsCanMove(pos.x + dx, pos.y + dy, dungeon.map) || visited[pos.x + dx, pos.y + dy] == true)
                {
                    continue;
                }
                else{
                    visited[pos.x + dx, pos.y + dy] = true;
                    cameFrom[pos.x + dx, pos.y + dy] = (pos.x, pos.y);
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
            pos = cameFrom[lastPos.x, lastPos.y];
        }
        if(lastPos.x == player.X && lastPos.y == player.Y)
        {
            player.TakeDamage(enemy.Attack);
        }
        else
        {
            if(IsOccupied(lastPos.x, lastPos.y) == null){
                enemy.Move(lastPos.x - enemy.X, lastPos.y - enemy.Y);
            }
        }
    }

    public void DrawUI()
    {
        Console.WriteLine("\n════════════════════════════════════════");
        Console.WriteLine($"HP: {player.Hp}/{player.MaxHp}  ATK: {player.Attack}  Floor: {floor}");
        Console.WriteLine("════════════════════════════════════════");
    }

    public void GameOver()
    {
        Console.Clear();
        Console.WriteLine("\n╔════════════════════════════════════════════╗");
        Console.WriteLine("║                                            ║");
        Console.WriteLine("║    G A M E    O V E R                      ║");
        Console.WriteLine("║                                            ║");
        Console.WriteLine($"║         You died on floor {floor:D3}               ║");
        Console.WriteLine("║         Press any key to quit              ║");
        Console.WriteLine("║                                            ║");
        Console.WriteLine("╚════════════════════════════════════════════╝");
    }
}