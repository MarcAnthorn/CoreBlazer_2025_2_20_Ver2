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
        HP -= damage.damage;
        if (HP <= 0)
        {
            Debug.Log($"敌人 {positionId} 死亡!");
            isDead = true;
        }
    }

}


public class Enemy_1001 : Enemy
{
    public Enemy_1001(params EnemySkill[] enemySkills)
    {
        id = 1001;
        name = "哥布林";
        HP = 90;
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
