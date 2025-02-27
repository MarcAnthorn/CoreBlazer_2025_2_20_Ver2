using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]           //代表下面一个类的数据可以序列化
public class Event          //
{
    public int libId;                           //事件库Id(依不同关卡来选)   ************************
    public int eventId;                         //事件Id(对应一个事件文本库)
    public MyEventType type;                    //事件类型(?如：弹窗事件/战斗事件/直接事件(如：陷阱)?)
    public List<EventOption> options;           //事件选项
    public Dictionary<int, KaidanText> textLib; //事件文本库，前面的int类型是文本的Id标识，文本库作用：按需取文本
    public string evDescription;                //事件描述(装载事件文本库中的事件，作用：装载实际显示的文本)

    //public int id;                            //当前事件的唯一标识
    public bool isTrigger;                      //是否触发过(对于弹窗事件)
    public int firstTextId;                     //首句Id

    public Event()
    {
        options = new List<EventOption>();
        textLib = new Dictionary<int, KaidanText>();
    }

    public enum MyEventType
    {
        None,       //默认值  
        Action1,    //事件1  
        Action2,    //事件2  
                    //其他事件类型  
    }

    [System.Serializable]
    public class KaidanText                     //怪诞文本(可以做成类似于结点的用法)
    {
        public int eventId;             //事件Id
        public int textId;              //文本Id
        public bool isKwaidan = true;   //文本类型：怪谈文本(true)/对话文本(false)
        public string text;             //文本内容
        public int nextId;              //下一个文本的Id(用于在textLib中查找,等于-1代表不存在下文)
        public int illustrationId;      //立绘Id
        public int bgId;                //背景Id
        //public KaidanText nextText;   //下一个怪诞文本
    }

    //public List<EventOption> GetOptions()           //事件选项
    //{
    //    return this.options;
    //}
    //public void SetOptions(int optionId, EventOption @eventOption)
    //{
    //    options[optionId] = @eventOption;
    //}

    //public Dictionary<int, KaidanText> GetTextLib() //事件文本库
    //{
    //    return textLib;
    //}
    //public void SetTextLib(int textId, KaidanText text)
    //{
    //    textLib[textId] = text;
    //}

    //public string GetEvDescription()                //事件描述(读取事件文本库)
    //{
    //    return evDescription;
    //}
    //public void SetEvDescription(string description)
    //{
    //    this.evDescription = description;
    //}

    public void ReadKaidanTextFrom(KaidanText begin)    //顺序读取怪诞文本
    {
        KaidanText text = begin;
        while (textLib.ContainsKey(text.textId))
        {
            Debug.Log($"文本Id：{textLib[text.textId].textId}, 文本内容：{textLib[text.textId].text}");
            //处理文字动态显示
            if (text.nextId != 0)
            {
                text = textLib[text.nextId];
            }
            else
            {
                return;
            }
        }

        return;
    }
    public void RecurReadKaidanTextFrom(int id)         //利用递归进行读取
    {
        if (!textLib.ContainsKey(id) || textLib[id].nextId == 0)
        {
            return;
        }
        Debug.Log($"文本Id：{textLib[id].textId}, 文本内容：{textLib[id].text}");
        //处理文字动态显示
        RecurReadKaidanTextFrom(textLib[id].nextId);

        return;
    }

}

[System.Serializable]
public class EventOption
{
    public int conditionId;                     //可选择条件属性Id(SAN <-> 1   STR <-> 2   SPD <-> 3)
    public int minCondition;                    //最小 可选择条件属性要求值(分别对应一项有关Id)
    public int maxCondition;                    //最大 可选择条件属性要求值(分别对应一项有关Id)
    public int itemId;                          //需求道具Id
    public string OpDescription;                //选项文本(?不被包含在事件文本库中，而是单作处理?)
    


    //属性操作的私有字段；
    private int nextId;
    public int NextId                           //下一个事件的Id
    {
        get
        {
            return nextId;  //原先这里直接返回属性本身会有bug
        }
        set
        {
            nextId = value;
            result = new EventResult();
            result.battleEvent = new BattleEvent()
            {
                id = nextId
                //其他的一些定义
            };
        }
    }

    public int optionId;                        //作为 每个事件 不同选项的唯一标识(1,2,3...)
    public bool isSeletable = false;

    public EventResult result;

    //触发事件的标准     !!!!!
    //public int minSAN = -1, maxSAN = -1;
    //public int minSTR = -1, maxSTR = -1;
    //public int minSPD = -1, maxSPD = -1;


    [System.Serializable]
    public class EventResult
    {
        public string outcome;                  //事件结果
        

        //委托中需要更新EventManager中的currentEventId;
        public Action myAction;                 //选择选项之后的处理
        public BattleEvent battleEvent;         //接入对应的战斗事件

        public void TriggerBtlEvent()
        {
            myAction();
        }

    }

    public bool LockOrNot(Player player)         //随角色属性变化而动态更新(在角色触发事件(ShowEvent)时调用)  !!!!!
    {
        int SAN = player.SAN;       //角色当前精神值
        int STR = player.STR;       //角色当前力量
        int SPD = player.SPD;       //角色当前速度
        if (optionId == 1 && minCondition < SAN && maxCondition > SAN ||
            optionId == 2 && minCondition < STR && maxCondition > STR ||
            optionId == 3 && minCondition < SPD && maxCondition > SPD)
        {
            isSeletable = true;             //符合条件
            return true;
        }

        isSeletable = false;                //不符合条件
        return false;
    }

}

[System.Serializable]
public class BattleEvent
{
    public int id;                          //战斗事件的Id
}





//[System.Serializable]
//public class EventDataContainer     //用于做Json数据和Event数据交换的中转站
//{
//    public List<Event> eventDatas;
//}

