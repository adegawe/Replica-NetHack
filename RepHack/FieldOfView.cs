namespace RepHack;
class FOV
{
    int[,] octants = {
        { 1, 0, 0, 1 },
        { 0, 1, 1, 0 },
        { 0, -1, 1, 0 },
        { -1, 0, 0, 1 },
        { -1, 0, 0, -1 },
        { 0, -1, -1, 0 },
        { 0, 1, -1, 0 },
        { 1, 0, 0, -1 },
    };
    int xx, xy, yx, yy;
    public bool[,] isVisible {get; private set;}
    private char[,] map;
    private int playerX, playerY, fovLength;

    public FOV(int width, int length, char[,] dungeonMap)
    {
        isVisible = new bool[length, width];
        map = dungeonMap;
    }

    private void CastLight(int distance, float startSlope, float endSlope)
    {
        if(distance >= fovLength){ return; }
        if(distance == 0){ return; }

        bool wasWall = false;
        float currentStartSlope = startSlope;

        for(int i = 0; i <= distance; i++)
        {
            float leftSlope = (i-0.5f) / distance;
            float rightSlope = (i+0.5f) / distance;
            bool isWall = false;
            if(leftSlope > endSlope || rightSlope < startSlope){ continue; }
            else
            {
                int x = playerX + distance * xx + i * xy;
                int y = playerY + distance * yx + i * yy;
                if(x >= 0 && y >= 0 && 
                   x < isVisible.GetLength(1) && y < isVisible.GetLength(0))
                {
                    isVisible[y, x] = true;
                    isWall = map[y, x] == '#';
                    if(!wasWall && isWall)
                    {
                        CastLight(distance + 1, currentStartSlope, leftSlope);
                    }
                    else if(wasWall && !isWall)
                    {
                        currentStartSlope = rightSlope;
                    }
                }
            }
            wasWall = isWall;
        }
        if (!wasWall)
        {
            CastLight(distance + 1, currentStartSlope, endSlope);
        }
    }

    public void ComputeFOV(int x, int y, int fovLen)
    {
        Array.Clear(isVisible, 0, isVisible.Length);
        playerX = x;
        playerY = y;
        fovLength = fovLen;
        for(int i = 0; i < 8; i++)
        {
            xx = octants[i, 0]; xy = octants[i, 1];
            yx = octants[i, 2]; yy = octants[i, 3];
            CastLight(1, 0.0f, 1.0f);
        }
    }
}