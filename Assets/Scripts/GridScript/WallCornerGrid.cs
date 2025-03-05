using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallCornerGrid : MonoBehaviour
{
    //这个是在自己的GridMap当中的坐标，和策划表中的坐标不同；
    //坐标位置是：matrix[xMap][yMap]
    public int xMap;
    public int yMap;
    private GridMap<WallCornerGrid> myMap;

    //当前GridMap的原点（起始点）
    private Vector3 originalPoint;
    //当前地块的中心点
    private Vector3 centerPoint;
    //当前地块需要偏移的量：(留出间距给扁墙)
    private Vector3 offset; 
    private float cellSize;

    //wallSizeMutiplier * cellSize == 墙体地块的宽度（较短边的长度 wallSize）
    private float wallSizeMutiplier = 0.25f;
    private float wallSize;

    //一个额外的量，用于空出间隔填充通路地块：
    //目前是cellSize：(也就是通路地块的宽度)
    private float intervalDistanceMutiplier = 1;
    private float intervalDistance;


    public void Init(GridMap<WallCornerGrid> _map, int _xMap, int _yMap, Vector3 _originalPoint, float _cellSize)
    {
        myMap = _map;
        xMap = _xMap;
        yMap = _yMap;
        originalPoint = _originalPoint;
        cellSize = _cellSize;
        wallSize = wallSizeMutiplier * _cellSize;
        intervalDistance = intervalDistanceMutiplier * _cellSize;
    }

    public Vector3 GetWorldPosition()
    {
        float centerX;
        float centerY;

        float basicOffset = cellSize + wallSize / 2;

        centerX = basicOffset + yMap * (cellSize + wallSize) + originalPoint.x;
        centerY = -(basicOffset + xMap * (cellSize + wallSize)) + originalPoint.y;

        centerPoint = new Vector3(centerX, centerY);
        return centerPoint;
    }
}
