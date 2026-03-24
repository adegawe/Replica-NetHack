struct Room
{
    public int x, y;
    public int width, length;

    public int CenterX => x + width / 2;
    public int CenterY => y + length / 2;
}

class Dungeon
{
    public char[,] map;

    public List<Room> roomList = new List<Room>();

    public int width, length;

    readonly Random random = new();

    public Dungeon()
    {
        width = 80;
        length = 30;

        map = new char[length, width];
    }

    public void InitDungeon()
    {
        for(int i = 0; i < length; i++)
        {
            for(int j = 0; j < width; j++)
            {
                map[i, j] = '#';
            }
        }
        int roomNum = random.Next(5, 7);
        CreateRoom(roomNum);
        roomList.Sort((a, b) => a.x.CompareTo(b.x));
        for(int i = 0; i < roomNum - 1; i++)
        {
            CreateCorridor(roomList[i].CenterX, roomList[i].CenterY, roomList[i+1].CenterX, roomList[i+1].CenterY);
        }
        map[roomList[roomNum - 1].CenterY, roomList[roomNum - 1].CenterX] = '>';
    }

    public void CreateRoom(int roomCount)
    {
        int tryNum = 0;
        while(roomCount > 0 && tryNum < 100){
            int x = random.Next(1, 63);
            int y = random.Next(1, 20);
            int roomWidth = random.Next(10, 15);
            int roomLength = random.Next(5, 8);

            if(!IsCanBuild(x, y, roomWidth, roomLength))
            {
                tryNum++;
                continue;
            }

            for(int i = y; i < y + roomLength; i++)
            {
                for(int j = x; j < x + roomWidth; j++)
                {
                    map[i, j] = '.';
                }
            }
            Room room = new Room
            {
                x = x,
                y = y,
                width = roomWidth,
                length = roomLength
            };
            roomList.Add(room);
            roomCount--;
            }
    }
    
    public bool IsCanBuild(int x, int y, int newWidth, int newLength)
    {
        foreach(Room room in roomList)
        {
            if(room.x + room.width - 1 > x && room.x < x + newWidth + 1 && room.y + room.length - 1 > y &&  room.y < y + newLength + 1)
            {
                return false;
            }
        }
        return true;
    }

    public void CreateCorridor(int Ax, int Ay, int Bx, int By)
    {
        int start = Math.Min(Ax, Bx);
        int end = Math.Max(Ax, Bx);
        for(int i = start; i <= end; i++)
        {
            map[Ay, i] = '.';
        }

        start = Math.Min(Ay, By);
        end = Math.Max(Ay, By);

        for(int j = start; j <= end; j++)
        {
            map[j, Bx] = '.';
        }
    }
}