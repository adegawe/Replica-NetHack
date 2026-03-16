class Game
{
    Player player = new();
    Dungeon dungeon = new();
    Control control = new();
    List<Enemy> enemyList = [];
    Random random = new();

    public void Start()
    {
        dungeon.roomList.Clear();
        enemyList.Clear();
        dungeon.InitDungeon();
        player.Spawn(dungeon.roomList[0].CenterX, dungeon.roomList[0].CenterY);
        for(int i = 0; i < 5; i++)
        {
            Enemy slime = new Slime();
            int randomRoom = random.Next(0, dungeon.roomList.Count);
            int x = random.Next(dungeon.roomList[randomRoom].x, dungeon.roomList[randomRoom].x + dungeon.roomList[randomRoom].width);
            int y = random.Next(dungeon.roomList[randomRoom].y, dungeon.roomList[randomRoom].y + dungeon.roomList[randomRoom].length);
            slime.Spawn(x, y);
            enemyList.Add(slime);
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
            Start();
        }
        EnemyTurn();
    }

    public void Render()
    {
        Console.SetCursorPosition(0, 0);
        for(int i = 0; i < dungeon.length; i++)
        {
            for(int j = 0; j < dungeon.width; j++)
            {
                bool enemyFlag = false;
                foreach(Enemy enemy in enemyList)
                {
                    if(j == enemy.X && i == enemy.Y)
                    {
                        enemyFlag = true;
                    }
                }
                if(j == player.X && i == player.Y)
                {
                    Console.Write('@');
                }
                else if (enemyFlag)
                {
                    Console.Write('S');
                }
                else
                {
                    Console.Write(dungeon.map[j, i]);
                }
            }
            Console.Write('\n');
        }
    }

    public void ProcessMove(int dx, int dy)
    {
        if(Control.IsCanMove(player.X + dx, player.Y + dy, dungeon.map))
        {
            foreach(Enemy enemy in enemyList)
            {
                if(enemy.X == player.X + dx && enemy.Y == player.Y + dy)
                {
                    enemy.TakeDamage(player.Attack);
                    return;
                }
            }
            player.Move(dx, dy);
        }
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
            if(!CanMove(lastPos.x, lastPos.y))
            enemy.Move(lastPos.x - enemy.X, lastPos.y - enemy.Y);
        }
    }
}