class Enemy
{
    public int X { get; private set; }
    public int Y { get; private set; }
    public int Hp { get; private set; }
    public int MaxHp { get; protected set; }
    public int Attack { get; protected set; }
    public char Symbol = 'E';
    public enum EnemyType { Goblin , Slime , Dragon};
    public EnemyType enemyType;

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

class Slime : Enemy
{
    public Slime()
    {
        MaxHp = 10;
        Attack = 3;
        Symbol = 'S';
    }
}

class Goblin : Enemy
{
    public Goblin()
    {
        MaxHp = 8;
        Attack = 4;
        Symbol = 'G';
    }
}