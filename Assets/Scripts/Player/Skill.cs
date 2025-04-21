using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//数据结构类：技能：
public abstract class Skill
{
    //技能id
    public int id;
    //技能名称：
    public string skillName;
    //技能图标名：
    public string skillIconPath;
    //技能伤害效果描述：
    public string skillDamageText;
    //技能buff / debuff 描述：
    public string skillBuffText;
    //技能消耗费用：
    public int skillCost;
    //技能的使用：
    abstract public void Use(Enemy enemy);    //传入攻击的enemy实例

}


//技能也应该是不同的实现，需要手动完成；
// 普通攻击(平A技能)
public class Skill_1002 : Skill
{

    override public void Use(Enemy enemy)
    {
        SkillManager.Instance.Skill_1002(enemy);
    }
}

// 至圣斩
public class Skill_1003 : Skill
{

    override public void Use(Enemy enemy)
    {
        SkillManager.Instance.Skill_1003(enemy);
    }
}
