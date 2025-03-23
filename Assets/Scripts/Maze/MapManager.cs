using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class MapManager : Singleton<MapManager>
{
    public Dictionary<int, Map> Maps = new Dictionary<int, Map>();

    protected override void Awake()
    {
        base.Awake();

        //LoadMapElements(1);
        //InitMap1Elements(1);
        AddMap(1);
        TestMapLoading();
    }

    void Update()
    {
        
    }

    public Map this[int key]            //索引器
    {
        get
        {
            return this.Maps[key];
        }
    }

    public void AddMap(int mapId)                   //!!先设定一个地图，之后再想办法拓展!!
    {
        Map map1 = new Map()
        {
            row = 41,
            colume = 41
        };
        Maps.Add(mapId, map1);
    }

    public MapElement CreateMapElement(int elementId)
    {
        MapElement element = null;
        switch (elementId)
        {
            case 10001:
                element = new Wall(elementId);     //普通墙壁
                return element;

            case 10003: //起始点
            case 10005: //明亮灯塔
            case 10007: //传送点
            case 10009: //普通通路
            case 20010: //
            case 20020: //
            case 20030: // 
                element = new Ground(elementId);  //起始点
                return element;
            default:
                Debug.LogError($"找不到Id为 {elementId} 的建筑类型，返回null");
                return null;
        }

    }

    public void TestMapLoading()
    {
        //LoadMapElements(1);
        //InitMap1Elements(1);
        // for (int i = 0; i < map1Index.GetLength(0); i++)        //map1Index.GetLength(0) ==> 行数
        // {
        //     for (int j = 0; j < map1Index.GetLength(1); j++)     //map1Index.GetLength(1) ==> 列数
        //     {
        //         Debug.Log($"【{i},{j}】==> value: {map1Index[i, j]}");
        //     }
        // }
    }

}
