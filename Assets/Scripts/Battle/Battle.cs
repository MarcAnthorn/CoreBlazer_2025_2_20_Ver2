using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 战斗类，进入战斗时将玩家与敌人拉入其中
public class Battle : Singleton<Battle>
{
    public Player player;
    public List<Enemy> enemies = new List<Enemy>();

    // 行动队列
    private Queue actionQueue = new Queue();
    // 当前回合
    private int currentTurn;
    // 角色攻击目标
    public int playerTarget;
    // 角色发动技能
    public int playerSkill;

    public Battle(Player player, params Enemy[] enemies)
    {
        this.player = player;
        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i].positionId = i + 1;
            this.enemies.Add(enemies[i]);
        }
    }

    public void BattleInit()
    {

    }

    // 角色攻击动画(假设有)
    public bool PlayerAttackAnimation()
    {
        return true;
    }

    // 行动队列更新方法
    public void UpdateActionQueue()
    {
        object temp = null;
        for(int i = 0; i < actionQueue.Count; i++)
        {
            temp = actionQueue.Dequeue();
            if(temp.GetType() != typeof(Player))
            {
                Enemy enemy = temp as Enemy;
                if (enemy.isDie)
                {
                    continue;
                }
                actionQueue.Enqueue(temp);
            }
        }
    }

    // 战斗开始
    public void BattleStart()
    {
        // 向行动队列中先后加入player和enemy
        actionQueue.Enqueue(player);
        for (int i = 0; i < enemies.Count; i++)
        {
            actionQueue.Enqueue(enemies[i + 1]);
        }

        // 初始化战斗信息
        currentTurn = 0;

    }

    // 进入角色回合
    public void EnterPlayerTurn()
    {
        playerTarget = 1;
        playerSkill = 1;

        StartCoroutine(InPlayerTurn());
    }

    // 表示现在正处于角色阶段
    IEnumerator InPlayerTurn()
    {
        while (true)
        {
            Debug.Log("等待玩家行动...");

            yield return new WaitForSeconds(3f);
        }
    }

    // 退出角色回合
    // 当玩家按下攻击键后调用此方法
    // 解释：在方法EnterPlayerTurn()与方法ExitPlayerTurn()中间进行 敌人选择，技能选择等行动
    // 按下攻击键则代表角色主动结束该回合(使用道具包含在中间那段逻辑里)
    public void ExitPlayerTurn()
    {
        StopCoroutine(InPlayerTurn());

        Debug.Log("角色释放技能！");
        player.ReleaseSkill(playerSkill, enemies[playerTarget]);
        // 阻塞，播放技能释放动画以及敌人受伤动画
        // 注意：播放动画的脚本处需要使用多线程
        while (true)
        {
            if (PlayerAttackAnimation())
            {
                break;
            }
        }

        // 检查敌人状态
        foreach(var e in enemies)
        {
            int count = 1;      // 用于执行一些范围伤害判定(如果有)

            // 进行一些判断
            if (e.isDie)
            {
                enemies.Remove(e);
                // 敌人消失动画
                e.DieAnimation();
            }

            count++;
        }
        // 更新行动队列
        UpdateActionQueue();
        // 更新敌人位置
        for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i].positionId = i + 1;
        }
        // 排到队尾
        actionQueue.Enqueue(actionQueue.Dequeue());
    }

    // 进入敌人回合
    public void EnterEnemyTurn(int positionId)
    {

    }

}
