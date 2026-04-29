using RepHack;
interface IRangedEnemy : IEnemyBehavior
{
    List<(int x, int y)> AttackLine { get; }
}