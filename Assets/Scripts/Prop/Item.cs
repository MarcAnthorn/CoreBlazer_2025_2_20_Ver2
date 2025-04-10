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
        if (usableScene[sceneId - 1] == 1)
        {
            return true;
        }

        return false;
    }

    public abstract void Use();

}



public class Item_101 : Item
{
    public Item_101()
    {
        string className = GetType().Name; // 获取类名
        string idStr = className.Substring(className.LastIndexOf('_') + 1); // 提取后缀数字部分
        if (int.TryParse(idStr, out int parsedId))
        {
            id = parsedId;
        }
        else
        {
            Debug.LogError($"Item类名格式错误，无法从 {className} 提取id");
        }
    }

    public override void Use()
    {
        Debug.Log($"道具 \"灯火助燃剂\" 使用！");
        //可视范围扩大5格
        int timerIndex;
        timerIndex = TimeManager.Instance.AddTimer(8f, () => OnStart(), () => OnComplete());

        PlayerManager.Instance.player.DebugInfo();
    }

    private void OnStart()
    {
        PlayerManager.Instance.PlayerAttributeChange(AttributeType.LVL, +20f);
    }

    private void OnComplete(){
        onCompleteCallback?.Invoke();
        PlayerManager.Instance.PlayerAttributeChange(AttributeType.LVL, -20f);
    }

}

public class Item_102 : Item
{
    public Item_102()
    {
        string className = GetType().Name; // 获取类名
        string idStr = className.Substring(className.LastIndexOf('_') + 1); // 提取后缀数字部分
        if (int.TryParse(idStr, out int parsedId))
        {
            id = parsedId;
        }
        else
        {
            Debug.LogError($"Item类名格式错误，无法从 {className} 提取id");
        }
        
    }

    public override void Use()
    {
        Debug.Log($"道具 \"封存的灯火\" 使用！");
        //获得后，后续每次额外获得2灯光值
        BuffManager.Instance.AddBuff(UseCase.GrowUp, BuffType.LVL_Change, CalculationType.Add, +2f);     //代表加成
        PlayerManager.Instance.player.DebugInfo();
    }

}

public class Item_103 : Item
{
    public Item_103()
    {
        string className = GetType().Name; // 获取类名
        string idStr = className.Substring(className.LastIndexOf('_') + 1); // 提取后缀数字部分
        if (int.TryParse(idStr, out int parsedId))
        {
            id = parsedId;
        }
        else
        {
            Debug.LogError($"Item类名格式错误，无法从 {className} 提取id");
        }
    }
    public override void Use()
    {
        Debug.Log($"道具 \"大力出奇迹\" 使用！");
        //使用后靠近特殊墙壁可砸碎
        int timerIndex;
        timerIndex = TimeManager.Instance.AddTimer(8f, () => OnStart(), () => OnComplete());
        PlayerManager.Instance.player.DebugInfo();
    }

    int index;
    private void OnStart()
    {
        index = BuffManager.Instance.AddBuff(UseCase.Maze, BuffType.LVL_Change, SpecialBuffType.DamageWall, DamageWall);
    }

    private void OnComplete(){
        onCompleteCallback?.Invoke();
        BuffManager.Instance.RemoveBuff(index);
    }

    private Func<float> DamageWall = null;                   //由Marc将实现方法写入其中
    public void SubscribeHandler(Func<float> handler)
    {
        if (DamageWall == null)
        {
            DamageWall += handler;
        }
    }
}

public class Item_104 : Item
{
    public Item_104()
    {
        string className = GetType().Name; // 获取类名
        string idStr = className.Substring(className.LastIndexOf('_') + 1); // 提取后缀数字部分
        if (int.TryParse(idStr, out int parsedId))
        {
            id = parsedId;
        }
        else
        {
            Debug.LogError($"Item类名格式错误，无法从 {className} 提取id");
        }
    }
    public override void Use()
    {
        Debug.Log($"道具 \"传送门\" 使用！");
        //使用后有50%概率直接到达该层迷宫终点，也有50%概率回到起点
        int randomNum = UnityEngine.Random.Range(0, 2);
        if (randomNum == 0)
        {
            //到达该层迷宫终点
            BackToStartPoint.Invoke();
        }
        else if (randomNum == 1)
        {
            //回到起点
            ReachToFinalPoint.Invoke();
        }
        PlayerManager.Instance.player.DebugInfo();
    }

    private Action BackToStartPoint = null;             //由Marc将实现方法写入其中
    private Action ReachToFinalPoint = null;            //由Marc将实现方法写入其中
    public void SubscribeHandlers(Action handler1, Action handler2)
    {
        if (BackToStartPoint == null)
        {
            BackToStartPoint += handler1;
        }
        if (ReachToFinalPoint == null)
        {
            ReachToFinalPoint += handler2;
        }
    }

}

public class Item_201 : Item
{
    public Item_201()
    {
        string className = GetType().Name; // 获取类名
        string idStr = className.Substring(className.LastIndexOf('_') + 1); // 提取后缀数字部分
        if (int.TryParse(idStr, out int parsedId))
        {
            id = parsedId;
        }
        else
        {
            Debug.LogError($"Item类名格式错误，无法从 {className} 提取id");
        }
    }
    public override void Use()
    {
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
    public Item_202()
    {
        string className = GetType().Name; // 获取类名
        string idStr = className.Substring(className.LastIndexOf('_') + 1); // 提取后缀数字部分
        if (int.TryParse(idStr, out int parsedId))
        {
            id = parsedId;
        }
        else
        {
            Debug.LogError($"Item类名格式错误，无法从 {className} 提取id");
        }
    }
    public override void Use()
    {
        Debug.Log($"道具 \"燃血战神\" 使用！");
        //生命值高于50%时，可扣除自身当前生命值的80%，使自身暴击率提升100%
        int timerIndex;
        timerIndex = TimeManager.Instance.AddTimer(6f, () => OnStart(), () => OnComplete());
        PlayerManager.Instance.player.DebugInfo();
    }

    bool isEffected;
    private void OnStart()
    {
        bool condition1 = (PlayerManager.Instance.player.HP.value / PlayerManager.Instance.player.HP.value_limit) >= 0.5;
        if (condition1)
        {
            PlayerManager.Instance.player.HP.value *= 0.2f;
            PlayerManager.Instance.player.CRIT_Rate.value += 1f;
            isEffected = true;
        }
    }

    private void OnComplete(){
        onCompleteCallback?.Invoke();
        if (isEffected)
        {
            PlayerManager.Instance.player.HP.value /= 0.2f;
            PlayerManager.Instance.player.CRIT_Rate.value -= 1f;
        }
    }

}

public class Item_203 : Item
{
    public Item_203()
    {
        string className = GetType().Name; // 获取类名
        string idStr = className.Substring(className.LastIndexOf('_') + 1); // 提取后缀数字部分
        if (int.TryParse(idStr, out int parsedId))
        {
            id = parsedId;
        }
        else
        {
            Debug.LogError($"Item类名格式错误，无法从 {className} 提取id");
        }
    }
    public override void Use()
    {
        Debug.Log($"道具 \"神愈术\" 使用！");
        //使用后生命值回满
        PlayerManager.Instance.player.HP.value = PlayerManager.Instance.player.HP.value_limit;
        PlayerManager.Instance.player.DebugInfo();
    }

}

public class Item_204 : Item
{
    public Item_204()
    {
        string className = GetType().Name; // 获取类名
        string idStr = className.Substring(className.LastIndexOf('_') + 1); // 提取后缀数字部分
        if (int.TryParse(idStr, out int parsedId))
        {
            id = parsedId;
        }
        else
        {
            Debug.LogError($"Item类名格式错误，无法从 {className} 提取id");
        }
    }
    public override void Use()
    {
        Debug.Log($"道具 \"最强防御\" 使用！");
        //使用后防御值短时间*1.5倍，在此期间暴击率归0
        int timerIndex;
        timerIndex = TimeManager.Instance.AddTimer(12f, () => OnStart(), () => OnComplete());
        PlayerManager.Instance.player.DebugInfo();
    }

    float temp;
    private void OnStart()
    {
        PlayerManager.Instance.player.DEF.value *= 1.5f;
        temp = PlayerManager.Instance.player.CRIT_Rate.value;
        PlayerManager.Instance.player.CRIT_Rate.value = 0f;
    }

    private void OnComplete(){
        onCompleteCallback?.Invoke();
        PlayerManager.Instance.player.DEF.value /= 1.5f;
        PlayerManager.Instance.player.CRIT_Rate.value += temp;
    }

}

public class Item_205 : Item
{
    public Item_205()
    {
        string className = GetType().Name; // 获取类名
        string idStr = className.Substring(className.LastIndexOf('_') + 1); // 提取后缀数字部分
        if (int.TryParse(idStr, out int parsedId))
        {
            id = parsedId;
        }
        else
        {
            Debug.LogError($"Item类名格式错误，无法从 {className} 提取id");
        }
    }
    public override void Use()
    {
        Debug.Log($"道具 \"最强攻击\" 使用！");
        //使用后力量短时间*1.5倍，在此期间防御值归0
        int timerIndex;
        timerIndex = TimeManager.Instance.AddTimer(12f, () => OnStart(), () => OnComplete());
        PlayerManager.Instance.player.DebugInfo();
    }

    float temp;
    private void OnStart()
    {
        PlayerManager.Instance.player.STR.value *= 1.5f;
        temp = PlayerManager.Instance.player.DEF.value;
        PlayerManager.Instance.player.CRIT_Rate.value = 0f;
    }

    private void OnComplete(){
        onCompleteCallback?.Invoke();
        PlayerManager.Instance.player.STR.value /= 1.5f;
        PlayerManager.Instance.player.DEF.value += temp;
    }

}

public class Item_206 : Item
{
    public Item_206()
    {
        string className = GetType().Name; // 获取类名
        string idStr = className.Substring(className.LastIndexOf('_') + 1); // 提取后缀数字部分
        if (int.TryParse(idStr, out int parsedId))
        {
            id = parsedId;
        }
        else
        {
            Debug.LogError($"Item类名格式错误，无法从 {className} 提取id");
        }
    }
    public override void Use()
    {
        Debug.Log($"道具 \"最强闪避\" 使用！");
        //使用后闪避短时间*1.8倍，在此期间受到的伤害*1.5倍
        int timerIndex;
        timerIndex = TimeManager.Instance.AddTimer(12f, () => OnStart(), () => OnComplete());
        PlayerManager.Instance.player.DebugInfo();
    }

    int index;
    private void OnStart()
    {
        PlayerManager.Instance.player.AVO.value *= 1.8f;
        index = BuffManager.Instance.AddBuff(UseCase.Battle, BuffType.HP_Change, CalculationType.Multiply, 1.0f / 1.5f);    //减益表示为除法
    }

    private void OnComplete(){
        onCompleteCallback?.Invoke();
        PlayerManager.Instance.player.AVO.value /= 1.8f;
        BuffManager.Instance.RemoveBuff(index);
    }

}


public class Item_207 : Item
{
    public Item_207()
    {
        string className = GetType().Name; // 获取类名
        string idStr = className.Substring(className.LastIndexOf('_') + 1); // 提取后缀数字部分
        if (int.TryParse(idStr, out int parsedId))
        {
            id = parsedId;
        }
        else
        {
            Debug.LogError($"Item类名格式错误，无法从 {className} 提取id");
        }
    }
    public override void Use()
    {
        Debug.Log($"道具 \"最强苟命王\" 使用！");
        //使用后短时间内生命值冻结，技能结束前未结束战斗生命值归0
        int timerIndex;
        timerIndex = TimeManager.Instance.AddTimer(12f, () => OnStart(), () => OnComplete());
        PlayerManager.Instance.player.DebugInfo();
    }

    int index;
    private void OnStart()
    {
        index = BuffManager.Instance.AddBuff(UseCase.Battle, BuffType.HP_Change, SpecialBuffType.NoDamage, () => BuffFunction());
    }

    private void OnComplete(){
        onCompleteCallback?.Invoke();
        PlayerManager.Instance.player.HP.value = 0;
        BuffManager.Instance.RemoveBuff(index);
    }

    private float BuffFunction()
    {
        return 0;   //表示将finalValue设置为0
    }

}

public class Item_208 : Item
{
    public Item_208()
    {
        string className = GetType().Name; // 获取类名
        string idStr = className.Substring(className.LastIndexOf('_') + 1); // 提取后缀数字部分
        if (int.TryParse(idStr, out int parsedId))
        {
            id = parsedId;
        }
        else
        {
            Debug.LogError($"Item类名格式错误，无法从 {className} 提取id");
        }
    }
    public override void Use()
    {
        Debug.Log($"道具 \"绝地反击\" 使用！");
        //生命值低于20%时可使用，使用后短时间内暴击率提升100%、攻击*1.5倍
        int timerIndex;
        timerIndex = TimeManager.Instance.AddTimer(12f, () => OnStart(), () => OnComplete());
        PlayerManager.Instance.player.DebugInfo();
    }

    private void OnStart()
    {
        PlayerManager.Instance.player.CRIT_Rate.value += 1f;
        PlayerManager.Instance.player.STR.value *= 1.5f;
    }

    private void OnComplete(){
        onCompleteCallback?.Invoke();
        PlayerManager.Instance.player.CRIT_Rate.value -= 1f;
        PlayerManager.Instance.player.STR.value /= 1.5f;
    }

}

public class Item_301 : Item
{
    public Item_301()
    {
        string className = GetType().Name; // 获取类名
        string idStr = className.Substring(className.LastIndexOf('_') + 1); // 提取后缀数字部分
        if (int.TryParse(idStr, out int parsedId))
        {
            id = parsedId;
        }
        else
        {
            Debug.LogError($"Item类名格式错误，无法从 {className} 提取id");
        }
    }
    public override void Use()
    {
        Debug.Log($"道具 \"灯光up\" 使用！");
        //获得后灯光值+20
        PlayerManager.Instance.player.LVL.value += 20;
        PlayerManager.Instance.player.DebugInfo();
    }

}

public class Item_302 : Item
{
    public Item_302()
    {
        string className = GetType().Name; // 获取类名
        string idStr = className.Substring(className.LastIndexOf('_') + 1); // 提取后缀数字部分
        if (int.TryParse(idStr, out int parsedId))
        {
            id = parsedId;
        }
        else
        {
            Debug.LogError($"Item类名格式错误，无法从 {className} 提取id");
        }
    }
    public override void Use()
    {
        Debug.Log($"道具 \"速!速!速!\" 使用！");
        //使用后在迷宫内移动速度短时间翻倍
        int timerIndex;
        timerIndex = TimeManager.Instance.AddTimer(10f, () => OnStart(), () => OnComplete());
        PlayerManager.Instance.player.DebugInfo();
    }

    private void OnStart()
    {
        PlayerManager.Instance.player.SPD.value *= 2f;
    }

    private void OnComplete(){
        onCompleteCallback?.Invoke();
        PlayerManager.Instance.player.SPD.value /= 2f;
    }

}

public class Item_303 : Item
{
    public Item_303()
    {
        string className = GetType().Name; // 获取类名
        string idStr = className.Substring(className.LastIndexOf('_') + 1); // 提取后缀数字部分
        if (int.TryParse(idStr, out int parsedId))
        {
            id = parsedId;
        }
        else
        {
            Debug.LogError($"Item类名格式错误，无法从 {className} 提取id");
        }
    }
    public override void Use()
    {
        Debug.Log($"道具 \"精神恢复剂\" 使用！");
        //使用后当前精神值+3
        PlayerManager.Instance.player.SAN.value += 3;
        PlayerManager.Instance.player.DebugInfo();
    }

}

public class Item_401 : Item
{
    public Item_401()
    {
        string className = GetType().Name; // 获取类名
        string idStr = className.Substring(className.LastIndexOf('_') + 1); // 提取后缀数字部分
        if (int.TryParse(idStr, out int parsedId))
        {
            id = parsedId;
        }
        else
        {
            Debug.LogError($"Item类名格式错误，无法从 {className} 提取id");
        }
    }
    public override void Use()
    {
        Debug.Log($"道具 \"生命果实\" 使用！");
        //生命值上限+10
        int timerIndex;
        timerIndex = TimeManager.Instance.AddTimer(12f, () => OnStart(), () => OnComplete());
        PlayerManager.Instance.player.DebugInfo();
    }

    private void OnStart()
    {
        PlayerManager.Instance.player.HP.value_limit += 10f;
        PlayerManager.Instance.player.HP.value += 10f;          //当前value随着limit_value一同变化
    }

    private void OnComplete(){
        onCompleteCallback?.Invoke();
        PlayerManager.Instance.player.HP.value_limit -= 10f;
        PlayerManager.Instance.player.HP.value -= 10f;
    }

}

public class Item_402 : Item
{
    public Item_402()
    {
        string className = GetType().Name; // 获取类名
        string idStr = className.Substring(className.LastIndexOf('_') + 1); // 提取后缀数字部分
        if (int.TryParse(idStr, out int parsedId))
        {
            id = parsedId;
        }
        else
        {
            Debug.LogError($"Item类名格式错误，无法从 {className} 提取id");
        }
    }
    public override void Use()
    {
        Debug.Log($"道具 \"防御果实\" 使用！");
        //防御值上限+10
        int timerIndex;
        timerIndex = TimeManager.Instance.AddTimer(12f, () => OnStart(), () => OnComplete());
        PlayerManager.Instance.player.DebugInfo();
    }

    private void OnStart()
    {
        PlayerManager.Instance.player.DEF.value_limit += 10f;
        PlayerManager.Instance.player.DEF.value += 10f;          //当前value随着limit_value一同变化
    }

    private void OnComplete(){
        onCompleteCallback?.Invoke();
        PlayerManager.Instance.player.DEF.value_limit -= 10f;
        PlayerManager.Instance.player.DEF.value -= 10f;
    }

}

public class Item_403 : Item
{
    public Item_403()
    {
        string className = GetType().Name; // 获取类名
        string idStr = className.Substring(className.LastIndexOf('_') + 1); // 提取后缀数字部分
        if (int.TryParse(idStr, out int parsedId))
        {
            id = parsedId;
        }
        else
        {
            Debug.LogError($"Item类名格式错误，无法从 {className} 提取id");
        }
    }
    public override void Use()
    {
        Debug.Log($"道具 \"攻击果实\" 使用！");
        //使用后力量+20
        int timerIndex;
        timerIndex = TimeManager.Instance.AddTimer(18f, () => OnStart(), () => OnComplete());
    }

    private void OnStart()
    {
        // PlayerManager.Instance.player.STR.value_limit += 10f;
        PlayerManager.Instance.player.STR.value += 20f;
        PlayerManager.Instance.player.DebugInfo();
    }

    private void OnComplete(){
        onCompleteCallback?.Invoke();
        // PlayerManager.Instance.player.STR.value_limit -= 10f;
        PlayerManager.Instance.player.STR.value -= 20f;
        PlayerManager.Instance.player.DebugInfo();
    }

}

public class Item_404 : Item
{
    public Item_404()
    {
        string className = GetType().Name; // 获取类名
        string idStr = className.Substring(className.LastIndexOf('_') + 1); // 提取后缀数字部分
        if (int.TryParse(idStr, out int parsedId))
        {
            id = parsedId;
        }
        else
        {
            Debug.LogError($"Item类名格式错误，无法从 {className} 提取id");
        }
    }
    public override void Use()
    {
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
    public Item_501()
    {
        string className = GetType().Name; // 获取类名
        string idStr = className.Substring(className.LastIndexOf('_') + 1); // 提取后缀数字部分
        if (int.TryParse(idStr, out int parsedId))
        {
            id = parsedId;
        }
        else
        {
            Debug.LogError($"Item类名格式错误，无法从 {className} 提取id");
        }
    }
    public override void Use()
    {
        Debug.Log($"道具 \"回血药\" 使用！");
        //当前生命值+5
        PlayerManager.Instance.player.HP.value += 5f;
        PlayerManager.Instance.player.DebugInfo();
    }

}

public class Item_502 : Item
{
    public Item_502()
    {
        string className = GetType().Name; // 获取类名
        string idStr = className.Substring(className.LastIndexOf('_') + 1); // 提取后缀数字部分
        if (int.TryParse(idStr, out int parsedId))
        {
            id = parsedId;
        }
        else
        {
            Debug.LogError($"Item类名格式错误，无法从 {className} 提取id");
        }
    }
    public override void Use()
    {
        Debug.Log($"道具 \"护身甲\" 使用！");
        //获得后防御+10
        PlayerManager.Instance.player.DEF.value += 10f;
        PlayerManager.Instance.player.DebugInfo();
    }

}

public class Item_503 : Item
{
    public Item_503()
    {
        string className = GetType().Name; // 获取类名
        string idStr = className.Substring(className.LastIndexOf('_') + 1); // 提取后缀数字部分
        if (int.TryParse(idStr, out int parsedId))
        {
            id = parsedId;
        }
        else
        {
            Debug.LogError($"Item类名格式错误，无法从 {className} 提取id");
        }
    }
    public override void Use()
    {
        Debug.Log($"道具 \"闪避\" 使用！");
        //获得后闪避+10
        PlayerManager.Instance.player.AVO.value += 10f;
        PlayerManager.Instance.player.DebugInfo();
    }

}

public class Item_504 : Item
{
    public Item_504()
    {
        string className = GetType().Name; // 获取类名
        string idStr = className.Substring(className.LastIndexOf('_') + 1); // 提取后缀数字部分
        if (int.TryParse(idStr, out int parsedId))
        {
            id = parsedId;
        }
        else
        {
            Debug.LogError($"Item类名格式错误，无法从 {className} 提取id");
        }
    }
    public override void Use()
    {
        Debug.Log($"道具 \"重拳出击\" 使用！");
        //获得后力量+10
        PlayerManager.Instance.player.STR.value += 10f;
        PlayerManager.Instance.player.DebugInfo();
    }
    
}

public class Item_505 : Item
{
    public Item_505()
    {
        string className = GetType().Name; // 获取类名
        string idStr = className.Substring(className.LastIndexOf('_') + 1); // 提取后缀数字部分
        if (int.TryParse(idStr, out int parsedId))
        {
            id = parsedId;
        }
        else
        {
            Debug.LogError($"Item类名格式错误，无法从 {className} 提取id");
        }
    }
    public override void Use()
    {
        Debug.Log($"道具 \"精神恍惚\" 使用！");
        //获得后精神值上限-10
        PlayerManager.Instance.player.SAN.value_limit += 10f;
        PlayerManager.Instance.player.DebugInfo();
    }

}

public class Item_506 : Item
{
    public Item_506()
    {
        string className = GetType().Name; // 获取类名
        string idStr = className.Substring(className.LastIndexOf('_') + 1); // 提取后缀数字部分
        if (int.TryParse(idStr, out int parsedId))
        {
            id = parsedId;
        }
        else
        {
            Debug.LogError($"Item类名格式错误，无法从 {className} 提取id");
        }
    }
    public override void Use()
    {
        Debug.Log($"道具 \"木剑\" 使用！");
        //获得后攻击+10
        PlayerManager.Instance.player.STR.value += 10f;
        PlayerManager.Instance.player.DebugInfo();
    }

}

public class Item_507 : Item
{
    public Item_507()
    {
        string className = GetType().Name; // 获取类名
        string idStr = className.Substring(className.LastIndexOf('_') + 1); // 提取后缀数字部分
        if (int.TryParse(idStr, out int parsedId))
        {
            id = parsedId;
        }
        else
        {
            Debug.LogError($"Item类名格式错误，无法从 {className} 提取id");
        }
    }
    public override void Use()
    {
        Debug.Log($"道具 \"宝剑\" 使用！");
        //获得后攻击+40
        PlayerManager.Instance.player.STR.value += 40f;
        PlayerManager.Instance.player.DebugInfo();
    }

}

public class Item_508 : Item
{
    public Item_508()
    {
        string className = GetType().Name; // 获取类名
        string idStr = className.Substring(className.LastIndexOf('_') + 1); // 提取后缀数字部分
        if (int.TryParse(idStr, out int parsedId))
        {
            id = parsedId;
        }
        else
        {
            Debug.LogError($"Item类名格式错误，无法从 {className} 提取id");
        }
    }
    public override void Use()
    {
        Debug.Log($"道具 \"神奇四面骰\" 使用！");
        //使用后随机获得以下影响：精神值上限+20、精神值上限+10、精神值不变、精神值变为10
        int randomNum = UnityEngine.Random.Range(0, 4);
        if (randomNum == 0)
        {
            PlayerManager.Instance.player.SAN.value_limit += 20f;
        }
        else if (randomNum == 1)
        {
            PlayerManager.Instance.player.SAN.value_limit += 10f;
        }
        else if (randomNum == 2)
        {
            PlayerManager.Instance.player.SAN.value += 0f;
        }
        else if (randomNum == 3)
        {
            PlayerManager.Instance.player.SAN.value = 10f;
        }
        PlayerManager.Instance.player.DebugInfo();
    }

}
