using System.Collections;
using System.Collections.Generic;
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

    AfterTurn = 1
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
    public int lastTurns;
    // 是否在回合开始时减少该Buff
    public bool ReduceAtBeginning;
    // 触发时机
    public TriggerTiming triggerTiming;
    // 是否能叠加
    public bool allowOverlying;
    // 叠加层数
    public int overlyingCount;

    public abstract void OnEffect();

}

public class BattleBuff_1001 : BattleBuff
{
    override public void OnEffect()
    {

    }
}


