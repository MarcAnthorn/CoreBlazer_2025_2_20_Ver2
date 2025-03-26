using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KaidanDisplayPanel : BasePanel
{
    public Button btnLastPage;
    public Button btnNextPage;
    public Button btnBack;   
    public Transform content;
    public List<GameObject> kaidanList = new List<GameObject>();
    public List<GameObject> shownList = new List<GameObject>();
    private int currentPage; 
    private int allPageCount;

    //测试用：
    //-------------------------------------------------
    private int kaidanCount = 40;
    private int kaidanPerPage = 12;
    //-------------------------------------------------

    protected override void Init()
    {
        btnLastPage.onClick.AddListener(()=>{
            SwitchPage(-1);
        });

        btnNextPage.onClick.AddListener(()=>{
            SwitchPage(1);
        });

        btnBack.onClick.AddListener(()=>{
            UIManager.Instance.HidePanel<KaidanDisplayPanel>();
        });

        //测试用：
        //-------------------------------------------------
        for(int i = 0; i < kaidanCount; i++)
        {
            GameObject kaidanObj = Resources.Load<GameObject>("TestResources/Kaidan");
            kaidanList.Add(kaidanObj);
        }

        for(int i = 0; i < kaidanPerPage; i++)
        {
            GameObject currentObj = Instantiate(kaidanList[i]);
            shownList.Add(currentObj);
            currentObj.gameObject.transform.SetParent(content, false);
        }
        //-------------------------------------------------

        currentPage = 1;
        allPageCount = (kaidanCount + kaidanPerPage - 1) / kaidanPerPage;
    }


    //测试用：换页：
    //-------------------------------------------------
    public void SwitchPage(int pageDelta)
    {
        foreach(var kaidanObj in shownList)
        {
            Destroy(kaidanObj);
        }
        shownList.Clear();

        currentPage += pageDelta;

        //越界处理：
        if(currentPage == 0)
            currentPage = allPageCount;

        else if(currentPage > allPageCount)
            currentPage = 1;
        
        //计算当前应该展示的是什么范围的内容：
        int leftIndex = (currentPage - 1) * kaidanPerPage;
        int rightIndex = currentPage * kaidanPerPage - 1 > kaidanCount ? kaidanCount - 1 : (currentPage) * kaidanPerPage - 1;

        for(int i = leftIndex; i <= rightIndex; i++)
        {
            GameObject currentObj = Instantiate(kaidanList[i]);
            shownList.Add(currentObj);
            currentObj.gameObject.transform.SetParent(content, false);
        }


        Debug.Log($"已更新，当前页：{currentPage}, 当前显示的范围是：{leftIndex} 到{rightIndex}");
    }
    //-------------------------------------------------
  

}
