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
    private List<GameObject> optionList; 

    public Transform rightSection;
    //用于控制事件描述延迟的浮点：
    private float textDisplayDelayTime;

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
        EventHub.Instance.AddEventListener<string>("UpdateDescriptionAfterOption", UpdateDescriptionAfterOption);

    //     // 测试用：
    //     currentEvent = new Event();
    //     currentEvent.textLib = new Dictionary<int, Event.KaidanText>();
    //     Event.KaidanText text1 = new Event.KaidanText();
    //     text1.textId = 1;
    //     text1.nextId = 2;
    //     text1.isKwaidan = false;
    //     text1.text = "盛大的魔术剧院空无一人，一位魔术师穿着满是血污的礼服站在舞台的正中央，舞台上堆满了毛绒玩偶的残肢断臂。";

    //      Event.KaidanText text2 = new Event.KaidanText();
    //     text2.textId = 2;
    //     text2.nextId = 3;
    //     text2.isKwaidan = false;
    //     text2.text = "他像是蜡像一样一动不动的站着，直到你靠近。";


    //      Event.KaidanText text3 = new Event.KaidanText();
    //     text3.textId = 3;
    //      text3.nextId = 4;
    //     text3.isKwaidan = true;
    //     text3.text = "“亲爱的女士们先生们，盛大的魔术表演即将开始！”";


    //      Event.KaidanText text4 = new Event.KaidanText();
    //     text4.textId = 4;
    //      text4.nextId = 0;
    //     text4.isKwaidan = false;
    //     text4.text = "魔术师的声音仿佛从空中传来，身体则一动不动。测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试测试";

    //     currentEvent.textLib.Add(1, text1);
    //     currentEvent.textLib.Add(2, text2);
    //     currentEvent.textLib.Add(3, text3);
    //     currentEvent.textLib.Add(4, text4);

    //     Debug.Log("Generated!");
        
    }

    private void OnDestroy()
    {
        EventHub.Instance.RemoveEventListener<string>("UpdateDescriptionAfterOption", UpdateDescriptionAfterOption);
    }





    //更新当前UI显示事件的方法；
    //包含：事件cg、事件描述文本、提示谜语、道具列表
    private void UpdateEvent()
    {
        //当前事件的获取一定要先于所有更新操作；
        // currentEvent = EventManager.Instance.BroadcastEvent();

        // currentEvent = EventManager.Instance.allEvents[1];
        //测试用：
        // currentEvent = EventManager.Instance.allEvents[1];
    
        //事件cg加载：


        //事件描述文本加载：
        //应该先加载事件红字介绍，0.3s之后再加载事件的描述部分；
        StartCoroutine(DisplayEventTextAndOptions());
    
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
            EventOptionBtn nowButtonScript = Instantiate<GameObject>(Resources.Load<GameObject>("EventOptionButton"), 
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


    //用于事件加载 / 延时加载事件描述 / 延时加载选项的协同程序：
    IEnumerator DisplayEventTextAndOptions()
    {
        var dic = currentEvent.textLib;
        var text = currentEvent.textLib[1];
        while(dic.ContainsKey(text.textId))
        {
            //如果是首行文本（key == 1），清除StringBuilder，不执行换行；
            if(text.isKwaidan && text.textId == 1)
            {
                textDisplayDelayTime = TextDisplayManager.Instance.DisplayText(txtEventDescription, 
                text.text, 
                Color.red,
                true,   //清除StringBuilder
                false); //不执行换行（第一行的文本不额外换行）
            }
            else if(!text.isKwaidan && text.textId == 1)
            {
                textDisplayDelayTime = TextDisplayManager.Instance.DisplayText(txtEventDescription, 
                text.text, 
                Color.white,
                true,
                false);
            }

            //如果是非首行文本（key != 1），不清除StringBuilder，执行换行；
            else if(text.isKwaidan)
            {
                textDisplayDelayTime = TextDisplayManager.Instance.DisplayText(txtEventDescription, 
                text.text, 
                Color.red,
                false,  
                true);
            }
            else if(!text.isKwaidan)
            {
                textDisplayDelayTime = TextDisplayManager.Instance.DisplayText(txtEventDescription, 
                text.text, 
                Color.white,
                false,
                true);
            }

            //虽然按理说textLib只会存储当前事件的文本，因此读取文本结束就会终止；
            //但是以防万一，设置一个break条件；
            if(!dic.ContainsKey(text.nextId))
            {
                break;
            }
            else
            {
                text = dic[text.nextId];
            }

            yield return new WaitForSeconds(textDisplayDelayTime);
        }

        UpdateOptions();
    }


    //选项触发后，需要更新当前事件的描述
    private void UpdateDescriptionAfterOption(string _text)
    {
        TextDisplayManager.Instance.DisplayText(txtEventDescription, _text, Color.white);
    }
   
}
