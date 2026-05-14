namespace RepHack;
class InstantRangedBehavior : IRangedEnemy
{
    private Random random = new();
    public List<(int x, int y)> AttackLine { get; } = new();
    public bool isPlayer { get; private set; }
    public void Execute(Enemy self, TurnContext ctx)
    {
        AttackLine.Clear();
        isPlayer = CalcuFov(self, ctx);
        (int x, int y) pos = ctx.pathfinding.GetNextStep(self, ctx.distanceMap, (x ,y) => ctx.IsOccupied(x, y));
        bool isHit = random.Next(0, 4) == 0;
        if(isPlayer && isHit)
        {
            ctx.player.TakeDamage(self.stats[StatType.Attack].Value);
        }
        else if(pos.x == ctx.player.X && pos.y == ctx.player.Y)
        {
            AttackLine.Clear();
            ctx.player.TakeDamage(self.stats[StatType.Attack].Value/5);
        }
        else
        {
            self.Move(pos.x - self.X, pos.y - self.Y);
        }
    }

    private bool CalcuFov(Enemy self, TurnContext ctx)
    {
        bool _isPlayer = false;
        int[] dx = {0, 0, 1, 1, 1, -1, -1, -1};
        int[] dy = {1, -1, 0, 1, -1, 0, -1, 1};
        for(int i = 0; i < 8; i++)
        {
            for(int j = 1; j < 20; j++)
            {
                if(self.X + j * dx[i] < 0 || self.X + j * dx[i] >= ctx.distanceMap.GetLength(1) ||
                self.Y + j * dy[i] < 0 || self.Y + j * dy[i] >= ctx.distanceMap.GetLength(0) ||
                    ctx.distanceMap[self.Y + j * dy[i], self.X + j * dx[i]].isBlocked)
                {
                    break;
                }
                if(ctx.player.X == self.X + j * dx[i] && ctx.player.Y == self.Y + j * dy[i])
                {
                    _isPlayer = true;
                }
                AttackLine.Add((self.X + j * dx[i], self.Y + j * dy[i]));
            }
        }
        return _isPlayer;
    }
}