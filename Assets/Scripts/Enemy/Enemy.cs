using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy
{
    // 敌人在场景内的位置id
    public int positionId;          // 假设存在多个敌人时，能够通过positionId来进行选择并攻击
    public float HP;
    public float STR;
    public bool isDie = false;

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
        
    }

    public void BeHurted(Damage damage)
    {
        HP -= damage.damage;
        if (HP <= 0)
        {
            Debug.Log($"敌人 {positionId} 死亡!");
            isDie = true;
        }
    }

}
