using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GridMap<T> where T : MonoBehaviour
{
    private int width;
    private int height;
    public int Width => width;
    public int Height => height;
    //一个方格的大小：
    private float cellSize;
    //存储cell的二维数组：
    private T[,] grid;
    //使用一个TextMeshPro数组，存储所有位置的TM P组件：
    private TextMeshPro[,] tmpGrid;
    //offset实际上就是我们grid的原点位置；
    private Vector3 offset;

    /// <summary>
    /// Grid的构造函数
    /// </summary>
    /// <param name="_width">Grid宽度</param>
    /// <param name="_height">Grid长度</param>
    /// <param name="_cellSize">每一个网格的大小</param>
    /// <param name="_offset">当前Grid的偏移量（默认为0）</param>
    /// <param name="initialCellObject">传入的委托参数，用于传递给外界内部的信息</param>
    public GridMap(int _width, int _height, float _cellSize, Vector3 _offset, Func<GridMap<T>, int, int, T> initialCellObject = null)
    {
        width = _width;
        height = _height;
        cellSize = _cellSize;
        offset= _offset;
        grid = new T[height, width];
        tmpGrid = new TextMeshPro[height, width];
        //初始化cell的数据；
        for(int i = 0; i < height; i++)
        {
            for(int j = 0; j < width; j++)
            {
                //问题来了，传入的类型都不知到会是什么，我们怎么知道应该初始化什么？
                //值类型还好说，可以用default；但是引用类型需要实例化，不能是null；
                //很简答。让外界实例化Grid的时候，传入一个自定义Func委托，利用返回值来初始化数据；
                //如果为默认值null，说明外界没有传入lambda表达式，说明是值类型；使用默认值；
                //利回调initialCellObject将当前的grid实例和cell的坐标传出去给外界；
                grid[i, j] = initialCellObject == null ? default(T) : initialCellObject(this, i, j);

            }

        }


        //测试：遍历二维数组，输出每一个cell的坐标信息：
        for(int i = 0; i < height; i++)
        {
            for(int j = 0; j < width; j++)
            {
                int x, y;
                GetGridIndex(GetWorldPosition(i ,j), out x, out y);
                Debug.Log($"x : {x}, y : {y}");
            }
           
        }
    }

    //实现一个重置当前cell中存储的对象的方法；value就是外界传入的新的cell实例；
    //设置一个委托参数，将如何设置text文本交给外部的数据结构类自己定义；
    public void SetObjectOfCell(Vector3 worldPosition, T value)
    {
        int x, y;
        GetGridIndex(worldPosition, out x, out y);
        if(x >= 0 && x < height && y >= 0 && y < width)
        {
            //坐标合法，进行值的设置：
            grid[x, y] = value;
            //为了让改变的值显示，我们同步调整对应的TMP数组；
            tmpGrid[x, y].text = grid[x, y].ToString();    
        }
    }

    //实现一个重置cell对象内部的value值显示，以及对应的text显示
    //需要外界自己进行文本的设置，同时自己确定cell实例内部数值的调整策略（是更改还是增量等等）；
    public void SetValueOfCellObject(Vector3 worldPosition, Action<T> setValueAction)
    {
        int x, y;
        GetGridIndex(worldPosition, out x, out y);
        if(x >= 0 && x < height && y >= 0 && y < width)
        {
            //为了让改变的值显示，我们同步调整对应的TMP数组；
            //使用回调将当前需要调整的文本对象传递出去，让外界去自定设置文本显示；
            setValueAction?.Invoke(GetObjectOfCell(worldPosition));          
        }
    }


    //实现一个SetValueOfCellObject的重载，
    //该重载是传入cell的下标，进行调整：
    public void SetValueOfCellObject(int x, int y, Action<T> setValueAction)
    {
        if(x >= 0 && x < height && y >= 0 && y < width)
        {
            //为了让改变的值显示，我们同步调整对应的TMP数组；
            //使用回调将当前需要调整的文本对象传递出去，让外界去自定设置文本显示；
            setValueAction?.Invoke(grid[x, y]);          
        }
    }

    //传入世界坐标系，获取对应位置cell的值：
    public T GetObjectOfCell(Vector3 worldPosition)
    {
        int x, y;
        GetGridIndex(worldPosition, out x, out y);
        if(x >= 0 && x < height && y >= 0 && y < width)
        {
            //坐标合法，返回对应值：
            return grid[x, y];
        }
        //如果没有cell，返回当前类型的默认值；
        else
            return default(T);
    }

    //实现一个GetObjectOfCell的重载，
    //该重载是传入cell的下标，返回实例：
    public T GetObjectOfCell(int x, int y)
    {
        if(x >= 0 && x < height && y >= 0 && y < width)
        {
            //坐标合法，返回对应值：
            return grid[x, y];
        }
        //如果没有cell，返回当前类型的默认值；
        else
            return default(T);
    }

    //一个获取世界坐标系的方法：传入当前cell坐标，返回Vector3；
    //注意：因为Unity中的x', y'（传入Vector3中的）和我们此处的二维数组的x y（访问grid）并不一样；
    //因此需要进行坐标上的转换，即x' = y; y' = -x;
    private Vector3 GetWorldPosition(int x, int y)
    {
        //             x' = y; y' = -x;
        return new Vector3(y, -x) * cellSize + offset;
    }

    //一个根据世界坐标系推出grid中的对应的x、y值的方法：
    //使用out关键字返回出x、y结果：

    private void GetGridIndex(Vector3 worldPosition, out int x, out int y)
    {
        //转换的规则本质同样也是Unity <-> grid之间的xy的规则；
        x = -Mathf.FloorToInt((worldPosition.y - offset.y) / cellSize);
        y = Mathf.FloorToInt((worldPosition.x - offset.x) / cellSize);
    }

    // private TextMeshPro CreateWorldText(string text, Transform parent, Vector3 localPosition, Color color, int fontSize = 4, int sortingOrder = 1)
    // {
    //     //创建带有TMP组建的GameObject
    //     GameObject gameObject = new GameObject("WorldText", typeof(TextMeshPro));
    //     Transform transform = gameObject.transform;
    //     transform.SetParent(parent, false);
    //     transform.localPosition = localPosition;
    //     TextMeshPro tmp = gameObject.GetComponent<TextMeshPro>();
    //     //设置字体文本、颜色、大小、对齐方式和显示层级；
    //     tmp.text = text;
    //     tmp.color = color;
    //     tmp.font = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
    //     tmp.fontSize = fontSize;
    //     tmp.alignment = TextAlignmentOptions.Center;
    //     tmp.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;
    //     return tmp;
    // }

}

