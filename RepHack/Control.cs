class Control
{
    public enum Actions {MoveUp, MoveDown, MoveLeft, MoveRight, Attack, PickUp, 
    Throw, Drink, Eat, Read, OpenInventory, Equip, Idle}
    public Actions action;
    Dictionary<ConsoleKey, Actions> keyMap;

    public Control()
    {
        keyMap = new()
        {
            {ConsoleKey.UpArrow, Actions.MoveUp},
            {ConsoleKey.DownArrow, Actions.MoveDown},
            {ConsoleKey.LeftArrow, Actions.MoveLeft},
            {ConsoleKey.RightArrow, Actions.MoveRight},
            {ConsoleKey.OemComma, Actions.PickUp},
            {ConsoleKey.Q, Actions.Drink},
            {ConsoleKey.I, Actions.OpenInventory}
        };
    }

    public Actions GetInput(){
        if(Console.KeyAvailable) {
            var key = Console.ReadKey(true).Key;
            if(keyMap.TryGetValue(key, out action)) {
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
        if(y >= map.GetLength(0) || x < 0 || y < 0 || x >= map.GetLength(1)){return false;}
        
        if(map[y, x] == '#'){return false;}
        return true;
    }
}