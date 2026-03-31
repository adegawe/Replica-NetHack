class Player
{
    public int X { get; private set; }
    public int Y { get; private set; }
    public int Hp { get; private set; }
    public int MaxHp { get; private set; }
    public int Attack { get; private set; }
    private readonly List<Item> inventory = [];
    int inventoryMax = 50;
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

    public void PickUp(Item item)
    {
        if(inventory.Count <= inventoryMax)
        {
            inventory.Add(item);
        }
    }

    public void Use(Item item, char alphabet){}

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