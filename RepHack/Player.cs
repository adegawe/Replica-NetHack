class Player
{
    public int X { get; private set; }
    public int Y { get; private set; }
    public int Hp { get; private set; }
    public int MaxHp { get; private set; }
    public int Attack { get; private set; }
    public readonly List<Item> inventory = [];
    public int inventoryMax = 50;
    public Player()
    {
        Hp = 15;
        Attack = 15;
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

    public void Use(int index)
    {
        inventory[index].Use(this);
        if(inventory[index].Consumable == true)
        {
            if(inventory[index].Uses <= 0)
            {
                inventory.Remove(inventory[index]);
            }
        }
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

    internal void Use(bool v)
    {
        throw new NotImplementedException();
    }

    internal void Use(bool v, out int result)
    {
        throw new NotImplementedException();
    }
}