class Game
{
    Player player = new();
    Dungeon dungeon = new();
    Control control = new();

    public void Start()
    {
        dungeon.InitDungeon();
        player.Spawn(dungeon.roomList[0].CenterX, dungeon.roomList[0].CenterY);
    }
    public void Update()
    {
        control.GetInput();
        switch(control.action){
            case Control.Actions.MoveUp:{
                if(Control.IsCanMove(player.X, player.Y - 1, dungeon.map))
                {
                    player.Move(0, -1);
                }
                break;
            }
            case Control.Actions.MoveDown:{
                if(Control.IsCanMove(player.X, player.Y - 1, dungeon.map))
                {
                    player.Move(0, 1);
                }
                break;
            }
            case Control.Actions.MoveLeft:{
                if(Control.IsCanMove(player.X, player.Y - 1, dungeon.map))
                {
                    player.Move(-1, 0);
                }
                break;
            }
            case Control.Actions.MoveRight:{
                if(Control.IsCanMove(player.X, player.Y - 1, dungeon.map))
                {
                    player.Move(1, 0);
                }
                break;
            }
        }
    }

    public void Render()
    {
        Console.SetCursorPosition(0, 0);
        for(int i = 0; i < dungeon.length; i++)
        {
            for(int j = 0; j < dungeon.width; j++)
            {
                if(j == player.X && i == player.Y)
                {
                    Console.Write('@');
                }
                else
                {
                    Console.Write(dungeon.map[j, i]);
                }
            }
            Console.Write('\n');
        }
    }
}