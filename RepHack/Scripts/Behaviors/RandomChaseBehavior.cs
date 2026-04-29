using RepHack;
class RandomChaseBehavior : IEnemyBehavior
{
    ChaseBehavior chase = new();
    Random random = new();
    int[] dx = {0, 0, 1, -1};
    int[] dy = {1, -1, 0, 0};
    public void Execute(Enemy self, TurnContext ctx)
    {
        (int x, int y) pos;
        int randomInt = random.Next(0, 100);
        if(randomInt > 50) chase.Execute(self, ctx);
        else
        {
            int index = random.Next(0, 4);
            pos = (dx[index] + self.X, dy[index] + self.Y);
            if(pos.x == ctx.player.X && pos.y == ctx.player.Y)
            {
                ctx.player.TakeDamage(self.Attack);
            }
            else if(ctx.IsOccupied(pos.x, pos.y) == null &&
                    !ctx.distanceMap[pos.y, pos.x].isBlocked)
            {
                self.Move(dx[index], dy[index]);
            }
        }
    }
}