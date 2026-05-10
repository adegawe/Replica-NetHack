namespace RepHack;
class Stat
{
    public int BaseValue { get; init; }
    public int Value => BaseValue + statModifiers.Sum(m => m.Value);
    private readonly List<StatModifier> statModifiers = new();
    public void AddModifier(StatModifier modifier)
    {
        statModifiers.Add(modifier);
    }
    public void RemoveAllFromSource(object source)
    {
        statModifiers.RemoveAll(m => m.Source == source);
    }
}