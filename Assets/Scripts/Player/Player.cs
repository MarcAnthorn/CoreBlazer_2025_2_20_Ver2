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

    public void ChargeValue(int change)     //用于调整角色属性
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
    public playerAttribute HP;          //生命    Health point
    public playerAttribute STR;         //力量    Strength
    public playerAttribute DEF;         //防御    Defense
    public playerAttribute LVL;         //灯光值  Light Value
    public playerAttribute SAN;         //SAN值   Sanity
    public playerAttribute SPD;         //速度    Speed

    public Dictionary<int, Item> bag;

    public Player()
    {
        HP = new playerAttribute();
        STR = new playerAttribute();
        DEF = new playerAttribute();
        LVL = new playerAttribute();
        SAN = new playerAttribute();
        SPD = new playerAttribute();

        bag = new Dictionary<int, Item>();
    }

    //这是啥？析构函数吗（Marc疑问）
    // ~Player()
    // {
    //     bag = null;
    // }

    public void GameOver()
    {

    }

}
