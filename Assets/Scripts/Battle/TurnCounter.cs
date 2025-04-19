using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 回合计数器
// 实现功能：
// 1.记录战斗回合，大回合结束后调用更新方法
// 2.管理战斗回合中的Buff和dot伤害结算
public class TurnCounter
{
    private int currentTurn;
    private List<BattleBuff> buffsInBattle = new List<BattleBuff>();

    public TurnCounter()
    {
        currentTurn = 0;
    }

    public void UpdateTurn()
    {
        currentTurn++;
    }

    public void AddBuff(BattleBuff buff)
    {
        buffsInBattle.Add(buff);
    }

}
