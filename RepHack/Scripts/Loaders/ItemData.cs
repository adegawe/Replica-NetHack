namespace RepHack;
class ItemData
{
    public string Name { get; set; } = "Unknown";
    public string Symbol { get; set; } = "?";
    public string Color { get; set; } = "White";
    public string Category { get; set; } = "Potion";
    public string EffectType { get; set; } = "heal";
    public int EffectValue { get; set; } = 1;
    public int MinFloor { get; set; } = 1;
    public int Weight { get; set; } = 1;
    public int Uses { get; set; } = 1;
    public int RemainingTime { get; set; }
    public bool Consumable { get; set; } = false;
}