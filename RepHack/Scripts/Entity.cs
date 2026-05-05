namespace RepHack;
class Entity
{
    public int X { get; private set; }
    public int Y { get; private set; }
    public int Hp { get; protected set; }
    public int MaxHp { get; protected set; }
    public int Attack { get; protected set; }
    public char Symbol { get; protected set; }
    public void Move(int dx, int dy)
    {
        int oldX = X, oldY = Y;
        X += dx;
        Y += dy;
        OnMoved(oldX, oldY);
    }
    protected virtual void OnMoved(int oldX, int oldY){}
    public void Spawn(int x, int y)
    {
        X = x;
        Y = y;
        Hp = MaxHp;
    }

    public void TakeDamage(int amount)
    {
        Hp -= amount;
        if(Hp <= 0)
        {
            Hp = 0;
            OnDied(X, Y);
        }
    }
    protected virtual void OnDied(int oldX, int oldY){}
    public void Heal(int amount)
    {
        Hp += amount;
        if(Hp > MaxHp)
        {
            Hp = MaxHp;
        }
    }
}