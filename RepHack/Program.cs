class Program
{
    static void Main()
    {
        Console.CursorVisible = false;
        Game game = new();
        game.Start();
        while (true)
        {
            if(Console.KeyAvailable) {
                game.Update();
                game.Render();
            }
            Thread.Sleep(16);
        }
    }
}