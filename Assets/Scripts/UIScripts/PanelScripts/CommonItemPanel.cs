using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommonItemPanel : ItemPanel
{

    public ScrollRect srCommonItemContainer;
    public List<GameObject> itemList = new List<GameObject>();
    protected override void Init()
    {
        //测试用
        // InitContent();   
        // foreach(var item in itemList)
        // {
        //     item.transform.SetParent(srCommonItemContainer.content, false);
        // }
    }

    protected override void RefreshItem()
    {
        //在刷新item的方法中，获取外部数据结构中存储的item；
        // foreach
    }


    // //测试用：初始化道具列表
    // private void InitContent()
    // {
    //     for(int i = 0; i < 20; i++)
    //     {
    //         GameObject prefab = Resources.Load<GameObject>("Item");
    //         itemList.Add(Instantiate<GameObject>(prefab));
    //     }
    // }

    //外部实现一个单例Item管理器，存储所有当前持有的Item
}

