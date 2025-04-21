using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : Singleton<SkillManager>
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 释放技能
    public void ReleaseSkill(int skillId, Enemy enemy)
    {
        switch (skillId)
        {
            case 1002:
                Skill_1002(enemy);
                break;
            case 1003:
                Skill_1003(enemy);
                break;
            default:
                break;
        }

    }

    public void Skill_1002(Enemy enemy)
    {
        Debug.Log("角色发动普通攻击！");
        //将STR属性值转化为 攻击值 
        float rowDamage = PlayerManager.Instance.player.STR.value * 1f;   //?? 假设伤害倍率就是100% ??
        SkillManager.Instance.DamageCalculation(enemy, rowDamage);
    }

    public void Skill_1003(Enemy enemy)
    {
        Debug.Log("角色发动至圣斩！");
        //将STR属性值转化为 攻击值 
        float rowDamage = PlayerManager.Instance.player.STR.value * 2.5f;   //?? 假设伤害倍率就是250% ??
        SkillManager.Instance.DamageCalculation(enemy, rowDamage);
    }

    // 伤害结算
    public void DamageCalculation(Enemy enemy, float rowDamage)
    {
        float damage = PlayerManager.Instance.CalculateDamageAfterBuff(AttributeType.HP, rowDamage);
        List<Damage> damages = PlayerManager.Instance.CauseDamage(enemy, damage);
        if (damages.Count == 0)
        {
            Debug.Log("角色发出的伤害被闪避了!");
        }
        else
        {
            foreach (var dmg in damages)
            {
                // 造成伤害之前进行一些加成计算
                dmg.damage = TurnCounter.Instance.CalculateWithEnemyBuff(TriggerTiming.CalculateDamage, enemy.positionId, dmg.damage);
                //调用敌人受击方法
                EnemyManager.Instance.EnemyHurted(enemy.positionId, dmg);
            }
        }
    }

}
