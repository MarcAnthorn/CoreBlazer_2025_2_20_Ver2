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

        Map map1 = new Map()
        {
            row = 31,
            colume = 31
        };

        Map map2 = new Map()
        {
            row = 41,
            colume = 41
        };

        Map map3 = new Map()
        {
            row = 43,
            colume = 55
        };

        Maps.Add(0, map0);
        Maps.Add(1, map1);
        Maps.Add(2, map2);
        Maps.Add(3, map3);    
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
                element = new Wall(elementId, 1);     //普通墙壁
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

            case 10015: //回收商品的NPC：
            case 20010: //poi事件
            case 20020: //
            case 20030: // 

//--------------以下是墙中鼠-----------------------
            case 20016: 
            case 20017:
            case 20018:
            case 20019:
            case 20025:
            case 20021:
//----------------------------------------------------

//--------------宝箱-----------------------
            case 20022:
            case 20023:
            case 20024:
            case 20026:

            //boss战宝箱：
            case 50007:

//----------------------------------------------------

            //新手关卡：
            case 30001:
            case 30002:
            case 30003:
            case 30004:
            case 30005:

//-------------第二层支线-------------------------------
            case 30016:
            case 30017:
            case 30018:

//-------------第三层支线-------------------------------
            case 40016:
            case 40017:



            case 50001: //塔罗牌点位

//----------战斗事件-------------------------------
            case 50002:
            case 50003:

            case 50004: //第一层boss点
            case 50005: //第二层boss点
            case 50006: //第三层boss点
                element = new Ground(elementId, 0);  //起始点
                return element;
            default:
                Debug.LogError($"找不到Id为 {elementId} 的建筑类型，返回null");
                return null;
        }

    }


}
