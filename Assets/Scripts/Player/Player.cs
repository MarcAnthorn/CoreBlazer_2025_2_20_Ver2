using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Progress;

public enum AttributeType
{
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
            if(change > 0 && this.id == 4)
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
    public PlayerAttribute HP;          //生命     Health point      id = 1
    public PlayerAttribute STR;         //力量     Strength          id = 2  
    public PlayerAttribute DEF;         //防御     Defense           id = 3 
    public PlayerAttribute LVL;         //灯光值   Light Value       id = 4  
    public PlayerAttribute SAN;         //SAN 值   Sanity            id = 5 
    public PlayerAttribute SPD;         //速度     Speed             id = 6 
    public PlayerAttribute CRIT_Rate;   //暴击率   Critical Hit Rate id = 7 
    public PlayerAttribute CRIT_DMG;    //暴击伤害 Critical Damage   id = 8 
    public PlayerAttribute HIT;         //连击     Hit               id = 9 
    public PlayerAttribute AVO;         //闪避值   AVO               id = 10
    //动态特殊属性

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
        HP.AddValue(-value);
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
