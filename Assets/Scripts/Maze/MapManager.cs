using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class MapManager : Singleton<MapManager>
{
    public Dictionary<int, Map> Maps = new Dictionary<int, Map>();

    public int[,] map1Index;
    public MapElement[,] map1;

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

    private void LoadMapElements(int mapId)
    {
        map1Index = new int[41, 41];
        for (int i = 0; i < MapManager.Instance[mapId].row; i++)              //根据地图长宽来进行打印
        {
            for (int j = 0; j < MapManager.Instance[mapId].colume; j++)         //初始化地图上所有的地块Id
            {
                map1Index[i, j] = -1;
            }
        }
        string path = null;
        if (mapId >= 1 && mapId <= 3)
        {
            path = Path.Combine(Application.dataPath, $"Resources/MapDatas/Map{mapId}.csv");    //命名规范！！！

        }
        else
        {
            Debug.LogError($"不存在Id为 {mapId} 的地图");
            return;
        }

        if (File.Exists(path))
        {
            string[] lines = File.ReadAllLines(path);       //分割每一行存入lines

            for (int i = 0; i < lines.Length; i++)          //从第四行开始遍历每一行，获得各列的信息
            {
                string line = lines[i];
                string[] values = line.Split(',');          //将每一列按照逗号分割

                if (values.Length >= 4)
                {
                    for (int j = 0; j < MapManager.Instance[mapId].colume; j++)
                    {
                        if (mapId == 1)
                        {
                            map1Index[i, j] = int.Parse(values[j]);
                        }
                        else if (mapId == 2)
                        {

                        }
                        else if (mapId == 3)
                        {

                        }
                        //else
                        //{
                        //    Debug.LogError($"地图Id为 {mapId} 的地图不存在");
                        //}
                    }
                }
                else if (values.Length <= 4)    //遇到空行主动退出
                {
                    break;
                }

            }
        }
        else
        {
            Debug.LogError($"找不到Id为 {mapId} 的地图");          //表示没有在路径中找到该文件
        }
    }

    public void InitMap1Elements(int mapId)                     //!!先设定一个地图，之后再想办法拓展!!
    {
        map1 = new MapElement[41, 41];
        for (int i = 0; i < map1Index.GetLength(0); i++)        //map1Index.GetLength(0) ==> 行数
        {
            for (int j = 0; j < map1Index.GetLength(1); j++)     //map1Index.GetLength(1) ==> 列数
            {
                map1[i, j] = CreateMapElement(map1Index[i, j]);
            }
        }

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
        LoadMapElements(1);
        InitMap1Elements(1);
        // for (int i = 0; i < map1Index.GetLength(0); i++)        //map1Index.GetLength(0) ==> 行数
        // {
        //     for (int j = 0; j < map1Index.GetLength(1); j++)     //map1Index.GetLength(1) ==> 列数
        //     {
        //         Debug.Log($"【{i},{j}】==> value: {map1Index[i, j]}");
        //     }
        // }
    }

}
