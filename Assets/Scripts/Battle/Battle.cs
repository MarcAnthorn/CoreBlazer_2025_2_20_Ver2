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
    // 回合计数器
    private TurnCounter turnCounter;
    // 角色攻击目标
    public int playerTarget;
    // 角色发动技能
    public int playerSkill;             // 即点即放：需要将对应技能按键点击设置为 为该字段赋值的功能
    // 角色行动点
    public int actionPoint;

    public Battle(Player player, params Enemy[] enemies)
    {
        this.player = player;
        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i].positionId = i + 1;
            this.enemies.Add(enemies[i]);
        }

        // 初始化战斗信息
        turnCounter = new TurnCounter(enemies);
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

    }

    // 进入角色回合
    public void EnterPlayerTurn()
    {
        playerTarget = 1;
        playerSkill = 1002;
        actionPoint = 3;

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

    private void PlayerAttack()
    {
        Debug.Log("角色释放技能！");
        actionPoint--;
        if(actionPoint == 1 && playerSkill == 1003)
        {
            Debug.Log("角色行动点不足！");
            return;
        }
        player.ReleaseSkill(playerSkill, enemies[playerTarget]);
        if(playerSkill == 1003)        // 如果释放了某些特殊技能，需要额外扣除一点行动点
        {
            actionPoint--;
        }
        // 阻塞，播放技能释放动画以及敌人受伤动画
        // 注意：播放动画的脚本处需要使用多线程
        while (true)
        {
            if (PlayerAttackAnimation())
            {
                break;
            }
        }
    }

    private void PlayerUseItem()
    {
        Debug.Log("角色使用道具！");
        // 调用角色使用道具的方法


    }

    // 解释：在方法EnterPlayerTurn()与方法ExitPlayerTurn()中间进行 敌人选择，技能选择等行动
    private void PlayerAction(bool isAttack)
    {
        StopCoroutine(InPlayerTurn());

        if (isAttack)
            PlayerAttack();
        else
            PlayerUseItem();

        // 行动点检查
        if (actionPoint > 0)
        {
            Debug.Log($"当前剩余{actionPoint}个行动点!");
        }
        else
        {
            Debug.Log("当前剩余0个行动点，轮到敌方行动!");
        }

        // 检查敌人状态
        foreach (var e in enemies)
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
        // 判断游戏状态
        if(enemies.Count == 0)
        {
            GameOver(true);
            return;
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

    // 退出角色回合
    // 按下结束键则代表角色主动结束该回合
    public void ExitPlayerTurn()
    {
        // 更新角色回合(并做出一些Buff处理)
        turnCounter.UpdatePlayerTurn();
        // 判断游戏状态
        if (player.isDie)
        {
            GameOver(false);
            return;
        }
        EnterEnemyTurn(1);
    }

    // 进入敌人回合
    public void EnterEnemyTurn(int positionId)
    {
        Debug.Log("敌人发动攻击！");
        enemies[positionId].BasicAttack(player);

        // 判断游戏状态
        if (player.isDie)
        {
            GameOver(false);
            return;
        }

        // 更新敌人回合(并做出一些Buff处理)
        turnCounter.UpdateEnemyTurn(positionId);

        // 排到队尾
        actionQueue.Enqueue(actionQueue.Dequeue());

        // 判断下一个行动的对象
        if(actionQueue.Peek().GetType() == typeof(Player))
        {
            EnterPlayerTurn();
        }
        else
        {
            EnterEnemyTurn(positionId + 1);
        }
    }



    // 游戏结束
    private void GameOver(bool isWin)
    {
        if (isWin)
        {
            Debug.Log("玩家胜利！");
        }
        else
        {
            Debug.Log("玩家落败！");
        }

        // 下面接奖励结算界面

    }

    // 下面是要与Button进行绑定的释放不同技能的方法
    // 玩家选择技能1
    public void SelectSkill1()
    {
        playerSkill = 1002;
        PlayerAction(true);
    }

    // 玩家选择技能2
    public void SelectSkill2()
    {
        playerSkill = 1003;
        PlayerAction(true);
    }

    // 下面是要与Button进行绑定的使用不同道具的方法(Marc应该已经实现了一些道具使用的方法了吧？)

}
