using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]           //代表下面一个类的数据可以序列化
public class Event : MonoBehaviour
{
    public int id;                              //每个不同事件的唯一标识
    public string EvDescription;                //事件描述
    public List<EventOption> options;           //事件选项
    public List<EventOption.EventResult> results;
    //public Dictionary<EventOption, EventOption.EventResult> results;

}

[System.Serializable]
public class EventOption
{
    public string OpDescription;
    public int eventId;             //所属事件的Id
    public int optionId;            //作为 每个事件 不同选项的唯一标识(1,2,3...)

    public Event nextEventId;

    private bool isSeletable = false;

    //触发事件的标准
    public int minHP, maxHP;
    public int minSTR, maxSTR;
    public int minDEF, maxDEF;
    public int minLVL, maxLVL;
    public int minSAN, maxSAN;


    [System.Serializable]
    public class EventResult
    {
        public string outcome;      //事件结果
        public int nextEventId;     //处理对应的事件
    }

    public bool LockOrNot()         //随角色属性变化而动态更新(在角色触发事件(ShowEvent)时调用)
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



[System.Serializable]
public class EventDataContainer     //用于做Json数据和Event数据交换的中转站
{
    public List<Event> eventDatas;
}

