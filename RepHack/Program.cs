class Program
{
    static void Main()
    {
        Dungeon dungeon = new Dungeon();
        dungeon.InitDungeon();
        for(int i = 0; i < dungeon.length; i++)
        {
            for(int j = 0; j < dungeon.width; j++)
            {
                Console.Write(dungeon.map[j, i]);
            }
            Console.Write('\n');
        }
    }
}