using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GodItemPanel : BasePanel
{
    public ScrollRect sr;
    private List<GameObject> itemList = new List<GameObject>();
    protected override void Init()
    {
        //测试用
        InitContent();

        
        foreach(var item in itemList)
        {
            
            item.transform.SetParent(sr.content, false);

        }
    }


    //测试用：初始化道具列表
    private void InitContent()
    {
        for(int i = 0; i < 20; i++)
        {
            GameObject prefab = Resources.Load<GameObject>("TestImage");
            itemList.Add(Instantiate<GameObject>(prefab));
        }
    }

}
