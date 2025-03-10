using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MapManager : Singleton<MapManager>
{
    public int[,] map1Index;
    public MapElement[,] map1;

    protected override void Awake()
    {
        base.Awake();

        LoadMapElements(1);
        InitMap1Elements(1);
    }

    void Update()
    {
        
    }

    private void LoadMapElements(int mapId)
    {
        map1Index = new int[40, 40];
        for (int i = 0; i < 100; i++)                           //�����ͼ��С����Ϊ100
        {
            for (int j = 0; map1Index[i, j] != -1; j++)         //��ʼ����ͼ�����еĵؿ�Id
            {
                map1Index[i, j] = -1;
            }
        }
        string path = null;
        if (mapId >= 1 && mapId <= 3)
        {
            path = Path.Combine(Application.dataPath, $"Resources/MapDatas/Map{mapId}.csv");
        }
        else
        {
            Debug.LogError($"������IdΪ {mapId} �ĵ�ͼ");
        }

        if (File.Exists(path))
        {
            string[] lines = File.ReadAllLines(path);       //�ָ�ÿһ�д���lines

            for (int i = 0; i < lines.Length; i++)          //�ӵ����п�ʼ����ÿһ�У���ø��е���Ϣ
            {
                string line = lines[i];
                string[] values = line.Split(',');          //��ÿһ�а��ն��ŷָ�

                if (int.Parse(values[0]) == mapId && values.Length >= 4 && int.Parse(values[2]) == 1)
                {
                    for (int j = 0; values[j] != null; j++)
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
                        else
                        {
                            Debug.LogError($"��ͼIdΪ {mapId} �ĵ�ͼ������");
                        }
                    }
                }
                else if (values.Length <= 4)    //�������������˳�
                {
                    break;
                }

            }
        }
        else
        {
            Debug.LogError($"�Ҳ���IdΪ {mapId} �ĵ�ͼ");
        }
    }

    public void InitMap1Elements(int mapId)                     //���趨һ����ͼ��֮������취��չ
    {
        map1 = new MapElement[40, 40];
        for (int i = 0; i < map1Index.GetLength(0); i++)        //map1Index.GetLength(0) ==> ����
        {
            for (int j = 0; j < map1Index.GetLength(1); j++)     //map1Index.GetLength(1) ==> ����
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
            case 1:
                element = new Wall();
                return element;
            case 2:
                element = new LightTower();
                return element;
            case 3:
                element = new Ground();
                return element;
            default:
                Debug.LogError($"�Ҳ���IdΪ {elementId} �Ľ������ͣ�����null");
                return null;
        }

    }

}
