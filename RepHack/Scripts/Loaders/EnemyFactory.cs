namespace RepHack;
static class EnemyFactory
{
    static Dictionary<string, Func<IEnemyBehavior>> behaviors;

    static EnemyFactory()
    {
        behaviors = new()
        {
            {"chase", () => new ChaseBehavior()},
            {"random_chase", () => new RandomChaseBehavior()},
            {"chase_close_buff", () => new ChaseCloseBuffBehavior()},
            {"chase_drain", () => new DrainOnHitChaseBehavior()},
            {"chase_ranged_warn", () => new WarnRangedBehavior()},
            {"chase_ranged_instant", () => new InstantRangedBehavior()},
        };
    }

    public static Enemy Create(EnemyData data)
    {
        Enemy enemy;
        if(behaviors.TryGetValue(data.Behavior, out Func<IEnemyBehavior>? behavior))
        {
            enemy = new Enemy(data, behavior());
        }
        else
        {
            enemy = new Enemy(data, behaviors["chase"]());
        }
        return enemy;
    }
}