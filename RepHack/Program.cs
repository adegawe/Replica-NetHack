namespace RepHack;
class Program
{
    static void Main()
    {
        const int FRAME_DELEAY = 16;
        Console.CursorVisible = false;
        Game game = new();
        game.Start();
        Console.Clear();
        while (game.gameOver == false)
        {
            if(Console.KeyAvailable) {
                game.Update();
            }
            if(game.gameOver == true)
            {
                game.GameOver();
                Thread.Sleep(200);
                return;
            }
            game.Render();
            Thread.Sleep(FRAME_DELEAY);
        }
    }
}