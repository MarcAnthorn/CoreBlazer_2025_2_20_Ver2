using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;


public class TestItem : MonoBehaviour
{
    
    void Start()
    {
//--------------------测试用；强制调用单例，使其执行初始化------------------

        var instance = LoadManager.Instance;
//--------------------测试用；强制调用单例，使其执行初始化------------------

        //测试用：初始化玩家的SceneIndex:
        PlayerManager.Instance.playerSceneIndex = E_PlayerSceneIndex.Battle;

        //测试用类，通过这个测试入口向背包中加入测试用道具：
        List<int> itemIDs = new List<int>()
        {
            101, 102, 103, 104,
            201, 202, 203, 204, 205, 206, 207, 208,
            301, 302, 303,
            401, 402, 403, 404,
            501, 502, 503, 504, 505, 506, 507, 508
        };


        // // 2. 初始化结果列表，每个至少一次
        // List<int> result = new List<int>(itemIDs);

        // // 3. 为每个ID再随机添加1~3次（因为已经有一次了，总次数最多是4）
        // foreach (int id in itemIDs)
        // {
        //     int extra = UnityEngine.Random.Range(0, 1); // 再额外出现0~2次，总共最多4次
        //     for (int i = 0; i < extra; i++)
        //     {
        //         result.Add(id);
        //     }
        // }

        //添加道具：
        foreach(int id in itemIDs)
        {
            ItemManager.Instance.AddItem(id);
        }

        // foreach(var item in ItemManager.Instance.inventory.itemCountDic.Keys)
        // {
        //     Debug.Log($"当前的Item是：{item.id}, 持有的数量是:{ItemManager.Instance.inventory.itemCountDic[item]}");
        // }


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
