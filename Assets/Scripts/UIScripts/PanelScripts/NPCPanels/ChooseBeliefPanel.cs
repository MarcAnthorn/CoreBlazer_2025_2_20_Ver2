using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ChooseBeliefPanel : BasePanel
{
    public GameObject npcObj;
    public TextMeshProUGUI txtSanity;
    public TextMeshProUGUI txtConversation;
    public Button btnExit;
    public Button btnLastPage;
    public Button btnNextPage;
    public Transform choiceContent;
    public Transform npcPos;

    //测试用：
    private E_NPCName currentNPCName;
    public UnityAction<E_NPCName> setNPCAction;

    //用于装载当前选项对象的List；
    private List<GameObject> optionList = new List<GameObject>();
    //用于装载当前显示的选项的List：
    private List<GameObject> shownList = new List<GameObject>();
    private int optionCount;
    private int optionPerPage = 3;
    private int currentPage; 
    private int allPageCount;

    protected override void Awake()
    {
        base.Awake();
        setNPCAction += SetCurrentNPC;
    }
    protected override void Init()
    {
        btnExit.onClick.AddListener(()=>{
            UIManager.Instance.HidePanel<ChooseBeliefPanel>();
            UIManager.Instance.ShowPanel<NPCInteractionPanel>().setNPCAction(currentNPCName);
        }); 

        btnLastPage.onClick.AddListener(()=>{
            SwitchPage(-1);
        });

        btnLastPage.onClick.AddListener(()=>{
            SwitchPage(1);
        });

        InitCurrentPanel();
    }

    void OnDestroy()
    {
        setNPCAction -= SetCurrentNPC;
    }

    //初始化当前需要的所有面板信息：
    private void InitCurrentPanel()
    {
        //通过事件中心获取当前面板的交互NPC:
        EventHub.Instance.EventTrigger<UnityAction<E_NPCName>>("BroadcastCurrentInteractingNPC", (_name)=>{
            currentNPCName = _name;
        });

        //当前应该显示的NPC立绘：


        //初始化sanity显示：
        txtSanity.text = PlayerManager.Instance.player.SAN.value.ToString();

        //获取对话文本：


        //获取选项列表：初始化翻页相关内容
        // foreach(var option in 数据结构)
        // {

        // }

        optionCount = optionList.Count;
        currentPage = 1;
        allPageCount = (optionCount + optionPerPage - 1) / optionPerPage;
        

    }

    public void SwitchPage(int pageDelta)
    {
        foreach(var optionObj in shownList)
        {
            Destroy(optionObj);
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
            GameObject currentObj = Instantiate(optionList[i]);
            shownList.Add(currentObj);
            currentObj.gameObject.transform.SetParent(choiceContent, false);
        }


        Debug.Log($"已更新，当前页：{currentPage}, 当前显示的范围是：{leftIndex} 到{rightIndex}");
    }

    private void SetCurrentNPC(E_NPCName _npcName)
    {
        Debug.Log($"NPC交互面板已显示，显示NPC为：{_npcName.ToString()}");
        //将当前交互的NPC
        currentNPCName = _npcName;
        string path = currentNPCName.ToString();
        npcObj = Instantiate(Resources.Load<GameObject>("NPC/" + path), npcPos, false);
    }
}
