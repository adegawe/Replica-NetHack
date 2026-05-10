namespace RepHack;
class BaseEffect : IItemEffect
{
    public void Apply(Player player, int value)
    {
        player.Heal(1);
    }
}