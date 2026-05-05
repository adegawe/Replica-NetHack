namespace RepHack;
class EnemyRegistry
{
    private Dictionary<(int x, int y), Enemy> enemyMap = new();
    public int count => enemyMap.Count;
    public IEnumerable<Enemy> enemyList => enemyMap.Values;
    public void Add(Enemy enemy)
    {
        enemyMap[(enemy.X, enemy.Y)] = enemy;
        enemy.Moved += OnEnemyMoved;
    }
    private void OnEnemyMoved(Enemy enemy, int x, int y)
    {
        enemyMap.Remove((x, y));
        enemyMap[(enemy.X, enemy.Y)] = enemy;
    }
    public Enemy? IsOccupied(int X, int Y)
    {
        enemyMap.TryGetValue((X, Y), out var enemy);
        return enemy;
    }
    public void Clear()
    {
        foreach (Enemy enemy in enemyList)
        {
            enemy.Moved -= OnEnemyMoved;
        }
        enemyMap.Clear();
    }
    public void Remove(Enemy enemy)
    {
        enemyMap.Remove((enemy.X, enemy.Y));
    }
}