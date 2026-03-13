class Control
{
    public enum Actions {MoveUp, MoveDown, MoveLeft, MoveRight, Attack, PickUp, UseItem, Idle}
    public Actions action;

    public Actions GetInput(){
        if(Console.KeyAvailable) {
            var key = Console.ReadKey(true).Key;
            if(key == ConsoleKey.RightArrow) {
                action = Actions.MoveRight;
                return action;
            } else if(key == ConsoleKey.LeftArrow) {
                action = Actions.MoveLeft;
                return action;
            }else if(key == ConsoleKey.DownArrow) {
                action = Actions.MoveDown;
                return action;
            }else if(key == ConsoleKey.UpArrow) {
                action = Actions.MoveUp;
                return action;
            }else
            {
                return action = Actions.Idle;
            }
        }
        return Actions.Idle;
    }

    public static bool IsCanMove(int x, int y, char[,] map)
    {
        if(x > map.GetLength(0) || x < 0 || y < 0 || y > map.GetLength(1)){return false;}
        
        if(map[x, y] == '#'){return false;}
        return true;
    }
}