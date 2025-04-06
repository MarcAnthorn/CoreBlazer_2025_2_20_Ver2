using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    DamageWall = 11
}

public class Buff
{
    public BuffType buffType = BuffType.NONE;
    public float extraValue = 0f;

    public Action buffAction;               //用作其他特殊处理(比如非数值处理)
    public bool isTrigger;                  //若满足触发条件 或 主动触发 设置为true(配合buffAction使用)

    public Buff(BuffType buffType, float extraChange = 0f, Action buffAction = null)
    {
        this.buffType = buffType;
        this.extraValue = extraChange;
    }

    public Buff(BuffType buffType, Action buffAction)
    {

        this.buffType = buffType;
        this.buffAction = buffAction;
    }

    public void TriggerBuff()
    {
        isTrigger = true;
    }

}
