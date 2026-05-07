namespace RepHack;
class Player : Entity
{
    public readonly List<Item> inventory = new();
    public List<ActiveEffect> activeEffects = new();
    private int baseFov = 12;
    public int fovLength = 12;
    public int inventoryMax = 50;
    public Player()
    {
        Attack = 50;
        Defense = 1;
        MaxHp = 70;
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

    public void AddEffect(ActiveEffect activeEffect)
    {
        activeEffects.Add(activeEffect);
    }
}