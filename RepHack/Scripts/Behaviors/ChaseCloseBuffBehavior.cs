using RepHack;
class ChaseCloseBuffBehavior : IEnemyBehavior
{
    public void Execute(Enemy self, TurnContext ctx)
    {
        const int MaxAddDamage = 5;

        (int x, int y) pos = ctx.pathfinding.GetNextStep(self, ctx.distanceMap, (x ,y) => ctx.IsOccupied(x, y));
        if(pos.x == ctx.player.X && pos.y == ctx.player.Y)
        {
            int bonusDamage = Math.Max(0, MaxAddDamage- ctx.distanceMap[self.Y, self.X].distance/MaxAddDamage);
            ctx.player.TakeDamage(self.Attack + bonusDamage);
        }
        else
        {
            self.Move(pos.x - self.X, pos.y - self.Y);
        }
    }
}