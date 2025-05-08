using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy
{
    public int id;
    public string name;
    public float HP;
    public float HP_limit;
    public float STR;
    public float DEF;
    public float CRIT_Rate;
    public float CRIT_DMG;
    public float AVO;
    public float HIT;
    public float SPD;
    public List<EnemySkill> enemySkills = new List<EnemySkill>();

    // 敌人在场景内的位置id
    public int positionId;          // 假设存在多个敌人时，能够通过positionId来进行选择并攻击
    public bool isDead = false;

    public List<BattleBuff> buffs = new List<BattleBuff>();

    // 敌人死亡动画
    public void DieAnimation()
    {

    }

    // public abstract void Attack(Player player);

    public void BeHurted(Damage damage)
    {
        Debug.LogWarning($"Now Enemy Damage is{damage.damage}");
        
        HP -= damage.damage;

        Debug.LogWarning($"Now Enemy HP is{HP}");
        if (HP <= 0)
        {
            Debug.Log("敌人死亡");        
            isDead = true;
        }
    }

    // 查找敌人身上是否存在某一类型的BattleBuff
    public bool ContainsBuff<T>(out BattleBuff buff)
    {
        foreach(var b in buffs)
        {
            if (b.GetType() == typeof(T) && b.overlyingCount != 0)
            {
                buff = b;
                return true;
            }
        }
        buff = null;
        return false;
    }

}


public class Enemy_1001 : Enemy
{
    public Enemy_1001(params EnemySkill[] enemySkills)
    {
        id = 1001;
        name = "哥布林";
        HP = 100;
        HP_limit = 100;
        STR = 12;
        DEF = 3;
        CRIT_Rate = 0.2f;
        CRIT_DMG = 1.2f;
        AVO = 0.2f;
        HIT = 0.3f;
        SPD = 9;
        foreach (var skl in enemySkills)
        {
            this.enemySkills.Add(skl);
        }
    }

    //override public void Attack(Player player)    //传入攻击的player实例
    //{
    //    Debug.Log("敌人发动攻击！");
    //    //将STR属性值转化为 攻击值 
    //    float rowDamage = STR * 1f;   //?? 假设伤害倍率就是100% ??
    //    EnemyManager.Instance.DamageCalculation(player, this, rowDamage);
    //}

}


public class Enemy_1002 : Enemy
{
    public Enemy_1002(params EnemySkill[] enemySkills)
    {
        id = 1002;
        name = "树精";
        HP = 40;
        HP_limit = 100;
        STR = 12;
        DEF = 3;
        CRIT_Rate = 0.2f;
        CRIT_DMG = 1.2f;
        AVO = 0.2f;
        HIT = 0.3f;
        SPD = 9;
        foreach (var skl in enemySkills)
        {
            this.enemySkills.Add(skl);
        }
    }

}

public class Enemy_1003 : Enemy
{
    public Enemy_1003(params EnemySkill[] enemySkills)
    {
        id = 1003;
        name = "女祭司";
        HP = 40;
        HP_limit = 100;
        STR = 12;
        DEF = 3;
        CRIT_Rate = 0.2f;
        CRIT_DMG = 1.2f;
        AVO = 0.2f;
        HIT = 0.3f;
        SPD = 9;
        foreach (var skl in enemySkills)
        {
            this.enemySkills.Add(skl);
        }
    }

}

public class Enemy_1004 : Enemy
{
    public Enemy_1004(params EnemySkill[] enemySkills)
    {
        id = 1004;
        name = "白国王";
        HP = 40;
        HP_limit = 100;
        STR = 12;
        DEF = 3;
        CRIT_Rate = 0.2f;
        CRIT_DMG = 1.2f;
        AVO = 0.2f;
        HIT = 0.3f;
        SPD = 9;
        foreach (var skl in enemySkills)
        {
            this.enemySkills.Add(skl);
        }
    }

}

public class Enemy_1005 : Enemy
{
    public Enemy_1005(params EnemySkill[] enemySkills)
    {
        id = 1005;
        name = "黑国王";
        HP = 40;
        HP_limit = 100;
        STR = 12;
        DEF = 3;
        CRIT_Rate = 0.2f;
        CRIT_DMG = 1.2f;
        AVO = 0.2f;
        HIT = 0.3f;
        SPD = 9;
        foreach (var skl in enemySkills)
        {
            this.enemySkills.Add(skl);
        }
    }

}

public class Enemy_1006 : Enemy
{
    public Enemy_1006(params EnemySkill[] enemySkills)
    {
        id = 1006;
        name = "牛头人";
        HP = 40;
        HP_limit = 100;
        STR = 12;
        DEF = 3;
        CRIT_Rate = 0.2f;
        CRIT_DMG = 1.2f;
        AVO = 0.2f;
        HIT = 0.3f;
        SPD = 9;
        foreach (var skl in enemySkills)
        {
            this.enemySkills.Add(skl);
        }
    }

}

public class Enemy_1007 : Enemy
{
    public Enemy_1007(params EnemySkill[] enemySkills)
    {
        id = 1007;
        name = "未知魔音";
        HP = 40;
        HP_limit = 100;
        STR = 12;
        DEF = 3;
        CRIT_Rate = 0.2f;
        CRIT_DMG = 1.2f;
        AVO = 0.2f;
        HIT = 0.3f;
        SPD = 9;
        foreach (var skl in enemySkills)
        {
            this.enemySkills.Add(skl);
        }
    }

}

public class Enemy_1008 : Enemy
{
    public Enemy_1008(params EnemySkill[] enemySkills)
    {
        id = 1008;
        name = "话痨死神";
        HP = 40;
        HP_limit = 100;
        STR = 12;
        DEF = 3;
        CRIT_Rate = 0.2f;
        CRIT_DMG = 1.2f;
        AVO = 0.2f;
        HIT = 0.3f;
        SPD = 9;
        foreach (var skl in enemySkills)
        {
            this.enemySkills.Add(skl);
        }
    }

}

public class Enemy_1009 : Enemy
{
    public Enemy_1009(params EnemySkill[] enemySkills)
    {
        id = 1009;
        name = "日下罪人";
        HP = 40;
        HP_limit = 100;
        STR = 12;
        DEF = 3;
        CRIT_Rate = 0.2f;
        CRIT_DMG = 1.2f;
        AVO = 0.2f;
        HIT = 0.3f;
        SPD = 9;
        foreach (var skl in enemySkills)
        {
            this.enemySkills.Add(skl);
        }
    }

}

public class Enemy_1010 : Enemy
{
    public Enemy_1010(params EnemySkill[] enemySkills)
    {
        id = 1010;
        name = "画中世界";
        HP = 40;
        HP_limit = 100;
        STR = 12;
        DEF = 3;
        CRIT_Rate = 0.2f;
        CRIT_DMG = 1.2f;
        AVO = 0.2f;
        HIT = 0.3f;
        SPD = 9;
        foreach (var skl in enemySkills)
        {
            this.enemySkills.Add(skl);
        }
    }

}

public class Enemy_1011 : Enemy
{
    public Enemy_1011(params EnemySkill[] enemySkills)
    {
        id = 1011;
        name = "科拉佐斯";
        HP = 40;
        HP_limit = 100;
        STR = 12;
        DEF = 3;
        CRIT_Rate = 0.2f;
        CRIT_DMG = 1.2f;
        AVO = 0.2f;
        HIT = 0.3f;
        SPD = 9;
        foreach (var skl in enemySkills)
        {
            this.enemySkills.Add(skl);
        }
    }

}

public class Enemy_1012 : Enemy
{
    public Enemy_1012(params EnemySkill[] enemySkills)
    {
        id = 1012;
        name = "鼠群意志";
        HP = 40;
        HP_limit = 100;
        STR = 12;
        DEF = 3;
        CRIT_Rate = 0.2f;
        CRIT_DMG = 1.2f;
        AVO = 0.2f;
        HIT = 0.3f;
        SPD = 9;
        foreach (var skl in enemySkills)
        {
            this.enemySkills.Add(skl);
        }
    }

}

public class Enemy_1013 : Enemy
{
    public Enemy_1013(params EnemySkill[] enemySkills)
    {
        id = 1013;
        name = "群山之主的思绪";
        HP = 40;
        HP_limit = 100;
        STR = 12;
        DEF = 3;
        CRIT_Rate = 0.2f;
        CRIT_DMG = 1.2f;
        AVO = 0.2f;
        HIT = 0.3f;
        SPD = 9;
        foreach (var skl in enemySkills)
        {
            this.enemySkills.Add(skl);
        }
    }

}

public class Enemy_1014 : Enemy
{
    public Enemy_1014(params EnemySkill[] enemySkills)
    {
        id = 1014;
        name = "格赫罗斯";
        HP = 40;
        HP_limit = 100;
        STR = 12;
        DEF = 3;
        CRIT_Rate = 0.2f;
        CRIT_DMG = 1.2f;
        AVO = 0.2f;
        HIT = 0.3f;
        SPD = 9;
        foreach (var skl in enemySkills)
        {
            this.enemySkills.Add(skl);
        }
    }

}
