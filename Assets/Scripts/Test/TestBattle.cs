using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//暂时先更改成Mono，挂在TestStart上，可以删除
public class TestBattle : MonoBehaviour
{
    private void Awake()
    {
        
        TestOfBattle();
    }

    void Update()
    {
        // if(Input.GetKeyDown(KeyCode.R))
        // {
        //     UIManager.Instance.ShowPanel<BattlePanel>();       
        // }
        
    }

    // 战斗系统测试
    public void TestOfBattle()
    {
        //（测试迁移到了BattlePanel中）
        // 定义玩家/敌人
        // PlayerManager.Instance.InitPlayer();
        // EnemySkill[] enemySkills = new EnemySkill[] { new EnemySkill_1001(), new EnemySkill_1002() };

        // Enemy enemy = new Enemy_1001(enemySkills);

        // //尝试给敌人加buff：
        // BattleBuff buff = new BattleBuff_1001();
        // enemy.buffs.Add(buff);
        
        // Enemy[] enemies = new Enemy[] { enemy };



        // 初始化战斗，随后启动战斗（迁移到了BattlePanel中）
        // BattleManager.Instance.BattleInit(PlayerManager.Instance.player, enemies);

        // BattleManager.Instance.BattleStart();
        // 角色发动攻击/使用道具
        // BattleManager.Instance.SelectSkill1();
        //BattleManager.Instance.SelectSkill2();
        // BattleManager.Instance.UseItem1();
    }

    // public static void ViewActionQueue(Queue actionQueue)
    // {
    //     Debug.Log("++++++ actionQueue view ++++++");
    //     foreach (var e in actionQueue)
    //     {
    //         Debug.Log(e.ToString());
    //     }
    //     Debug.Log("++++++ actionQueue view ++++++");
    // }
}
