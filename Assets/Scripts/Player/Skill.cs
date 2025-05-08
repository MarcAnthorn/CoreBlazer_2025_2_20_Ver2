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

    public float skillDamage;

    //技能的使用：
    abstract public void Use();    //传入攻击的enemy实例


    //调用后返回当前该技能的伤害的属性：
    //需要按照Skill的伤害结算公式 结合当前的玩家属性进行结算后返回
    //此处假设是1；
    public float SkillDamage => skillDamage;
    //如：8 + 玩家力量值的Skill：
    // public int SkillDamage => 8 + PlayerManager.Instance.player.STR.value;

}

// --------------- 角色技能 ---------------
// 格斗
public class Skill_1001 : Skill
{
    public Skill_1001()
    {
        id = 1001;
        skillIconPath = skillName = "格斗";
        skillDamageText = "等额于力量值";
        skillBuffText = "平a攻击，玩家自带的技能，使用平a攻击的逻辑结算伤害";
        skillCost = 1;

        skillDamage = PlayerManager.Instance.player.STR.value * 1f;
        
    }

    override public void Use()
    {
        BattleManager.Instance.SelectSkill_1001();
        
    }
}

// 毒针
public class Skill_1002 : Skill
{
    public Skill_1002()
    {
        id = 1002;
        skillIconPath = skillName = "毒针";
        skillDamageText = "8+20%*速度值";
        skillBuffText = "攻击施加中毒";
        skillCost = 0;

        skillDamage = PlayerManager.Instance.player.SPD.value * 0.2f + 8;
    }

    override public void Use()
    {
        BattleManager.Instance.SelectSkill_1002();
    }
}

// 新月之辉
public class Skill_1003 : Skill
{
    public Skill_1003()
    {
        id = 1003;
        skillIconPath = skillName = "新月之辉";
        skillDamageText = "50";
        skillCost = 3;
        skillBuffText = "攻击命中后，有较高概率附带10层易伤";

        skillDamage = 50;
    }

    override public void Use()
    {
        BattleManager.Instance.SelectSkill_1003();
    }
}

// 心火
public class Skill_1004 : Skill
{
    public Skill_1004()
    {
        id = 1004;
        skillIconPath = skillName = "心火";
        skillDamageText = "10";
        skillBuffText = "对自己造成伤害，并获得'燃血狂怒'buff";
        skillCost = 1;

        skillDamage = 0;
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
        skillIconPath = skillName = "破势击";
        skillDamageText = "50";
        skillBuffText = "①debuff的持有者受到力量伤害后，施加12层易伤。予debuff持有者的对手2层力量增伤";
        skillCost = 2;
        skillDamage = 50;
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
        skillIconPath = skillName = "缚心铎声";
        skillDamageText = "50";
        skillBuffText = "①debuff的持有者受到力量伤害后，给对手施加3层dot增伤和3层怪谈增伤";
        skillCost = 2;

        skillDamage = 50;
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
        skillIconPath = skillName = "落日";
        skillDamageText = "50";
        skillBuffText = "①debuff的持有者受到dot伤害或道具伤害后，给对手施加3层力量增伤";
        skillCost = 2;

        skillDamage = 50;
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
        skillIconPath = skillName = "湖中女的复仇";
        skillDamageText = "50";
        skillBuffText = "①debuff的持有者受到怪谈伤害后，予debuff持有者的对手施加3层力量增伤";
        skillCost = 2;

        skillDamage = 50;
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
        skillIconPath = skillName = "魔音灌耳";
        skillDamageText = "50";
        skillBuffText = "①debuff的持有者受到dot伤害后，予debuff持有者的对手施加3层怪谈增伤和3层易伤";
        skillCost = 2;

        skillDamage = 50;
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
        skillIconPath = skillName = "伤口污染";
        skillDamageText = "50";
        skillBuffText = "①debuff的持有者受到dot伤害后，给对手施加3层易伤";
        skillCost = 2;

        skillDamage = 50;
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
        skillIconPath = skillName = "鼠群意志";
        skillDamageText = "20";
        skillBuffText = "①对手受到dot伤害后，自己+30%连击";
        skillCost = 1;

        skillDamage = 20;
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
        skillIconPath = skillName = "最后一次守护";
        skillDamageText = "0";
        skillBuffText = "抵挡一次致命伤害。（剩余1点生命值）";
        skillCost = 1;

        skillDamage = 0;
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

    public float skillDamage;


    //调用后返回当前该技能的伤害的属性：
    //需要按照Skill的伤害结算公式 结合当前的玩家属性进行结算后返回
    //此处假设是1；
    public int SkillDamage => 1;
    //如：8 + 玩家力量值的Skill：
    // public int SkillDamage => 8 + PlayerManager.Instance.player.STR.value;

}

// 格斗
public class EnemySkill_1001 : EnemySkill
{

    override public void Use(Enemy enemy)
    {
        EnemyManager.Instance.EnemySkill_1001(enemy);
    }
}

// 毒针
public class EnemySkill_1002 : EnemySkill
{

    override public void Use(Enemy enemy)
    {
        EnemyManager.Instance.EnemySkill_1002(enemy);
    }
}

// 新月之辉
public class EnemySkill_1003 : EnemySkill
{

    override public void Use(Enemy enemy)
    {
        EnemyManager.Instance.EnemySkill_1003(enemy);
    }
}

// 心火
public class EnemySkill_1004 : EnemySkill
{

    override public void Use(Enemy enemy)
    {
        EnemyManager.Instance.EnemySkill_1004(enemy);
    }
}

// 破势击
public class EnemySkill_1005 : EnemySkill
{

    override public void Use(Enemy enemy)
    {
        EnemyManager.Instance.EnemySkill_1005(enemy);
    }
}

// 缚心铎声
public class EnemySkill_1006 : EnemySkill
{

    override public void Use(Enemy enemy)
    {
        EnemyManager.Instance.EnemySkill_1006(enemy);
    }
}

// 落日
public class EnemySkill_1007 : EnemySkill
{

    override public void Use(Enemy enemy)
    {
        EnemyManager.Instance.EnemySkill_1007(enemy);
    }
}

// 湖中女的复仇
public class EnemySkill_1008 : EnemySkill
{

    override public void Use(Enemy enemy)
    {
        EnemyManager.Instance.EnemySkill_1008(enemy);
    }
}

// 魔音灌耳
public class EnemySkill_1009 : EnemySkill
{

    override public void Use(Enemy enemy)
    {
        EnemyManager.Instance.EnemySkill_1009(enemy);
    }
}

// 伤口污染
public class EnemySkill_1010 : EnemySkill
{

    override public void Use(Enemy enemy)
    {
        EnemyManager.Instance.EnemySkill_1010(enemy);
    }
}

// 恨意凝视
public class EnemySkill_1012 : EnemySkill
{

    override public void Use(Enemy enemy)
    {
        EnemyManager.Instance.EnemySkill_1012(enemy);
    }
}

// 瘟疫吐息
public class EnemySkill_1013 : EnemySkill
{

    override public void Use(Enemy enemy)
    {
        EnemyManager.Instance.EnemySkill_1013(enemy);
    }
}

// 怨念
public class EnemySkill_1016 : EnemySkill
{

    override public void Use(Enemy enemy)
    {
        EnemyManager.Instance.EnemySkill_1016(enemy);
    }
}

// 鼠群意志
public class EnemySkill_1020 : EnemySkill
{

    override public void Use(Enemy enemy)
    {
        BattleManager.Instance.SelectSkill_1020();
    }
}

// 禁咒
public class EnemySkill_1022 : EnemySkill
{

    override public void Use(Enemy enemy)
    {
        EnemyManager.Instance.EnemySkill_1022(enemy);
    }
}

// 最后一次守护
public class EnemySkill_1023 : EnemySkill
{

    override public void Use(Enemy enemy)
    {
        BattleManager.Instance.SelectSkill_1023();
    }
}
