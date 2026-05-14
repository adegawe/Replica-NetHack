namespace RepHack;
class DrainOnHitChaseBehavior : IEnemyBehavior
{
    public void Execute(Enemy self, TurnContext ctx)
    {
        (int x, int y) pos = ctx.pathfinding.GetNextStep(self, ctx.distanceMap, (x ,y) => ctx.IsOccupied(x, y));
            
        if(pos.x == ctx.player.X && pos.y == ctx.player.Y)
        {
            ctx.player.TakeDamage(self.stats[StatType.Attack].Value);
            self.Heal(self.stats[StatType.Attack].Value-ctx.player.stats[StatType.Defense].Value);
        }
        else
        {
            self.Move(pos.x - self.X, pos.y - self.Y);
        }
    }
}