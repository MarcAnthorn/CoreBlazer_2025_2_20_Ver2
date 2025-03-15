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

    public playerAttribute(int id, string name, int level, string icon, int type)
    {
        this.id = id;
        this.name = name;
        this.level = level;
        this.icon = icon;
        this.type = type;
        this.value = 0f;         //初始化为0
    }

    public void ChargeValue(float change)     //用于调整角色属性
    {
        if (type == 0)          //整数
        {
            value += change;
        }
        else                    //万分比
        {
            value += change * 0.0001f;
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
        HP = new playerAttribute();
        //测试用：
        HP.value = 100;
        HP.type = 1;


        STR = new playerAttribute();
        DEF = new playerAttribute();
        LVL = new playerAttribute();
        SAN = new playerAttribute();
        SPD = new playerAttribute();
        CRIT_Rate = new playerAttribute();
        CRIT_DMG = new playerAttribute();
        HIT = new playerAttribute();
        AVO = new playerAttribute();

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

}
