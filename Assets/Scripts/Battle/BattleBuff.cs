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

public enum ReduceTiming
{
    NONE = 0,

    Begin = 1,
    After = 2
}

public enum TriggerTiming
{
    NONE = 0,

    AfterTurn = 1,
    CalculateDebuffDamage = 2,
    CalculateGoodBuffDamage = 3,
    Immediate = 4,
    BeHit = 5,
    IsGoingToDie = 6
}

public abstract class BattleBuff
{
    public int id;
    public string name;
    // 影响的属性
    public BuffType type;
    // 战斗Buff类型
    public BattleBuffType buffType = BattleBuffType.NONE;
    // Buff影响的伤害类型
    public DamageType damageType = DamageType.NONE;
    // 计算方式
    public CalculationType calculationType;
    // Buff影响数值
    public float influence;
    // 持续回合数上限
    public int lastTurnLimit;
    // 实例当前持续回合数
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
    // Buff达到触发条件时造成的影响
    public abstract void OnEffect(int flag);
    // Buff加成增添
    public abstract void OnStart(int flag);
    // Buff加成去除
    public abstract void OnEnd(int flag);

    // 用于获取子类的static字段overlyingCount
    public int GetOverlyingCount()
    {
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
        type = BuffType.HP_Change;
        buffType = BattleBuffType.Dot;
        calculationType = CalculationType.Add;
        influence = 10;
        lastTurnLimit = 3;
        lastTurns = 3;
        ReduceAtBeginning = false;
        triggerTiming = TriggerTiming.AfterTurn;
        allowOverlying = true;
        overlyingLimit = 99;
        // overlyingCount++;
        buffDescriptionText = "\"①在每个回合结束时的时候触发中毒伤害。②层数归零或者倒计时归零时移除该dot。\"";
    }
    
    // flag表示Buff持有者：flag == 0 时，持有者为角色
    public override void OnEffect(int flag)
    {
        if(flag == 0)
        {
            BattleManager.Instance.player.HP.AddValue(-10 * overlyingCount);
        
            //更新伤害显示面板：
            //注意：要想触发事件，必须要是浮点型
            EventHub.Instance.EventTrigger("UpdateDamangeText", (float)(10 * overlyingCount), true);    
        }

        else if(flag == 1)
        {
            BattleManager.Instance.enemies[0].HP -= 10 * overlyingCount;

            EventHub.Instance.EventTrigger("UpdateDamangeText", (float)(10 * overlyingCount), false);


        }

        else
        {
            Debug.LogError($"当前中毒buff的flag不存在，flag:{flag}");
        }

    }

    public override void OnStart(int flag) { }

    public override void OnEnd(int flag) { }

}

public class BattleBuff_1002 : BattleBuff
{
    // 叠加的层数
    public static int overlyingCount = 0;

    public BattleBuff_1002()
    {
        id = 1002;
        name = "易伤";
        type = BuffType.HP_Change;
        buffType = BattleBuffType.Debuff;
        calculationType = CalculationType.Multiply;
        damageType = DamageType.STR | DamageType.Skill | DamageType.Dot;
        influence = 0.05f;
        lastTurnLimit = 2;
        lastTurns = 2;
        ReduceAtBeginning = false;
        triggerTiming = TriggerTiming.CalculateDebuffDamage;
        allowOverlying = true;
        overlyingLimit = 99;

        buffDescriptionText = "\"①在伤害结算的时候影响公式中的乘区②层数归零或者倒计时归零时移除该debuff。\"";
    }

    override public void OnEffect(int 占位)
    {
        // 此处不实现方法，具体处理要等到伤害计算时根据influence和calculationType进行计算
    }

    public override void OnStart(int flag) { }

    public override void OnEnd(int flag) { }

}

public class BattleBuff_1003 : BattleBuff
{
    // 叠加的层数
    public static int overlyingCount = 0;

    public BattleBuff_1003()
    {
        id = 1003;
        name = "燃血狂怒";
        type = BuffType.CRIT_Rate_Change;
        buffType = BattleBuffType.GoodBuff;
        calculationType = CalculationType.Multiply;
        influence = 1.0f;
        lastTurnLimit = 2;
        lastTurns = 2;
        ReduceAtBeginning = true;
        triggerTiming = TriggerTiming.Immediate;
        allowOverlying = false;
        overlyingLimit = 1;

        buffDescriptionText = "\"①生命值高于50%时，可扣除自身当前生命值的80%，使自身暴击率提升100%\r\n②层数归零或者倒计时归零时移除该buff。\"";
    }

    override public void OnEffect(int 占位)
    {
        // 此处不实现方法，具体处理要等到伤害计算时根据influence和calculationType进行计算
    }

    public override void OnStart(int flag = 0)
    {
        if (flag == 0)
        {
            Player player = BattleManager.Instance.player;
            if (player.HP.value >= 1/2 * player.HP.value_limit)
            {
                player.HP.MultipleValue(0.2f);
                player.CRIT_Rate.AddValue(1.0f);
            }
            else
            {
                Debug.Log("不满足使用条件");
            }
        }
    }

    public override void OnEnd(int flag = 0)
    {
        Player player = BattleManager.Instance.player;
        player.CRIT_Rate.AddValue(-1.0f);
    }

}

public class BattleBuff_1004 : BattleBuff
{
    // 叠加的层数
    public static int overlyingCount = 0;

    public BattleBuff_1004()
    {
        id = 1004;
        name = "破韧";
        damageType = DamageType.STR;
        calculationType = CalculationType.NONE;
        influence = 0f;
        lastTurnLimit = 2;
        lastTurns = 2;
        ReduceAtBeginning = true;
        triggerTiming = TriggerTiming.BeHit;
        allowOverlying = false;
        overlyingLimit = 1;

        buffDescriptionText = "\"①debuff的持有者力量伤害后，给该debuff的持有者施加12层易伤，给该debuff的施加者施加2层力量增伤\"";
    }

    override public void OnEffect(int flag)
    {
        if (flag == 0)  // 此时玩家持有Buff
        {
            BattleBuff buff1 = new BattleBuff_1002();
            BattleBuff buff2 = new BattleBuff_1023();
            for(int i = 0; i < 12; i++)
            {
                TurnCounter.Instance.AddPlayerBuff(buff1);
            }
            for (int i = 0; i < 2; i++)
            {
                TurnCounter.Instance.AddEnemyBuff(buff2, 0);
            }
        }

        else if (flag == 1)
        {
            BattleBuff buff1 = new BattleBuff_1002();
            BattleBuff buff2 = new BattleBuff_1023();
            for (int i = 0; i < 2; i++)
            {
                TurnCounter.Instance.AddPlayerBuff(buff2);
            }
            for (int i = 0; i < 12; i++)
            {
                TurnCounter.Instance.AddEnemyBuff(buff1, 0);
            }
        }
    }

    public override void OnStart(int flag = 0) { }

    public override void OnEnd(int flag = 0) { }

}

public class BattleBuff_1005 : BattleBuff
{
    // 叠加的层数
    public static int overlyingCount = 0;

    public BattleBuff_1005()
    {
        id = 1005;
        name = "缚心";
        damageType = DamageType.STR;
        calculationType = CalculationType.NONE;
        influence = 0f;
        lastTurnLimit = 2;
        lastTurns = 2;
        ReduceAtBeginning = true;
        triggerTiming = TriggerTiming.BeHit;
        allowOverlying = false;
        overlyingLimit = 1;

        buffDescriptionText = "\"①debuff的持有者力量伤害后，给该debuff的持有者施加3层dot增伤和该debuff的施加者3层怪谈增伤\"";
    }

    override public void OnEffect(int flag)
    {
        if (flag == 0)  // 此时玩家持有Buff
        {
            BattleBuff buff1 = new BattleBuff_1025();
            BattleBuff buff2 = new BattleBuff_1024();
            for (int i = 0; i < 3; i++)
            {
                TurnCounter.Instance.AddPlayerBuff(buff1);
            }
            for (int i = 0; i < 3; i++)
            {
                TurnCounter.Instance.AddEnemyBuff(buff2, 0);
            }
        }

        else if (flag == 1)
        {
            BattleBuff buff1 = new BattleBuff_1025();
            BattleBuff buff2 = new BattleBuff_1024();
            for (int i = 0; i < 3; i++)
            {
                TurnCounter.Instance.AddPlayerBuff(buff2);
            }
            for (int i = 0; i < 3; i++)
            {
                TurnCounter.Instance.AddEnemyBuff(buff1, 0);
            }
        }
    }

    public override void OnStart(int flag = 0) { }

    public override void OnEnd(int flag = 0) { }

}

public class BattleBuff_1006 : BattleBuff
{
    // 叠加的层数
    public static int overlyingCount = 0;

    public BattleBuff_1006()
    {
        id = 1006;
        name = "余晖";
        damageType = DamageType.Dot | DamageType.Item;
        calculationType = CalculationType.NONE;
        influence = 0f;
        lastTurnLimit = 2;
        lastTurns = 2;
        ReduceAtBeginning = true;
        triggerTiming = TriggerTiming.BeHit;
        allowOverlying = true;
        overlyingLimit = 99;

        buffDescriptionText = "\"①debuff的持有者受到dot伤害或道具伤害后，给该debuff的施加者施加3层力量增伤\"";
    }

    override public void OnEffect(int flag)
    {
        if (flag == 0)  // 此时玩家持有Buff
        {
            BattleBuff buff2 = new BattleBuff_1023();
            for (int i = 0; i < 3; i++)
            {
                TurnCounter.Instance.AddEnemyBuff(buff2, 0);
            }
        }

        else if (flag == 1)
        {
            BattleBuff buff2 = new BattleBuff_1023();
            for (int i = 0; i < 3; i++)
            {
                TurnCounter.Instance.AddPlayerBuff(buff2);
            }
        }
    }

    public override void OnStart(int flag = 0) { }

    public override void OnEnd(int flag = 0) { }

}

public class BattleBuff_1007 : BattleBuff
{
    // 叠加的层数
    public static int overlyingCount = 0;

    public BattleBuff_1007()
    {
        id = 1007;
        name = "战斗意志";
        damageType = DamageType.Skill;
        calculationType = CalculationType.NONE;
        influence = 0f;
        lastTurnLimit = 2;
        lastTurns = 2;
        ReduceAtBeginning = true;
        triggerTiming = TriggerTiming.BeHit;
        allowOverlying = true;
        overlyingLimit = 99;

        buffDescriptionText = "\"①debuff的持有者受到怪谈伤害后，给debuff的施加者施加3层力量增伤\"";
    }

    override public void OnEffect(int flag)
    {
        if (flag == 0)  // 此时玩家持有Buff
        {
            BattleBuff buff2 = new BattleBuff_1023();
            for (int i = 0; i < 3; i++)
            {
                TurnCounter.Instance.AddEnemyBuff(buff2, 0);
            }
        }

        else if (flag == 1)
        {
            BattleBuff buff2 = new BattleBuff_1023();
            for (int i = 0; i < 3; i++)
            {
                TurnCounter.Instance.AddPlayerBuff(buff2);
            }
        }
    }

    public override void OnStart(int flag = 0) { }

    public override void OnEnd(int flag = 0) { }

}

public class BattleBuff_1008 : BattleBuff
{
    // 叠加的层数
    public static int overlyingCount = 0;

    public BattleBuff_1008()
    {
        id = 1008;
        name = "魔音污染";
        damageType = DamageType.Dot;
        calculationType = CalculationType.NONE;
        influence = 0f;
        lastTurnLimit = 2;
        lastTurns = 2;
        ReduceAtBeginning = true;
        triggerTiming = TriggerTiming.BeHit;
        allowOverlying = false;
        overlyingLimit = 1;

        buffDescriptionText = "\"①debuff的持有者受到dot伤害后，给debuff的施加者施加3层怪谈增伤和3层易伤\"";
    }

    override public void OnEffect(int flag)
    {
        if (flag == 0)  // 此时玩家持有Buff
        {
            BattleBuff buff1 = new BattleBuff_1002();
            BattleBuff buff2 = new BattleBuff_1024();
            for (int i = 0; i < 3; i++)
            {
                TurnCounter.Instance.AddEnemyBuff(buff1, 0);
            }
            for (int i = 0; i < 3; i++)
            {
                TurnCounter.Instance.AddEnemyBuff(buff2, 0);
            }
        }

        else if (flag == 1)
        {
            BattleBuff buff1 = new BattleBuff_1002();
            BattleBuff buff2 = new BattleBuff_1024();
            for (int i = 0; i < 3; i++)
            {
                TurnCounter.Instance.AddPlayerBuff(buff1);
            }
            for (int i = 0; i < 3; i++)
            {
                TurnCounter.Instance.AddPlayerBuff(buff2);
            }
        }
    }

    public override void OnStart(int flag = 0) { }

    public override void OnEnd(int flag = 0) { }

}

public class BattleBuff_1009 : BattleBuff
{
    // 叠加的层数
    public static int overlyingCount = 0;

    public BattleBuff_1009()
    {
        id = 1009;
        name = "伤口感染";
        damageType = DamageType.Dot;
        calculationType = CalculationType.NONE;
        influence = 0f;
        lastTurnLimit = 2;
        lastTurns = 2;
        ReduceAtBeginning = true;
        triggerTiming = TriggerTiming.BeHit;
        allowOverlying = true;
        overlyingLimit = 99;

        buffDescriptionText = "\"①debuff的持有者受到dot伤害后，给debuff的施加者施加3层易伤\"";
    }

    override public void OnEffect(int flag)
    {
        if (flag == 0)  // 此时玩家持有Buff
        {
            BattleBuff buff1 = new BattleBuff_1002();
            for (int i = 0; i < 3; i++)
            {
                TurnCounter.Instance.AddEnemyBuff(buff1, 0);
            }
        }

        else if (flag == 1)
        {
            BattleBuff buff1 = new BattleBuff_1002();
            for (int i = 0; i < 3; i++)
            {
                TurnCounter.Instance.AddPlayerBuff(buff1);
            }
        }
    }

    public override void OnStart(int flag = 0) { }

    public override void OnEnd(int flag = 0) { }

}

public class BattleBuff_1019 : BattleBuff
{
    // 叠加的层数
    public static int overlyingCount = 0;

    public BattleBuff_1019()
    {
        id = 1019;
        name = "鼠群围攻";
        damageType = DamageType.Dot;
        calculationType = CalculationType.NONE;
        influence = 0f;
        lastTurnLimit = 2;
        lastTurns = 2;
        ReduceAtBeginning = true;
        triggerTiming = TriggerTiming.BeHit;
        allowOverlying = true;
        overlyingLimit = 5;

        buffDescriptionText = "\"①debuff的持有者受到dot伤害后，自己+30%连击\"";
    }

    override public void OnEffect(int flag)
    {
        if (flag == 0)  // 此时玩家持有Buff
        {
            BattleManager.Instance.player.HIT.AddValue(0.3f * overlyingCount);
        }
        else if (flag == 1)
        {
            BattleManager.Instance.enemies[0].HP += 0.3f * overlyingCount;
        }
    }

    public override void OnStart(int flag = 0) { }

    public override void OnEnd(int flag = 0) { }

}

public class BattleBuff_1022 : BattleBuff
{
    // 叠加的层数
    public static int overlyingCount = 0;

    public BattleBuff_1022()
    {
        id = 1022;
        name = "守护";
        type = BuffType.HP_Change;
        buffType = BattleBuffType.GoodBuff;
        calculationType = CalculationType.Multiply;
        influence = 0.1f;
        lastTurnLimit = 2;
        lastTurns = 2;
        ReduceAtBeginning = true;
        triggerTiming = TriggerTiming.IsGoingToDie;
        allowOverlying = false;
        overlyingLimit = 1;

        buffDescriptionText = "\"抵挡一次致命伤害。（剩余1点生命值）\"";
    }

    override public void OnEffect(int flag = 0)
    {
        if (flag == 0)
        {
            BattleManager.Instance.player.HP.SetValue(1.0f);
        }
        else if (flag == 1)
        {
            BattleManager.Instance.enemies[0].HP = 0;
        }
    }

    public override void OnStart(int flag = 0) { }

    public override void OnEnd(int flag = 0) { }

}





// 以下为附加类Buff

public class BattleBuff_1023 : BattleBuff
{
    // 叠加的层数
    public static int overlyingCount = 0;

    public BattleBuff_1023()
    {
        id = 1023;
        name = "力量增伤";
        type = BuffType.HP_Change;
        damageType = DamageType.STR;
        buffType = BattleBuffType.GoodBuff;
        calculationType = CalculationType.Multiply;
        influence = 0.1f;
        lastTurnLimit = 2;
        lastTurns = 2;
        ReduceAtBeginning = true;
        triggerTiming = TriggerTiming.CalculateGoodBuffDamage;
        allowOverlying = true;
        overlyingLimit = 99;

        buffDescriptionText = "\"①在对应伤害结算的时候影响公式中的乘区\r\n②层数归零或者倒计时归零时移除该debuff。\"";
    }

    override public void OnEffect(int 占位)
    {
        // 此处不实现方法，具体处理要等到伤害计算时根据influence和calculationType进行计算
    }

    public override void OnStart(int flag = 0) { }

    public override void OnEnd(int flag = 0) { }

}

public class BattleBuff_1024 : BattleBuff
{
    // 叠加的层数
    public static int overlyingCount = 0;

    public BattleBuff_1024()
    {
        id = 1024;
        name = "怪谈技能增伤";
        type = BuffType.HP_Change;
        damageType = DamageType.Skill;
        buffType = BattleBuffType.GoodBuff;
        calculationType = CalculationType.Multiply;
        influence = 0.1f;
        lastTurnLimit = 2;
        lastTurns = 2;
        ReduceAtBeginning = true;
        triggerTiming = TriggerTiming.CalculateGoodBuffDamage;
        allowOverlying = true;
        overlyingLimit = 99;

        buffDescriptionText = "\"①在对应伤害结算的时候影响公式中的乘区\r\n②层数归零或者倒计时归零时移除该debuff。\"";
    }

    override public void OnEffect(int 占位)
    {
        // 此处不实现方法，具体处理要等到伤害计算时根据influence和calculationType进行计算
    }

    public override void OnStart(int flag = 0) { }

    public override void OnEnd(int flag = 0) { }

}

public class BattleBuff_1025 : BattleBuff
{
    // 叠加的层数
    public static int overlyingCount = 0;

    public BattleBuff_1025()
    {
        id = 1025;
        name = "dot增伤";
        type = BuffType.HP_Change;
        damageType = DamageType.Dot;
        buffType = BattleBuffType.GoodBuff;
        calculationType = CalculationType.Multiply;
        influence = 0.5f;
        lastTurnLimit = 2;
        lastTurns = 2;
        ReduceAtBeginning = true;
        triggerTiming = TriggerTiming.CalculateGoodBuffDamage;
        allowOverlying = true;
        overlyingLimit = 99;

        buffDescriptionText = "\"①在对应伤害结算的时候影响公式中的乘区\r\n②层数归零或者倒计时归零时移除该debuff。\"";
    }

    override public void OnEffect(int 占位)
    {
        // 此处不实现方法，具体处理要等到伤害计算时根据influence和calculationType进行计算
    }

    public override void OnStart(int flag = 0) { }

    public override void OnEnd(int flag = 0) { }

}
