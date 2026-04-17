namespace RepHack;

public class Tile
{
    public bool isBlocked;
    public int distance;
}
class Pathfinding
{
    char[,] dungeonMap;
    (int, int)[,] cameFrom;
    bool[,] visited;
    Queue<(int x, int y)> queue = new();
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

    public Tile[,] Dijkstra(int playerX, int playerY, Func<int, int, Enemy?> isOccupied, bool isNextMap)
    {
        if(isNextMap){ ClearMap();}
        ClearDistances();
        queue.Clear();

        (int dx, int dy)[] dirs = {(0,1), (0,-1), (1,0), (-1,0)};

        while(queue.Count() > 0)
        {
            (int x, int y)pos = queue.Dequeue();
            for(int i = 0; i < 4; i++)
            {
                int x = pos.x;
            }
        }
        return map;
    }

    private void ClearMap()
    {
        for(int i = 0; i < map.GetLength(1); i++)
        {
            for(int j = 0; j < map.GetLength(0); j++)
            {
                map[i, j].isBlocked = dungeonMap[i, j] == '#';
                
            }
        }
    }
    private void ClearDistances()
    {
        for(int i = 0; i < map.GetLength(1); i++)
        {
            for(int j = 0; j < map.GetLength(0); j++)
            {
                map[i, j].distance = int.MaxValue;
            }
        }
    }
}