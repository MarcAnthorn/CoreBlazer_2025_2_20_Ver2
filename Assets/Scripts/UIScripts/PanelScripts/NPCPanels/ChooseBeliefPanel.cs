using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ChooseBeliefPanel : BasePanel
{
    public Image imgNPC;
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
    // //用于装载当前显示的选项的List：（因为一个NPC的选项好像只有2个，因此直接被替代）
    // private List<GameObject> shownList = new List<GameObject>();

    public ChooseBeliefButton btnScript1;
    public ChooseBeliefButton btnScript2;
    // private int optionCount;
    // private int optionPerPage = 3;
    // private int currentPage; 
    // private int allPageCount;

    protected override void Awake()
    {
        base.Awake();

        EventHub.Instance.AddEventListener<int>("BuyItemCallback", UpdateConversation);
    }

    void OnDestroy()
    {
        EventHub.Instance.RemoveEventListener<int>("BuyItemCallback", UpdateConversation);
    }
    
    protected override void Init()
    {
        btnExit.onClick.AddListener(()=>{
            UIManager.Instance.HidePanel<ChooseBeliefPanel>();
            EventHub.Instance.EventTrigger<bool>("Freeze", false);
        }); 

        // btnLastPage.onClick.AddListener(()=>{
        //     SwitchPage(-1);
        // });

        // btnLastPage.onClick.AddListener(()=>{
        //     SwitchPage(1);
        // });
    }

    //初始化当前需要的所有面板信息：
    public void InitCurrentPanel(E_NPCName _npcName)
    {
        // //通过事件中心获取当前面板的交互NPC:
        // EventHub.Instance.EventTrigger<UnityAction<E_NPCName>>("BroadcastCurrentInteractingNPC", (_name)=>{
        //     currentNPCName = _name;
        // });

        Debug.Log($"NPC交互面板已显示，显示NPC为：{_npcName.ToString()}");
        //将当前交互的NPC
        currentNPCName = _npcName;
        string path = Path.Combine("ArtResources", currentNPCName.ToString());
        imgNPC.sprite = Resources.Load<Sprite>(path);
        imgNPC.SetNativeSize();

        //初始化sanity显示：
        txtSanity.text = PlayerManager.Instance.player.SAN.value.ToString();

        //获取最后对话文本：
        //并且初始化相关的选项：
        switch(_npcName){
            case E_NPCName.奈亚拉:
                txtConversation.text = "那可以看看祂的好东西，不保证物有所值哦。";
                btnScript1.Init(611);
                btnScript2.Init(612);
            break;
            case E_NPCName.优格:
                txtConversation.text = "想了解点什么？宇宙的真理可不是能轻易参透的。";
                btnScript1.Init(613);
                btnScript2.Init(614);
            break;
            case E_NPCName.莎布:
                txtConversation.text = "祂也给我了不少好东西，来看看吧！";
                btnScript1.Init(615);
                btnScript2.Init(616);
            break;
            default:    //此处是格赫罗斯
                txtConversation.text = "......";
                // btnScript1.Init(611);
                // btnScript2.Init(612);
            break;
        }


        // optionCount = optionList.Count;
        // currentPage = 1;
        // allPageCount = (optionCount + optionPerPage - 1) / optionPerPage;
    }

    //购买之后的更新对话的回调函数：
    //参数用不上，只是为了匹配事件中心的回调签名
    private void UpdateConversation(int _int)
    {
        switch(currentNPCName){
            case E_NPCName.奈亚拉:
                txtConversation.text = "欢迎光临，下次再来！！";
            break;
            case E_NPCName.优格:
                txtConversation.text = "愿你不会停下对真理的探索。";
            break;
            case E_NPCName.莎布:
                txtConversation.text = "希望这些能让你转危为安。";
            break;
            default:    //此处是格赫罗斯
                txtConversation.text = "......";
            break;
        }
    }

    //翻页：暂时用不到；
    // public void SwitchPage(int pageDelta)
    // {
    //     foreach(var optionObj in shownList)
    //     {
    //         Destroy(optionObj);
    //     }
    //     shownList.Clear();

    //     currentPage += pageDelta;

    //     //越界处理：
    //     if(currentPage == 0)
    //         currentPage = allPageCount;

    //     else if(currentPage > allPageCount)
    //         currentPage = 1;
        
    //     //计算当前应该展示的是什么范围的内容：
    //     int leftIndex = (currentPage - 1) * optionPerPage;
    //     int rightIndex = currentPage * optionPerPage - 1 > optionCount ? optionCount - 1 : currentPage * optionPerPage - 1;

    //     for(int i = leftIndex; i <= rightIndex; i++)
    //     {
    //         GameObject currentObj = Instantiate(optionList[i]);
    //         shownList.Add(currentObj);
    //         currentObj.gameObject.transform.SetParent(choiceContent, false);
    //     }


    //     Debug.Log($"已更新，当前页：{currentPage}, 当前显示的范围是：{leftIndex} 到{rightIndex}");
    // }

}
