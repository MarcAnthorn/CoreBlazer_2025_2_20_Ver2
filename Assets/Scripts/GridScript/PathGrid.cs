using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private GridMap<PathGrid> myMap;

    //当前GridMap的原点（起始点）
    private Vector3 originalPoint;
    //当前地块的中心点
    private Vector3 centerPoint;
    //当前地块需要偏移的量：(留出间距给扁墙)
    private Vector3 offset; 
    private float cellSize;

    //一个额外的量，用于空出间隔填充扁墙空间：
    //目前是cellSize的四分之一：(也就是墙地块的宽度)
    private float intervalDistanceMutiplier = 0.25f;
    private float intervalDistance;


    Vector3 centerPosition;
    public void Init(GridMap<PathGrid> _map, int _xMap, int _yMap, Vector3 _originalPoint, float _cellSize)
    {
        myMap = _map;
        xMap = _xMap;
        yMap = _yMap;
        originalPoint = _originalPoint;
        cellSize = _cellSize;
        intervalDistance = intervalDistanceMutiplier * _cellSize;
    }


    public Vector3 GetWorldPosition()
    {     
        //注意：因为Unity中的x', y'（传入Vector3中的）和我们此处的二维数组的x y（访问grid）并不一样；
        //因此需要进行坐标上的转换，即x' = y; y' = -x;
        offset = new Vector3(yMap, -xMap) * intervalDistance;
        //此处返回的是地块对象的中心点
        centerPoint = new Vector3(yMap + cellSize / 2, -xMap - cellSize / 2) * cellSize + originalPoint + offset;
        return centerPoint;
    }

}
