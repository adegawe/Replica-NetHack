namespace RepHack;
class Renderer
{
    Dungeon dungeon;
    Player player;
    FOV fov;
    List<Enemy> enemyList;
    List<Item> itemList;
    
    char[,] buffer;
    public Dictionary<char, ConsoleColor> colorMap {get; private set;}

    public Renderer(Dungeon d, Player p, FOV f, List<Enemy> e, List<Item> i)
    {
        dungeon = d;
        player = p;
        fov = f;
        enemyList = e;
        itemList = i;
        buffer = new char[dungeon.length, dungeon.width];
        colorMap = new()
        {
            {'@', ConsoleColor.DarkRed},
            {'S', ConsoleColor.Cyan},
            {'G', ConsoleColor.DarkGreen},
            {'!', ConsoleColor.DarkMagenta},
            {'>', ConsoleColor.Yellow},
            {'#', ConsoleColor.White},
            {'.', ConsoleColor.Black},
            {'░', ConsoleColor.Black}
        };
    }

    public void Render(int floor)
    {
        fov.ComputeFOV(player.X, player.Y, player.fovLength);
        Console.SetCursorPosition(0, 0);
        DrawCall();
        PrintBuffer();
        DrawUI(floor);
    }

    public void DrawCall()
    {
        Array.Clear(buffer, 0, buffer.Length);
        for(int i = 0; i < dungeon.length; i++)
        {
            for(int j = 0; j < dungeon.width; j++)
            {
                if (fov.isVisible[i, j] || fov.isExplored[i, j]) 
                {
                    buffer[i, j] = dungeon.map[i, j];
                }
                else 
                {
                    buffer[i, j] = '░';
                }
            }
        }
        foreach(Item item in itemList)
        {
            if (fov.isVisible[item.Y, item.X])
            {
                buffer[item.Y, item.X] = item.Symbol;
            }
        }
        foreach(Enemy enemy in enemyList)
        {
            if (fov.isVisible[enemy.Y, enemy.X])
            {
                buffer[enemy.Y, enemy.X] = enemy.Symbol;
            }
        }
        buffer[player.Y, player.X] = player.Symbol;
    }

    public void PrintBuffer()
    {
        Console.SetCursorPosition(0, 0);
        for(int i = 0; i < dungeon.length; i++)
        {
            for(int j = 0; j < dungeon.width; j++)
            {
                char text = buffer[i, j];
                if(fov.isVisible[i, j] && colorMap.TryGetValue(text, out var color)) {}
                else
                {
                    color = ConsoleColor.Black;
                }
                Console.ForegroundColor = color;
                Console.Write(buffer[i, j]);
            }
            Console.Write('\n');
        }
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