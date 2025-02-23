using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GodItemPanel : ItemPanel
{
    public ScrollRect srGodItemContainer;
    private List<GameObject> itemList = new List<GameObject>();
    protected override void Init()
    {
        //测试用
        InitContent();

        
        foreach(var item in itemList)
        {
            
            item.transform.SetParent(srGodItemContainer.content, false);

        }
    }

    //重写的抽象方法：刷新当前Panel中的Item的方法；
    protected override void RefreshItem()
    {
        
    }


    //测试用：初始化道具列表
    private void InitContent()
    {
        for(int i = 0; i < 20; i++)
        {
            GameObject prefab = Resources.Load<GameObject>("Item");
            itemList.Add(Instantiate<GameObject>(prefab));
        }
    }



}
