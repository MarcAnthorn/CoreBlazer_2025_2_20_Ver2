using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TarotDisplayPanel : BasePanel
{

    public Button btnLastPage;
    public Button btnNextPage;
    public Button btnBack;  

    public Transform content;
    public List<GameObject> tarotList = new List<GameObject>();
    public List<GameObject> shownList = new List<GameObject>();
    private int currentPage; 
    private int allPageCount;

    //测试用：
    //-------------------------------------------------
    private int tarotCount = 33;
    private int tarotPerPage = 10;
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
            UIManager.Instance.HidePanel<TarotDisplayPanel>();
        });


        //测试用：
        //-------------------------------------------------
        for(int i = 0; i < tarotCount; i++)
        {
            GameObject tarotObj = Resources.Load<GameObject>("TestResources/Tarot");
            tarotList.Add(tarotObj);
        }

        for(int i = 0; i < tarotPerPage; i++)
        {
            GameObject currentObj = Instantiate(tarotList[i]);
            shownList.Add(currentObj);
            currentObj.gameObject.transform.SetParent(content, false);
        }
        //-------------------------------------------------

        currentPage = 1;
        allPageCount = (tarotCount + tarotPerPage - 1) / tarotPerPage;
    }

    public void SwitchPage(int pageDelta)
    {
        foreach(var tarotObj in shownList)
        {
            Destroy(tarotObj);
        }
        shownList.Clear();

        currentPage += pageDelta;

        //越界处理：
        if(currentPage == 0)
            currentPage = allPageCount;

        else if(currentPage > allPageCount)
            currentPage = 1;
        
        //计算当前应该展示的是什么范围的内容：
        int leftIndex = (currentPage - 1) * tarotPerPage;
        int rightIndex = currentPage * tarotPerPage - 1 > tarotCount ? tarotCount - 1 : currentPage * tarotPerPage - 1;

        for(int i = leftIndex; i <= rightIndex; i++)
        {
            GameObject currentObj = Instantiate(tarotList[i]);
            shownList.Add(currentObj);
            currentObj.gameObject.transform.SetParent(content, false);
        }


        Debug.Log($"已更新，当前页：{currentPage}, 当前显示的范围是：{leftIndex} 到{rightIndex}");
    }
  

  

}