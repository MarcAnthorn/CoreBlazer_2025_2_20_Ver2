using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TestBattle : Singleton<TestBattle>
{
    new public void Awake()
    {
        base.Awake();
        TestOfBattle();
    }

    // 战斗系统测试
    public void TestOfBattle()
    {
        // 定义玩家/敌人
        PlayerManager.Instance.InitPlayer();
        EnemySkill[] enemySkills = new EnemySkill[] { new EnemySkill_1001(), new EnemySkill_1002() };
        Enemy[] enemies = new Enemy[] { new Enemy_1001(enemySkills) };
        // 初始化战斗，随后启动战斗
        BattleManager.Instance.BattleInit(PlayerManager.Instance.player, enemies);
        BattleManager.Instance.BattleStart();
        // 角色发动攻击/使用道具
        BattleManager.Instance.SelectSkill1();
        //BattleManager.Instance.SelectSkill2();
        BattleManager.Instance.UseItem1();
    }

    public void ViewActionQueue(Queue actionQueue)
    {
        Debug.Log("++++++ actionQueue view ++++++");
        foreach (var e in actionQueue)
        {
            Debug.Log(e.ToString());
        }
        Debug.Log("++++++ actionQueue view ++++++");
    }
}
