﻿using System;
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

    public void BeHurted(float value)
    {
        HP -= value;
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
        name = "大章鱼";
        HP = 40;
        HP_limit = 40;
        STR = 18;
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

public class Enemy_1002 : Enemy
{
    public Enemy_1002(params EnemySkill[] enemySkills)
    {
        id = 1002;
        name = "树精";
        HP = 40;
        HP_limit = 40;
        STR = 20;
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
        HP_limit = 40;
        STR = 18;
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
        HP = 100;
        HP_limit = 100;
        STR = 15;  // 修正：从18改为15
        DEF = 3;
        CRIT_Rate = 0.2f;
        CRIT_DMG = 2.0f;
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
        HP = 50;
        HP_limit = 50;
        STR = 30;
        DEF = 20;
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
        HP = 90;
        HP_limit = 90;
        STR = 30;
        DEF = 10;
        CRIT_Rate = 0.6f;
        CRIT_DMG = 2.0f;
        AVO = 0.2f;
        HIT = 0.3f;
        SPD = 20;
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
        HP = 70;
        HP_limit = 70;
        STR = 50;
        DEF = 20;
        CRIT_Rate = 0.2f;
        CRIT_DMG = 1.2f;
        AVO = 0.2f;
        HIT = 0.5f;
        SPD = 15;
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
        HP = 70;
        HP_limit = 70;
        STR = 55;  // 修正：从100改为55
        DEF = 5;
        CRIT_Rate = 0.8f;
        CRIT_DMG = 1.2f;
        AVO = 0.2f;
        HIT = 0.5f;
        SPD = 10;
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
        HP = 200;
        HP_limit = 200;
        STR = 40;  // 修正：从70改为40
        DEF = 30;
        CRIT_Rate = 0.2f;
        CRIT_DMG = 1.2f;
        AVO = 0.5f;
        HIT = 0.3f;
        SPD = 20;
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
        HP = 70;
        HP_limit = 70;
        STR = 40;  // 修正：从80改为40
        DEF = 10;
        CRIT_Rate = 0.3f;
        CRIT_DMG = 1.2f;
        AVO = 0.2f;
        HIT = 0.3f;
        SPD = 10;
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
}

public class Enemy_1012 : Enemy
{
    public Enemy_1012(params EnemySkill[] enemySkills)
    {
        id = 1012;
        name = "鼠群意志";
        HP = 100;
        HP_limit = 100;
        STR = 20;
        DEF = 3;
        CRIT_Rate = 0.5f;
        CRIT_DMG = 1.2f;
        AVO = 0.4f;
        HIT = 2.5f;
        SPD = 20;
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
        HP = 180;
        HP_limit = 180;
        STR = 55;  // 修正：从60改为55
        DEF = 35;  // 修正：从40改为35
        CRIT_Rate = 0.7f;
        CRIT_DMG = 1.7f;
        AVO = 0.2f;
        HIT = 0.5f;
        SPD = 1;
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
        HP = 300;
        HP_limit = 300;
        STR = 70;  // 修正：从80改为70
        DEF = 30;
        CRIT_Rate = 0.2f;
        CRIT_DMG = 2.0f;
        AVO = 0.2f;
        HIT = 1.5f;
        SPD = 20;
        foreach (var skl in enemySkills)
        {
            this.enemySkills.Add(skl);
        }
    }
}

public class Enemy_1015 : Enemy
{
    public Enemy_1015(params EnemySkill[] enemySkills)
    {
        id = 1015;
        name = "死灵之书";
        HP = 90;
        HP_limit = 90;
        STR = 30;  // 修正：从80改为70
        DEF = 8;
        CRIT_Rate = 0.5f;
        CRIT_DMG = 1.3f;
        AVO = 0.2f;
        HIT = 0.4f;
        SPD = 8;
        foreach (var skl in enemySkills)
        {
            this.enemySkills.Add(skl);
        }
    }
}

public class Enemy_1016 : Enemy
{
    public Enemy_1016(params EnemySkill[] enemySkills)
    {
        id = 1016;
        name = "达贡";
        HP = 210;
        HP_limit = 210;
        STR = 75;  // 修正：从80改为70
        DEF = 30;
        CRIT_Rate = 0.1f;
        CRIT_DMG = 1.2f;
        AVO = 0.1f;
        HIT = 0.35f;
        SPD = 17;
        foreach (var skl in enemySkills)
        {
            this.enemySkills.Add(skl);
        }
    }
}