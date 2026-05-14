namespace RepHack;
using System.Text;
class Renderer
{
    Dungeon dungeon;
    Player player;
    FOV fov;
    EnemyRegistry enemyRegistry;
    List<Item> itemList;
    StringBuilder sb = new();
    HashSet<(int x, int y)> dangerCells = new();
    
    char[,] buffer;
    public Dictionary<char, ConsoleColor> colorMap {get; private set;}

    public Renderer(Dungeon d, Player p, FOV f, EnemyRegistry e, List<Item> i)
    {
        dungeon = d;
        player = p;
        fov = f;
        enemyRegistry = e;
        itemList = i;
        buffer = new char[dungeon.length, dungeon.width];
        colorMap = new()
        {
            {'@', ConsoleColor.Blue},
            {'!', ConsoleColor.DarkMagenta},
            {'?', ConsoleColor.Green},
            {'>', ConsoleColor.Yellow},
            {'#', ConsoleColor.White},
            {'.', ConsoleColor.DarkGray},
            {'░', ConsoleColor.DarkGray}
        };
    }

    public void Render(int floor)
    {
        fov.ComputeFOV(player.X, player.Y, player.stats[StatType.FovLength].Value);
        Console.SetCursorPosition(0, 0);
        DrawCall();
        PrintBuffer();
        DrawUI(floor);
    }

    public void DrawCall()
    {
        Array.Clear(buffer, 0, buffer.Length);
        dangerCells.Clear();
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
        foreach(Enemy enemy in enemyRegistry.enemyList)
        {
            if (fov.isVisible[enemy.Y, enemy.X])
            {
                buffer[enemy.Y, enemy.X] = enemy.Symbol;
            }
            var ranged = enemy.GetRangedBehavior();
            if (ranged != null)
            {
                dangerCells.UnionWith(ranged.AttackLine);
            }
        }
        buffer[player.Y, player.X] = player.Symbol;
    }

    public void PrintBuffer()
    {
        Console.SetCursorPosition(0, 0);
        ConsoleColor lastCs = ConsoleColor.Black;
        for(int i = 0; i < dungeon.length; i++)
        {
            for(int j = 0; j < dungeon.width; j++)
            {
                char text = buffer[i, j];
                ConsoleColor color;
                if(fov.isVisible[i, j]){
                    if(dangerCells.Contains((j, i)))
                    {
                        color = ConsoleColor.Red;
                    }
                    else{ colorMap.TryGetValue(text, out color); }
                }
                else{color = ConsoleColor.DarkGray;}
                if(lastCs == color)
                {
                    sb.Append(text);
                    lastCs = color;
                }
                else{
                    Console.ForegroundColor = lastCs;
                    Console.Write(sb.ToString());
                    sb.Clear();
                    sb.Append(text);
                    lastCs = color;
                }
            }
            Console.ForegroundColor = lastCs;
            Console.Write(sb.ToString());
            sb.Clear();
            Console.Write('\n');
        }
    }
    public void DrawUI(int floor)
    {
        Console.WriteLine("\n════════════════════════════════════════");
        Console.WriteLine($"HP: {player.Hp}/{player.stats[StatType.MaxHp].Value}  ATK: {player.stats[StatType.Attack].Value}  DEF: {player.stats[StatType.Defense].Value}  Floor: {floor}");
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

    public void DrawAct(string message)
    {
        Console.WriteLine("\n════════════════════════════════════════");
        Console.WriteLine($"{message}");
        Console.WriteLine("════════════════════════════════════════");
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

    public void RegisterEnemyColors(List<EnemyData> EnemyData)
    {
        foreach(var enemyData in EnemyData)
        {
            var color = Enum.Parse<ConsoleColor>(enemyData.Color);
            colorMap.Add(enemyData.Symbol[0], color);
        }
    }
}