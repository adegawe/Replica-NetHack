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
        if(dungeon.map[player.X, player.Y] == '>')
        {
            Start();
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
}