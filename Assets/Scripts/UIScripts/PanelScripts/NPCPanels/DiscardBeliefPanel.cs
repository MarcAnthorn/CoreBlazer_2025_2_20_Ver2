using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DiscardBeliefPanel: BasePanel
{

    public Image imgNPC;
    public TextMeshProUGUI txtSanity;
    public TextMeshProUGUI txtConversation;
    public Button btnExit;
    public Button btnLastPage;
    public Button btnNextPage;
    public Transform npcPos;
    public Transform choiceContent;


    //用于装载当前选项对象的List；
    private List<GameObject> optionList = new List<GameObject>();
    // //用于装载当前显示的选项的List:
    private List<GameObject> shownList = new List<GameObject>();

    public DiscardBeliefButton btnScript1;
    public DiscardBeliefButton btnScript2;
    public DiscardBeliefButton btnScript3;
    public DiscardBeliefButton btnScript4;
    public DiscardBeliefButton btnScript5;
    public DiscardBeliefButton btnScript6;
    private int optionCount;
    private int optionPerPage = 2;
    private int currentPage; 
    private int allPageCount;
    private string rootPath = "ArtResources/AVG";

    protected override void Awake()
    {
        base.Awake();

        EventHub.Instance.AddEventListener<int>("DiscardItemCallback", UpdateConversation);
    }

    void OnDestroy()
    {
        EventHub.Instance.RemoveEventListener<int>("DiscardItemCallback", UpdateConversation);
    }
    
    protected override void Init()
    {
        btnExit.onClick.AddListener(()=>{
            UIManager.Instance.HidePanel<DiscardBeliefPanel>();
            EventHub.Instance.EventTrigger<bool>("Freeze", false);
        }); 

        btnLastPage.onClick.AddListener(()=>{
            SwitchPage(-1);
        });

        btnNextPage.onClick.AddListener(()=>{
            SwitchPage(1);
        });

        InitCurrentPanel();
    }

    //初始化当前需要的所有面板信息：
    public void InitCurrentPanel()
    {

        string path = Path.Combine(rootPath, "格赫罗斯");
        imgNPC.sprite = Resources.Load<Sprite>(path);
        imgNPC.SetNativeSize();

        //初始化sanity显示：
        txtSanity.text = PlayerManager.Instance.player.SAN.value.ToString();

        txtConversation.text =  "哼哼，你终于愿意弃暗投明了吗，让我来帮你吧！";

        
        btnScript1.Init(2011);
        btnScript2.Init(2012);
        btnScript3.Init(2013);
        btnScript4.Init(2014);
        btnScript5.Init(2015);
        btnScript6.Init(2016);

        optionList.Add(btnScript1.gameObject);
        optionList.Add(btnScript2.gameObject);
        optionList.Add(btnScript3.gameObject);
        optionList.Add(btnScript4.gameObject);
        optionList.Add(btnScript5.gameObject);
        optionList.Add(btnScript6.gameObject);


        optionCount = optionList.Count;
        currentPage = 1;
        allPageCount = (optionCount + optionPerPage - 1) / optionPerPage;

        shownList.Add(btnScript1.gameObject);
        shownList.Add(btnScript2.gameObject);
    }

    //购买之后的更新对话的回调函数：
    //参数用不上，只是为了匹配事件中心的回调签名
    private void UpdateConversation(int _int)
    {
        txtConversation.text = "就这些吗？远远不够啊.........";
    }

    // 翻页：暂时用不到；
    public void SwitchPage(int pageDelta)
    {
        foreach(var optionObj in shownList)
        {
            optionObj.SetActive(false);
        }
        shownList.Clear();

        currentPage += pageDelta;

        //越界处理：
        if(currentPage == 0)
            currentPage = allPageCount;

        else if(currentPage > allPageCount)
            currentPage = 1;
        
        //计算当前应该展示的是什么范围的内容：
        int leftIndex = (currentPage - 1) * optionPerPage;
        int rightIndex = currentPage * optionPerPage - 1 > optionCount ? optionCount - 1 : currentPage * optionPerPage - 1;

        for(int i = leftIndex; i <= rightIndex; i++)
        {
            optionList[i].SetActive(true);
            GameObject currentObj = optionList[i];
            shownList.Add(currentObj);
        }


        Debug.Log($"已更新，当前页：{currentPage}, 当前显示的范围是：{leftIndex} 到{rightIndex}");
    }

}
