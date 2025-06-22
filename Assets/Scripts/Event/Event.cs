using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]           //代表下面一个类的数据可以序列化
public class Event          //
{
    public int libId;                           //事件库Id(依不同关卡来选)   ************************
    public int eventId;                         //事件Id(对应一个事件文本库)
    public MyEventType eventType;               //事件类型(主线事件/怪谈事件)
    public int grade;                           //事件分级

    public List<EventOption> options;           //事件选项
    public Dictionary<int, KaidanText> textLib; //事件文本库，前面的int类型是文本的Id标识，文本库作用：按需取文本
    public string evDescription;                //事件描述(装载事件文本库中的事件，作用：装载实际显示的文本)

    //public int id;                            //当前事件的唯一标识
    public bool isTrigger;                      //是否触发过(对于弹窗事件)
    public int firstTextId;                     //首句Id

    public bool hasResult = false;
    public EventResult result = null;

    public Event()
    {
        eventType = new MyEventType();
        options = new List<EventOption>();
        textLib = new Dictionary<int, KaidanText>();
        //result = new EventResult();
    }
    ~Event()
    {
        options.Clear();
        textLib = null;
    }

    public enum MyEventType
    {
        None,       //默认值  
        Action1,    //事件1  
        Action2,    //事件2  
                    //其他事件类型  
    }

    public class EventResult
    {
        public int resultId;                        //事件结果(影响的属性Id)
        public int spId;                            //特殊结果(另作处理)
        public float change_HP;                     //HP影响(加值)
        public float change_HP_rate;                //HP影响(百分比)
        public float change_STR;                    //STR影响(加值)
        public float change_STR_rate;               //STR影响(百分比)
        public float change_DEF;                    //DEF影响(加值)
        public float change_DEF_rate;               //DEF影响(百分比)
        public float change_LVL;                    //LVL影响(加值)
        public float change_LVL_rate;               //LVL影响(百分比)
        public float change_SAN;                    //SAN影响(加值)
        public float change_SAN_rate;               //SAN影响(百分比)
        public float change_SPD;                    //SPD影响(加值)
        public float change_SPD_rate;               //SPD影响(百分比)
        public float change_CRIT_Rate;              //CRIT_rate影响(加值)
        public float change_CRIT_Rate_rate;         //CRIT_rate影响(百分比)
        public float change_CRIT_DMG;               //CRIT_DMG影响(加值)
        public float change_CRIT_DMG_rate;          //CRIT_DMG影响(百分比)
        public float change_HIT;                    //HIT影响(加值)
        public float change_HIT_rate;               //HIT影响(百分比)
        public float change_AVO;                    //AVO影响(加值)
        public float change_AVO_rate;               //AVO影响(百分比)



        //新增的结果字段：
        //后续的战斗敌人id：
        public int enemyId;

        //发放的道具id：
        public int itemId; 

        //发放的装备id：
        public int equipmentId;

        //发送的道具数量：
        public int itemCount;

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

    public void ExecuteResult(Player player)                //执行事件结果
    {
        if (hasResult)
        {
            player.HP.AddValue(this.result.change_HP);
            player.HP.MultipleValue( this.result.change_HP_rate + 1);

            player.STR.AddValue(this.result.change_STR);
            player.STR.MultipleValue( this.result.change_STR_rate + 1);

            player.DEF.AddValue(this.result.change_DEF);
            player.DEF.MultipleValue(this.result.change_DEF_rate + 1);

            player.LVL.AddValue(this.result.change_LVL);
            player.LVL.MultipleValue(this.result.change_LVL_rate + 1);

            player.SAN.AddValue(this.result.change_SAN);
            player.SAN.MultipleValue(this.result.change_SAN_rate + 1);

            player.SPD.AddValue(this.result.change_SPD);
            player.SPD.MultipleValue(this.result.change_SPD_rate + 1);

            player.CRIT_Rate.AddValue(this.result.change_CRIT_Rate);
            player.CRIT_Rate.MultipleValue(this.result.change_CRIT_Rate_rate + 1);

            player.CRIT_DMG.AddValue(this.result.change_CRIT_DMG);
            player.CRIT_DMG.MultipleValue( this.result.change_CRIT_DMG_rate + 1);

            player.HIT.AddValue(this.result.change_HIT);
            player.HIT.MultipleValue( this.result.change_HIT_rate + 1);

            player.AVO.AddValue(this.result.change_AVO);
            player.AVO.MultipleValue(this.result.change_AVO_rate + 1);


            //如果有战斗，那么尝试将战斗id播放给BattlePanel：
            if(this.result.enemyId != 0)
            {
                //等待文本播放结束：
                EventHub.Instance.AddEventListener("TriggerEventBattle", TriggerEventBattle);
            }
            

            //如果只是道具投放，那么就会将道具加入到玩家背包：
            else if(result.itemId != 0)
            {
                ItemManager.Instance.AddItem(result.itemId, true, result.itemCount);
            }

            //如果装备不是0，那么需要投放装备：
            if(result.equipmentId != 0){
                EquipmentManager.Instance.AddEquipment(result.equipmentId);
            }

        }
        else
        {
            Debug.LogWarning("正在尝试访问一个结果为 null 的事件结果");
        }

    }

    public void TriggerEventBattle()
    {
        Debug.Log($"选项战斗事件触发, id:{result.enemyId}");
        TriggerBattle(this.result.enemyId);
    }

    public void TriggerBattle(int enemyId)
    {
        //关闭事件面板：
        UIManager.Instance.HidePanel<GameMainPanel>();
        
        UIManager.Instance.ShowPanel<BattlePanel>().InitEnemyInfo(enemyId);

        //触发之后，就移除自己：
        EventHub.Instance.RemoveEventListener("TriggerEventBattle", TriggerEventBattle);
    }

    // public void ReadKaidanTextFrom(KaidanText begin)    //顺序读取怪诞文本
    // {
    //     KaidanText text = begin;
    //     while (textLib.ContainsKey(text.textId))
    //     {
    //         Debug.Log($"文本Id：{textLib[text.textId].textId}, 文本内容：{textLib[text.textId].text}");
    //         //处理文字动态显示
    //         if (text.nextId != 0)
    //         {
    //             text = textLib[text.nextId];
    //         }
    //         else
    //         {
    //             return;
    //         }
    //     }

    //     return;
    // }
    // public void RecurReadKaidanTextFrom(int id)         //利用递归进行读取
    // {
    //     if (!textLib.ContainsKey(id) || textLib[id].nextId == 0)
    //     {
    //         return;
    //     }
    //     Debug.Log($"文本Id：{textLib[id].textId}, 文本内容：{textLib[id].text}");
    //     //处理文字动态显示
    //     RecurReadKaidanTextFrom(textLib[id].nextId);

    //     return;
    // }

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
            result = new OptionResult(nextId);
            result.battleEvent = new BattleEvent()
            {
                id = nextId
                //其他的一些定义
            };
        }
    }

    public int optionId;                        //作为 每个事件 不同选项的唯一标识(1,2,3...)
    public bool isSeletable = false;

    public OptionResult result;

    [System.Serializable]
    public class OptionResult
    {
        public int changeId;
        public float change;
        //测试用
        private int nextId;
        public OptionResult(int _nextId)
        {
            myAction += Test;
            nextId = _nextId;
        }

        public string outcome;                  //事件结果
        

        //委托中需要更新EventManager中的currentEventId;
        public UnityAction myAction;         //选择选项之后的处理
       
        //测试用：
        public void Test()
        {
            EventManager.Instance.currentEventId = nextId;
            Debug.Log("当前事件id已更新： " + EventManager.Instance.currentEventId);
        }

        public void TriggerBtlEvent()
        {

        }
        public BattleEvent battleEvent;         //接入对应的战斗事件



    }
   

    public bool LockOrNot(Player player)         //随角色属性变化而动态更新(在角色触发事件(ShowEvent)时调用)  !!!!!
    {
        float SAN = player.SAN.value;       //角色当前精神值
        float STR = player.STR.value;       //角色当前力量
        float SPD = player.SPD.value;       //角色当前速度
        float AVO = player.AVO.value; 

        //测试用！！！！因为目前选项的需求数值还没填完，很多都是0；（Marc修改）
        
        // if ((optionId == 1 && minCondition < SAN && maxCondition > SAN  ||
        //       optionId == 2 && minCondition < STR && maxCondition > STR  ||
        //       optionId == 3 && minCondition < SPD && maxCondition > SPD ||
        //       optionId == 10 && minCondition < AVO && maxCondition > AVO &&
        //       itemId == 0) || (itemId != 0 && player.bag.ContainsKey(itemId)))
        // {
        //     isSeletable = true;             //符合条件
        //     return true;
        // }

        // isSeletable = false;                //不符合条件
        // return false;

        //测试用！因为目前选项的需求数值还没填完，很多都是0；
        return true;
    }

    public string ConditionName()
    {
        switch(conditionId)
        {
            case 0:
            return "";
            case 1:
            return "生命";
            case 2:
            return "力量";
            case 3:
            return "防御";
            case 4:
            return "理智";
            case 5:
            return "生命值";
            case 6:
            return "速度";
            case 7:
            return "暴击率";
            case 8:
            return "暴击伤害";
            case 9:
            return "连击";
            case 10:
            return "闪避值";
            default:
            return null;
        }
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

