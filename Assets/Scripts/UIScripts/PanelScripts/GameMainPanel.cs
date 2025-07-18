using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameMainPanel : BasePanel
{
    
    public Slider sliderHealth;
    public Slider sliderSanity;
    public Slider sliderLight;

    public TextMeshProUGUI txtHealth;
    public TextMeshProUGUI txtSanity;
    public TextMeshProUGUI txtLight;
    public TextMeshProUGUI txtStrength;
    public TextMeshProUGUI txtSpeed;
    // public TextMeshProUGUI txtIntroduction;
    public TextMeshProUGUI txtEventDescription;
    public TextMeshProUGUI txtRiddleTip; 
    public Button btnToGodItem;
    public Button btnToCommonItem;

    public Button btnQuitBlackSpace;
    public RectTransform rtOptionsContainer;
    private List<GameObject> optionList = new List<GameObject>(); 

    public Transform rightSection;
    //当前的事件对象
    private Event currentEvent;

    //持有的两个道具面板：
    public GameObject commonItemPanelObject;

    public GameObject godItemPanelObject;
    public Image imgCg;

    private bool isDetectingCloseInput = false;
    private bool isOptionsUpdated = false;

    //记录：切换之前玩家的场景，方便切换回去：
    private E_PlayerSceneIndex playerSceneIndexFormer;


    protected override void Init()
    {
        TextManager.SetContentFont(this.gameObject);
        //关闭主面板：
        UIManager.Instance.HidePanel<MainPanel>();


        btnQuitBlackSpace.gameObject.SetActive(false);
        isOptionsUpdated = false;

        //关闭PlayerBase中的Escape检测，使其打不开背包面板：
        EventHub.Instance.EventTrigger<E_DetectInputType>("CloseSpecificDetectInput", E_DetectInputType.Escape);

        //事件面板出现，更新玩家的所处场景的index：
        playerSceneIndexFormer = PlayerManager.Instance.playerSceneIndex;
        
        PlayerManager.Instance.playerSceneIndex = E_PlayerSceneIndex.Event;

        UpdateAttributeText();
        //默认显示的是神明道具面板；
        godItemPanelObject.SetActive(true);
        
        btnToGodItem.onClick.AddListener(()=>{
            godItemPanelObject.SetActive(true);
            commonItemPanelObject.SetActive(false);
        });

        btnToCommonItem.onClick.AddListener(()=>{
            commonItemPanelObject.SetActive(true);
            godItemPanelObject.SetActive(false);
        });

        btnQuitBlackSpace?.onClick.AddListener(()=>{
            if(isDetectingCloseInput && btnQuitBlackSpace.gameObject.activeSelf)
            {
                UIManager.Instance.HidePanel<GameMainPanel>();
            }
        });




        //当前面板显示，更新面板内容：
        //测试用：
        isDetectingCloseInput = false;
        

        //冻结玩家
        EventHub.Instance.EventTrigger<bool>("Freeze", true);

        UpdateEvent();          //更新事件相关内容

        
    }

    protected override void Awake()
    {
        base.Awake();
        EventHub.Instance.AddEventListener("UpdateEvent", UpdateEvent);
        EventHub.Instance.AddEventListener("UpdateOptions", UpdateOptions);   
        EventHub.Instance.AddEventListener("ClearOptions", ClearOptions);

        //这是一个多播委托：存在任何对玩家属性做出调整的地方，都需要调用这个委托；
        EventHub.Instance.AddEventListener("UpdateAllUIElements", UpdateAttributeText);
    
    }

    private void OnDestroy()
    {
        EventHub.Instance.RemoveEventListener("UpdateEvent", UpdateEvent);
        EventHub.Instance.RemoveEventListener("UpdateOptions", UpdateOptions);
        EventHub.Instance.RemoveEventListener("ClearOptions", ClearOptions);
        EventHub.Instance.RemoveEventListener("UpdateAllUIElements", UpdateAttributeText);


        EventHub.Instance.EventTrigger("TriggerEventBattle");       //try to trigger battle(if there is a battle);
    

        //解冻玩家
        EventHub.Instance.EventTrigger<bool>("Freeze", false);

        //复原PlayerBase中的Escape检测；
        EventHub.Instance.EventTrigger<E_DetectInputType>("UnlockSpecificDetectInput", E_DetectInputType.Escape);

        UIManager.Instance.ShowPanel<MainPanel>();


        //事件面板销毁，更新回切换之前的场景：
        PlayerManager.Instance.playerSceneIndex = playerSceneIndexFormer;
    }

    //更新面板属性的方法，所有存在属性更新（如道具使用等等，最后都需要调用这个方法以确保显示的属性文本的更新）
    private void UpdateAttributeText()
    {
        txtHealth.text = $"{(int)PlayerManager.Instance.player.HP.value} / {(int)PlayerManager.Instance.player.HP.value_limit}";
        txtSanity.text = $"{(int)PlayerManager.Instance.player.SAN.value} / {(int)PlayerManager.Instance.player.SAN.value_limit}";
        txtLight.text = $"{(int)PlayerManager.Instance.player.LVL.value} / {(int)PlayerManager.Instance.player.LVL.value_limit}";

        txtStrength.text = $"力量：{(int)PlayerManager.Instance.player.STR.value}";
        txtSpeed.text = $"速度：{(int)PlayerManager.Instance.player.SPD.value}";


        //更新Sliders：
        sliderHealth.value = PlayerManager.Instance.player.HP.value / PlayerManager.Instance.player.HP.value_limit;
        sliderLight.value = PlayerManager.Instance.player.LVL.value / PlayerManager.Instance.player.LVL.value_limit;
        sliderSanity.value = PlayerManager.Instance.player.SAN.value / PlayerManager.Instance.player.SAN.value_limit;
    }





    //更新当前UI显示事件的方法；
    //包含：事件cg、事件描述文本、提示谜语、道具列表
    private void UpdateEvent()
    {
        Debug.Log("事件开始更新");
        //当前事件的获取一定要先于所有更新操作；
        currentEvent = EventManager.Instance.BroadcastEvent();

        //事件描述文本加载：
        //应该先加载事件红字介绍，0.3s之后再加载事件的描述部分；

        DislayText();


        //如果当前事件是含有结果的事件，那么需要执行事件结果：
        if(currentEvent.hasResult)
        {
            currentEvent.ExecuteResult(PlayerManager.Instance.player);
            UIManager.Instance.ShowPanel<ToastPanel>().SetEventResult(currentEvent.result);
            UpdateAttributeText();
        }      

    }

    //由于选项需要等待文本输出之后再显示，因此额外设置一个更新方法：
    //这个方法在文本显示之后，通过事件中心进行调用；
    private void UpdateOptions()
    {
        if(isOptionsUpdated)
            return;
        
        isOptionsUpdated = true;
        //事件选项加载：
        int invalidOptionsCount = 0;
        foreach(var option in currentEvent.options)
        {
            if(option.OpDescription == "0")
            {
                invalidOptionsCount++;
                if(invalidOptionsCount == 3)
                {
                    //说明文本播放结束了；
                    isDetectingCloseInput = true;
                    btnQuitBlackSpace.gameObject.SetActive(true);

                    //如果当前玩家生命归零了，那么文本结束后直接死亡：
                    if(PlayerManager.Instance.player.HP.value <= 0 || PlayerManager.Instance.player.SAN.value <= 0 )
                    {
                        EventHub.Instance.EventTrigger("OnPlayerDead");
                    }
                    break;
                }
                    
                continue;
            }
            //遍历每一个option数据结构；
            //访问数据结构前，动态创建btnEventOption:
            EventOptionBtn nowButtonScript = Instantiate<GameObject>(Resources.Load<GameObject>("EventOptionBtn"), 
                rtOptionsContainer.transform, 
                false).GetComponent<EventOptionBtn>();
            
            //修改当前Button的属性判断描述和选项描述：
            //当前的Button应该是只有唯一的属性判断；应该是根据EventOption数据结构中存储的Option的id决定的；
            nowButtonScript.setRequirementAction(option.ConditionName(), option.minCondition, option.maxCondition);
            Debug.Log(option.ConditionName());
            nowButtonScript.setDescriptionAction(option.OpDescription);

            //订正当前Button是否可交互：
            nowButtonScript.setInteractableAction(option.LockOrNot(PlayerManager.Instance.player));
            //将当前的Button脚本应当持有的实例传入：
            nowButtonScript.setOptionAction(option);
            //将当前事件的选项游戏对象加入optionList：
            optionList.Add(nowButtonScript.gameObject);

            //重新开放锁：
            isOptionsUpdated = false;
        }
    }

    private void DislayText()
    {
        var dic = currentEvent.textLib;
        var text = currentEvent.textLib[1];
        int cgId = -1;

        while(dic.ContainsKey(text.textId))
        {
            //发现cgid变化；更新cg：
            if(cgId != text.illustrationId){
                cgId = text.illustrationId;
                string path = Path.Combine("ArtResources", "怪谈事件图", cgId.ToString());

                Debug.Log($"now cg path is :{path}");
                imgCg.sprite = Resources.Load<Sprite>(path);
                // imgCg.SetNativeSize();

            }
            //如果是首行文本（key == 1），清除StringBuilder，不执行换行；
            if(text.isKwaidan && text.textId == 1)
            {
                TextDisplayManager.Instance.BuildText(txtEventDescription, 
                text.text, 
                Color.red,
                true,   //清除StringBuilder
                false); //不执行换行（第一行的文本不额外换行）
            }
            else if(!text.isKwaidan && text.textId == 1)
            {
                TextDisplayManager.Instance.BuildText(txtEventDescription, 
                text.text, 
                Color.white,
                true,
                false);
            }
 
            //如果是非首行文本（key != 1），不清除StringBuilder，执行换行；
            else if(text.isKwaidan)
            {
                TextDisplayManager.Instance.BuildText(txtEventDescription, 
                text.text, 
                Color.red,
                false,  
                true);
            }
            else if(!text.isKwaidan)
            {
                TextDisplayManager.Instance.BuildText(txtEventDescription, 
                text.text, 
                Color.white,
                false,
                true);
            }
            
            if(!dic.ContainsKey(text.nextId))
            {
                //当前文本读取结束，开始调用文本管理器的协同程序，执行文字的逐字显示
                TextDisplayManager.Instance.DisplayTextInSequence();
                break;
            }
            else
            {
                text = dic[text.nextId];
            }
        }

        //播放结束：尝试进行调用战斗：
        //该脚本在：Event中的ExecuteResult中 被注册；
        

    }   



    private void ClearOptions()
    {
        foreach(var optionObj in optionList)
        {
            GameObject.Destroy(optionObj);
        }
        optionList.Clear();
    }
    

             
   
   
}
