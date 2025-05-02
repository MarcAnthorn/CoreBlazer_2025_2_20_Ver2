using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

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
    public void ReleaseSkill(ref int actionPoint, int skillId, Player player, Enemy enemy)
    {
        switch (skillId)
        {
            case 1001:
                Skill_1001(enemy);
                if(actionPoint >= 1)
                    actionPoint -= 1;
                else
                    Debug.Log("角色行动点不足以释放技能 格斗");
                break;
            case 1002:
                Skill_1002(enemy);
                if (actionPoint >= 1)
                    actionPoint -= 1;           // 这里原本消耗行动点数量为 0 ???
                else
                    Debug.Log("角色行动点不足以释放技能 毒针");
                break;
            default:
                break;
        }

    }

    // 结算角色造成的伤害
    public void DamageCalculation(Enemy enemy, float rowDamage, Action action = null)
    {
        // 计算防御收益
        rowDamage = Mathf.Max(0, rowDamage - enemy.DEF);
        // 计算角色身上的Buff加成
        float damage = PlayerManager.Instance.CalculateDamageAfterBuff(AttributeType.HP, rowDamage);

        Debug.Log($"Damage before buff is{damage}");
        
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

                //结算出的对敌人的伤害
                Debug.LogWarning($"结算出的对敌人的伤害：{dmg.damage}");
                
                //调用敌人受击方法
                EnemyManager.Instance.EnemyHurted(enemy.positionId, dmg);

                if(action != null)
                {
                    action.Invoke();
                }
            }
        }
    }

    // 格斗
    public void Skill_1001(Enemy enemy)
    {
        Debug.Log("角色发动 格斗！");
        //将STR属性值转化为 攻击值 
        float rowDamage = PlayerManager.Instance.player.STR.value * 1f;
        SkillManager.Instance.DamageCalculation(enemy, rowDamage);
    }

    // 毒针
    public void Skill_1002(Enemy enemy)
    {
        Debug.Log("角色发动 毒针！");
        float rowDamage = PlayerManager.Instance.player.SPD.value * 0.2f + 8;
        Action action = () => AddBuffToSkill_1002(enemy);
        SkillManager.Instance.DamageCalculation(enemy, rowDamage, action);
    }
    private void AddBuffToSkill_1002(Enemy enemy)
    {
        BattleBuff buff = new BattleBuff_1001();
        enemy.buffs.Add(buff);
    }

    // 新月之辉
    public void Skill_1003(Enemy enemy)
    {
        Debug.Log("角色发动 新月之辉！");
        float rowDamage = 50;
        SkillManager.Instance.DamageCalculation(enemy, rowDamage);
    }

    // 心火
    public void Skill_1004(Player player)
    {
        Debug.Log("角色发动 心火！");
        player.HP.value -= 10;
        // 逻辑要改很多，暂时不写了
        // AddBuffToSkill_1004();
    }
    private void AddBuffToSkill_1004()
    {
        BattleBuff buff = new BattleBuff_1003();
        TurnCounter.Instance.playerBuffs.Add(buff);
    }

}
