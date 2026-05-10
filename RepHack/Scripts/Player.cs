namespace RepHack;
class Player : Entity
{
    public readonly List<Item> inventory = new();
    public List<ActiveEffect> activeEffects = new();
    public Player()
    {
        stats[StatType.Attack] = new Stat { BaseValue = 50 };
        stats[StatType.Defense] = new Stat { BaseValue = 1 };
        stats[StatType.MaxHp] = new Stat { BaseValue = 70 };
        Hp = 70;
        stats[StatType.FovLength] = new Stat { BaseValue = 12 };
        stats[StatType.InventoryMax] = new Stat { BaseValue = 50 };
        Symbol = '@';
    }

    public void PickUp(Item item)
    {
        if(inventory.Count < stats[StatType.InventoryMax].Value)
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
    public void TickEffect()
    {
        foreach(var effect in activeEffects) { effect.Tick(); }
        var expired = activeEffects.Where(e => e.IsExpired()).ToList();
        foreach (var effect in expired)
            foreach (var stat in stats.Values)
                stat.RemoveAllFromSource(effect);
        
        activeEffects.RemoveAll(e => e.IsExpired());
    }
}