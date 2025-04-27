using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public enum BattleBuffType
{
    NONE = 0,

    Dot = 1,
    GoodBuff = 2,
    Debuff = 3
}

public enum TriggerTiming
{
    NONE = 0,

    AfterTurn = 1,
    CalculateDamage = 2
}

public abstract class BattleBuff
{
    public int id;
    public string name;
    // 战斗Buff类型
    public BattleBuffType buffType;
    // 计算方式
    public CalculationType calculationType;
    // Buff影响数值
    public float influence;
    // 持续回合数
    protected int _lastTurns;
    public int lastTurns
    {
        get { return _lastTurns; }
        set 
        { 
            _lastTurns = value;
            if(_lastTurns <= 0)
            {
                this.isEnd = true;
            }
        }
    }

//----------------------------Marc添加字段----------------------------------------
    //buff的图标路径（用于在战斗面板显示）：
    public string buffIconPath;

    //buff文本描述：
    public string buffDescriptionText;

    //buff是否显示在UI上：(也就是是不是新添加的UI)
    public bool isShownOnUI;


//----------------------------Marc添加字段----------------------------------------


    // 是否在回合开始时减少该Buff
    public bool ReduceAtBeginning;
    // 触发时机
    public TriggerTiming triggerTiming;
    // 是否能叠加
    public bool allowOverlying;
    // 叠加层数上限
    public int overlyingLimit;
    

    // 是否结束
    public bool isEnd = false;

    public abstract void OnEffect();

    // 用于获取子类的static字段overlyingCount
    public int GetOverlyingCount()
    {
        Debug.LogWarning("Try get overlying layer count");
        Type type = this.GetType();         // GetType()获取派生类的类型

        //Marc修改：原先的反射找不到静态字段；
        FieldInfo fieldInfo = type.GetField("overlyingCount", 
            BindingFlags.Static | 
            BindingFlags.Public | 
            BindingFlags.FlattenHierarchy);

        if (fieldInfo != null)
        {
            int overlyingCount = (int)fieldInfo.GetValue(null);
            return overlyingCount;
        }
        else
        {
            Debug.LogWarning($"找不到字段！, 返回-1");
            return -1;
        }

    }

    // 用于获取子类的static字段overlyingCount并进行自加1
    public void OverlyingCountPlus(int value)
    {
        Type type = this.GetType();         // GetType()获取派生类的类型
        FieldInfo fieldInfo = type.GetField("overlyingCount", 
            BindingFlags.Static | 
            BindingFlags.Public | 
            BindingFlags.FlattenHierarchy);

        if (fieldInfo != null)
        {
            int overlyingCount = (int)fieldInfo.GetValue(null);
            overlyingCount += value;                            // 加上value
            fieldInfo.SetValue(null, overlyingCount);           // 重新赋回去
        }
        else
        {
            Debug.LogWarning($"找不到字段！, 返回-1");
        }
    }

}

public class BattleBuff_1001 : BattleBuff
{
    // 叠加的层数
    public static int overlyingCount = 0;

    public BattleBuff_1001()
    {
        id = 1001;
        name = "中毒";
        buffType = BattleBuffType.Dot;
        calculationType = CalculationType.Add;
        influence = 10;
        lastTurns = 3;
        ReduceAtBeginning = false;
        triggerTiming = TriggerTiming.AfterTurn;
        allowOverlying = true;
        overlyingLimit = 99;
        // overlyingCount++;
    }

    override public void OnEffect()
    {
        PlayerManager.Instance.player.HP.AddValue(-10);
    }
}

public class BattleBuff_1002 : BattleBuff
{
    // 叠加的层数
    public static int overlyingCount = 0;

    public BattleBuff_1002()
    {
        id = 1002;
        name = "易伤";
        buffType = BattleBuffType.Debuff;
        calculationType = CalculationType.Multiply;
        influence = 5;
        lastTurns = 2;
        ReduceAtBeginning = false;
        triggerTiming = TriggerTiming.CalculateDamage;
        allowOverlying = true;
        overlyingLimit = 99;

    }

    override public void OnEffect()
    {
        // 此处不实现方法，具体处理要等到伤害计算时根据influence和calculationType进行计算
    }
}


