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


    //调用后返回当前该技能的伤害的属性：
    //需要按照Skill的伤害结算公式 结合当前的玩家属性进行结算后返回
    //此处假设是1；
    public int SkillDamage => 1;
    //如：8 + 玩家力量值的Skill：
    // public int SkillDamage => 8 + PlayerManager.Instance.player.STR.value;

}


//技能也应该是不同的实现，需要手动完成；
// --------------- 角色技能 ---------------
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

// --------------- 敌人技能 ---------------
// 

public abstract class EnemySkill
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
    abstract public void Use(Enemy enemy);    // 传入使用该技能的enemy实例


    //调用后返回当前该技能的伤害的属性：
    //需要按照Skill的伤害结算公式 结合当前的玩家属性进行结算后返回
    //此处假设是1；
    public int SkillDamage => 1;
    //如：8 + 玩家力量值的Skill：
    // public int SkillDamage => 8 + PlayerManager.Instance.player.STR.value;

}

public class EnemySkill_1001 : EnemySkill
{

    override public void Use(Enemy enemy)
    {
        EnemyManager.Instance.EnemySkill_1001(enemy);
    }
}

public class EnemySkill_1002 : EnemySkill
{

    override public void Use(Enemy enemy)
    {
        EnemyManager.Instance.EnemySkill_1002(enemy);
    }
}