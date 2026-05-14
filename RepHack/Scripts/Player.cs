namespace RepHack;
class Player : Entity
{
    public readonly List<Item> inventory = new();
    public List<ActiveEffect> activeEffects = new();
    public Item? equippedWeapon;
    public Item? equippedArmor;
    public Player()
    {
        stats[StatType.Attack] = new Stat { BaseValue = 50 };
        stats[StatType.Defense] = new Stat { BaseValue = 140 };
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
    public void Equip(Item item)
    {
        if(item.category == Item.ItemType.Weapon)
        {
            this.equippedWeapon = item;
        }
        if(item.category == Item.ItemType.Armor)
        {
            this.equippedArmor = item;
        }
        foreach(var (statType, value) in item.equipBonuses)
        {
            var mod = new StatModifier(value, StatModType.Flat, item);
            stats[statType].AddModifier(mod);
        }
        Console.WriteLine($"{item.category}의 {item.displayName}을 착용했다.");
        
    }
    public void UnEquip(Item item)
    {
        if(item.category == Item.ItemType.Weapon)
        {
            this.equippedWeapon = null;
        }
        if(item.category == Item.ItemType.Armor)
        {
            this.equippedArmor = null;
        }
        foreach(var (statType, value) in item.equipBonuses)
        {
            stats[statType].RemoveAllFromSource(item);
        }
    }
}