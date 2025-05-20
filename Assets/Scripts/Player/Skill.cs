using System.Collections;
using System.Collections.Generic;
using System.IO;
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
        skillName = "格斗";
        skillIconPath = Path.Combine("ArtResources", "Buff", "力量增益");
        
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
        skillName = "毒针";
        skillIconPath = Path.Combine("ArtResources", "Buff", "中毒");
        
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
        skillName = "新月之辉";
        skillIconPath = Path.Combine("ArtResources", "Buff", "易伤");
        
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
        skillName = "心火";
        skillIconPath = Path.Combine("ArtResources", "Buff", "暴击率");
        
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
        skillName = "破势击";
        skillIconPath = Path.Combine("ArtResources", "Buff", "力量增益");
        
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
        skillName = "缚心铎声";
        skillIconPath = Path.Combine("ArtResources", "Buff", "易伤");
        
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
        skillName = "落日";
        skillIconPath = Path.Combine("ArtResources", "Buff", "力量增益");
        
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
        skillName = "湖中女的复仇";
        skillIconPath = Path.Combine("ArtResources", "Buff", "力量增益");
        
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
        skillName = "魔音灌耳";
        skillIconPath = Path.Combine("ArtResources", "Buff", "易伤");
        
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
        skillName = "伤口污染";
        skillIconPath = Path.Combine("ArtResources", "Buff", "易伤");
        
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

// 幽影呓语
public class Skill_1011 : Skill
{
    public Skill_1011()
    {
        id = 1011;
        skillName = "幽影呓语";
        skillIconPath = Path.Combine("ArtResources", "Buff", "中毒");
        
        skillDamageText = "20";
        skillBuffText = "①debuff的持有者受到怪谈伤害后，给对手施加3层易伤";
        skillCost = 2;

        skillDamage = 20;
    }

    override public void Use()
    {
        BattleManager.Instance.SelectSkill_1011();
    }
}

// 恨意凝视
public class Skill_1012 : Skill
{
    public Skill_1012()
    {
        id = 1012;
        skillName = "恨意凝视";
        skillIconPath = Path.Combine("ArtResources", "Buff", "中毒");
        
        skillDamageText = "20";
        skillBuffText = "①debuff的持有者受到力量伤害后，给对手施加3层力量增伤";
        skillCost = 2;

        skillDamage = 20;
    }

    override public void Use()
    {
        BattleManager.Instance.SelectSkill_1012();
    }
}

// 瘟疫吐息
public class Skill_1013 : Skill
{
    public Skill_1013()
    {
        id = 1013;
        skillName = "瘟疫吐息";
        skillIconPath = Path.Combine("ArtResources", "Buff", "中毒");
        
        skillDamageText = "20";
        skillBuffText = "①debuff的持有者受到道具伤害后，给予debuff持有者的对手施加3层dot增伤和3层怪谈增伤";
        skillCost = 2;

        skillDamage = 20;
    }

    override public void Use()
    {
        BattleManager.Instance.SelectSkill_1013();
    }
}

// 探知深空
public class Skill_1014 : Skill
{
    public Skill_1014()
    {
        id = 1014;
        skillName = "探知深空";
        skillIconPath = Path.Combine("ArtResources", "Buff", "中毒");
        
        skillDamageText = "20";
        skillBuffText = "①debuff的持有者受到怪谈伤害后，给予debuff持有者的对手施加3层dot增伤和3层力量增伤";
        skillCost = 2;

        skillDamage = 20;
    }

    override public void Use()
    {
        BattleManager.Instance.SelectSkill_1014();
    }
}

// 饥饿掠夺
public class Skill_1015 : Skill
{
    public Skill_1015()
    {
        id = 1015;
        skillName = "饥饿掠夺";
        skillIconPath = Path.Combine("ArtResources", "Buff", "中毒");
        
        skillDamageText = "20";
        skillBuffText = "①debuff的持有者受到道具伤害后，给予debuff持有者的对手施加3层力量增伤";
        skillCost = 2;

        skillDamage = 20;
    }

    override public void Use()
    {
        BattleManager.Instance.SelectSkill_1015();
    }
}

// 怨念
public class Skill_1016 : Skill
{
    public Skill_1016()
    {
        id = 1016;
        skillName = "怨念";
        skillIconPath = Path.Combine("ArtResources", "Buff", "中毒");
        
        skillDamageText = "20";
        skillBuffText = "①debuff持有者的受到怪谈伤害后，给予debuff持有者的对手施加3层dot增伤和3层力量增伤";
        skillCost = 2;

        skillDamage = 20;
    }

    override public void Use()
    {
        BattleManager.Instance.SelectSkill_1016();
    }
}

// 深海的呼唤
public class Skill_1017 : Skill
{
    public Skill_1017()
    {
        id = 1017;
        skillName = "深海的呼唤";
        skillIconPath = Path.Combine("ArtResources", "Buff", "易伤");
        
        skillDamageText = "20";
        skillBuffText = "①debuff持有者受到力量伤害或道具伤害后，给予debuff持有者的对手施加3层易伤";
        skillCost = 2;

        skillDamage = 20;
    }

    override public void Use()
    {
        BattleManager.Instance.SelectSkill_1017();
    }
}

// 深渊之主的回音
public class Skill_1018 : Skill
{
    public Skill_1018()
    {
        id = 1018;
        skillName = "深渊之主的回音";
        skillIconPath = Path.Combine("ArtResources", "Buff", "易伤");
        
        skillDamageText = "20";
        skillBuffText = "①对手受到dot伤害后，给对手施加3层易伤";
        skillCost = 2;

        skillDamage = 20;
    }

    override public void Use()
    {
        BattleManager.Instance.SelectSkill_1018();
    }
}

// 风起之时
public class Skill_1019 : Skill
{
    public Skill_1019()
    {
        id = 1019;
        skillName = "风起之时";
        skillIconPath = Path.Combine("ArtResources", "Buff", "连击");
        
        skillDamageText = "20";
        skillBuffText = "①对手受到道具伤害后，自己+30%连击";
        skillCost = 2;

        skillDamage = 20;
    }

    override public void Use()
    {
        BattleManager.Instance.SelectSkill_1019();
    }
}

// 鼠群意志
public class Skill_1020 : Skill
{
    public Skill_1020()
    {
        id = 1020;
        skillName = "鼠群意志";
        skillIconPath = Path.Combine("ArtResources", "Buff", "连击");
        
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

// 神骨之鞭
public class Skill_1021 : Skill
{
    public Skill_1021()
    {
        id = 1021;
        skillName = "风起之时";
        skillIconPath = Path.Combine("ArtResources", "Buff", "力量增益");
        
        skillDamageText = "20";
        skillBuffText = "①debuff持有者受到怪谈伤害或力量伤害后，给对手施加3层力量增伤";
        skillCost = 2;

        skillDamage = 20;
    }

    override public void Use()
    {
        BattleManager.Instance.SelectSkill_1021();
    }
}

// 禁咒
public class Skill_1022 : Skill
{
    public Skill_1022()
    {
        id = 1022;
        skillName = "禁咒";
        skillIconPath = Path.Combine("ArtResources", "Buff", "易伤");
        
        skillDamageText = "20";
        skillBuffText = "①对手受到怪谈伤害后，给予debuff持有者的对手施加3层易伤和3层dot增伤";
        skillCost = 2;

        skillDamage = 20;
    }

    override public void Use()
    {
        BattleManager.Instance.SelectSkill_1022();
    }
}

// 最后一次守护
public class Skill_1023 : Skill
{
    public Skill_1023()
    {
        id = 1023;
        skillName = "最后一次守护";
        skillIconPath = Path.Combine("ArtResources", "Buff", "力量增益");
        
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
    public EnemySkill_1001()
    {
        skillName = "格斗";
    }
    
    override public void Use(Enemy enemy)
    {
        EnemyManager.Instance.EnemySkill_1001(enemy);
    }
}

// 毒针
public class EnemySkill_1002 : EnemySkill
{
    public EnemySkill_1002()
    {
        skillName = "毒针";
    }

    override public void Use(Enemy enemy)
    {
        EnemyManager.Instance.EnemySkill_1002(enemy);
    }
}

// 新月之辉
public class EnemySkill_1003 : EnemySkill
{
    public EnemySkill_1003()
    {
        skillName = "新月之辉";
    }

    override public void Use(Enemy enemy)
    {
        EnemyManager.Instance.EnemySkill_1003(enemy);
    }
}

// 心火
public class EnemySkill_1004 : EnemySkill
{
    public EnemySkill_1004()
    {
        skillName = "心火";
    }

    override public void Use(Enemy enemy)
    {
        EnemyManager.Instance.EnemySkill_1004(enemy);
    }
}

// 破势击
public class EnemySkill_1005 : EnemySkill
{
    public EnemySkill_1005()
    {
        skillName = "破势击";
    }

    override public void Use(Enemy enemy)
    {
        EnemyManager.Instance.EnemySkill_1005(enemy);
    }
}

// 缚心铎声
public class EnemySkill_1006 : EnemySkill
{
    public EnemySkill_1006()
    {
        skillName = "缚心铎声";
    }

    override public void Use(Enemy enemy)
    {
        EnemyManager.Instance.EnemySkill_1006(enemy);
    }
}

// 落日
public class EnemySkill_1007 : EnemySkill
{
    public EnemySkill_1007()
    {
        skillName = "落日";
    }

    override public void Use(Enemy enemy)
    {
        EnemyManager.Instance.EnemySkill_1007(enemy);
    }
}

// 湖中女的复仇
public class EnemySkill_1008 : EnemySkill
{
    public EnemySkill_1008()
    {
        skillName = "湖中女的复仇";
    }

    override public void Use(Enemy enemy)
    {
        EnemyManager.Instance.EnemySkill_1008(enemy);
    }
}

// 魔音灌耳
public class EnemySkill_1009 : EnemySkill
{
    public EnemySkill_1009()
    {
        skillName = "魔音灌耳";
    }

    override public void Use(Enemy enemy)
    {
        EnemyManager.Instance.EnemySkill_1009(enemy);
    }
}

// 伤口污染
public class EnemySkill_1010 : EnemySkill
{
    public EnemySkill_1010()
    {
        skillName = "伤口污染";
    }

    override public void Use(Enemy enemy)
    {
        EnemyManager.Instance.EnemySkill_1010(enemy);
    }
}

// 恨意凝视
public class EnemySkill_1012 : EnemySkill
{
    public EnemySkill_1012()
    {
        skillName = "恨意凝视";
    }

    override public void Use(Enemy enemy)
    {
        EnemyManager.Instance.EnemySkill_1012(enemy);
    }
}

// 瘟疫吐息
public class EnemySkill_1013 : EnemySkill
{
    public EnemySkill_1013()
    {
        skillName = "瘟疫吐息";
    }

    override public void Use(Enemy enemy)
    {
        EnemyManager.Instance.EnemySkill_1013(enemy);
    }
}

// 怨念
public class EnemySkill_1016 : EnemySkill
{
    public EnemySkill_1016()
    {
        skillName = "怨念";
    }

    override public void Use(Enemy enemy)
    {
        EnemyManager.Instance.EnemySkill_1016(enemy);
    }
}

// 鼠群意志
public class EnemySkill_1020 : EnemySkill
{
    public EnemySkill_1020()
    {
        skillName = "鼠群意志";
    }

    override public void Use(Enemy enemy)
    {
        BattleManager.Instance.SelectSkill_1020();
    }
}

// 禁咒
public class EnemySkill_1022 : EnemySkill
{
    public EnemySkill_1022()
    {
        skillName = "禁咒";
    }

    override public void Use(Enemy enemy)
    {
        EnemyManager.Instance.EnemySkill_1022(enemy);
    }
}

// 最后一次守护
public class EnemySkill_1023 : EnemySkill
{
    public EnemySkill_1023()
    {
        skillName = "最后一次守护";
    }

    override public void Use(Enemy enemy)
    {
        BattleManager.Instance.SelectSkill_1023();
    }
}
