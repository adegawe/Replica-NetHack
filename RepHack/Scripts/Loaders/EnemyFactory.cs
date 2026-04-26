using RepHack;
static class EnemyFactory
{
    static Dictionary<string, Func<IEnemyBehavior>> behaviors;

    static EnemyFactory()
    {
        behaviors = new()
        {
            {"chase", () => new ChaseBehavior()},
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