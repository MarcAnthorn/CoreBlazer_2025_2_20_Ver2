using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;



[System.Serializable]
public abstract class Item                  //所有道具类的基类
{
    public string name;
    public int id;
    public ItemType type;
    public bool isImmediate;
    public int useTimes;
    public int[] usableScene = new int[] { 0, 0, 0 };   // 1代表可使用
    public bool resetAfterDeath;
    public bool quickEquip;
    public bool reObtain;
    public int maxLimit;                                //???感觉不用上面的reObtain也行
    public bool isPermanent;
    public float EffectiveTime;                         //表示回合时：(int)(EffectiveTime/2)
    public string instruction;                          //表示使用说明
    public string description;                          //表示道具文案

    //Mar添加；判断当前Item是否生效中、是否结束生效的字段：
    //这个字段决定UI更新的时候是否加上黑色蒙版；
    public bool isInUse;
    public Buff buff;
    //Marc添加：道具生效结束之后的回调函数：
    public UnityAction onCompleteCallback = null;
    // 新增：统一的OnComplete委托
    public Action onCompleteDelegate;

    //Marc添加：该道具是否被插入左/右插槽：
    public bool isSlottedToLeft;
    public bool isSlottedToRight;
    public Dictionary<E_AffectPlayerAttributeType, float> effectFinalValueDic = new Dictionary<E_AffectPlayerAttributeType, float>();

    public int timerIndex = -1;

    public enum ItemType
    {
        None = 0,
        God_Maze = 1,
        God_Battle = 2,
        Maze = 3,
        Battle = 4,
        Normal = 5,
        Tarot = 6               //塔罗牌
    }

    public bool CanUseOrNot(int sceneId)        // 1-事件选择 2-战斗场景 3-迷宫内
    {
        //如果是安全屋，直接拒绝使用：
        if(sceneId == 4)
            return false;
        if (usableScene[sceneId - 1] == 1)
        {
            return true;
        }

        return false;
    }

    public abstract void Use();

}



//使用后灯光值立刻+20，短时间内不会衰减
public class Item_101 : Item
{
    public Item_101() { onCompleteDelegate = OnComplete; }
    public override void Use()
    {
        effectFinalValueDic.Clear();
        Debug.Log($"道具 \"灯火助燃剂\" 使用！");
        timerIndex = TimeManager.Instance.AddTimer(8f, () => OnStart(), () => OnComplete());
        
        PlayerController.SetPlayerAttribute(AttributeType.LVL, PlayerManager.Instance.player.LVLValue + 40);
    }

    private void OnStart()
    {
        // PlayerManager.Instance.PlayerAttributeChange(AttributeType.LVL, +40f);

        EventHub.Instance.EventTrigger("ResumeLight", 40f);
        PlayerManager.Instance.player.DebugInfo();
        //锁定玩家的灯光值：
        EventHub.Instance.EventTrigger("TriggerLightShrinking", false);
    }

    private void OnComplete()
    {
        onCompleteCallback?.Invoke();
        // PlayerManager.Instance.PlayerAttributeChange(AttributeType.LVL, -40f);
        PlayerManager.Instance.player.DebugInfo();
        //解锁：
        EventHub.Instance.EventTrigger("TriggerLightShrinking", true);
    }

}

public class Item_102 : Item
{
    public override void Use()
    {
        effectFinalValueDic.Clear();
        Debug.Log($"道具 \"封存的灯火\" 使用！");
        //获得后，后续每次额外获得2灯光值
        // BuffManager.Instance.AddBuff(UseCase.GrowUp, BuffType.LVL_Change, CalculationType.Add, +2f);     //代表加成

        EventHub.Instance.AddEventListener("AddExtraLight", AddExtraLight);


        PlayerManager.Instance.player.DebugInfo();
    }


    private void AddExtraLight()
    {
        // 使用统一的PlayerController.SetPlayerAttribute方法同时设置LVL值和上限
        var currentLVL = PlayerManager.Instance.player.LVLValue;
        var currentLimit = PlayerManager.Instance.player.LVL.value_limit;
        PlayerController.SetPlayerAttribute(AttributeType.LVL, currentLVL, currentLimit + 10);
    }


}

public class Item_103 : Item
{
    public Item_103() { onCompleteDelegate = UsedCallback; }

    public override void Use()
    {
        effectFinalValueDic.Clear();
        Debug.Log($"道具 \"大力出奇迹\" 使用！");

        EventHub.Instance.EventTrigger<bool>("LockBreakerOrNot", false);    //解锁：

        //注册取消道具的方法：
        EventHub.Instance.AddEventListener("UsedCallback", UsedCallback);

        // //使用后靠近特殊墙壁可砸碎
        // int timerIndex;
        // timerIndex = TimeManager.Instance.AddTimer(8f, () => OnStart(), () => OnComplete());
        // PlayerManager.Instance.player.DebugInfo();
    }

    private void UsedCallback()
    {
        //移除对应的响应事件：
        EventHub.Instance.RemoveEventListener("UsedCallback", UsedCallback);

        EventHub.Instance.EventTrigger<bool>("LockBreakerOrNot", true);    //加锁：

        //执行生效结束后的回调：
        onCompleteCallback?.Invoke();

    }

    // int index;
    // private void OnStart()
    // {
    //     index = BuffManager.Instance.AddBuff(UseCase.Maze, BuffType.LVL_Change, SpecialBuffType.DamageWall, DamageWall);
    // }

    // private void OnComplete()
    // {
    //     onCompleteCallback?.Invoke();
    //     BuffManager.Instance.RemoveBuff(index);
    // }

    // private Func<float> DamageWall = null;                   //由Marc将实现方法写入其中
    // public void SubscribeHandler(Func<float> handler)
    // {
    //     if (DamageWall == null)
    //     {
    //         DamageWall += handler;
    //     }
    // }
}
/*
    NONE = 0,
    HP = 1,
    STR = 2,
    DEF = 3,
    LVL = 4,
    SAN = 5,
    SPD = 6,
    CRIT_Rate = 7,
    CRIT_DMG = 8,
    HIT = 9,
    AVO = 10
}*/
public class Item_104 : Item
{
    public override void Use()
    {
        effectFinalValueDic.Clear();
        Debug.Log($"道具 \"传送门\" 使用！");
        //使用后有50%概率直接到达该层迷宫终点，也有50%概率回到起点
        int randomNum = UnityEngine.Random.Range(0, 2);
        if (randomNum == 0)
        {

            Debug.LogWarning("To Start Point");
            StartHander();
        }
        else if (randomNum == 1)
        {

            Debug.LogWarning("To End Point");
            EndHander();
        }

        UIManager.Instance.HidePanel<InventoryPanel>();
        
        // EventHub.Instance.EventTrigger("RefreshItemsInPanel");
    }

    // private Action BackToStartPoint = null;             //由Marc将实现方法写入其中（已完成）
    // private Action ReachToFinalPoint = null;            //由Marc将实现方法写入其中

    private void StartHander()
    {
        EventHub.Instance.EventTrigger<Vector3>("SetPlayerPosition", GameLevelManager.Instance.mapIndexStartPointDic[(int)GameLevelManager.Instance.gameLevelType]);
    }

    private void EndHander()
    {
        EventHub.Instance.EventTrigger<Vector3>("SetPlayerPosition", GameLevelManager.Instance.mapIndexEndPointDic[(int)GameLevelManager.Instance.gameLevelType]);
    }


}

public class Item_201 : Item
{
    public override void Use()
    {
        effectFinalValueDic.Clear();
        Debug.Log($"道具 \"除你武器!\" 使用！");
        //使用后敌方陷入短暂眩晕6s
        MakeDizzy.Invoke();

    }

    private Action MakeDizzy = null;                   //由Marc将实现方法写入其中
    public void SubscribeHandler(Action handler)
    {
        if (MakeDizzy == null)
        {
            MakeDizzy += handler;
        }
    }

}

public class Item_202 : Item
{
    public Item_202() { onCompleteDelegate = OnComplete; }
    public override void Use()
    {
        effectFinalValueDic.Clear();
        Debug.Log($"道具 \"燃血战神\" 使用！");
        //生命值高于50%时，可扣除自身当前生命值的80%，使自身暴击率提升100%
        timerIndex = TimeManager.Instance.AddTimer(6f, () => OnStart(), () => OnComplete());
        PlayerManager.Instance.player.DebugInfo();
        
        // 使用统一的PlayerController.SetPlayerAttribute方法设置HP值，保持原有上限
        var currentHP = PlayerManager.Instance.player.HPValue;
        var hpLimit = PlayerManager.Instance.player.HP.value_limit;
        PlayerController.SetPlayerAttribute(AttributeType.HP, currentHP * 0.2f);
        
        var currentCritRate = PlayerManager.Instance.player.CRIT_RateValue;
        PlayerManager.Instance.player.AddAttrValue(AttributeType.CRIT_Rate, 1f);
    }

    bool isEffected;
    private void OnStart()
    {
        bool condition1 = (PlayerManager.Instance.player.HPValue / PlayerManager.Instance.player.HP.value_limit) >= 0.5;
        if (condition1)
        {
            // 使用PlayerController.SetPlayerAttribute统一设置属性值
            var currentHP = PlayerManager.Instance.player.HPValue;
            var hpLimit = PlayerManager.Instance.player.HP.value_limit;
            PlayerController.SetPlayerAttribute(AttributeType.HP, Mathf.Min(currentHP * 0.2f, hpLimit));
            
            var currentCritRate = PlayerManager.Instance.player.CRIT_RateValue;
            var critLimit = PlayerManager.Instance.player.CRIT_Rate.value_limit;
            PlayerController.SetPlayerAttribute(AttributeType.CRIT_Rate, Mathf.Min(currentCritRate + 1f, critLimit));
            isEffected = true;
        }
    }

    private void OnComplete()
    {
        onCompleteCallback?.Invoke();
        if (isEffected)
        {
            // 使用PlayerController.SetPlayerAttribute统一恢复属性值
            var currentHP = PlayerManager.Instance.player.HPValue;
            var hpLimit = PlayerManager.Instance.player.HP.value_limit;
            PlayerController.SetPlayerAttribute(AttributeType.HP, Mathf.Min(currentHP / 0.2f, hpLimit));
            
            var currentCritRate = PlayerManager.Instance.player.CRIT_RateValue;
            PlayerController.SetPlayerAttribute(AttributeType.CRIT_Rate, Mathf.Max(currentCritRate - 1f, 0));
        }
    }

}

public class Item_203 : Item
{
    public override void Use()
    {
        effectFinalValueDic.Clear();
        Debug.Log($"道具 \"神愈术\" 使用！");

        effectFinalValueDic.Add(E_AffectPlayerAttributeType.生命值, PlayerManager.Instance.player.HP.value_limit);

        //使用后生命值回满
        PlayerController.SetPlayerAttribute(AttributeType.HP, PlayerManager.Instance.player.HP.value_limit);
        PlayerManager.Instance.player.DebugInfo();
    }

}

public class Item_204 : Item
{
    public Item_204() { onCompleteDelegate = OnComplete; }
    public override void Use()
    {
        effectFinalValueDic.Clear();
        Debug.Log($"道具 \"最强防御\" 使用！");
        //使用后防御值短时间*1.5倍，在此期间暴击率归0
        timerIndex = TimeManager.Instance.AddTimer(12f, () => OnStart(), () => OnComplete());
        PlayerManager.Instance.player.DebugInfo();
        PlayerController.SetPlayerAttribute(AttributeType.DEF, Mathf.Min(PlayerManager.Instance.player.DEFValue * 1.5f, PlayerManager.Instance.player.DEF.value_limit));
        PlayerController.SetPlayerAttribute(AttributeType.CRIT_Rate, 0);
    }

    float temp;
    float defenseValueTmp;
    private void OnStart()
    {
        // 使用PlayerController.SetPlayerAttribute统一设置防御值，保持上限不变
        var currentDEF = PlayerManager.Instance.player.DEFValue;
        var defLimit = PlayerManager.Instance.player.DEF.value_limit;
        defenseValueTmp = currentDEF * 1.5f;
        PlayerController.SetPlayerAttribute(AttributeType.DEF, Mathf.Min(defenseValueTmp, defLimit));

        temp = PlayerManager.Instance.player.CRIT_RateValue;
        PlayerController.SetPlayerAttribute(AttributeType.CRIT_Rate, 0); // 将暴击率归0
    }

    private void OnComplete()
    {
        onCompleteCallback?.Invoke();

        // 使用PlayerController.SetPlayerAttribute统一恢复属性值
        var originalDEF = defenseValueTmp / 1.5f; // 计算原始防御值
        PlayerController.SetPlayerAttribute(AttributeType.DEF, originalDEF);
        PlayerController.SetPlayerAttribute(AttributeType.CRIT_Rate, temp); // 恢复原来的暴击率值
    }

}

public class Item_205 : Item
{
    public Item_205() { onCompleteDelegate = OnComplete; }
    public override void Use()
    {
        effectFinalValueDic.Clear();
        Debug.Log($"道具 \"最强攻击\" 使用！");
        //使用后力量短时间*1.5倍，在此期间防御值归0
        timerIndex = TimeManager.Instance.AddTimer(12f, () => OnStart(), () => OnComplete());
        PlayerManager.Instance.player.DebugInfo();
        PlayerController.SetPlayerAttribute(AttributeType.STR, Mathf.Min(PlayerManager.Instance.player.STRValue * 1.5f,PlayerManager.Instance.player.STR.value_limit));

    }

    float temp;
    private void OnStart()
    {
        // 使用PlayerController.SetPlayerAttribute统一设置力量值，保持上限不变
        var currentSTR = PlayerManager.Instance.player.STRValue;
        var strLimit = PlayerManager.Instance.player.STR.value_limit;
        PlayerController.SetPlayerAttribute(AttributeType.STR, Mathf.Min(currentSTR * 1.5f, strLimit));
        
        temp = PlayerManager.Instance.player.DEF.value;
        PlayerController.SetPlayerAttribute(AttributeType.DEF, 0); // 将防御值归0
    }

    private void OnComplete()
    {
        onCompleteCallback?.Invoke();
        // 使用PlayerController.SetPlayerAttribute统一恢复属性值
        var currentSTR = PlayerManager.Instance.player.STRValue;
        var strLimit = PlayerManager.Instance.player.STR.value_limit;
        PlayerController.SetPlayerAttribute(AttributeType.STR, Mathf.Min(currentSTR / 1.5f, strLimit));
        PlayerController.SetPlayerAttribute(AttributeType.DEF, temp); // 恢复原来的防御值
    }

}

public class Item_206 : Item
{
    public Item_206() { onCompleteDelegate = OnComplete; }
    public override void Use()
    {
        effectFinalValueDic.Clear();
        Debug.Log($"道具 \"最强闪避\" 使用！");
        //使用后闪避短时间*1.8倍，在此期间受到的伤害*1.5倍
        timerIndex = TimeManager.Instance.AddTimer(12f, () => OnStart(), () => OnComplete());
        PlayerManager.Instance.player.DebugInfo();
        effectFinalValueDic.Add(E_AffectPlayerAttributeType.闪避值, Mathf.Min(PlayerManager.Instance.player.AVO.value * 1.8f,PlayerManager.Instance.player.AVO.value_limit));
    }

    int index;
    private void OnStart()
    {
        // 使用PlayerController.SetPlayerAttribute统一设置闪避值
        var currentAVO = PlayerManager.Instance.player.AVO.value;
        var avoLimit = PlayerManager.Instance.player.AVO.value_limit;
        PlayerController.SetPlayerAttribute(AttributeType.AVO, Mathf.Min(currentAVO * 1.8f, avoLimit));
        index = BuffManager.Instance.AddBuff(UseCase.Battle, BuffType.HP_Change, CalculationType.Multiply, 1.0f / 1.5f);    //减益表示为除法
    }

    private void OnComplete()
    {
        onCompleteCallback?.Invoke();
        // 使用PlayerController.SetPlayerAttribute统一恢复闪避值
        var currentAVO = PlayerManager.Instance.player.AVO.value;
        var avoLimit = PlayerManager.Instance.player.AVO.value_limit;
        PlayerController.SetPlayerAttribute(AttributeType.AVO, Mathf.Min(currentAVO / 1.8f, avoLimit));
        BuffManager.Instance.RemoveBuff(index);
    }

}


public class Item_207 : Item
{
    public Item_207() { onCompleteDelegate = OnComplete; }
    public override void Use()
    {
        effectFinalValueDic.Clear();
        Debug.Log($"道具 \"最强苟命王\" 使用！");
        //使用后短时间内生命值冻结，技能结束前未结束战斗生命值归0
        timerIndex = TimeManager.Instance.AddTimer(12f, () => OnStart(), () => OnComplete());
        PlayerManager.Instance.player.DebugInfo();
    }

    int index;
    private void OnStart()
    {
        index = BuffManager.Instance.AddBuff(UseCase.Battle, BuffType.HP_Change, SpecialBuffType.NoDamage, () => BuffFunction());
    }

    private void OnComplete()
    {
        onCompleteCallback?.Invoke();
        PlayerController.SetPlayerAttribute(AttributeType.HP, 0);
        BuffManager.Instance.RemoveBuff(index);
    }

    private float BuffFunction()
    {
        return 0;   //表示将finalValue设置为0
    }

}

public class Item_208 : Item
{
    public Item_208() { onCompleteDelegate = OnComplete; }
    public override void Use()
    {
        effectFinalValueDic.Clear();
        Debug.Log($"道具 \"绝地反击\" 使用！");
        //生命值低于20%时可使用，使用后短时间内暴击率提升100%、攻击*1.5倍
        timerIndex = TimeManager.Instance.AddTimer(12f, () => OnStart(), () => OnComplete());
        PlayerManager.Instance.player.DebugInfo();
        effectFinalValueDic.Add(E_AffectPlayerAttributeType.暴击率, Mathf.Min(PlayerManager.Instance.player.CRIT_Rate.value + 1f,PlayerManager.Instance.player.CRIT_Rate.value_limit));
        effectFinalValueDic.Add(E_AffectPlayerAttributeType.力量值, Mathf.Min(PlayerManager.Instance.player.STRValue * 1.5f,PlayerManager.Instance.player.STR.value_limit));
    }

    private void OnStart()
    {
        // 使用PlayerController.SetPlayerAttribute统一设置暴击率和力量值
        var currentCritRate = PlayerManager.Instance.player.CRIT_RateValue;
        var critLimit = PlayerManager.Instance.player.CRIT_Rate.value_limit;
        PlayerController.SetPlayerAttribute(AttributeType.CRIT_Rate, Mathf.Min(currentCritRate + 1f, critLimit));
        
        var currentSTR = PlayerManager.Instance.player.STRValue;
        var strLimit = PlayerManager.Instance.player.STR.value_limit;
        PlayerController.SetPlayerAttribute(AttributeType.STR, Mathf.Min(currentSTR * 1.5f, strLimit));
    }

    private void OnComplete()
    {
        onCompleteCallback?.Invoke();
        // 使用PlayerController.SetPlayerAttribute统一恢复属性值
        var currentCritRate = PlayerManager.Instance.player.CRIT_RateValue;
        PlayerController.SetPlayerAttribute(AttributeType.CRIT_Rate, Mathf.Max(currentCritRate - 1f, 0));
        
        var currentSTR = PlayerManager.Instance.player.STRValue;
        var strLimit = PlayerManager.Instance.player.STR.value_limit;
        PlayerController.SetPlayerAttribute(AttributeType.STR, Mathf.Min(currentSTR / 1.5f, strLimit));
    }

}

public class Item_301 : Item
{
    public override void Use()
    {
        effectFinalValueDic.Clear();
        Debug.Log($"道具 \"灯光up\" 使用！");

        effectFinalValueDic.Add(E_AffectPlayerAttributeType.灯光值, Mathf.Min(PlayerManager.Instance.player.LVLValue + 20,PlayerManager.Instance.player.LVL.value_limit));
        
        //获得后灯光值+20
        PlayerManager.Instance.player.AddAttrValue(AttributeType.LVL, 20);
        PlayerManager.Instance.player.DebugInfo();

        
    }

}

public class Item_302 : Item
{
    public Item_302() { onCompleteDelegate = OnComplete; }
    public override void Use()
    {
        effectFinalValueDic.Clear();
        Debug.Log($"道具 \"速!速!速!\" 使用！");
        //使用后在迷宫内移动速度短时间翻倍
        timerIndex = TimeManager.Instance.AddTimer(10f, () => OnStart(), () => OnComplete());
        PlayerManager.Instance.player.DebugInfo();
    }

    private void OnStart()
    {
        // 使用PlayerController.SetPlayerAttribute统一设置速度值
        var currentSPD = PlayerManager.Instance.player.SPDValue;
        var spdLimit = PlayerManager.Instance.player.SPD.value_limit;
        PlayerController.SetPlayerAttribute(AttributeType.SPD, Mathf.Min(currentSPD * 2f, spdLimit));
    }

    private void OnComplete()
    {
        onCompleteCallback?.Invoke();
        // 使用PlayerController.SetPlayerAttribute统一恢复速度值
        var currentSPD = PlayerManager.Instance.player.SPDValue;
        var spdLimit = PlayerManager.Instance.player.SPD.value_limit;
        PlayerController.SetPlayerAttribute(AttributeType.SPD, Mathf.Min(currentSPD / 2f, spdLimit));
    }

}

public class Item_303 : Item
{
    public override void Use()
    {
        effectFinalValueDic.Clear();
        Debug.Log($"道具 \"精神恢复剂\" 使用！");

        effectFinalValueDic.Add(E_AffectPlayerAttributeType.精神值, Mathf.Min(PlayerManager.Instance.player.SANValue + 3,PlayerManager.Instance.player.SAN.value_limit));

        //使用后当前精神值+3
        PlayerManager.Instance.player.AddAttrValue(AttributeType.SAN, 3);
        PlayerManager.Instance.player.DebugInfo();

         
    }

}

public class Item_401 : Item
{
    public Item_401() { onCompleteDelegate = OnComplete; }
    public override void Use()
    {
        effectFinalValueDic.Clear();
        Debug.Log($"道具 \"生命果实\" 使用！");
        //生命值上限+10
        timerIndex = TimeManager.Instance.AddTimer(12f, () => OnStart(), () => OnComplete());
        PlayerManager.Instance.player.DebugInfo();
        effectFinalValueDic.Add(E_AffectPlayerAttributeType.生命值, PlayerManager.Instance.player.HPValue + 10);
        effectFinalValueDic.Add(E_AffectPlayerAttributeType.生命值上限, PlayerManager.Instance.player.HP.value_limit + 10);
    }

    private void OnStart()
    {
        //AddValueLimit方法已修正：所有需要调整数值&上限值的，直接使用这个方法；该方法会同步加成数值和上限值；
        PlayerManager.Instance.player.HP.AddValueLimit(10f);        
    }

    private void OnComplete()
    {
        onCompleteCallback?.Invoke();
        PlayerManager.Instance.player.HP.AddValueLimit(-10f);
    }

}

public class Item_402 : Item
{
    public Item_402() { onCompleteDelegate = OnComplete; }
    public override void Use()
    {
        effectFinalValueDic.Clear();
        Debug.Log($"道具 \"防御果实\" 使用！");
        //防御值上限+10
        timerIndex = TimeManager.Instance.AddTimer(12f, () => OnStart(), () => OnComplete());
        PlayerManager.Instance.player.DebugInfo();
        effectFinalValueDic.Add(E_AffectPlayerAttributeType.防御值, PlayerManager.Instance.player.DEF.value + 10);
        effectFinalValueDic.Add(E_AffectPlayerAttributeType.防御值上限, PlayerManager.Instance.player.DEF.value_limit + 10);
    }

    private void OnStart()
    {
        PlayerManager.Instance.player.DEF.AddValueLimit(10f);

    }

    private void OnComplete()
    {
        onCompleteCallback?.Invoke();
        PlayerManager.Instance.player.DEF.AddValueLimit(-10f);
    }

}

public class Item_403 : Item
{
    public Item_403() { onCompleteDelegate = OnComplete; }
    public override void Use()
    {
        effectFinalValueDic.Clear();
        Debug.Log($"道具 \"攻击果实\" 使用！");
        //使用后力量+20
        timerIndex = TimeManager.Instance.AddTimer(18f, () => OnStart(), () => OnComplete());
        effectFinalValueDic.Add(E_AffectPlayerAttributeType.力量值, Mathf.Min(PlayerManager.Instance.player.STRValue + 20,PlayerManager.Instance.player.STR.value_limit));
    }

    private void OnStart()
    {
        // PlayerManager.Instance.player.STR.value_limit += 10f;
        PlayerManager.Instance.player.STR.AddValue(20f);
        PlayerManager.Instance.player.DebugInfo();
    }

    private void OnComplete()
    {
        onCompleteCallback?.Invoke();
        // PlayerManager.Instance.player.STR.value_limit -= 10f;
        PlayerManager.Instance.player.STR.AddValue(-20f);
        PlayerManager.Instance.player.DebugInfo();
    }

}

public class Item_404 : Item
{
    public override void Use()
    {
        effectFinalValueDic.Clear();
        Debug.Log($"道具 \"嫁衣\" 使用！");
        //受到致命伤时使用，可抵消一次致命伤害
        BuffManager.Instance.AddBuff(UseCase.AfterDie, BuffType.HP_Change, SpecialBuffType.OnceDontDie, () => OnceDontDie());
        PlayerManager.Instance.player.DebugInfo();
    }

    //此处应该在战斗系统定义完毕后完善，记录受到致命伤害前 角色的HP
    private float OnceDontDie()
    {
        return 1;   //暂时定义为剩余 1 点血量
    }

}

public class Item_501 : Item
{
    public override void Use()
    {
        effectFinalValueDic.Clear();
        Debug.Log($"道具 \"回血药\" 使用！");

        effectFinalValueDic.Add(E_AffectPlayerAttributeType.生命值, Mathf.Min(PlayerManager.Instance.player.HPValue + 5,PlayerManager.Instance.player.HP.value_limit));

        //当前生命值+5
        PlayerManager.Instance.player.AddAttrValue(AttributeType.HP, 5f);
        PlayerManager.Instance.player.DebugInfo();

        
    }

}

public class Item_502 : Item
{
    public override void Use()
    {
        effectFinalValueDic.Clear();
        Debug.Log($"道具 \"护身甲\" 使用！");

        effectFinalValueDic.Add(E_AffectPlayerAttributeType.防御值, Mathf.Min(PlayerManager.Instance.player.DEFValue + 10,PlayerManager.Instance.player.DEF.value_limit));

        //获得后防御+10
        PlayerManager.Instance.player.AddAttrValue(AttributeType.DEF, 10f);
        PlayerManager.Instance.player.DebugInfo();

        
    }

}

public class Item_503 : Item
{
    public override void Use()
    {
        effectFinalValueDic.Clear();
        Debug.Log($"道具 \"闪避\" 使用！");

        effectFinalValueDic.Add(E_AffectPlayerAttributeType.闪避值, Mathf.Min(PlayerManager.Instance.player.AVO.value + 10,PlayerManager.Instance.player.AVO.value_limit));

        //获得后闪避+10
        PlayerManager.Instance.player.AddAttrValue(AttributeType.AVO, 10f);
        PlayerManager.Instance.player.DebugInfo();

        
    }

}

public class Item_504 : Item
{
    public override void Use()
    {
        effectFinalValueDic.Clear();
        Debug.Log($"道具 \"重拳出击\" 使用！");
        effectFinalValueDic.Add(E_AffectPlayerAttributeType.力量值, Mathf.Min(PlayerManager.Instance.player.STRValue + 10,PlayerManager.Instance.player.STR.value_limit));

        //获得后力量+10
        PlayerManager.Instance.player.AddAttrValue(AttributeType.STR, 10f);
        PlayerManager.Instance.player.DebugInfo();

        
    }

}

public class Item_505 : Item
{
    public override void Use()
    {
        effectFinalValueDic.Clear();

        effectFinalValueDic.Add(E_AffectPlayerAttributeType.精神值上限, PlayerManager.Instance.player.SAN.value_limit - 10);

        //获得后精神值上限-10
        PlayerManager.Instance.player.SAN.AddValueLimit(-10f);
        PlayerManager.Instance.player.DebugInfo();

        
    }

}

public class Item_506 : Item
{
    public override void Use()
    {
        effectFinalValueDic.Clear();
        Debug.Log($"道具 \"木剑\" 使用！");

         effectFinalValueDic.Add(E_AffectPlayerAttributeType.力量值, Mathf.Min(PlayerManager.Instance.player.STRValue + 10,PlayerManager.Instance.player.STR.value_limit));

        //获得后攻击+10
        PlayerManager.Instance.player.AddAttrValue(AttributeType.STR, 10f);
        PlayerManager.Instance.player.DebugInfo();

       
    }

}

public class Item_507 : Item
{
    public override void Use()
    {
        effectFinalValueDic.Clear();
        Debug.Log($"道具 \"宝剑\" 使用！");

        effectFinalValueDic.Add(E_AffectPlayerAttributeType.力量值, Mathf.Min(PlayerManager.Instance.player.STRValue + 40,PlayerManager.Instance.player.STR.value_limit));

        //获得后攻击+40
        PlayerManager.Instance.player.AddAttrValue(AttributeType.STR, 40f);
        PlayerManager.Instance.player.DebugInfo();

        
    }


}

public class Item_508 : Item
{
    public override void Use()
    {
        effectFinalValueDic.Clear();
        Debug.Log($"道具 \"神奇四面骰\" 使用！");
        //使用后随机获得以下影响：精神值上限+20、精神值上限+10、精神值不变、精神值变为10
        int randomNum = UnityEngine.Random.Range(0, 4);
        if (randomNum == 0)
        {
            effectFinalValueDic.Add(E_AffectPlayerAttributeType.精神值上限, PlayerManager.Instance.player.SAN.value_limit + 20);
            PlayerManager.Instance.player.SAN.AddValueLimit(20f);

            
        }
        else if (randomNum == 1)
        {
            effectFinalValueDic.Add(E_AffectPlayerAttributeType.精神值上限, PlayerManager.Instance.player.SAN.value_limit + 10);
            PlayerManager.Instance.player.SAN.AddValueLimit(10f);
            
        }
        else if (randomNum == 2)
        {
            PlayerManager.Instance.player.SAN.AddValue(0f);
        }
        else if (randomNum == 3)
        {
            effectFinalValueDic.Add(E_AffectPlayerAttributeType.精神值, 10);
            PlayerManager.Instance.player.SAN.AddValueLimit(-10f);
            PlayerManager.Instance.player.SAN.AddValue(-10f);
            
        }
        PlayerManager.Instance.player.DebugInfo();
    }

}

public class Item_509 : Item
{
    public override void Use()
    {
        
    }
}

//返回骨片
public class Item_510 : Item
{
    Transform playerTransform;
    public override void Use()
    {
        if(playerTransform == null)
        {
            playerTransform = PlayerManager.Instance.PlayerTransform;
        }

        //清除所有可能的层级buff：
        EventHub.Instance.EventTrigger("ResetFloorDiffer");
        GameLevelManager.Instance.lastTeleportPoint = playerTransform.position;
        LoadSceneManager.Instance.LoadSceneAsync("ShelterScene");

    }
}


public class Item_511 : Item
{
    public Item_511() { onCompleteDelegate = UsedCallback; }

    public override void Use()
    {
        effectFinalValueDic.Clear();

        PlayerManager.Instance.player.HP.AddValueLimit(100);
        PlayerManager.Instance.player.DEF.AddValueLimit(15);
        //注册取消道具的方法：
        EventHub.Instance.AddEventListener("UsedCallback", UsedCallback);
        
    }

    private void UsedCallback()
    {
        //移除对应的响应事件：
        EventHub.Instance.RemoveEventListener("UsedCallback", UsedCallback);

        PlayerManager.Instance.player.HP.AddValueLimit(-100);
        PlayerManager.Instance.player.DEF.AddValueLimit(-15);

        //执行生效结束后的回调：
        onCompleteCallback?.Invoke();

    }
}


public class Item_512 : Item
{
    public Item_512() { onCompleteDelegate = UsedCallback; }

    public override void Use()
    {
        effectFinalValueDic.Clear();

        PlayerManager.Instance.player.SAN.AddValue(164);
        PlayerManager.Instance.player.AVO.AddValue(0.15f);
        //注册取消道具的方法：
        EventHub.Instance.AddEventListener("UsedCallback", UsedCallback);
        
    }

    private void UsedCallback()
    {
        //移除对应的响应事件：
        EventHub.Instance.RemoveEventListener("UsedCallback", UsedCallback);

        // PlayerManager.Instance.player.SAN.AddValue(-164);
        PlayerManager.Instance.player.AVO.AddValue(-0.15f);

        //执行生效结束后的回调：
        onCompleteCallback?.Invoke();

    }
}


public class Item_513 : Item
{
   public Item_513() { onCompleteDelegate = UsedCallback; }
    public override void Use()
    {
        effectFinalValueDic.Clear();

        PlayerManager.Instance.player.STR.AddValueLimit(20);
        PlayerManager.Instance.player.CRIT_DMG.AddValueLimit(0.25f);
        PlayerManager.Instance.player.CRIT_Rate.AddValueLimit(0.25f);
        PlayerManager.Instance.player.HIT.AddValueLimit(0.35f);
        //注册取消道具的方法：
        EventHub.Instance.AddEventListener("UsedCallback", UsedCallback);
        
    }

    private void UsedCallback()
    {
        //移除对应的响应事件：
        EventHub.Instance.RemoveEventListener("UsedCallback", UsedCallback);

       
        PlayerManager.Instance.player.STR.AddValueLimit(-20);
        PlayerManager.Instance.player.CRIT_DMG.AddValueLimit(-0.25f);
        PlayerManager.Instance.player.CRIT_Rate.AddValueLimit(-0.25f);
        PlayerManager.Instance.player.HIT.AddValueLimit(-0.35f);

        //执行生效结束后的回调：
        onCompleteCallback?.Invoke();

    }
}


public class Item_514 : Item
{
    public Item_514() { onCompleteDelegate = UsedCallback; }

    private bool isTriggered = false;
    private float strDelta;
    private float hpDelta;
    public override void Use()
    {
        effectFinalValueDic.Clear();

        EventHub.Instance.AddEventListener("Item514OnEffect", Item514OnEffect);
        EventHub.Instance.AddEventListener("Item514OnEffectOff", Item514OnEffectOff);

        //注册取消道具的方法：
        EventHub.Instance.AddEventListener("UsedCallback", UsedCallback);



    }

    //在死亡后手动执行的道具销毁：
    private void UsedCallback()
    {
        //移除对应的响应事件：
        EventHub.Instance.RemoveEventListener("UsedCallback", UsedCallback);

        EventHub.Instance.RemoveEventListener("Item514OnEffect", Item514OnEffect);
        EventHub.Instance.RemoveEventListener("Item514OnEffectOff", Item514OnEffectOff);

        //如果生效过，那么需要取消生效：
        if(isTriggered)
        {
            Item514OnEffectOff();
        }

        //执行生效结束后的回调：
        onCompleteCallback?.Invoke();

    }


    private void Item514OnEffect()
    {
        isTriggered = true;
        strDelta = PlayerManager.Instance.player.STRValue;
        hpDelta = PlayerManager.Instance.player.HPValue;
        //道具效果：
        Debug.Log("Item514OnEffect");
        PlayerController.SetPlayerAttribute(AttributeType.STR, strDelta * 2);
        PlayerController.SetPlayerAttribute(AttributeType.HP, hpDelta + 25, PlayerManager.Instance.player.HP.value_limit + 25);
        PlayerController.SetPlayerAttribute(AttributeType.LVL, PlayerManager.Instance.player.LVL.value_limit + 150, PlayerManager.Instance.player.LVL.value_limit + 150);
        PlayerController.SetPlayerAttribute(AttributeType.LVL, PlayerManager.Instance.player.LVL.value_limit - 0.3f, PlayerManager.Instance.player.LVL.value_limit);

        UIManager.Instance.ShowPanel<WarningPanel>().SetWarningText("道具「科拉佐斯的思绪」已生效");
        

    }

    private void Item514OnEffectOff()
    {
        if (isTriggered)
        {
            Debug.Log("Item514OnEffectOff");
            isTriggered = false;
            PlayerController.SetPlayerAttribute(AttributeType.STR, strDelta);
            PlayerController.SetPlayerAttribute(AttributeType.HP, PlayerManager.Instance.player.HP.value_limit - 25, PlayerManager.Instance.player.HP.value_limit - 25);
            PlayerController.SetPlayerAttribute(AttributeType.LVL, PlayerManager.Instance.player.LVLValue - 150, PlayerManager.Instance.player.LVL.value_limit - 150);
            PlayerController.SetPlayerAttribute(AttributeType.LVL, PlayerManager.Instance.player.LVLValue);

            UIManager.Instance.ShowPanel<WarningPanel>().SetWarningText("道具「科拉佐斯的思绪」已不再生效");
            
        }
        
    }
}


public class Item_515 : Item
{
    public Item_515() { onCompleteDelegate = UsedCallback; }

    private bool isTriggered = false;
    private float strDelta;
    public override void Use()
    {
        effectFinalValueDic.Clear();

        EventHub.Instance.AddEventListener("Item515OnEffect", Item515OnEffect);
        EventHub.Instance.AddEventListener("Item515OnEffectOff", Item515OnEffectOff);

        //注册取消道具的方法：
        EventHub.Instance.AddEventListener("UsedCallback", UsedCallback);
        
        

    }

    //在死亡后手动执行的道具销毁：
    private void UsedCallback()
    {
        //移除对应的响应事件：
        EventHub.Instance.RemoveEventListener("UsedCallback", UsedCallback);

        EventHub.Instance.RemoveEventListener("Item515OnEffect", Item515OnEffect);
        EventHub.Instance.RemoveEventListener("Item515OnEffectOff", Item515OnEffectOff);

        //如果生效过，那么需要取消生效：
        if(isTriggered)
        {
            Item515OnEffectOff();
        }

        //执行生效结束后的回调：
        onCompleteCallback?.Invoke();

    }


    private void Item515OnEffect()
    {
        isTriggered = true;
        strDelta = PlayerManager.Instance.player.STR.value;
        //道具效果：
        PlayerManager.Instance.player.STR.AddValue(strDelta);
        PlayerManager.Instance.player.DEF.AddValueLimit(25);
        PlayerManager.Instance.player.LVL.AddValueLimit(150);

        UIManager.Instance.ShowPanel<WarningPanel>().SetWarningText("道具「山岳的凝视」已生效");

    }

    private void Item515OnEffectOff()
    {
        if(isTriggered)
        {
            isTriggered = false;
            PlayerManager.Instance.player.STR.AddValue(-strDelta);
            PlayerManager.Instance.player.DEF.AddValueLimit(-25);
            PlayerManager.Instance.player.LVL.AddValueLimit(-150);

            UIManager.Instance.ShowPanel<WarningPanel>().SetWarningText("道具「山岳的凝视」已不再生效");
        }
    }


}



//不论是什么情况，都不会清除这个道具以及它的效果；
public class Item_516 : Item
{
    public Item_516() { onCompleteDelegate = UsedCallback; }

    public override void Use()
    {
        effectFinalValueDic.Clear();

        EventHub.Instance.AddEventListener("Item516OnEffect", Item516OnEffect);

    }

    //在死亡后手动执行的道具销毁：
    private void UsedCallback()
    {
        //移除对应的响应事件：

        //执行生效结束后的回调：
        onCompleteCallback?.Invoke();

    }

    private void Item516OnEffect()
    {
        // 使用PlayerController.SetPlayerAttribute统一设置属性值，保持限制
        var currentSTR = PlayerManager.Instance.player.STRValue;
        var strLimit = PlayerManager.Instance.player.STR.value_limit;
        PlayerController.SetPlayerAttribute(AttributeType.STR, Mathf.Min(currentSTR * 1.08f, strLimit * 1.08f), strLimit * 1.08f);
        
        var currentDEF = PlayerManager.Instance.player.DEFValue;
        var defLimit = PlayerManager.Instance.player.DEF.value_limit;
        PlayerController.SetPlayerAttribute(AttributeType.DEF, Mathf.Min(currentDEF * 1.08f, defLimit * 1.08f), defLimit * 1.08f);

        var currentHP = PlayerManager.Instance.player.HPValue;
        var hpLimit = PlayerManager.Instance.player.HP.value_limit;
        PlayerController.SetPlayerAttribute(AttributeType.HP, currentHP, hpLimit * 1.08f);

        var currentLVL = PlayerManager.Instance.player.LVLValue;
        var lvlLimit = PlayerManager.Instance.player.LVL.value_limit;
        PlayerController.SetPlayerAttribute(AttributeType.LVL, currentLVL, lvlLimit * 1.08f);

        PlayerManager.Instance.player.AddAttrValue(AttributeType.SAN, 5);

    }


}

//塔罗牌：
public class Item_601 : Item
{
    public override void Use()
    {
        PlayerManager.Instance.player.SAN.AddValue(5);
    }
}

public class Item_602 : Item
{
    public override void Use()
    {
        PlayerManager.Instance.player.SAN.AddValue(5);
    }
}

public class Item_603 : Item
{
    public override void Use()
    {
        PlayerManager.Instance.player.SAN.AddValue(5);
    }
}

public class Item_604 : Item
{
    public override void Use()
    {
        PlayerManager.Instance.player.SAN.AddValue(5);
    }
}

public class Item_605 : Item
{
    public override void Use()
    {
        PlayerManager.Instance.player.SAN.AddValue(5);
    }
}

public class Item_606 : Item
{
    public override void Use()
    {
        PlayerManager.Instance.player.SAN.AddValue(5);
    }
}

public class Item_607 : Item
{
    public override void Use()
    {
        PlayerManager.Instance.player.SAN.AddValue(5);
    }
}

public class Item_608 : Item
{
    public override void Use()
    {
        PlayerManager.Instance.player.SAN.AddValue(5);
    }
}

public class Item_609 : Item
{
    public override void Use()
    {
        PlayerManager.Instance.player.SAN.AddValue(5);
    }
}

public class Item_610 : Item
{
    public override void Use()
    {
        PlayerManager.Instance.player.SAN.AddValue(5);
    }
}

public class Item_611 : Item
{
    public override void Use()
    {
        PlayerManager.Instance.player.SAN.AddValue(5);
    }
}

public class Item_612 : Item
{
    public override void Use()
    {
        PlayerManager.Instance.player.SAN.AddValue(5);
    }
}

public class Item_613 : Item
{
    public override void Use()
    {
        PlayerManager.Instance.player.SAN.AddValue(5);
    }
}

public class Item_614 : Item
{
    public override void Use()
    {
        PlayerManager.Instance.player.SAN.AddValue(5);
    }
}

public class Item_615 : Item
{
    public override void Use()
    {
        PlayerManager.Instance.player.SAN.AddValue(5);
    }
}

public class Item_616 : Item
{
    public override void Use()
    {
        PlayerManager.Instance.player.SAN.AddValue(5);
    }
}

public class Item_617 : Item
{
    public override void Use()
    {
        PlayerManager.Instance.player.SAN.AddValue(5);
    }
}

public class Item_618 : Item
{
    public override void Use()
    {
        PlayerManager.Instance.player.SAN.AddValue(5);
    }
}

public class Item_619 : Item
{
    public override void Use()
    {
        PlayerManager.Instance.player.SAN.AddValue(5);
    }
}

public class Item_620 : Item
{
    public override void Use()
    {
        PlayerManager.Instance.player.SAN.AddValue(5);
    }
}

public class Item_2011 : Item
{
    //获得本道具后，处于装备状态的所有怪谈装备当前耐久度及其上限+1
    public override void Use()
    {
       foreach(var equipment in EquipmentManager.Instance.equipmentList)
       {
            if(equipment.isEquipped)
            {
                equipment.currentDuration++;
                equipment.maxDuration++;

                //应该还需要更新Equipment的UI；
                EventHub.Instance.EventTrigger("UpdateEquipmentUI", equipment);
            }
       }
    }
}
public class Item_2012 : Item
{
    //获得本道具后，处于未装备状态的随机3件装备耐久度及其上限+3
    public override void Use()
    {
       foreach(var equipment in EquipmentManager.Instance.equipmentList)
        {
            if(!equipment.isEquipped)
            {
                equipment.currentDuration += 3;
                equipment.maxDuration += 3;

                //应该还需要更新Equipment的UI；
                EventHub.Instance.EventTrigger("UpdateEquipmentUI", equipment);
            }
        }
    }


}
public class Item_2013 : Item
{
    //使用后：当前生命值+100，防御+10
    public override void Use()
    {
        PlayerManager.Instance.player.HP.AddValue(100);
        PlayerManager.Instance.player.DEF.AddValue(100);
    }


}

public class Item_2014 : Item
{
    //使用后：当前灯光值与生命值以及他们的上限+50。
    public override void Use()
    {
        PlayerManager.Instance.player.LVL.AddValueLimit(50);
        PlayerManager.Instance.player.HP.AddValueLimit(50);
    }

}

public class Item_2015 : Item
{
    //持有该道具时：每次战斗胜利后随机获得2件怪谈道具。
    public override void Use()
    {
        //战斗后调用该事件：
        EventHub.Instance.AddEventListener("GetRandomTwoItemsAfterBattle", GetRandomTwoItemsAfterBattle);
    }

    //取消持有的方法：
    public void Unuse()
    {
        EventHub.Instance.RemoveEventListener("GetRandomTwoItemsAfterBattle", GetRandomTwoItemsAfterBattle);
    }


    private void GetRandomTwoItemsAfterBattle()
    {
        var dic = LoadManager.Instance.allItems;

        List<int> keys = new List<int>(dic.Keys);

        // 随机打乱
        for (int i = 0; i < keys.Count; i++)
        {
            int randomIndex = UnityEngine.Random.Range(i, keys.Count);
            (keys[i], keys[randomIndex]) = (keys[randomIndex], keys[i]);
        }

        // 抽前两个
        int key1 = keys[0];
        int key2 = keys[1];

        ItemManager.Instance.AddItem(key1, key2);
    }
}

public class Item_2016 : Item
{
    //持有该道具时：进入下一层关卡以及安全屋后，当前力量/防御/速度以及其上限随机一项属性+10
    public override void Use()
    {
        //进入关卡 / 安全屋调用；
        //进入关卡在TestMazeStart中；
        Debug.Log("Item_616 Used");
        EventHub.Instance.AddEventListener("RandomBenifit", RandomBenifit);
    }

    //取消持有的方法：
    public void Unuse()
    {
        EventHub.Instance.RemoveEventListener("RandomBenifit", RandomBenifit);
    }

    private void RandomBenifit()
    {
        Debug.LogWarning("RandomBenifit is triggerd!");
        int randomNum = UnityEngine.Random.Range(1, 4); // 注意上限是4，不包含4！
        switch(randomNum){
            case 1:
                PlayerManager.Instance.player.STR.AddValueLimit(10);
            break;
                
            case 2:
                PlayerManager.Instance.player.SPD.AddValueLimit(10);
            break;
                    
            case 3:
                PlayerManager.Instance.player.DEF.AddValueLimit(10);
            break;
        }
    }
}
//标识Item的作用属性
public enum E_AffectPlayerAttributeType
{
    None = 0,
    生命值 = 1,
    生命值上限 = 2,
    力量值 = 3,
    力量值上限 = 4,
    防御值 = 5,
    防御值上限 = 6,
    灯光值 = 7,
    灯光值上限 = 8,
    精神值 = 9,
    精神值上限 = 10,
    速度值 = 11,
    速度值上限 = 12,
    暴击率 = 13,
    暴击伤害 = 14,
    连击率 = 15,

    闪避值 = 16,


}
