using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class EnemyManager : Singleton<EnemyManager>
{
    public Dictionary<int, Enemy> enemies = new Dictionary<int, Enemy>();   // <positionId, enemy>

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private BuffType GetEnemyBuffType(AttributeType type)
    {
        switch (type)
        {
            case AttributeType.HP:
                return BuffType.HP_Change;
            case AttributeType.STR:
                return BuffType.STR_Change;
            case AttributeType.DEF:
                return BuffType.DEF_Change;
            case AttributeType.LVL:
                return BuffType.LVL_Change;
            case AttributeType.SAN:
                return BuffType.SAN_Change;
            case AttributeType.SPD:
                return BuffType.SPD_Change;
            case AttributeType.CRIT_Rate:
                return BuffType.CRIT_Rate_Change;
            case AttributeType.CRIT_DMG:
                return BuffType.CRIT_DMG_Change;
            case AttributeType.HIT:
                return BuffType.HIT_Change;
            case AttributeType.AVO:
                return BuffType.AVO_Change;
            default:
                return BuffType.NONE;
        }
    }

    //战斗内的 敌人对玩家造成影响的方法(一般来说就是HP和SAN值上的影响)
    public float CalculateDamageAfterBuff(AttributeType type, float value)
    {
        //这是为了明确buff要不要执行(根据buffType来判断)
        BuffType buffType = GetEnemyBuffType(type);
        //finalValue表示 战斗过程 造成的实际数值变化
        float finalValue = BuffManager.Instance.BuffEffectInBattle(buffType, value);

        return finalValue;
    }

    // 计算敌人造成的伤害
    public void DamageCalculation(Player player, Enemy enemy, float rowDamage)
    {
        // 计算防御收益
        rowDamage -= player.DEF.value;
        // 计算敌人身上的Buff
        float damageValue = EnemyManager.Instance.CalculateDamageAfterBuff(AttributeType.HP, rowDamage);
        List<Damage> damages = EnemyManager.Instance.CauseDamage(enemy, damageValue);
        if (damages.Count == 0)
        {
            Debug.Log("敌人发出的伤害被闪避了!");
        }
        else
        {
            foreach (var dmg in damages)
            {
                // 造成伤害之前进行一些加成计算
                dmg.damage = TurnCounter.Instance.CalculateWithPlayerBuff(TriggerTiming.CalculateDamage, dmg.damage);
                //调用玩家受击方法
                PlayerManager.Instance.player.BeHurted(dmg);
            }

        }
    }

    // 造成伤害
    public List<Damage> CauseDamage(Enemy enemy, float singleDamage)
    {
        List<Damage> damages = new List<Damage>();
        if (JugdeAvoid(enemy))
        {
            return damages;
        }
        else
        {
            JudgeHit(enemy, singleDamage, out damages);
        }

        return damages;
    }

    //命中判定
    private bool JugdeAvoid(Enemy enemy)
    {
        float avo = enemy.AVO;
        float random = UnityEngine.Random.Range(0f, 1f);
        if (random < avo)
        {
            return true;
        }

        return false;
    }

    //连击判定
    private void JudgeHit(Enemy enemy, float singleDamage, out List<Damage> damages)
    {
        /*连击判定(由Player类来处理每一次连击的效果)
         *  对每一次连击进行暴击判定
         */
        List<Damage> damages_return = new List<Damage>();

        float hit = enemy.HIT;
        int baseHit = (int)Math.Ceiling(hit);       //向上取整
        float hitRate = hit + 1 - baseHit;
        float crit_rate = enemy.CRIT_Rate;
        float crit_dmg = enemy.CRIT_DMG;
        for (int i = 0; i < baseHit + 1; i++)
        {
            Damage tempDamage = new Damage();
            float random1 = UnityEngine.Random.Range(0f, 1f);
            if (random1 < crit_rate)
            {
                tempDamage.damage = singleDamage * (1 + crit_dmg);
                tempDamage.isCritical = true;
            }
            else
            {
                tempDamage.isCritical = false;
            }

            if (baseHit == 0)
            {
                float random2 = UnityEngine.Random.Range(0f, 1f);
                if (random2 < hitRate)
                {
                    continue;
                }
                else
                {
                    break;
                }
            }

            damages_return.Add(tempDamage);
            baseHit--;
        }

        damages = damages_return;

    }

    public void EnemyHurted(int id, Damage damage)
    {
        // 在这里可以添加一些对伤害的检测(比如检测是否是暴击伤害) + 局内效果实现


        enemies[id].BeHurted(damage);
    }

    // 敌人技能定义处
    // 拳打脚踢
    public void EnemySkill_1001(Enemy enemy)
    {
        Debug.Log("敌人发动 拳打脚踢！");
        //将STR属性值转化为 攻击值 
        float rowDamage = enemy.STR * 1f;   //?? 假设伤害倍率就是100% ??
        EnemyManager.Instance.DamageCalculation(PlayerManager.Instance.player, enemy, rowDamage);
    }

    // 毒针
    public void EnemySkill_1002(Enemy enemy)
    {
        Debug.Log("敌人发动 毒针！");
        //将STR属性值转化为 攻击值 
        float rowDamage = enemy.STR * 0.2f;   //?? 假设伤害倍率就是20% ??
        EnemyManager.Instance.DamageCalculation(PlayerManager.Instance.player, enemy, rowDamage);
    }

}
