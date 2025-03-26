using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemDisplayPanel : BasePanel
{
    public Button btnLastPage;
    public Button btnNextPage;
    public Button btnBack;   

    public Transform content;
    public List<GameObject> itemList = new List<GameObject>();
    public List<GameObject> shownList = new List<GameObject>();
    private int currentPage; 
    private int allPageCount;

    //测试用：
    //-------------------------------------------------
    private int itemCount = 40;
    private int itemPerPage = 12;
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
            UIManager.Instance.HidePanel<ItemDisplayPanel>();
        });


        //测试用：
        //-------------------------------------------------
        for(int i = 0; i < itemCount; i++)
        {
            GameObject itemObj = Resources.Load<GameObject>("TestResources/Item");
            itemList.Add(itemObj);
        }

        for(int i = 0; i < itemPerPage; i++)
        {
            GameObject currentObj = Instantiate(itemList[i]);
            shownList.Add(currentObj);
            currentObj.gameObject.transform.SetParent(content, false);
        }
        //-------------------------------------------------

        currentPage = 1;
        allPageCount = (itemCount + itemPerPage - 1) / itemPerPage;
    }

    public void SwitchPage(int pageDelta)
    {
        foreach(var itemObj in shownList)
        {
            Destroy(itemObj);
        }
        shownList.Clear();

        currentPage += pageDelta;

        //越界处理：
        if(currentPage == 0)
            currentPage = allPageCount;

        else if(currentPage > allPageCount)
            currentPage = 1;
        
        //计算当前应该展示的是什么范围的内容：
        int leftIndex = (currentPage - 1) * itemPerPage;
        int rightIndex = currentPage * itemPerPage - 1 > itemCount ? itemCount - 1 : currentPage * itemPerPage - 1;

        for(int i = leftIndex; i <= rightIndex; i++)
        {
            GameObject currentObj = Instantiate(itemList[i]);
            shownList.Add(currentObj);
            currentObj.gameObject.transform.SetParent(content, false);
        }


        Debug.Log($"已更新，当前页：{currentPage}, 当前显示的范围是：{leftIndex} 到{rightIndex}");
    }
  

}