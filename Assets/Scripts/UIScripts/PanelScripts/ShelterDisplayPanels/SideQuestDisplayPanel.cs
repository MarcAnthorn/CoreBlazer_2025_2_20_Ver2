using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SideQuestDisplayPanel: BasePanel
{
    public Button btnLastPage;
    public Button btnNextPage;
    public Button btnBack;  

    public Transform content;
    public List<GameObject> sideQuestList = new List<GameObject>(); 
    public List<GameObject> shownList = new List<GameObject>();
    private int currentPage; 
    private int allPageCount;

    //测试用：
    //-------------------------------------------------
    private int storyCount = 40;
    private int storyPerPage = 12;
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
             UIManager.Instance.HidePanel<SideQuestDisplayPanel>();
        });


        //测试用：
        //-------------------------------------------------
        for(int i = 0; i < storyCount; i++)
        {
            GameObject storyObj = Resources.Load<GameObject>("TestResources/SideQuest");
            sideQuestList.Add(storyObj);
        }

        for(int i = 0; i < storyPerPage; i++)
        {
            GameObject currentObj = Instantiate(sideQuestList[i]);
            shownList.Add(currentObj);
            currentObj.gameObject.transform.SetParent(content, false);
        }
        //-------------------------------------------------

        currentPage = 1;
        allPageCount = (storyCount + storyPerPage - 1) / storyPerPage;
    }

    public void SwitchPage(int pageDelta)
    {
        foreach(var storyObj in shownList)
        {
            Destroy(storyObj);
        }
        shownList.Clear();

        currentPage += pageDelta;

        //越界处理：
        if(currentPage == 0)
            currentPage = allPageCount;

        else if(currentPage > allPageCount)
            currentPage = 1;
        
        //计算当前应该展示的是什么范围的内容：
        int leftIndex = (currentPage - 1) * storyPerPage;
        int rightIndex = currentPage * storyPerPage - 1 > storyCount ? storyCount - 1 : currentPage * storyPerPage - 1;

        for(int i = leftIndex; i <= rightIndex; i++)
        {
            GameObject currentObj = Instantiate(sideQuestList[i]);
            shownList.Add(currentObj);
            currentObj.gameObject.transform.SetParent(content, false);
        }


        Debug.Log($"已更新，当前页：{currentPage}, 当前显示的范围是：{leftIndex} 到{rightIndex}");
    }
  

  

}
