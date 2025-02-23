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
    public EventType type;                      //事件类型(?如：弹窗事件/直接事件(陷阱/)?)
    public int textLibId;                       //事件文本库Id
    public List<EventOption> options;           //事件选项

    //public string GUAI_Description;             
    public Dictionary<int, KaidanText> textLib; //  !!!!!!!!!!!!!!
    public string EvDescription;                //事件描述(?装载事件文本库中的事件?)

    public int id;                              //当前事件的唯一标识
    public bool isTrigger;                      //是否触发过(对于弹窗事件)

    public enum EventType
    {
        None,       // 默认值  
        Action1,    // 事件1  
        Action2,    // 事件2  
                    // 其他事件类型  
    }

}

[System.Serializable]
public class EventOption
{
    public int[] conditions = new int[4];       //可选择条件属性要求值(分别对应四项有关Id)
    public string OpDescription;                //选项文本(?不被包含在事件文本库中，而是单作处理?)

    public int optionId;                        //作为 每个事件 不同选项的唯一标识(1,2,3...)
    public bool isSeletable = false;

    public EventResult result;

    //触发事件的标准     !!!!!
    public int minHP, maxHP;
    public int minSTR, maxSTR;
    public int minDEF, maxDEF;
    public int minLVL, maxLVL;
    public int minSAN, maxSAN;


    [System.Serializable]
    public class EventResult
    {
        public string outcome;      //事件结果
        //
        public Action myAction;     
        public int nextEventId;     //处理对应的事件

        public void TriggerEvent()
        {
            myAction();
        }

    }

    public bool LockOrNot()         //随角色属性变化而动态更新(在角色触发事件(ShowEvent)时调用)  !!!!!
    {
        int HP = PlayerManager.Instance.player.HP;
        int STR = PlayerManager.Instance.player.STR;
        int DEF = PlayerManager.Instance.player.DEF;
        int LVL = PlayerManager.Instance.player.LVL;
        int SAN = PlayerManager.Instance.player.SAN;
        if (DataProcessor.Instance.LowerThanStandard(HP, minHP) &&
            DataProcessor.Instance.UpToStandard(HP, maxHP) &&
            DataProcessor.Instance.LowerThanStandard(STR, minSTR) &&
            DataProcessor.Instance.UpToStandard(STR, maxSTR) &&
            DataProcessor.Instance.LowerThanStandard(DEF, minDEF) &&
            DataProcessor.Instance.UpToStandard(DEF, maxDEF) &&
            DataProcessor.Instance.LowerThanStandard(LVL, minLVL) &&
            DataProcessor.Instance.UpToStandard(LVL, maxLVL) &&
            DataProcessor.Instance.LowerThanStandard(SAN, minSAN) &&
            DataProcessor.Instance.UpToStandard(SAN, maxSAN))
        {
            isSeletable = true;             //符合条件
            return true;
        }

        isSeletable = false;                //不符合条件
        return false;
    }

}

public class KaidanText                     //怪诞文本
{
    public string description;
    public int textId;
}





//[System.Serializable]
//public class EventDataContainer     //用于做Json数据和Event数据交换的中转站
//{
//    public List<Event> eventDatas;
//}

