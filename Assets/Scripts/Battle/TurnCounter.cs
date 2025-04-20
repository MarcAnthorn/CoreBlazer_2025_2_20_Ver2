using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 回合计数器
// 实现功能：
// 1.记录战斗回合，大回合结束后调用更新方法
// 2.管理战斗回合中的Buff和dot伤害结算
public class TurnCounter
{
    private int playerTurn;
    private List<int> enemyTurns = new List<int>();
    private List<BattleBuff> buffsInBattle = new List<BattleBuff>();

    public TurnCounter(params Enemy[] enemies)
    {
        playerTurn = 0;
        for(int i = 0; i < enemies.Length; i++)
        {
            enemyTurns.Add(0);
        }
    }
    
    // 更新玩家回合
    public void UpdatePlayerTurn()
    {
        playerTurn++;
        DealWithPlayerBuff(TriggerTiming.AfterTurn);
    }

    // 更新positionId指定的敌方回合
    public void UpdateEnemyTurn(int positionId)
    {
        enemyTurns[positionId]++;

    }

    // 添加战斗Buff
    public void AddBattleBuff(BattleBuff buff)
    {
        buffsInBattle.Add(buff);
    }

    // 处理(包括结算)玩家Buff
    private void DealWithPlayerBuff(TriggerTiming triggerTiming)
    {
        foreach(var buff in buffsInBattle)
        {
            if(buff.triggerTiming == triggerTiming)
            {
                buff.OnEffect();
            }
        }
    }

}
