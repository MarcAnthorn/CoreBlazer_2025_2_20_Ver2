using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
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

    protected override void Init()
    {
        //默认显示的是神明道具面板；
        UIManager.Instance.ShowPanel<GodItemPanel>().transform.SetParent(rightSection, false);
        btnToGodItem.onClick.AddListener(()=>{
            UIManager.Instance.ShowPanel<GodItemPanel>().transform.SetParent(rightSection, false);
            UIManager.Instance.HidePanel<CommonItemPanel>();
        });

        btnToCommonItem.onClick.AddListener(()=>{
            UIManager.Instance.ShowPanel<CommonItemPanel>().transform.SetParent(rightSection, false);
            UIManager.Instance.HidePanel<GodItemPanel>();
        });


        //当前面板显示，更新面板内容：
        //测试用：
        UpdateEvent();          //更新事件相关内容
        UpdateAttribute();      //更新当前玩家属性显示

        
    }

    protected override void Awake()
    {
        base.Awake();
        EventHub.Instance.AddEventListener("UpdateOptions", UpdateOptions);
        EventHub.Instance.AddEventListener<string>("UpdateDescriptionAfterOption", UpdateDescriptionAfterOption);


    }

    private void OnDestroy()
    {
        EventHub.Instance.RemoveEventListener("UpdateOptions", UpdateOptions);
        EventHub.Instance.RemoveEventListener<string>("UpdateDescriptionAfterOption", UpdateDescriptionAfterOption);

    }





    //更新当前UI显示事件的方法；
    //包含：事件cg、事件描述文本、提示谜语、道具列表
    private void UpdateEvent()
    {
        Debug.Log("事件开始更新");
        //当前事件的获取一定要先于所有更新操作；
        // currentEvent = EventManager.Instance.BroadcastEvent();

        // currentEvent = EventManager.Instance.allEvents[1];
        //测试用：
        currentEvent = EventManager.Instance.startEvents[1];
    
        //事件cg加载：


        //事件描述文本加载：
        //应该先加载事件红字介绍，0.3s之后再加载事件的描述部分；
        DislayText();

    
        //更新提示谜语：


        //更新当前道具列表；
        //当前类不负责道具列表的刷新；交给ItemPanel（2个）自己实现
        //此处相当于只是发布事件：RefreshItem，同时会触发两个ItemPanel的内部订阅事件RefreshItem
        

    }

    //更新当前UI显示玩家属性的方法：
    private void UpdateAttribute()
    {
        //此处只是读取Player暴露给外部的属性数值：
        //如果需要Slider额外的实现效果（如属性增减时的数值条变化效果），可以使用LeanTween
        //如：将血量更新为当前玩家血量：
        //（未完成所有调整，有待讨论）
        sliderHealth.value = PlayerManager.Instance.Health;
        txtHealth.SetText("生命属性值：{0}", PlayerManager.Instance.Health);
        
    }


    //由于选项需要等待文本输出之后再显示，因此额外设置一个更新方法：
    private void UpdateOptions()
    {
        //事件选项加载：
        optionList.Clear();
        foreach(var option in currentEvent.options)
        {
            //遍历每一个option数据结构；
            //访问数据结构前，动态创建btnEventOption:
            EventOptionBtn nowButtonScript = Instantiate<GameObject>(Resources.Load<GameObject>("EventOptionBtn"), 
                rtOptionsContainer.transform, 
                false).GetComponent<EventOptionBtn>();
            
            //修改当前Button的属性判断描述和选项描述：
            //当前的Button应该是只有唯一的属性判断；应该是根据EventOption数据结构中存储的Option的id决定的；
            // nowButtonScript.setRequirementAction(option.op);
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


    //选项触发后，需要更新当前事件的描述
    private void UpdateDescriptionAfterOption(string _text)
    {
        // TextDisplayManager.Instance.BuildText(txtEventDescription, _text, Color.white);
    }
   
}
