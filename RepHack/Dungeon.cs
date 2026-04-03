struct Room
{
    public int x, y;
    public int width, length;

    public int CenterX => x + width / 2;
    public int CenterY => y + length / 2;
}

class Node
{
    public int x, y;
    public int width, length;
    public Node? leftChild;
    public Node? rightChild;
    public Room? room;

    public Node(int x, int y, int width, int length)
    {
        this.x = x;
        this.y = y;
        this.width = width;
        this.length = length;
    }
    public int RoomX => room!.Value.x;
    public int RoomY => room!.Value.y;
    public int RoomWidth => room!.Value.width;
    public int RoomLength => room!.Value.length;

    public int RoomCenterX => room!.Value.CenterX;
    public int RoomCenterY => room!.Value.CenterY;
}

class Dungeon
{
    public char[,] map;
    public List<Node> roomList = new List<Node>();
    public int width, length;
    private int minWidth = 12;
    private int minLength = 8;
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
        Node node = new(x : 0, y : 0, width : this.width, length : this.length);
        BSP(node);
        int roomNum = Math.Min(random.Next(5, 7), roomList.Count());
        int count = 0;
        foreach(Node tempNode in roomList)
        {
            if(count > roomNum){
                break;
            }
            CreateRoom(tempNode);
            count++;
        }
        for(int i = 0; i < roomNum - 1; i++)
        {
            if(roomList[i].room != null){
                CreateCorridor(roomList[i].RoomCenterX, roomList[i].RoomCenterY, roomList[i+1].RoomCenterX, roomList[i+1].RoomCenterY);
            }
        }
        map[roomList[roomNum - 1].RoomCenterY, roomList[roomNum - 1].RoomCenterX] = '>';
    }

    public void CreateRoom(Node node)
    {
        int roomWidth = random.Next(minWidth/3, node.width-4);
        int roomLength = random.Next(minLength/2, node.length-2);
        int x = node.x + random.Next(1, node.width - roomWidth);
        int y = node.y + random.Next(1, node.length - roomLength);
        node.room = new Room { x = x, y = y, width = roomWidth, length = roomLength};

        for(int i = y; i < y + roomLength; i++)
        {
            for(int j = x; j < x + roomWidth; j++)
            {
                map[i, j] = '.';
            }
        }
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

    public void BSP(Node parent)
    {
        if(parent.width < minWidth * 2|| parent.length < minLength * 2)
        {
            roomList.Add(parent);
            return;
        }

        int randomWidth = random.Next(minWidth , parent.width-minWidth);
        int randomLength = random.Next(minLength , parent.length-minLength);
        bool splitWidth;
        
        if((float)parent.width/parent.length > 1.5)
        {
            splitWidth = true;
        }
        else if((float)parent.length/parent.width > 1.5)
        {
            splitWidth = false;
        }
        else
        {
            splitWidth = random.Next(0, 2) == 0;
        }
        if (splitWidth)
        {
            parent.leftChild = new Node(parent.x, parent.y, randomWidth, parent.length);
            parent.rightChild = new Node(parent.x + randomWidth, parent.y, parent.width - randomWidth, parent.length);
            BSP(parent.leftChild);
            BSP(parent.rightChild);
        }
        else
        {
            parent.leftChild = new Node(parent.x, parent.y, parent.width, randomLength);
            parent.rightChild = new Node(parent.x, parent.y + randomLength, parent.width, parent.length - randomLength);
            BSP(parent.leftChild);
            BSP(parent.rightChild);
        }
    }
}