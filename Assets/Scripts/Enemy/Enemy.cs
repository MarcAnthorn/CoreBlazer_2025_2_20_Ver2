using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy
{
    public float HP;
    public float STR;
    public float AVO;

    // 敌人在场景内的位置id
    public int positionId;          // 假设存在多个敌人时，能够通过positionId来进行选择并攻击
    public bool isDead = false;

    public List<BattleBuff> buffs = new List<BattleBuff>();

    public Enemy()
    {
        HP = 100;
        STR = 10;
    }

    // 敌人死亡动画
    public void DieAnimation()
    {

    }

    public void BasicAttack(Player player)    //传入攻击的player实例
    {
        Debug.Log("敌人发动攻击！");
        //将STR属性值转化为 攻击值 
        float rowDamage = STR * 1f;   //?? 假设伤害倍率就是100% ??
        float damageValue = EnemyManager.Instance.CalculateDamageAfterBuff(AttributeType.HP, rowDamage);
        Damage damage = EnemyManager.Instance.CauseDamage(player, damageValue);
        if (damage.damage == -1)
        {
            Debug.Log("敌人发出的伤害被闪避了!");
        }
        else
        {
            // 造成伤害之前进行一些加成计算
            damage.damage = TurnCounter.Instance.CalculateWithPlayerBuff(TriggerTiming.CalculateDamage, damage.damage);
            //调用玩家受击方法
            PlayerManager.Instance.player.BeHurted(damage);
        }
    }

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
