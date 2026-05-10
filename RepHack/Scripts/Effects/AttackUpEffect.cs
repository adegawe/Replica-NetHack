namespace RepHack;
class AttackUpEffect : ActiveEffect
{
    public override void Apply(Player player, int value)
    {
        if(player.activeEffects.Contains(this)){ return; }
        var statModifier = new StatModifier(5, StatModType.Flat, this);
        player.stats[StatType.Attack].AddModifier(statModifier);
        player.AddEffect(this);
    }
}