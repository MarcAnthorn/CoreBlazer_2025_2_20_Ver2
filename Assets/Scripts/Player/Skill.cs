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
    abstract public void Use();    //传入攻击的enemy实例


    //调用后返回当前该技能的伤害的属性：
    //需要按照Skill的伤害结算公式 结合当前的玩家属性进行结算后返回
    //此处假设是1；
    public int SkillDamage => 1;
    //如：8 + 玩家力量值的Skill：
    // public int SkillDamage => 8 + PlayerManager.Instance.player.STR.value;

}


//技能也应该是不同的实现，需要手动完成；
// --------------- 角色技能 ---------------
// 格斗
public class Skill_1001 : Skill
{
    public Skill_1001()
    {
        id = 1001;
    }

    override public void Use()
    {
        // SkillManager.Instance.Skill_1001(enemy);
        BattleManager.Instance.SelectSkill_1001();
    }
}

// 毒针
public class Skill_1002 : Skill
{
    public Skill_1002()
    {
        id = 1002;
    }

    override public void Use()
    {
        // SkillManager.Instance.Skill_1002(enemy);
        BattleManager.Instance.SelectSkill_1002();
    }
}

// 新月之辉
public class Skill_1003 : Skill
{
    public Skill_1003()
    {
        id = 1003;
    }

    override public void Use()
    {
        // SkillManager.Instance.Skill_1002(enemy);
        BattleManager.Instance.SelectSkill_1003();
    }
}

// 心火
public class Skill_1004 : Skill
{
    public Skill_1004()
    {
        id = 1004;
    }

    override public void Use()
    {
        BattleManager.Instance.SelectSkill_1004();
    }
}

// 破势击
public class Skill_1005 : Skill
{
    public Skill_1005()
    {
        id = 1005;
    }

    override public void Use()
    {
        BattleManager.Instance.SelectSkill_1005();
    }
}

// 缚心铎声
public class Skill_1006 : Skill
{
    public Skill_1006()
    {
        id = 1006;
    }

    override public void Use()
    {
        BattleManager.Instance.SelectSkill_1006();
    }
}

// 落日
public class Skill_1007 : Skill
{
    public Skill_1007()
    {
        id = 1007;
    }

    override public void Use()
    {
        BattleManager.Instance.SelectSkill_1007();
    }
}

// 湖中女的复仇
public class Skill_1008 : Skill
{
    public Skill_1008()
    {
        id = 1008;
    }

    override public void Use()
    {
        BattleManager.Instance.SelectSkill_1008();
    }
}

// 魔音灌耳
public class Skill_1009 : Skill
{
    public Skill_1009()
    {
        id = 1009;
    }

    override public void Use()
    {
        BattleManager.Instance.SelectSkill_1009();
    }
}

// 伤口污染
public class Skill_1010 : Skill
{
    public Skill_1010()
    {
        id = 1010;
    }

    override public void Use()
    {
        BattleManager.Instance.SelectSkill_1010();
    }
}

// 鼠群意志
public class Skill_1020 : Skill
{
    public Skill_1020()
    {
        id = 1020;
    }

    override public void Use()
    {
        BattleManager.Instance.SelectSkill_1020();
    }
}

// 最后一次守护
public class Skill_1023 : Skill
{
    public Skill_1023()
    {
        id = 1023;
    }

    override public void Use()
    {
        BattleManager.Instance.SelectSkill_1023();
    }
}


// --------------- 敌人技能 ---------------

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
