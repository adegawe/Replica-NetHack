class Renderer
{
    Dungeon dungeon;
    Player player;
    List<Enemy> enemyList;
    List<Item> itemList;
    
    public Renderer(Dungeon d, Player p, List<Enemy> e, List<Item> i, int f)
    {
        dungeon = d;
        player = p;
        enemyList = e;
        itemList = i;
    }

    public char[,] DrawCall()
    {
        char[,] buffer = new char[dungeon.length, dungeon.width];
        for(int i = 0; i < dungeon.length; i++)
        {
            for(int j = 0; j < dungeon.width; j++)
            {
                buffer[i, j] = dungeon.map[i, j];
            }
        }
        foreach(Item item in itemList)
        {
            buffer[item.Y, item.X] = item.Symbol;
        }
        foreach(Enemy enemy in enemyList)
        {
            buffer[enemy.Y, enemy.X] = enemy.Symbol;
        }
        buffer[player.Y, player.X] = '@';
        return buffer;
    }
    public void DrawUI(int floor)
    {
        Console.WriteLine("\n════════════════════════════════════════");
        Console.WriteLine($"HP: {player.Hp}/{player.MaxHp}  ATK: {player.Attack}  Floor: {floor}");
        Console.WriteLine("════════════════════════════════════════");
    }

    public void DrawInventory()
    {
        Console.Clear();
        Console.WriteLine("\n════════════════════════════════════════════════════════════════════════════════");
        Console.Write("|");
        for(int i = 0; i < player.inventory.Count; i++)
        {
            Console.Write($"{player.inventory[i].displayName} (Remain...{player.inventory[i].Uses})");
            Console.Write("|");
            if(i != 0 && i%4 == 0){ Console.Write("\n"); Console.Write("|"); }
        }
        Console.Write("\n");
        Console.WriteLine("════════════════════════════════════════════════════════════════════════════════");
    }

    public void GameOver(int floor)
    {
        Console.Clear();
        Console.WriteLine("\n╔════════════════════════════════════════════╗");
        Console.WriteLine("║                                            ║");
        Console.WriteLine("║            G A M E    O V E R              ║");
        Console.WriteLine("║                                            ║");
        Console.WriteLine($"║           You died on floor {floor:D3}            ║");
        Console.WriteLine("║           Press any key to quit            ║");
        Console.WriteLine("║                                            ║");
        Console.WriteLine("╚════════════════════════════════════════════╝");
    }
}