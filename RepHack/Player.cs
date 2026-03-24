class Player
{
    public int X { get; private set; }
    public int Y { get; private set; }
    public int Hp { get; private set; }
    public int MaxHp { get; private set; }
    public int Attack { get; private set; }
    private readonly Item[] inventory = new Item[50];
    public Player()
    {
        Hp = 15;
        Attack = 5;
        MaxHp = 20;
    }

    public void Move(int dx, int dy)
    {
        X += dx;
        Y += dy;
    }

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
            //GameOver
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