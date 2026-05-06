namespace RepHack;
abstract class ActiveEffect
{
    int remainingTurns = 1;
    public void Tick()
    {
        remainingTurns--;
    }
    public virtual void OnApply(Player p){}
    public bool IsExpired()
    {
        return remainingTurns <= 0;
    }
}