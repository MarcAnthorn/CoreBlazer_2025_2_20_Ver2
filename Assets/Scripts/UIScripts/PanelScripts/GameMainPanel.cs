using System.Collections;
using System.Collections.Generic;
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
    public Button btnSetting;
    public Button btnQuit;
    public RectTransform rtOptionsContainer;
    private List<GameObject> optionList = new List<GameObject>(); 

    public Transform rightSection;
    //当前的事件对象
    private Event currentEvent;

    //持有的两个道具面板：
    public GameObject commonItemPanelObject;
    public GameObject godItemPanelObject;

    private bool isDetectingCloseInput = false;


    protected override void Init()
    {
        godItemPanelObject.SetActive(true);
        //事件面板出现，更新玩家的所处场景的index：
        PlayerManager.Instance.playerSceneIndex = E_PlayerSceneIndex.Event;

        UpdateAttributeText();
        //默认显示的是神明道具面板；
        // UIManager.Instance.ShowPanel<GodItemPanel>().transform.SetParent(rightSection, false);
        btnToGodItem.onClick.AddListener(()=>{
            godItemPanelObject.SetActive(true);
            commonItemPanelObject.SetActive(false);
        });

        btnToCommonItem.onClick.AddListener(()=>{
            commonItemPanelObject.SetActive(true);
            godItemPanelObject.SetActive(false);
        });

        btnQuit.onClick.AddListener(()=>{
            if(isDetectingCloseInput)
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
        EventHub.Instance.AddEventListener("UpdateAttributeText", UpdateAttributeText);
    
    }

    private void OnDestroy()
    {
        EventHub.Instance.RemoveEventListener("UpdateEvent", UpdateEvent);
        EventHub.Instance.RemoveEventListener("UpdateOptions", UpdateOptions);
        EventHub.Instance.RemoveEventListener("ClearOptions", ClearOptions);
        EventHub.Instance.RemoveEventListener("UpdateAttributeText", UpdateAttributeText);
    

        //解冻玩家
        EventHub.Instance.EventTrigger<bool>("Freeze", false);

        //事件面板销毁，更新会迷宫场景：
        PlayerManager.Instance.playerSceneIndex = E_PlayerSceneIndex.Event;
    }

    //更新面板属性的方法，所有存在属性更新（如道具使用等等，最后都需要调用这个方法以确保显示的属性文本的更新）
    private void UpdateAttributeText()
    {
        txtHealth.text = $"{(int)PlayerManager.Instance.player.HP.value} / {(int)PlayerManager.Instance.player.HP.value_limit}";
        txtSanity.text = $"{(int)PlayerManager.Instance.player.SAN.value} / {(int)PlayerManager.Instance.player.SAN.value_limit}";
        txtLight.text = $"{(int)PlayerManager.Instance.player.LVL.value} / {(int)PlayerManager.Instance.player.LVL.value_limit}";

        txtStrength.text = $"力量：{(int)PlayerManager.Instance.player.STR.value}";
        txtSpeed.text = $"速度：{(int)PlayerManager.Instance.player.SPD.value}";
    }





    //更新当前UI显示事件的方法；
    //包含：事件cg、事件描述文本、提示谜语、道具列表
    private void UpdateEvent()
    {
        Debug.Log("事件开始更新");
        //当前事件的获取一定要先于所有更新操作；
        currentEvent = EventManager.Instance.BroadcastEvent();

    
        //事件cg加载：


        //事件描述文本加载：
        //应该先加载事件红字介绍，0.3s之后再加载事件的描述部分；

        DislayText();

    
        //更新提示谜语：


        //更新当前道具列表；
        //当前类不负责道具列表的刷新；交给ItemPanel（2个）自己实现
        //此处相当于只是发布事件：RefreshItem，同时会触发两个ItemPanel的内部订阅事件RefreshItem

        //如果当前事件是含有结果的事件，那么需要执行事件结果：
        if(currentEvent.hasResult)
        {
            currentEvent.ExecuteResult(PlayerManager.Instance.player);
            UpdateAttributeText();
        }      

    }

    //由于选项需要等待文本输出之后再显示，因此额外设置一个更新方法：
    //这个方法在文本显示之后，通过事件中心进行调用；
    private void UpdateOptions()
    {
        //事件选项加载：
        int invalidOptionsCount = 0;
        foreach(var option in currentEvent.options)
        {
            if(option.OpDescription == "0")
            {
                invalidOptionsCount++;
                if(invalidOptionsCount == 3)
                {
                    //
                    isDetectingCloseInput = true;
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
        }
    }

    private void DislayText()
    {
        var dic = currentEvent.textLib;
        var text = currentEvent.textLib[1];

        while(dic.ContainsKey(text.textId))
        {
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
