namespace RepHack;
class Enemy : Entity
{
    IEnemyBehavior behavior;
    public IRangedEnemy? GetRangedBehavior()
    {
        return behavior as IRangedEnemy;
    }
    public Enemy(EnemyData data, IEnemyBehavior behavior)
    {
        MaxHp = data.Hp;
        Hp = data.Hp;
        Attack = data.Attack;
        Symbol = data.Symbol[0];
        this.behavior = behavior;
    }
    public event Action<Enemy, int, int>? Moved;
    protected override void OnMoved(int oldX, int oldY)
    {
        Moved?.Invoke(this, oldX, oldY);
    }
    public virtual void Act(TurnContext ctx){ behavior.Execute(this, ctx);}
    public virtual void OnHit(Player p, TurnContext ctx){}
}