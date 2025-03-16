using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct playerAttribute           //角色属性（在读取角色信息表时再实例化）
{
    public int id;
    public string name;
    public int level;
    public string icon;
    public int type;
    public float value;

    public playerAttribute(int id, int level = 0, int type = 0, string name = null, string icon = null)
    {
        this.id = id;
        this.name = name;
        this.level = level;
        this.icon = icon;
        this.type = type;
        this.value = 0f;         //初始化为0
    }

    public void ChangeValue(float change)     //用于调整角色属性
    {
        if (type == 0)          //整数
        {
            value += change;
        }
        else                    //万分比
        {
            //Marc调整：调整前：
            //value += change * 0.0001f;
            value += value * change * 0.0001f;
        }

        if (value < 0)
        {
            Debug.Log($"属性id：{this.id}，属性名称：{this.name}  已达最小值");
            value = 0;          //假设 属性值 不能为负值
        }

        if (value > 100 && type == 0)
        {
            Debug.Log($"属性id：{this.id}，属性名称：{this.name}  已达最大值");
            value = 100;        //假设 整数类型属性值 最大为100
        }
    }

}

public class Player               //存储角色信息等
{
    public playerAttribute HP;          //生命     Health point      id = 1
    public playerAttribute STR;         //力量     Strength          id = 2  
    public playerAttribute DEF;         //防御     Defense           id = 3 
    public playerAttribute LVL;         //灯光值   Light Value       id = 4  
    public playerAttribute SAN;         //SAN 值   Sanity            id = 5 
    public playerAttribute SPD;         //速度     Speed             id = 6 
    public playerAttribute CRIT_Rate;   //暴击率   Critical Hit Rate id = 7 
    public playerAttribute CRIT_DMG;    //暴击伤害 Critical Damage   id = 8 
    public playerAttribute HIT;         //连击     Hit               id = 9 
    public playerAttribute AVO;         //闪避值   AVO               id = 10

    public Dictionary<int, Item> bag;   //??感觉用List来存会好一些??

    public Player()
    {
        HP = new playerAttribute(1);
        HP.value = 100;
        HP.type = 1;

        STR = new playerAttribute(2);
        STR.value = 10;

        DEF = new playerAttribute(3);
        DEF.value = 1;

        LVL = new playerAttribute(4);
        LVL.value = 1;

        SAN = new playerAttribute(5);
        SAN.value = 40;
        
        SPD = new playerAttribute(6);
        SPD.value = 10;

        CRIT_Rate = new playerAttribute(7);
        CRIT_Rate.value = 0.1f;
        CRIT_Rate.type = 1;

        CRIT_DMG = new playerAttribute(8);
        HIT = new playerAttribute(9);

        AVO = new playerAttribute(10);
        AVO.value = 0.3f;
        AVO.type = 1;

        DebugInfo();

        bag = new Dictionary<int, Item>();
    }

    //这是啥？析构函数吗（Marc疑问）(是滴，对性能优化时用，但非必要)
    // ~Player()
    // {
    //     bag = null;
    // }

    public void GameOver()
    {

    }

    public void DebugInfo()
    {
        Debug.LogWarning($"HP: {HP.value}, \n STR:{STR.value}, \n DEF:{DEF.value}, \n SAN:{SAN.value}, \n SPD:{SPD.value}, \n CRIT_Rate:{CRIT_Rate.value}, \n CRIT_DMG:{CRIT_DMG.value}, \n HIT:{HIT.value}, \n AVO:{AVO.value}");
    }

}
