class Program
{
    static void Main()
    {
        Console.CursorVisible = false;
        Game game = new();
        game.Start();
        Console.Clear();
        game.Render();
        while (game.gameOver == false)
        {
            if(Console.KeyAvailable) {
                game.Update();
                game.Render();
            }
            if(game.gameOver == true)
            {
                game.GameOver();
                Thread.Sleep(200);
            }
            Thread.Sleep(16);
        }
    }
}