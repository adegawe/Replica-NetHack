namespace RepHack;
class Item
{    
    public int X { get; private set; }
    public int Y { get; private set; }
    public string displayName = "";
    public int weight;
    public char Symbol;
    public bool PickedUp = false;
    public bool Consumable;
    public int Uses = 0;
    public int effectValue = 0;
    public Dictionary<StatType, int> equipBonuses = new();
    IItemEffect itemEffect;

    public enum ItemType { Potion, Scroll, Food, Ring, Armor, Weapon, Wand, Tool };
    public ItemType category;
    public enum BlessState { Cursed, Normal, Blessed };
    public BlessState blessState;
    public Item(ItemData data, IItemEffect effect)
    {
        displayName = data.Name;
        Symbol = data.Symbol[0];
        if(Enum.TryParse<ItemType>(data.Category, out ItemType type))
        {
            this.category = type;
        }
        if(this.category == ItemType.Weapon || this.category == ItemType.Armor)
        {
            this.equipBonuses = data.EquipBonuses;
        }
        this.itemEffect = effect;
        if (effect is ActiveEffect ae) ae.SetTurns(data.RemainingTime);
        this.effectValue = data.EffectValue;
        this.Uses = data.Uses;
        this.Consumable = data.Consumable;
    }
    public void Spawn(int x, int y)
    {
        X = x;
        Y = y;
    }

    public bool Use(Player player) 
    {
        if(Consumable && Uses > 0)
        {
            itemEffect.Apply(player, effectValue);
            Uses--;
        }
        return Uses <= 0;
    }
}