namespace RepHack;
class AttackUpEffect : ActiveEffect, IItemEffect
{
    public void Apply(Player player, int value)
    {
        foreach(ActiveEffect effect in player.activeEffects)
        {
            if(effect == this) { return; }
        }
        player.AddEffect(this);
    }
    public override void OnApply(Player player)
    {
        
    }
}