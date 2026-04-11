class Pathfinding
{
    (int, int)[,] cameFrom;
    bool[,] visited;
    Queue<(int x, int y)> queue = new();

    public Pathfinding(int width, int length)
    {
        cameFrom = new (int, int)[length, width];
        visited = new bool[length, width];
    }
    public (int x, int y)? BFS(Enemy enemy, int playerX, int playerY, char[,] map, Func<int, int, Enemy?> isOccupied)
    {
        Array.Clear(cameFrom, 0, cameFrom.Length);
        Array.Clear(visited, 0, visited.Length);
        queue.Clear();
        (int dx, int dy)[] dirs = {(0,1), (0,-1), (1,0), (-1,0)};
        bool flag = true;
        visited[enemy.Y, enemy.X] = true;
        (int x, int y) pos = (enemy.X, enemy.Y);
        while (flag)
        {
            foreach(var (dx, dy) in dirs)
            {
                if(!Control.IsCanMove(pos.x + dx, pos.y + dy, map) || visited[pos.y + dy, pos.x + dx])
                {
                    continue;
                }
                else
                {
                    visited[pos.y + dy, pos.x + dx] = true;
                    cameFrom[pos.y + dy, pos.x + dx] = (pos.x, pos.y);
                    queue.Enqueue((pos.x + dx, pos.y + dy));
                }
            }
            if(queue.Count() == 0)
            {
                return null;
            }
            pos = queue.Dequeue();
            if(pos.x == playerX && pos.y == playerY)
            {
                flag = false;
            }
        }
        var lastPos = pos;
        while(pos != (enemy.X, enemy.Y))
        {
            lastPos = pos;
            pos = cameFrom[lastPos.y, lastPos.x];
        }
        if(isOccupied(lastPos.x, lastPos.y) == null)
        {
            return lastPos;
        }
        else
        {
            return null;
        }
    }
}