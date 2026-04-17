namespace RepHack;

public class Tile
{
    public bool isBlocked;
    public int distance;
    public int cost;
}
class Pathfinding
{
    char[,] dungeonMap;
    (int, int)[,] cameFrom;
    bool[,] visited;
    Queue<(int x, int y)> queue = new();
    PriorityQueue<(int x, int y), int> prioQueue = new();
    Tile[,] map;

    public Pathfinding(int width, int length, char[,] dm)
    {
        cameFrom = new (int, int)[length, width];
        visited = new bool[length, width];
        map = new Tile[length, width];
        dungeonMap = dm;
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

    public Tile[,] Dijkstra(int playerX, int playerY, Func<int, int, Enemy?> isOccupied, Func<int, int, bool> isEnemyAt, bool isNextMap)
    {
        if(isNextMap){ ClearMap((x, y) => isEnemyAt(x, y));}
        ClearDistances();
        prioQueue.Clear();
        Array.Clear(visited, 0, visited.Length);

        (int dx, int dy)[] dirs = {(0,1), (0,-1), (1,0), (-1,0)};
        map[playerY, playerX].distance = 0;
        prioQueue.Enqueue((playerX, playerY), 0);
        while(prioQueue.Count > 0)
        {
            (int x, int y)pos = prioQueue.Dequeue();
            if(visited[pos.y, pos.x]) { continue; }
            visited[pos.y, pos.x] = true;

            foreach(var dir in dirs)
            {
                int x = pos.x + dir.dx;
                int y = pos.y + dir.dy;
                if(map[y, x].isBlocked || visited[y, x]){ continue; }

                int moveCost = map[y, x].cost;
                if(map[y, x].distance > map[pos.y, pos.x].distance + moveCost)
                {
                    map[y, x].distance = map[pos.y, pos.x].distance + moveCost;
                    prioQueue.Enqueue((x, y), map[pos.y, pos.x].distance + moveCost);
                }
            }
        }
        return map;
    }

    public (int x, int y) GetNextStep(Enemy enemy, Tile[,] map, Func<int, int, Enemy?> isOccupied)
    {
        (int dx, int dy)[] dirs = {(0,1), (0,-1), (1,0), (-1,0)};
        int minValue = map[enemy.X, enemy.Y].distance;
        (int x, int y) minPos = (enemy.X, enemy.Y);
        foreach(var dir in dirs)
        {
            int tempX = enemy.X + dir.dx;
            int tempY = enemy.Y + dir.dy;
            if (tempX < 0 || tempX >= map.GetLength(1) || 
            tempY < 0 || tempY >= map.GetLength(0)){ continue; }
            if(map[tempY, tempX].isBlocked) { continue; }
            if(isOccupied(tempX, tempY) != null){ continue; }
            if(map[tempY, tempX].distance < minValue)
            {
                minValue = map[tempY, tempX].distance;
                minPos = (tempX, tempY);
            }
        }
        return minPos;
    }

    private void ClearMap(Func<int, int, bool> isEnemyAt)
    {
        for(int i = 0; i < map.GetLength(0); i++)
        {
            for(int j = 0; j < map.GetLength(1); j++)
            {
                if (map[i, j] == null) 
                {
                    map[i, j] = new Tile();
                }
                map[i, j].isBlocked = dungeonMap[i, j] == '#';
                if (isEnemyAt(j, i))
                {
                    map[i, j].cost = 10;
                }
                else
                {
                    map[i, j].cost = 1;
                }
            }
        }
    }
    private void ClearDistances()
    {
        for(int i = 0; i < map.GetLength(0); i++)
        {
            for(int j = 0; j < map.GetLength(1); j++)
            {
                map[i, j].distance = int.MaxValue;
            }
        }
    }
}