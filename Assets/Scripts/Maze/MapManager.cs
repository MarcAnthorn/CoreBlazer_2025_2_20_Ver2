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
        AddMap();

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

    public void AddMap()                   //!!先设定一个地图，之后再想办法拓展!!
    {
        
        Map map0 = new Map()
        {
            row = 21,
            colume = 21
        };

        Map map2 = new Map()
        {
            row = 41,
            colume = 41
        };

        Maps.Add(0, map0);
        Maps.Add(2, map2);
        
    }

    public MapElement CreateMapElement(int elementId)
    {
        //如果是-1就跳过：
        if(elementId == -1)
            return null;
        MapElement element = null;
        switch (elementId)
        {
            case 10001:
            case 10003: //假墙
            case 10014: //特殊墙壁 
                element = new Wall(elementId);     //普通墙壁
                return element;

            case 10004: //起始点
            case 10005: //明亮灯塔
            case 10007: //传送点
            case 10008: //终点
            case 10009: //普通通路

            case 10010: //特殊的门A
            case 10011: //特殊的门（开启）A
            case 10012:
            case 10013:

            case 20010: //
            case 20020: //
            case 20030: // 

            //接下来的都是NPC事件，但是形式上也是PathGrid:
            case 30001:
            case 30002:
            case 30003:
            case 30004:
            case 30005:
                element = new Ground(elementId);  //起始点
                return element;
            default:
                Debug.LogError($"找不到Id为 {elementId} 的建筑类型，返回null");
                return null;
        }

    }


}
