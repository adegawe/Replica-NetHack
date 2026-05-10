namespace RepHack;
class PoisonEffect : ActiveEffect
{
    private Player player;
    private int value;
    public override void Apply(Player player, int value)
    {
        this.player = player;
        this.value = value;
        player.AddEffect(this);
    }
    public override void OnTick()
    {
        player.TakeDamage(this.value);
    }
}