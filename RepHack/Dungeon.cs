struct Room
{
    public int x, y;
    public int width, length;
}

class Dungeon
{
    public char[,] map;

    public List<Room> roomList = new List<Room>();

    public int width, length;

    readonly Random random = new();

    public Dungeon()
    {
        width = 100;
        length = 100;

        map = new char[width, length];
    }

    public void InitDungeon()
    {
        
    }

    public void CreateRoom(int roomCount)
    {
        while(roomCount > 0){
            int x = random.Next(0, 70);
            int y = random.Next(0, 70);
            int roomWidth = random.Next(15, 30);
            int roomLength = random.Next(15, 30);
            bool flag = true;

            foreach (Room room in roomList)
            {
                if(!IsCanBuild(x, y))
                {
                    flag = false;
                    break;
                }
            }
            if (flag)
            {
                for(int i = x; i < x + roomWidth; i++)
                {
                    for(int j = y; j < y + roomLength; j++)
                    {
                        map[i, j] = '.';
                    }
                }
                Room room = new Room();
                room.x = x;
                room.y = y;
                room.width = width;
                room.length = length;
                roomList.Add(room);
                roomCount--;
            }
        }
    }
    
    public bool IsCanBuild(int x, int y)
    {
        foreach(Room room in roomList)
        {
            if(room.x + room.width > x || room.y + room.length)
            {
                return true;
            }
        }
        return false;
    }

    public void CreateCorridor(int Ax, int Ay, int Bx, int By)
    {
        
    }
}