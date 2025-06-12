using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PathGrid : MonoBehaviour
{
    //策划表格中的坐标数据，从1开始；
    //y是列坐标；
    //x是行坐标

    //计算得出两个坐标之间的关系是：
    // x / 2 - 1 == xMap;
    // y / 2 - 1 == yMap;
    //那么实际上通过xMap yMap也能访问策划的表信息了
    public int x;
    public int y;

    //这个是在自己的GridMap当中的坐标，和策划表中的坐标不同；
    //坐标位置是：matrix[xMap][yMap]
    public int xMap;
    public int yMap;
    //当前地块的id：
    public int id;
    private GridMap<PathGrid> myMap;

    //当前GridMap的原点（起始点）
    private Vector3 originalPoint;
    //当前地块的中心点
    private Vector3 centerPoint;
    //当前地块需要偏移的量：(留出间距给扁墙)
    private Vector3 offset; 
    private float basicOffset;
    private float cellSize;

    //一个额外的量，用于空出间隔填充扁墙空间：
    //目前是cellSize的四分之一：(也就是墙地块的宽度)
    private float intervalDistanceMutiplier = 0.25f;
    private float intervalDistance;


    Vector3 centerPosition;
    public void Init(GridMap<PathGrid> _map, int _xMap, int _yMap, Vector3 _originalPoint, float _cellSize, int _id = -1)
    {
        myMap = _map;
        xMap = _xMap;
        yMap = _yMap;
        originalPoint = _originalPoint;
        cellSize = _cellSize;
        intervalDistance = intervalDistanceMutiplier * _cellSize;
        id = _id;
    }


    public Vector3 GetWorldPosition()
    {     
        //注意：因为Unity中的x', y'（传入Vector3中的）和我们此处的二维数组的x y（访问grid）并不一样；
        //因此需要进行坐标上的转换，即x' = y; y' = -x;

        // offset = new Vector3(yMap, -xMap) * intervalDistance;
        basicOffset = cellSize / 2;
        //此处返回的是地块对象的中心点
        float centerX = yMap * (cellSize + intervalDistance) + basicOffset + originalPoint.x;
        float centerY = -(xMap * (cellSize + intervalDistance) + basicOffset) + originalPoint.y;
        centerPoint = new Vector3(centerX, centerY);
        return centerPoint;
    }

}
