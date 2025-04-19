using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    // 造成伤害
    public Damage CauseDamage(Player player, float singleDamage)
    {
        Damage damage = new Damage();
        if (JugdeAvoid(player))
        {
            damage.damage = -1;
            return damage;
        }

        damage.damage = singleDamage;
        return damage;
    }

    //命中判定
    private bool JugdeAvoid(Player player)
    {
        float avo = PlayerManager.Instance.player.AVO.value;        //!!先假设这是敌人的闪避值!!
        float random = UnityEngine.Random.Range(0f, 1f);
        if (random < avo)
        {
            return true;
        }

        return false;
    }

    public void EnemyHurted(int id, Damage damage)
    {
        // 在这里可以添加一些对伤害的检测(比如检测是否是暴击伤害) + 局内效果实现

        enemies[id].BeHurted(damage);
    }

}
