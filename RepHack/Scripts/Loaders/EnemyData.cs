using RepHack;
class EnemyData
{
    public string Name { get; set; } = "Unknown";
    public int Hp { get; set; } = 10;
    public int Attack { get; set; } = 1;
    public string Symbol { get; set; } = "?";
    public string Color { get; set; } = "White";
    public string Behavior { get; set; } = "chase";
    public int MinFloor { get; set; } = 1;
    public int Weight { get; set; } = 1;
    public bool IsBoss { get; set; } = false;
}