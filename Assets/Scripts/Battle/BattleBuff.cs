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

public class BattleBuff
{
    protected int id;
    protected string name;
    protected BattleBuffType buffType;
    protected CalculationType calculationType;
    // Buff影响数值
    protected float influence;
    // 持续回合数
    protected int lastTurns;
}

