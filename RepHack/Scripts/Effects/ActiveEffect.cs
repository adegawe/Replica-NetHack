namespace RepHack;
abstract class ActiveEffect : IItemEffect
{
    protected int remainingTurns;
    public void SetTurns(int turns) => remainingTurns = turns;
    public abstract void Apply(Player p, int value);
    public void Tick()
    {
        remainingTurns--;
        OnTick();
    }
    public virtual void OnTick() {}
    public virtual bool IsExpired()
    {
        return remainingTurns <= 0;
    }
}