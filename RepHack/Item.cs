class Item
{    
    public int X { get; private set; }
    public int Y { get; private set; }
    public string name = "";
    public int weight;
    public char Symbol = '?';

    public enum ItemType { Potion, Scroll, Food, Ring, Armor, Weapon, Wand, Tool };

    public ItemType type;

    public enum BlessState { Cursed, Normal, Blessed };

    public BlessState blessState;

    public void Spawn(int x, int y)
    {
        X = x;
        Y = y;
    }
}

class WeaponItem : Item
{
    public int damage;
    public enum WeaponType { Sword, Axe, Spear };
    public WeaponType weaponType;
    public WeaponItem()
    {
        Symbol = ')';
    }
}

class PotionItem : Item
{
    public PotionItem()
    {
        Symbol = '!';
    }
    public int healAmount = 5;
}