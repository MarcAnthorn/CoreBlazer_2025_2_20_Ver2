using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UIElements;

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

public class Player               //存储角色信息等
{
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

        public void AddValue(float change)
        {
            value += change;
        }

        public void MultipleValue(float change)
        {
            value *= change;
        }

        public void SetValue(float value)
        {
            this.value = value;
        }

        public void AddValueLimit(float change)
        {
            value += change;
        }

        public void MultipleValueLimit(float change)
        {
            value *= change;
        }

        public void SetValueLimit(float value)
        {
            this.value = value;
        }

        //public void ChangeValue(float change)     //用于调整角色属性
        //{
        //    if (type == 0)          //整数
        //    {
        //        value += change;
        //    }
        //    else                    //万分比
        //    {
        //        //Marc调整：调整前：
        //        //value += change * 0.0001f;
        //        value += value * change * 0.0001f;
        //    }

        //    if (value < 0)
        //    {
        //        Debug.Log($"属性id：{this.id}，属性名称：{this.name}  已达最小值");
        //        value = 0;          //假设 属性值 不能为负值
        //    }

        //    if (value > 100 && type == 0)
        //    {
        //        Debug.Log($"属性id：{this.id}，属性名称：{this.name}  已达最大值");
        //        value = 100;        //假设 整数类型属性值 最大为100
        //    }
        //}

    }

    //静态基本属性
    //public int HP_limit = 100;
    //动态基本属性
    public PlayerAttribute HP;
    // public PlayerAttribute HP           //生命     Health point      id = 1
    // {
    //     get { return _HP; }
    //     set 
    //     {
    //         _HP = value;
    //         if (_HP.value <= 0) isDie = true;
    //     }
    // }
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
    public Dictionary<int, Item> bag;   //??感觉用List来存会好一些??

    public Player()
    {
        DebugInfo();
    }

    // 游戏(暂时)结束
    public void GameOver()
    {

    }

    public void BeHurted(Damage damage)
    {
        HP.AddValue(damage.damage);
        if (HP.value <= 0)
        {
            Debug.Log("玩家死亡!");
            GameOver();
        }
    }

    public void DebugInfo()
    {
        Debug.LogWarning($"HP: {HP.value}, \n STR:{STR.value}, \n DEF:{DEF.value}, \n SAN:{SAN.value}, \n LVL:{LVL.value}, \n SPD:{SPD.value}, \n CRIT_Rate:{CRIT_Rate.value}, \n CRIT_DMG:{CRIT_DMG.value}, \n HIT:{HIT.value}, \n AVO:{AVO.value}");
    }

}


public class Damage
{
    //是否是暴击伤害
    public bool isCritical;
    //伤害量
    public float damage;
}
