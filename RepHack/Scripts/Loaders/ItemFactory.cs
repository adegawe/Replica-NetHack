namespace RepHack;
static class ItemFactory
{
    static Dictionary<string, Func<IItemEffect>> effects;

    static ItemFactory()
    {
        effects = new()
        {
            {"water", () => new BaseEffect()},
            {"heal", () => new HealEffect()},
            {"attackBonus", () => new AttackUpEffect()},
            {"defenseBonus", () => new DefenseUpEffect()},
            {"dizzy", () => new DizzyEffect()},
            {"poison", () => new PoisonEffect()},
        };
    }

    public static Item Create(ItemData data)
    {
        Item item;
        if(effects.TryGetValue(data.EffectType, out Func<IItemEffect>? effect))
        {
            item = new Item(data, effect());
        }
        else
        {
            item = new Item(data, effects["water"]());
        }
        return item;
    }
}