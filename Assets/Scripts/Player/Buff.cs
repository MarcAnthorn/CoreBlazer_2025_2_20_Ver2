using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// buff作用属性
public enum BuffType            //角色属性变动前 先检查BuffType 然后做出对应处理
{
    NONE = 0,

    HP_Change = 1,
    STR_Change = 2,
    DEF_Change = 3,
    LVL_Change = 4,
    SAN_Change = 5,
    SPD_Change = 6,
    CRIT_Rate_Change = 7,
    CRIT_DMG_Change = 8,
    HIT_Change = 9,
    AVO_Change = 10,
}
// buff计算方式
public enum CalculationType
{
    NONE = 0,

    Add = 1,
    Multiply = 2
}

// buff特殊能力
public enum SpecialBuffType
{
    NONE = 0,

    DamageWall = 1,
    NoDamage = 2,
    OnceDontDie = 3
}

// buff作用目标
public enum Target
{
    NONE = 0,

    Player = 1,
    Enemy = 2,
    Other = 3
}

// buff作用场景(表示能够作用的条件)
public enum UseCase
{
    NONE = 0,

    GrowUp = 1,
    Battle = 2,
    Maze = 3,
    AfterDie = 4
}

public class Buff
{
    public BuffType buffType = BuffType.NONE;
    public CalculationType calculationType = CalculationType.NONE;
    public SpecialBuffType specialBuffType = SpecialBuffType.NONE;
    public UseCase useCase = UseCase.NONE;
    // 表示Buff加成的数值
    public float extraValue = 0f;

    public Func<float> buffFunction;        //用作其他特殊处理(比如破墙)
    public bool isTrigger;                  //若满足触发条件 或 主动触发 设置为true(配合buffFunction使用)


    public Buff(UseCase useCase, BuffType buffType, CalculationType calculationType, float extraChange = 0f)
    {
        this.useCase = useCase;
        this.buffType = buffType;
        this.calculationType = calculationType;
        this.extraValue = extraChange;
    }

    public Buff(UseCase useCase, BuffType buffType, SpecialBuffType specialBuffType, Func<float> buffFunction)
    {
        this.useCase = useCase;
        this.buffType = buffType;
        this.specialBuffType = specialBuffType;
        this.buffFunction = buffFunction;
    }

    public void TriggerBuff()
    {
        isTrigger = true;
    }

}
