struct Room
{
    public int x, y;
    public int width, length;

    public int CenterX => x + width / 2;
    public int CenterY => y + length / 2;
    public bool isActive;
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
    
    public bool isActive = false;
}

class Dungeon
{
    public int roomNum;
    public char[,] map;
    public List<Node> roomList = new List<Node>();
    public int width, length;
    private int minWidth = 12;
    private int minLength = 7;
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
        roomNum = Math.Min(random.Next(5, 7), roomList.Count);
        int count = 0;
        foreach(Node tempNode in roomList)
        {
            CreateRoom(tempNode);
            count++;
        }
        var selectedNodes = roomList.OrderBy(x => random.Next()).Take(roomNum).ToList();
        foreach (Node nod in selectedNodes)
        {
            nod.isActive = true;
            for(int i = nod.RoomX; i < nod.RoomX + nod.RoomWidth; i++)
            {
                for(int j = nod.RoomY; j < nod.RoomY + nod.RoomLength; j++)
                {
                    map[j, i] = '.';
                }
            }
        }
        ConnectRoom(node);
        var lastNode = selectedNodes[^1];
        map[lastNode.RoomCenterY, lastNode.RoomCenterX] = '>';
    }

    public void CreateRoom(Node node)
    {
        int roomWidth, roomLength;
        do {
            roomWidth = random.Next(minWidth/2, node.width-4);
            roomLength = random.Next(minLength/2, node.length-2);
        } while(roomWidth * 3 < roomLength);

        int x = node.x + random.Next(1, node.width - roomWidth);
        int y = node.y + random.Next(1, node.length - roomLength);
        node.room = new Room { x = x, y = y, width = roomWidth, length = roomLength};
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

    public Room? ConnectRoom(Node node)
    {
        Room? room = node.room;
        if(node.isActive)
        {
            return node.room;
        }
        if(node.leftChild == null && node.rightChild == null)
        {
            return null;
        }
        
        Room? leftRoom = ConnectRoom(node.leftChild!);
        Room? rightRoom = ConnectRoom(node.rightChild!);
        if(leftRoom.HasValue && rightRoom.HasValue)
        {
            CreateCorridor(leftRoom.Value.CenterX, leftRoom.Value.CenterY, rightRoom.Value.CenterX, rightRoom.Value.CenterY);
        }

        return leftRoom ?? rightRoom;
    }
}