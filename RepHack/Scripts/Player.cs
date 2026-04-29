namespace RepHack;
class Player : Entity
{
    public readonly List<Item> inventory = new();
    public int fovLength = 12;
    public int inventoryMax = 50;
    public Player()
    {
        Attack = 50;
        MaxHp = 20;
        Symbol = '@';
    }

    public void PickUp(Item item)
    {
        if(inventory.Count < inventoryMax)
        {
            inventory.Add(item);
        }
    }

    public void Use(int index)
    {
        bool shouldRemove = inventory[index].Use(this);
        if(shouldRemove){ inventory.RemoveAt(index); }
    }
}