using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;


public class TestItem : MonoBehaviour
{

    void Awake()
    {
       var call =  EventHub.Instance;
       Debug.Log("Try add items in awake!");
    }
    void Start()
    {

        
// //--------------------测试用；强制调用单例，使其执行初始化------------------

//         var instance = LoadManager.Instance;
//         var instance2 = EventHub.Instance;
// //--------------------测试用；强制调用单例，使其执行初始化------------------

    
        // //测试用类，通过这个测试入口向背包中加入测试用道具：
        // List<int> itemIDs = new List<int>()
        // {
        //     101, 102, 103, 104,
        //     201, 202, 203, 204, 205, 206, 207, 208,
        //     301, 302, 303,
        //     401, 402, 403, 404,
        //     501, 502, 503, 504, 505, 506, 507, 508
        // };

        // Debug.Log("Try add items!");

        // //为了战斗，先注释Item的添加：（因为有一些道具会加防御，导致看不到对玩家伤害）
        // if(!ItemManager.Instance.isAdded)
        // {
        //     ItemManager.Instance.isAdded = true;
        //     //添加道具：
        //     foreach(int id in itemIDs)
        //     {
        //         //表格更改之后，部分道具被删除了；因此添加之前，先检查是否存在对应的key：
        //         if(LoadManager.Instance.allItems.ContainsKey(id))
        //             ItemManager.Instance.AddItem(id);
        //     }

        // }



 
    
        // ItemManager.Instance.AddItem(510);
        // ItemManager.Instance.AddItem(101);

         // 获取所有道具id


        //测试破墙：
        // ItemManager.Instance.AddItem(601);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.C))
        {
                ItemManager.Instance.AddItem(
101, 102, 103, 104, 301, 302, 303, 401, 402, 403, 501, 502, 503, 504, 505, 506, 507, 508, 509, 510, 511, 512, 513, 514, 515, 516, 601, 602, 603, 604, 605, 606, 607, 608, 609, 610, 611, 612, 613, 614, 615, 616, 617, 618, 619, 620
);
        }
    }
}
