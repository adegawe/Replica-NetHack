namespace RepHack;
public enum StatModType { Flat }
class StatModifier
{
    public readonly int Value;
    public readonly StatModType Type;
    public readonly object Source;
    public StatModifier(int value, StatModType type, object source)
    {
        Value = value;
        Type = type;
        Source = source;
    }
}