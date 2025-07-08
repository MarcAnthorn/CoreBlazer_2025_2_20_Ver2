using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Progress;
using UnityEngine.SceneManagement;

/// <summary>
/// 玩家属性类型枚举
/// 定义了游戏中所有可用的玩家属性类型
/// </summary>
public enum AttributeType
{
    /// <summary>
    /// 无效属性类型，用于默认值或错误处理
    /// </summary>
    NONE = 0,
    
    /// <summary>
    /// 生命值 - 玩家的血量，归零时死亡
    /// </summary>
    HP = 1,
    
    /// <summary>
    /// 力量值 - 影响攻击伤害
    /// </summary>
    STR = 2,
    
    /// <summary>
    /// 防御值 - 减少受到的物理伤害
    /// </summary>
    DEF = 3,
    
    /// <summary>
    /// 灯光值 - 影响视野范围和光照半径，关系到生存机制
    /// </summary>
    LVL = 4,
    
    /// <summary>
    /// 精神值 - 理智值，过低可能导致负面效果，归零时死亡
    /// </summary>
    SAN = 5,
    
    /// <summary>
    /// 速度值 - 影响移动速度和行动顺序
    /// </summary>
    SPD = 6,
    
    /// <summary>
    /// 暴击率 - 造成暴击伤害的概率
    /// </summary>
    CRIT_Rate = 7,
    
    /// <summary>
    /// 暴击伤害 - 暴击时的伤害倍率
    /// </summary>
    CRIT_DMG = 8,
    
    /// <summary>
    /// 命中率 - 攻击命中目标的概率（0-1之间）
    /// </summary>
    HIT = 9,
    
    /// <summary>
    /// 闪避值 - 躲避敌方攻击的概率（0-1之间）
    /// </summary>
    AVO = 10
}

[System.Serializable]
public class Player               //存储角色信息等
{
    [System.Serializable]
    public struct PlayerAttribute           //角色属性（在读取角色信息表时再实例化）
    {
        public int id;
        public string name;
        public int level;
        public string icon;
        public int type;
        public float value;
        public float value_limit;           //角色属性的上限值
        public float minLimit;


        public PlayerAttribute(int id, int level = 0, int type = 0, string name = null, string icon = null)
        {
            this.id = id;
            this.name = name;
            this.level = level;
            this.icon = icon;
            this.type = type;
            this.value = 0f;         //初始化为0
            this.value_limit = 100;
            this.minLimit = 1;

        }

        //修正方法：所有只是调整数值的，但是不调整上限值的，使用这个方法；
        //这个方法会将当前加成后的数值和上限值之间取较小值；
        public void AddValue(float change)
        {
            value = Mathf.Min(value + change, value_limit);  
            //如果是灯光值，并且change > 0,尝试触发道具102的效果：
            if (change > 0 && this.id == 4)
                EventHub.Instance.EventTrigger("AddExtraLight");  

        }

        public void MultipleValue(float change)
        {
            value = Mathf.Min(value * change, value_limit);

            //如果是灯光值，并且change > 0,尝试触发道具102的效果：
            if(change > 1f)
                EventHub.Instance.EventTrigger("AddExtraLight"); 
        }

        public void SetValue(float value)
        {
            this.value = value;
        }

        //修正方法：所有需要调整数值&上限值的，直接使用这个方法；
        public void AddValueLimit(float change)
        {
            //如果是灯光值，并且change > 0,尝试触发道具102的效果：
            if(change > 0 && this.id == 4)
                EventHub.Instance.EventTrigger("AddExtraLight"); 

            value += change;
            value_limit += change;
        }

        public void SetValueToLimit()
        {
            this.value = this.value_limit;
        }

        public void MultipleValueLimit(float change)
        {
            float delta = value * change;
            value_limit += delta;
            value += delta;
        }

        public void SetValueLimit(float value)
        {
            this.value = value;
        }

    }
    
    //静态基本属性
    //public int HP_limit = 100;
    //动态基本属性

    // 属性字典，type为索引
    public Dictionary<int, PlayerAttribute> attributeDict = new Dictionary<int, PlayerAttribute>();

    // 兼容旧代码的字段（只做映射，不再作为主存储）
    public PlayerAttribute HP { get { return GetAttr(AttributeType.HP); } set { SetAttr(AttributeType.HP, value); } }
    public float HPValue { get => GetAttr(AttributeType.HP).value; set { var attr = GetAttr(AttributeType.HP); attr.value = value; SetAttr(AttributeType.HP, attr); } }

    public PlayerAttribute STR { get { return GetAttr(AttributeType.STR); } set { SetAttr(AttributeType.STR, value); } }
    public float STRValue { get => GetAttr(AttributeType.STR).value; set { var attr = GetAttr(AttributeType.STR); attr.value = value; SetAttr(AttributeType.STR, attr); } }

    public PlayerAttribute DEF { get { return GetAttr(AttributeType.DEF); } set { SetAttr(AttributeType.DEF, value); } }
    public float DEFValue { get => GetAttr(AttributeType.DEF).value; set { var attr = GetAttr(AttributeType.DEF); attr.value = value; SetAttr(AttributeType.DEF, attr); } }

    public PlayerAttribute LVL { get { return GetAttr(AttributeType.LVL); } set { SetAttr(AttributeType.LVL, value); } }
    public float LVLValue { get => GetAttr(AttributeType.LVL).value; set { var attr = GetAttr(AttributeType.LVL); attr.value = value; SetAttr(AttributeType.LVL, attr); } }
    public float LVLLimit { get => GetAttr(AttributeType.LVL).value_limit; set { var attr = GetAttr(AttributeType.LVL); attr.value_limit = value; SetAttr(AttributeType.LVL, attr); } }

    public PlayerAttribute SAN { get { return GetAttr(AttributeType.SAN); } set { SetAttr(AttributeType.SAN, value); } }
    public float SANValue { get => GetAttr(AttributeType.SAN).value; set { var attr = GetAttr(AttributeType.SAN); attr.value = value; SetAttr(AttributeType.SAN, attr); } }

    public PlayerAttribute SPD { get { return GetAttr(AttributeType.SPD); } set { SetAttr(AttributeType.SPD, value); } }
    public float SPDValue { get => GetAttr(AttributeType.SPD).value; set { var attr = GetAttr(AttributeType.SPD); attr.value = value; SetAttr(AttributeType.SPD, attr); } }

    public PlayerAttribute CRIT_Rate { get { return GetAttr(AttributeType.CRIT_Rate); } set { SetAttr(AttributeType.CRIT_Rate, value); } }
    public float CRIT_RateValue { get => GetAttr(AttributeType.CRIT_Rate).value; set { var attr = GetAttr(AttributeType.CRIT_Rate); attr.value = value; SetAttr(AttributeType.CRIT_Rate, attr); } }

    public PlayerAttribute CRIT_DMG { get { return GetAttr(AttributeType.CRIT_DMG); } set { SetAttr(AttributeType.CRIT_DMG, value); } }
    public float CRIT_DMGValue { get => GetAttr(AttributeType.CRIT_DMG).value; set { var attr = GetAttr(AttributeType.CRIT_DMG); attr.value = value; SetAttr(AttributeType.CRIT_DMG, attr); } }

    public PlayerAttribute HIT { get { return GetAttr(AttributeType.HIT); } set { SetAttr(AttributeType.HIT, value); } }
    public float HITValue { get => GetAttr(AttributeType.HIT).value; set { var attr = GetAttr(AttributeType.HIT); attr.value = value; SetAttr(AttributeType.HIT, attr); } }

    public PlayerAttribute AVO { get { return GetAttr(AttributeType.AVO); } set { SetAttr(AttributeType.AVO, value); } }
    public float AVOValue { get => GetAttr(AttributeType.AVO).value; set { var attr = GetAttr(AttributeType.AVO); attr.value = value; SetAttr(AttributeType.AVO, attr); } }

    // 通过type获取属性，若不存在则返回默认
    public PlayerAttribute GetAttr(AttributeType type)
    {
        if (attributeDict.TryGetValue((int)type, out var attr))
            return attr;
        return default;
    }
    // 通过type设置属性
    public void SetAttr(AttributeType type, PlayerAttribute attr)
    {
        attributeDict[(int)type] = attr;
    }

    // 新增：安全的属性值修改方法
    public void AddAttrValue(AttributeType type, float change)
    {
        var attr = GetAttr(type);
        attr.value = Mathf.Min(attr.value + change, attr.value_limit);
        // 确保血量不低于0
        if (type == AttributeType.HP)
            attr.value = Mathf.Max(0, attr.value);
        SetAttr(type, attr);
    }

    public void SetAttrValue(AttributeType type, float value)
    {
        var attr = GetAttr(type);
        if (attr.value_limit < value)
        {
            Debug.LogWarning($"尝试设置属性 {type} 的值为 {value}，但超过了上限 {attr.value_limit}，将被限制为上限值。");
            value = attr.value_limit; // 限制为上限值
        }
        attr.value = value;
        SetAttr(type, attr);
    }

    // 新增：同时设置属性值和上限的方法
    public void SetAttrValueAndLimit(AttributeType type, float value, float valueLimit)
    {
        var attr = GetAttr(type);
        attr.value = value;
        attr.value_limit = valueLimit;
        SetAttr(type, attr);
    }

    public bool isDie = false;
   

    public Player()
    {
    }

    // // 游戏(暂时)结束
    // public void GameOver()
    // {

    // }

    public void BeHurted(float value)
    {
        // 修复：使用安全的属性修改方法
        AddAttrValue(AttributeType.HP, -value);
    }

    public void DebugInfo()
    {
        Debug.LogWarning($"HP: {HP.value}, \n STR:{STR.value}, \n DEF:{DEF.value}, \n SAN:{SAN.value}, \n LVL:{LVL.value}, \n SPD:{SPD.value}, \n CRIT_Rate:{CRIT_Rate.value}, \n CRIT_DMG:{CRIT_DMG.value}, \n HIT:{HIT.value}, \n AVO:{AVO.value}");
    }

}

[Flags]
public enum DamageType : uint
{
    NONE = 0,

    STR = 0x01,
    Item = 0x02,
    Skill = 0x04,
    Dot = 0x08
}

public class Damage
{
    // 不要直接访问!!!改用索引器访问
    private List<SingleDamage> damageValues = new List<SingleDamage>();

    public bool isAvoided;                      // 闪避标识
    public bool isCombo;                        // 连击标识
    public DamageType damageType;               // 伤害类型

    public class SingleDamage
    {
        public bool isCritical;                 // 暴击标识
        public float damage;                      // 伤害量

        public SingleDamage(bool isCritical, float damage)
        {
            this.isCritical = isCritical;
            this.damage = damage;
        }
    }

    public Damage(DamageType damageType)
    {
        this.damageType = damageType;
    }

    public SingleDamage this[int index]
    {
        get 
        {
            if (index < 0 || index >= damageValues.Count)
            {
            
                throw new IndexOutOfRangeException($"索引超出范围 get, count is {damageValues.Count}, index is {index}");
            }
            return damageValues[index]; 
        }
        set 
        {
            if (index < 0 || index >= damageValues.Count)
                throw new IndexOutOfRangeException("索引超出范围 set");
            damageValues[index] = value;
        }
    }

    public int GetSize()
    {
        return damageValues.Count;
    }

    public void AddSingleDamage(bool isCritical, float value)
    {
        damageValues.Add(new SingleDamage(isCritical, (int)value));
    }

    // 解析伤害 迁移到BattlePanel中；
    // //注意需要区分伤害的施加来源
    // //其中，0是表示玩家受伤；1是表示敌人受伤
    // public void ParseDamage(int flag)
    // {
    //     if(flag == 0)
    //     {
    //         //玩家受伤
    //     }

    //     else
    //     {
    //         //敌人受伤
    //     }
    // }
}
