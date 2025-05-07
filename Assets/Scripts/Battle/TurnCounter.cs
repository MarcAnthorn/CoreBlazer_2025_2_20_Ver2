using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 回合计数器
// 实现功能：
// 1.记录战斗回合，大回合结束后调用更新方法
// 2.管理战斗回合中的Buff和dot伤害结算
public class TurnCounter : Singleton<TurnCounter>
{
    //玩家回合计数：
    private int playerTurn;

    private List<int> enemyTurns = new List<int>();

    //当前的玩家buff：(原先为什么是private,我更新为了public,我在BattlePanel中需要访问这个Buff列表)
    public List<BattleBuff> playerBuffs = new List<BattleBuff>();

    // 更新回合计数器
    public void InitTurnCounter(params Enemy[] enemies)
    {
        playerTurn = 0;
        for (int i = 0; i < enemies.Length; i++)
        {
            enemyTurns.Add(0);
        }
    }

    // 清理回合计数器
    public void ClearTurnCounter()
    {
        playerTurn = -1;
        enemyTurns.Clear();
        playerBuffs.Clear();
    }

    
    // 更新玩家回合
    public void UpdatePlayerTurn()
    {
        DealWithPlayerBuff(TriggerTiming.AfterTurn);

        playerTurn++;
        int n = playerBuffs.Count;
        bool cleanBattleBuff_1001 = false;
        for (int i = 0; i < n; i++)
        {
            playerBuffs[i].lastTurns--;

            Debug.Log($"buff:{playerBuffs[i]} current left rounds:{playerBuffs[i].lastTurns}, index is {i}, now i is{i}");

            if (playerBuffs[i].isEnd)
            {
                EventHub.Instance.EventTrigger("UpdateBuffUI", playerBuffs[i]);
                BattleBuff _buff = playerBuffs[i];

                RemovePlayerBuff(playerBuffs[i]);
                i--; // 在这里立刻执行 i--，不然还是会越界（上界）

                if (_buff.GetType() == typeof(BattleBuff_1001) && !cleanBattleBuff_1001)
                {
                    cleanBattleBuff_1001 = true; 
                    for (int j = 0; j < playerBuffs.Count; j++)
                    {
                        if (playerBuffs[j].GetType() == typeof(BattleBuff_1001))
                        {
                            RemovePlayerBuff(playerBuffs[j]);
                            j--; // 删除元素后，j--，防止跳过下一个
                        }
                    }
                }
                
            }
            n = playerBuffs.Count;        // buff数量更新
            
        }

        //更新Buff的回调函数：(位于BattlePanel)
        Debug.LogWarning("player's buff is updated");
        EventHub.Instance.EventTrigger("UpdateAllUIElements");

    }

    // 更新positionId指定的敌方回合
    public void UpdateEnemyTurn(int positionId)
    {
        DealWithEnemyBuff(TriggerTiming.AfterTurn, positionId);

        enemyTurns[positionId]++;
        Enemy enemy = BattleManager.Instance.enemies[positionId];
        int n = enemy.buffs.Count;
        bool cleanBattleBuff_1001 = false;
        for (int i = 0; i < n; i++)
        {
            enemy.buffs[i].lastTurns--;

            Debug.Log($"buff:{enemy.buffs[i]} current left rounds:{enemy.buffs[i].lastTurns}, index is {i}, now i is{i}");

            if (enemy.buffs[i].isEnd)
            {
                EventHub.Instance.EventTrigger("UpdateBuffUI",  enemy.buffs[i]);
                BattleBuff _buff = enemy.buffs[i];

                RemoveEnemyBuff(enemy.buffs[i], positionId);
                i--; // 在这里立刻执行 i--，不然还是会越界（上界）

                if (_buff.GetType() == typeof(BattleBuff_1001) && !cleanBattleBuff_1001)
                {
                    cleanBattleBuff_1001 = true; 
                    for (int j = 0; j < enemy.buffs.Count; j++)
                    {
                        if (enemy.buffs[j].GetType() == typeof(BattleBuff_1001))
                        {
                            RemoveEnemyBuff(enemy.buffs[j], positionId);
                            j--; // 删除元素后，j--，防止跳过下一个
                        }
                    }
                }
            }

            n =  enemy.buffs.Count; //更新n，不然i会越界；
        }

        //更新Buff的回调函数：
        Debug.LogWarning("enemy's buff is updated");
        EventHub.Instance.EventTrigger("UpdateAllUIElements");

    }

    // 添加角色Buff
    public void AddPlayerBuff(BattleBuff buff)
    {
        bool isAddLock = false;
        Type itemType = buff.GetType();
        foreach (var existingItem in playerBuffs)
        {
            if (existingItem.GetType() == itemType)
            {
                Debug.Log($"已经有同类 {itemType.Name}，不添加buff");
                isAddLock = true;
            }
        }
            
        if(!isAddLock)
            playerBuffs.Add(buff);

        if (buff.GetOverlyingCount() < buff.overlyingLimit)
        {
            buff.OverlyingCountPlus(1);
            buff.OnStart(0);
        }
        else
        {
            Debug.Log($"Buff{buff.GetType()}叠加层数已达上限");
            // 对于不可叠加的Buff来说，刷新持续回合
            if(buff.overlyingLimit == 1)
            {
                buff.lastTurns = buff.lastTurnLimit;
            }
        }
    }

    // 添加敌人Buff
    public void AddEnemyBuff(BattleBuff buff, int positionId = 0)
    {
        var list = BattleManager.Instance.enemies[positionId].buffs;
        bool isAddLock = false;
        Type itemType = buff.GetType();
        foreach (var existingItem in list)
        {
            if (existingItem.GetType() == itemType)
            {
                Debug.Log($"已经有同类 {itemType.Name}，不添加buff");
                isAddLock = true;
            }
        }
            
        if(!isAddLock)
            BattleManager.Instance.enemies[positionId].buffs.Add(buff);

        if (buff.GetOverlyingCount() < buff.overlyingLimit)
        {

            buff.OverlyingCountPlus(1);
            buff.OnStart(1);
        }
        else
        {
            Debug.Log($"Buff{buff.GetType()}叠加层数已达上限");
        }

        EventHub.Instance.EventTrigger("UpdateAllUIElements");
    }

    // 移除角色Buff
    public void RemovePlayerBuff(BattleBuff buff)
    {
        playerBuffs.Remove(buff);

        buff.OnEnd(0);
        buff.OverlyingCountPlus(-1);
    }

    // 移除敌人Buff
    public void RemoveEnemyBuff(BattleBuff buff, int positionId)
    {
        BattleManager.Instance.enemies[positionId].buffs.Remove(buff);

        buff.OnEnd(1);
        buff.OverlyingCountPlus(-1);
    }

    // 查找角色身上是否存在某一类型的BattleBuff
    public bool ContainsBuff<T>(out BattleBuff buff)
    {
        foreach (var b in playerBuffs)
        {
            if (b.GetType() == typeof(T) && b.GetOverlyingCount() != 0)
            {
                buff = b;
                return true;
            }
        }
        buff = null;
        return false;
    }

    // 结算玩家自身Buff的直接效果
    private void DealWithPlayerBuff(TriggerTiming triggerTiming)
    {
        foreach(var buff in playerBuffs)
        {
            if(buff.triggerTiming == triggerTiming)
            {
                Debug.LogWarning($"player buff affected!, buff is:{buff.name}");
                // 结算玩家
                buff.OnEffect(0);
            }
        }
    }

    // 结算指定敌人自身Buff的直接效果
    private void DealWithEnemyBuff(TriggerTiming triggerTiming, int positionId)
    {
        foreach (var buff in BattleManager.Instance.enemies[positionId].buffs)
        {
            if (buff.triggerTiming == triggerTiming)
            {
                Debug.LogWarning($"enemy buff affected!, buff is:{buff.name}");
                //结算敌方；
                buff.OnEffect(1);
            }
        }
    }

    // 处理玩家身上Buff带来的加成效果
    public float CalculateWithPlayerBuff(TriggerTiming triggerTiming, DamageType damageType, float value)
    {
        if(playerBuffs.Count == 0)
            return value;


        CalculationType calType = CalculationType.NONE;
        float extraValue = 0;
        if (playerBuffs.Count == 0)
        {
            return value;
        }
        foreach (var buff in playerBuffs)
        {
            calType = buff.calculationType;
            if (buff.triggerTiming == triggerTiming && buff.damageType.HasFlag(damageType))
            {
                extraValue += buff.influence;
            }
        }

        value = CalculationAfterBuff(calType, value, extraValue);

        Debug.Log($"Turn counter cal player value is {value}");

        return value;
        // return 0f;
    }

    // 处理指定敌人身上Buff带来的加成效果
    public float CalculateWithEnemyBuff(TriggerTiming triggerTiming, DamageType damageType, int positionId, float value)
    {
     
        if(BattleManager.Instance.enemies[positionId].buffs.Count == 0)
            return value;

        CalculationType calType = CalculationType.NONE;
        float extraValue = 0;

        foreach (var buff in BattleManager.Instance.enemies[positionId].buffs)
        {
            calType = buff.calculationType;
            if (buff.triggerTiming == triggerTiming && buff.damageType.HasFlag(damageType))
            {
                extraValue += buff.influence * buff.GetOverlyingCount();
            }
        }

        value = CalculationAfterBuff(calType, value, extraValue);

        Debug.Log($"Turn counter cal enemy value is {value}");

        return value;
    }

    // 角色在受到自身Buff影响下的 战斗内的 攻击伤害值计算
    public float PlayerBuffsBuffEffectInBattle(BuffType type, float value)
    {
        float finalValue = value;
        // float finalValue = 0;

        for (int i = 0; i < playerBuffs.Count; i++)
        {
            if (type == BuffType.HP_Change && playerBuffs[i].triggerTiming == TriggerTiming.CalculateGoodBuffDamage)    //战斗时增益/减益
            {
                finalValue = GetFinalValueFromBuff(playerBuffs[i], value, playerBuffs[i].influence);
            }
        }

        return finalValue;

    }

    // 敌人在受到自身Buff影响下的 战斗内的 攻击伤害值计算
    public float EnemyBuffsBuffEffectInBattle(Enemy enemy, BuffType type, float value)
    {
        float finalValue = 0f;

        for (int i = 0; i < enemy.buffs.Count; i++)
        {
            if (type == BuffType.HP_Change && enemy.buffs[i].triggerTiming == TriggerTiming.CalculateGoodBuffDamage)    //战斗时增益/减益
            {
                finalValue = GetFinalValueFromBuff(enemy.buffs[i], value, enemy.buffs[i].influence);
            }
        }

        Debug.Log($"final value in EnemyBuffsBuffEffectInBattle:{finalValue}");
        return finalValue;

    }

    //按照BuffType处理
    private float GetFinalValueFromBuff(BattleBuff buff, float value, float extraValue)
    {
        float finalValue;
        finalValue = CalculationAfterBuff(buff.calculationType, value, extraValue);

        return finalValue;
    }

    //按照CalculationType处理
    private float CalculationAfterBuff(CalculationType type, float value, float extraValue)
    {
        switch (type)
        {
            case CalculationType.Add:
                value += extraValue;
                return value;
            case CalculationType.Multiply:
                value *= (1.0f + extraValue);
                return value;
            case CalculationType.NONE:      //如果CalculationType为NONE其实就说明了该buff不涉及到伤害计算
                value = 0;
                return value;
            default:
                return 0;
        }
    }

}
