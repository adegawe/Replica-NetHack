class Enemy
{
    public int X { get; private set; }
    public int Y { get; private set; }
    public int Hp { get; private set; }
    public int MaxHp { get; private set; }
    public int Attack { get; private set; }

    public void Move(int dx, int dy)
    {
        X += dx;
        Y += dy;
    }

    public void TakeDamage(int amount)
    {
        Hp -= amount;
        if(Hp <= 0)
        {
            Hp = 0;
            //Died
        }
    }

    public void Heal(int amount)
    {
        Hp += amount;
        if(Hp > MaxHp)
        {
            Hp = MaxHp;
        }
    }
}