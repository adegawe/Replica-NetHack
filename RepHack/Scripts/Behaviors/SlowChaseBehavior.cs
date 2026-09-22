namespace RepHack;
class SlowChaseBehavior : IEnemyBehavior
{
    int interval, counter;
    ChaseBehavior chase = new();
    public SlowChaseBehavior(int interval = 2)
    {
        this.interval = interval;
        this.counter = 0;
    }
    public void Execute(Enemy self, TurnContext ctx)
    {
        counter++;
        if(counter >= interval)
        {
            counter = 0;
            chase.Execute(self, ctx);
        }
    }
}