using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallGrid : MonoBehaviour
{
    //这个是在自己的GridMap当中的坐标，和策划表中的坐标不同；
    //坐标位置是：matrix[xMap][yMap]
    public int xMap;
    public int yMap;
    private GridMap<WallGrid> myMap;

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

    public void Init(GridMap<WallGrid> _map, int _xMap, int _yMap, Vector3 _originalPoint, float _cellSize)
    {
        myMap = _map;
        xMap = _xMap;
        yMap = _yMap;
        originalPoint = _originalPoint;
        cellSize = _cellSize;
        wallSize = wallSizeMutiplier * _cellSize;
        intervalDistance = intervalDistanceMutiplier * _cellSize;
    }




    //计算当前墙体地块是竖直的还是
    //返回true是竖直，不然就是水平的；关系的加载资源的不同
    public bool JudgeVertical()
    {
        //根据行index判断：
        if(xMap % 2 == 0)
        {
            return true;
        }
        return false;
    }

    public Vector3 GetWorldPosition()
    {
        //当前墙体在「世界坐标下」，中心点的x、y值：
        //别忘了，计算世界坐标x用的是yMap;
        //计算世界坐标y用的是-xMap；
        float centerX;
        float centerY;


        //如果当前墙体是竖直，那么世界坐标的y值，其实和自己同行的通行地块世界坐标的y值是一样的；
       
        if(JudgeVertical())
        {
            float basicOffset1 = intervalDistance + wallSize / 2;
            centerX = yMap * (intervalDistance + wallSize) + originalPoint.x + basicOffset1;
            float basicOffset2 = intervalDistance / 2;
            centerY = -((xMap / 2) * (intervalDistance + wallSize) + basicOffset2) + originalPoint.y;
        }

        else
        {
            float basicOffset1 = intervalDistance / 2;
            centerX = yMap * (intervalDistance + wallSize) + basicOffset1 + originalPoint.x;

            float basicOffset2 = intervalDistance + wallSize / 2;
            centerY = -((xMap / 2) * (intervalDistance + wallSize) + basicOffset2) + originalPoint.y; 
        }

        centerPoint = new Vector3(centerX, centerY);
        return centerPoint;

    }
}
