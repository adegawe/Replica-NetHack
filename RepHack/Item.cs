namespace RepHack;
class Item
{    
    public int X { get; private set; }
    public int Y { get; private set; }
    public string displayName = "";
    public int weight;
    public char Symbol = '?';
    public bool PickedUp = false;
    public bool Consumable = false;
    public int Uses = 0;

    public enum ItemType { Potion, Scroll, Food, Ring, Armor, Weapon, Wand, Tool };

    public ItemType type;

    public enum BlessState { Cursed, Normal, Blessed };

    public BlessState blessState;

    public void Spawn(int x, int y)
    {
        X = x;
        Y = y;
    }

    public virtual void Use(Player player) { }
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
    
    public override void Use(Player player) {  }
}

class PotionItem : Item
{
    public int healAmount = 5;
    public PotionItem()
    {
        Symbol = '!';
        displayName = "Potion";
        Uses = 2;
        Consumable = true;
    }
    public override void Use(Player player) { player.Heal(healAmount); Uses -= 1; }
}