using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//寻路脚本，对外暴露唯一接口：public List<Vector3> FindPath(Vector3 monsterPosition, Vector3 targetPosition)
//获取当前玩家的位置，返回包含所有可达路径节点的Vector3信息，以供怪物使用进行移动；
//脚本通过GameLevelManager的gameLevelType，获取当前寻路的地图index，以获取当前的地图信息

//寻路脚本需要持有：
//1.所有地图的PathNode的信息(内部存在将坐标转换为Vector3的接口)；通过在Awake中初始化获取
//2.所有地图的坐标逻辑信息，也就是策划表中的信息；通过Awake中访问LoadManager获取并且保存引用；

//实现的目标：
//1.获取当前玩家的世界坐标位置，并且转化为对应地图的PathNode的index信息：playerIndex
//2.使用playerIndex，通过A* 获取路径index；
//3.通过GridMap的GetWorldPosition，将路径转化为在地图中的世界坐标信息；存储在List中；
//4.返回该List，供怪物对象使用；
public class PathFindingManager : Singleton<PathFindingManager>
{
    //当前的地图数量:4
    private int mapCount = 4;
    private float cellSize = 1;

    //一个额外的量，用于空出间隔填充扁墙空间：
    //目前是cellSize的四分之一：(也就是墙地块的宽度)
    private float intervalDistanceMutiplier = 0.25f;
    private float intervalDistance;
    private float basicOffset;
    //地图的GridMap信息：
    private Dictionary<int, PathNode[,]> dicGridMap = new Dictionary<int, PathNode[,]>();
    //地图的策划表格信息,通过访问LoadManager后，转换类型后保存；
    private Dictionary<int, MapElement[,]> dicElementMap = new Dictionary<int, MapElement[,]>();

    //当前的策划地图：
    //使用地图坐标可以转化为策划坐标；使用策划坐标进行寻路；
    private PathNode[,] nowPathNodeMap;

    private PriorityQueue<PathNode> openList;
    private List<PathNode> closedList;
    private List<PathNode> temPathList;
    private List<Vector3> pathList;
    private Vector3 offset;
    private int[] manhattanMove = new int[5]{-1, 0, 1, 0, -1};
    private bool isInitialized = false;

    protected override void Awake()
    {
        base.Awake();
        if (!isInitialized)
        {
            Init();
            isInitialized = true;
        }
    }
    private void Init()
    {
        intervalDistance = intervalDistanceMutiplier * cellSize;
        basicOffset = cellSize / 2;
        //偏移量：(0.402982 - cellsize / 2, -0.861083 + cellsize / 2,)
        offset = new Vector3(0.4029f - cellSize / 2, -0.861f + cellSize / 2);

        LoadOriginalMapElements();
        LoadWorldSpaceMapGrids();
    }
    private void TestF()
    {
        for (int i = 0; i < 10; i++)
        {
            Vector3 worldPosition = new Vector3(UnityEngine.Random.Range(0f, 10f), UnityEngine.Random.Range(-10f, 0f), 0);
            int x, y;
            GetGridIndex(worldPosition, out x, out y);
            Debug.LogWarning($"{worldPosition}的当前的坐标是：x:{x}, y:{y}");
        }
    }
    //初始化策划表格的地图信息；
    //其中，如果是路径，那么MapElement.flag == 0;
    //如果不是路径，那么MapElement.flag == 1;
    private void LoadOriginalMapElements()
    {
        dicElementMap.Add(0, LoadManager.Instance.mapTutorialFloor);
        dicElementMap.Add(1, LoadManager.Instance.mapFirstFloor);
        dicElementMap.Add(2, LoadManager.Instance.mapSecondFloor);
        dicElementMap.Add(3, LoadManager.Instance.mapThirdFloor);
        dicElementMap.Add(4, LoadManager.Instance.mapCentralFloor);
    }

    //初始化空间坐标系下的地图内容；
    //注意：此处初始化的只包含路径，而不是全部元素；
    //因此，从策划表中获取的index，需要经过转换才是该GridMap中对应的路径的index；
    private void LoadWorldSpaceMapGrids()
    {
        for (int mapIndex = 0; mapIndex < mapCount; mapIndex++)
        {
            int sizeX = MapManager.Instance[mapIndex].row;
            int sizeY = MapManager.Instance[mapIndex].colume;

            //注意：这个pathNodes是按照策划表格的规模存储的；
            //因此，大小完全和策划表格的大小一致；PathNode存储的坐标也是策划坐标下的；
            //策划坐标转地图坐标是：地图坐标 * 2 + 1 = 策划坐标 
            PathNode[,] pathNodes = new PathNode[sizeX, sizeY];

            for (int i = 0; i < sizeX; i++)
            {
                for (int j = 0; j < sizeY; j++)
                {
                    if (dicElementMap[mapIndex][i, j] == null)
                        continue;

                    //通过(i, j, offset, cellSize)信息就可以知道[i, j]的PathNode的世界坐标
                    //通过MapElement获取这个位置的地块的cost
                    //flag == 0是通路；不然就是墙壁；
                    pathNodes[i, j] = new PathNode(i, j, offset, cellSize, dicElementMap[mapIndex][i, j].flag);
                }
            }
            //将加载的内容加入字典：
            dicGridMap.Add(mapIndex, pathNodes);
        }
    }


    public List<Vector3> FindPath(Vector3 monsterPosition, Vector3 targetPosition)
    {
        if (!isInitialized)
        {
            Init();
            isInitialized = true;
        }
            
        if (pathList == null)
            pathList = new List<Vector3>();

        else
            pathList.Clear();
            
        //当前的地图：
        int mapIndex = (int)GameLevelManager.Instance.gameLevelType;
        nowPathNodeMap = dicGridMap[mapIndex];

        //获取目标点的坐标信息：
        //注意，获取的坐标直接就是策划的坐标下：
        int endX, endY, startX, startY;
        GetGridIndex(targetPosition, out endX, out endY);

        //获取当前怪物所处的坐标信息：
        GetGridIndex(monsterPosition, out startX, out startY);

        Debug.LogWarning($"Current Start position is {monsterPosition}, Index is:{startX}, {startY}\nCurrent End position is {targetPosition},Index is:{endX}, {endY}");
        //开始寻路进程：
        List<PathNode> nodeResultList = FindPathManhattan(startX, startY, endX, endY);

        //显示路径的连线：
        // LineRenderer lineRenderer = gameObject.GetComponent<LineRenderer>();
        // if (lineRenderer == null)
        // {
        //     lineRenderer = gameObject.AddComponent<LineRenderer>();
        // }
        // lineRenderer.positionCount = nodeResultList.Count;
        // lineRenderer.startWidth = 0.1f;
        // lineRenderer.endWidth = 0.1f;
        // lineRenderer.endColor = Color.red; 

        //将节点信息全部转化为世界坐标信息：
        foreach (var node in nodeResultList)
        {
            pathList.Add(node.myWorldPosition);
        }

        // for (int i = 0; i < pathList.Count - 1; i++)
        // {
        //     lineRenderer.SetPosition(i, pathList[i]);
        //     lineRenderer.SetPosition(i + 1, pathList[i + 1]);
        // }
        return pathList;

    }

    //声明寻路算法函数，传入源点坐标和目标点坐标，返回存储了路径节点的容器作为结果
    private List<PathNode> FindPathManhattan(int startX, int startY, int endX, int endY)
    {
        //获取当前起始点和终点的节点实例：
        PathNode startNode = nowPathNodeMap[startX, startY];
        PathNode endNode = nowPathNodeMap[endX, endY];
        int width = nowPathNodeMap.GetLength(0);
        int height = nowPathNodeMap.GetLength(1);

        //A*数据容器：
        //优先级队列，根据PathNode的cost进行降序排序；
        if(openList == null)
            openList = new PriorityQueue<PathNode>(Comparer<PathNode>.Create((a, b) => (int)(a.cost - b.cost)));

        //closedList实际上就是visited；
        if (closedList == null)
            closedList = new List<PathNode>();
        else
            closedList.Clear();

        //遍历所有的节点，初始化信息：
        for (int i = 0; i < nowPathNodeMap.GetLength(0); i++)
        {
            for (int j = 0; j < nowPathNodeMap.GetLength(1); j++)
            {
                PathNode curNode = nowPathNodeMap[i, j];
                curNode.distance = int.MaxValue;
                //初始化所有cost：
                curNode.CalCost();
                //初始化前驱节点：
                curNode.prevNode = null;

            }
        }
        //初始化源点的实际距离和预估距离：
        startNode.distance = 0;
        startNode.estimate = CalManhattan(startNode, endNode);
        startNode.CalCost();

        openList.Push(startNode);
        //A*进程：
        while (openList.Count > 0)
        {
            PathNode cur = openList.Pop();
            int x = cur.X;
            int y = cur.Y;
            if (closedList.Contains(cur))
                continue;
            //如果到达终点，返回路径列表：
            if (cur == endNode)
                return CalPath(endNode);

            cur.myWorldPosition = GetWorldPosition(x, y);
            closedList.Add(cur);

            //进行BFS：
            for (int i = 0; i < 4; i++)
            {
                int xn = x + manhattanMove[i];
                int yn = y + manhattanMove[i + 1];
                PathNode next;
                if (xn >= 0 && xn < width && yn >= 0 && yn < height && !closedList.Contains(nowPathNodeMap[xn, yn]) && nowPathNodeMap[xn, yn].flag != 1)
                {
                    next = nowPathNodeMap[xn, yn];
                    if (cur.distance + 1 < next.distance)
                    {
                        //更新所有相关信息；然后将next入堆：
                        next.distance = cur.distance + 1;
                        next.estimate = CalManhattan(next, endNode);
                        next.CalCost();
                        next.prevNode = cur;

                        openList.Push(next);
                    }
                }
            }
        }

        Debug.LogWarning("无可用路径！");
        return null;

    }

    private int CalManhattan(PathNode a, PathNode b)
    {
        int xDistance = Mathf.Abs(a.X - b.X);
        int yDistance = Mathf.Abs(a.Y - b.Y);
        return xDistance + yDistance;
    }

    //计算路径列表的方法：
    private List<PathNode> CalPath(PathNode endNode)
    {
        if (temPathList == null)
            temPathList = new List<PathNode>();
        else
            temPathList.Clear();

        temPathList.Add(endNode);

        PathNode cur = endNode;
        
        while (cur.prevNode != null)
        {
            temPathList.Add(cur.prevNode);
            cur = cur.prevNode;
        }

        //在返回之前，将逆向索引路径颠倒一下，变成正向索引：
        temPathList.Reverse();
        return temPathList;
    }

    //传入世界坐标下的坐标，获取该地块在策划坐标下的index：
    public void GetGridIndex(Vector3 worldPosition, out int x, out int y)
    {
        //首先，对当前所处的地块进行规整处理；
        //也就是：如果当前所处的地块位置不是地块中心，则将其规范为最近的合法地块中心位置：
        float rawY = (worldPosition.x - offset.x - basicOffset) * 2 / (cellSize + intervalDistance) + 1;
        float rawX = (-(worldPosition.y - offset.y) - basicOffset) * 2 / (cellSize + intervalDistance);
        Debug.Log($"raw index is{rawX}, {rawY}");
        
        int normalizedY = ((int)(rawY / 2) * 2) + 1;
        int normalizedX = (int)(rawX / 2) * 2 + 1;
        x = normalizedX;
        y = normalizedY;
    }

    //传入策划坐标下的index，获取地块在世界坐标下的坐标；
    private Vector3 GetWorldPosition(int xMap, int yMap)
    {
        //注意：因为Unity中的x', y'（传入Vector3中的）和我们此处的二维数组的x y（访问grid）并不一样；
        //因此需要进行坐标上的转换，即x' = y; y' = -x;
        Vector3 centerPoint;
        //intervalDistance == 0.25;
        //basicOffset == 0.5   
        //此处返回的是地块对象的中心点
        //offset是基于起始点的偏移；
        float centerX = yMap / 2 * (cellSize + intervalDistance) + basicOffset + offset.x;
        float centerY = -(xMap / 2 * (cellSize + intervalDistance) + basicOffset) + offset.y;
        centerPoint = new Vector3(centerX, centerY);
        return centerPoint;
    }

}



public class PathNode
{
    //策划表格中的坐标数据，从1开始；
    //y是列坐标；
    //x是行坐标
    //那么实际上通过xMap yMap也能访问策划的表信息了
    public int X => x;
    public int Y => y;

    public int x;
    public int y;
    public int flag;
    //代价：实际距离源点的曼哈顿距离；   
    public float distance;
    //代价：启发函数给出的预估距离：
    public float estimate;
    //代价总和，用于堆结构中的排序：
    public float cost;
    //节点自己坐标对应的地块的世界坐标：
    public Vector3 myWorldPosition;

    //一个节点引用，引用上一个连通当前节点的节点实例（也就是前驱节点）
    public PathNode prevNode;

    public void CalCost()
    {
        cost = distance + estimate;
    }

    //初始化的时候，使用的是策划坐标
    public PathNode(int _x, int _y, Vector3 _originalPoint, float _cellSize, int _flag)
    {
        x = _x;
        y = _y;
        flag = _flag;
    }

}


//优先级队列：
public class PriorityQueue<T>
{
    private const int InitialCapacity = 4;

        private T[] _arr;
        private int _lastItemIndex;
        private IComparer<T> _comparer;

        public PriorityQueue()
            : this(InitialCapacity, Comparer<T>.Default)
        {
        }

        public PriorityQueue(int capacity)
            : this(capacity, Comparer<T>.Default)
        {
        }

        public PriorityQueue(Comparison<T> comparison)
            : this(InitialCapacity, Comparer<T>.Create(comparison))
        {
        }

        public PriorityQueue(IComparer<T> comparer)
            : this(InitialCapacity, comparer)
        {
        }

        public PriorityQueue(int capacity, IComparer<T> comparer)
        {
            _arr = new T[capacity];
            _lastItemIndex = -1;
            _comparer = comparer;
        }

        public int Count
        {
            get
            {
                return _lastItemIndex + 1;
            }
        }

        public void Push(T item)
        {
            if (_lastItemIndex == _arr.Length - 1)
            {
                Resize();
            }

            _lastItemIndex++;
            _arr[_lastItemIndex] = item;

            HeapInsert(_lastItemIndex);
        }

        public T Pop()
        {
            if (_lastItemIndex == -1)
            {
                throw new InvalidOperationException("The heap is empty");
            }

            T removedItem = _arr[0];
            _arr[0] = _arr[_lastItemIndex];
            _lastItemIndex--;

            MinHeapifyDown(0);

            return removedItem;
        }

        public T Peek()
        {
            if (_lastItemIndex == -1)
            {
                throw new InvalidOperationException("The heap is empty");
            }

            return _arr[0];
        }

        public void Clear()
        {
            _lastItemIndex = -1;
        }

        //HeapInsert方法：
        private void HeapInsert(int index)
        {
            if (index == 0)
            {
                return;
            }

            int childIndex = index;
            int parentIndex = (index - 1) / 2;

            if (_comparer.Compare(_arr[childIndex], _arr[parentIndex]) < 0)
            {
                // swap the parent and the child
                T temp = _arr[childIndex];
                _arr[childIndex] = _arr[parentIndex];
                _arr[parentIndex] = temp;

                HeapInsert(parentIndex);
            }
        }


        //小根堆的Heapify：
        private void MinHeapifyDown(int index)
        {
            int leftChildIndex = index * 2 + 1;
            int rightChildIndex = index * 2 + 2;
            int smallestItemIndex = index; // The index of the parent

            if (leftChildIndex <= _lastItemIndex &&
                _comparer.Compare(_arr[leftChildIndex], _arr[smallestItemIndex]) < 0)
            {
                smallestItemIndex = leftChildIndex;
            }

            if (rightChildIndex <= _lastItemIndex &&
                _comparer.Compare(_arr[rightChildIndex], _arr[smallestItemIndex]) < 0)
            {
                smallestItemIndex = rightChildIndex;
            }

            if (smallestItemIndex != index)
            {
                // swap the parent with the smallest of the child items
                T temp = _arr[index];
                _arr[index] = _arr[smallestItemIndex];
                _arr[smallestItemIndex] = temp;

                MinHeapifyDown(smallestItemIndex);
            }
        }

        //扩容方法，防止出现数组容量不够的情况：
        private void Resize()
        {
            T[] newArr = new T[_arr.Length * 2];
            for (int i = 0; i < _arr.Length; i++)
            {
                newArr[i] = _arr[i];
            }

            _arr = newArr;
        }

}
