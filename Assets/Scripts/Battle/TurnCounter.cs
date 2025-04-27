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

    //当前的玩家buff：
    private List<BattleBuff> PlayerBuffs = new List<BattleBuff>();

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
        PlayerBuffs.Clear();
    }

    
    // 更新玩家回合
    public void UpdatePlayerTurn()
    {
        DealWithPlayerBuff(TriggerTiming.AfterTurn);

        playerTurn++;
        int n = PlayerBuffs.Count;
        bool cleanBattleBuff_1001 = false;
        for (int i = 0; i < n; i++)
        {
            PlayerBuffs[i].lastTurns--;
            if (PlayerBuffs[i].isEnd)
            {
                RemovePlayerBuff(PlayerBuffs[i]);
                if (PlayerBuffs[i].GetType() == typeof(BattleBuff_1001) && !cleanBattleBuff_1001)
                {
                    for (int j = 0; j < n; j++)
                    {
                        if (PlayerBuffs[i].GetType() == typeof(BattleBuff_1001))
                        {
                            if (j < i)
                            {
                                i--;
                            }
                            RemovePlayerBuff(PlayerBuffs[j]);
                            j--;
                                
                            n = PlayerBuffs.Count;        // buff数量更新
                        }
                    }
                }
                i--;

                n = PlayerBuffs.Count;        // buff数量更新
            }
        }

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
            if (enemy.buffs[i].isEnd)
            {
                RemoveEnemyBuff(enemy.buffs[i], positionId);
                if (enemy.buffs[i].GetType() == typeof(BattleBuff_1001) && !cleanBattleBuff_1001)
                {
                    for (int j = 0; j < n; j++)
                    {
                        if (enemy.buffs[i].GetType() == typeof(BattleBuff_1001))
                        {
                            if (j < i)
                            {
                                i--;
                            }
                            RemoveEnemyBuff(enemy.buffs[j], positionId);
                            j--;

                            n = enemy.buffs.Count;        // buff数量更新
                        }
                    }
                }
                i--;

                n = enemy.buffs.Count;        // buff数量更新
            }
        }

    }

    // 添加角色Buff
    public void AddPlayerBuff(BattleBuff buff)
    {
        PlayerBuffs.Add(buff);
        if (buff.GetOverlyingCount() < buff.overlyingLimit)
        {
            buff.OverlyingCountPlus(1);
        }
        else
        {
            Debug.Log($"Buff{buff.GetType()}叠加层数已达上限");
        }
    }

    // 添加敌人Buff
    public void AddEnemyBuff(BattleBuff buff, int positionId)
    {
        BattleManager.Instance.enemies[positionId].buffs.Add(buff);
        if (buff.GetOverlyingCount() < buff.overlyingLimit)
        {
            buff.OverlyingCountPlus(1);
        }
        else
        {
            Debug.Log($"Buff{buff.GetType()}叠加层数已达上限");
        }
    }

    // 移除角色Buff
    public void RemovePlayerBuff(BattleBuff buff)
    {
        PlayerBuffs.Remove(buff);

        buff.OverlyingCountPlus(-1);
    }

    // 移除敌人Buff
    public void RemoveEnemyBuff(BattleBuff buff, int positionId)
    {
        BattleManager.Instance.enemies[positionId].buffs.Add(buff);

        buff.OverlyingCountPlus(-1);
    }

    // 结算玩家Buff
    private void DealWithPlayerBuff(TriggerTiming triggerTiming)
    {
        foreach(var buff in PlayerBuffs)
        {
            if(buff.triggerTiming == triggerTiming)
            {
                buff.OnEffect();
            }
        }
    }

    // 结算指定敌人Buff
    private void DealWithEnemyBuff(TriggerTiming triggerTiming, int positionId)
    {
        foreach (var buff in BattleManager.Instance.enemies[positionId].buffs)
        {
            if (buff.triggerTiming == triggerTiming)
            {
                buff.OnEffect();
            }
        }
    }

    // 处理玩家身上Buff带来的加成效果
    public float CalculateWithPlayerBuff(TriggerTiming triggerTiming, float value)
    {
        CalculationType calType = CalculationType.NONE;
        float extraValue = 0;
        foreach (var buff in PlayerBuffs)
        {
            calType = buff.calculationType;
            if (buff.triggerTiming == triggerTiming)
            {
                extraValue += buff.influence;
            }
        }

        value = BuffManager.Instance.CalculationAfterBuff(calType, value, extraValue);

        return value;
    }

    // 处理指定敌人身上Buff带来的加成效果
    public float CalculateWithEnemyBuff(TriggerTiming triggerTiming, int positionId, float value)
    {
        CalculationType calType = CalculationType.NONE;
        float extraValue = 0;
        foreach (var buff in BattleManager.Instance.enemies[positionId].buffs)
        {
            calType = buff.calculationType;
            if (buff.triggerTiming == triggerTiming)
            {
                extraValue += buff.influence;
            }
        }

        value = BuffManager.Instance.CalculationAfterBuff(calType, value, extraValue);

        return value;
    }

}
