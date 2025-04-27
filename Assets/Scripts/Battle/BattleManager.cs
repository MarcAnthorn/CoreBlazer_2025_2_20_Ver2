using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 战斗类，进入战斗时将玩家与敌人拉入其中
public class BattleManager : Singleton<BattleManager>
{
    public Player player;
    public List<Enemy> enemies = new List<Enemy>();

    // 行动队列
    private Queue actionQueue = new Queue();
    // 回合计数器
    // public TurnCounter turnCounter;
    // 角色攻击目标
    public int playerTarget;
    // 角色发动的技能id
    public int playerSkill;             // 即点即放：需要将对应技能按键点击设置为 为该字段赋值的功能
    // 角色使用的道具id
    public int itemId;
    // 角色行动点
    public int actionPoint;

    //是否点击结束回合的bool标识：
    public bool isRoundEndTriggered = false;

    //协程句柄：
    private Coroutine roundCoroutine = null;

    // 初始化战斗
    // 在战斗开始之前调用；
    public void BattleInit(Player player, params Enemy[] enemies)
    {
        this.player = player;
        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i].positionId = i + 1;
            this.enemies.Add(enemies[i]);
        }

        // 初始化回合计数器
        TurnCounter.Instance.InitTurnCounter(enemies);
        roundCoroutine = null;
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
                if (enemy.isDead)
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
        //调整当前玩家的playerSceneIndex
        PlayerManager.Instance.playerSceneIndex = E_PlayerSceneIndex.Battle;

        // 向行动队列中先后加入player和enemy
        actionQueue.Enqueue(player);
        for (int i = 0; i < enemies.Count; i++)
        {
            actionQueue.Enqueue(enemies[i]);
        }


        TestBattle.Instance.ViewActionQueue(actionQueue);

        if(player.SPD.value >= enemies[0].SPD)
        {
            Debug.Log("角色速度：" + player.SPD.value + "  >=  " + "敌人速度：" + enemies[0].SPD);
            Debug.Log("角色优先攻击！");
            EnterPlayerTurn();
        }
        else
        {
            Debug.Log("角色速度：" + player.SPD.value + "  <  " + "敌人速度：" + enemies[0].SPD);
            Debug.Log("敌人优先攻击！");
            EnterEnemyTurn(1);
        }
    }

    // 进入角色回合
    public void EnterPlayerTurn()
    {
        playerTarget = 1;
        playerSkill = 1002;
        actionPoint = 3;

        roundCoroutine = StartCoroutine(InPlayerTurn());
    }

    // 表示现在正处于角色阶段
    IEnumerator InPlayerTurn()
    {
        Debug.Log("等待玩家行动...");
        isRoundEndTriggered = false;
        
        //外部通过触发isRoundEndTriggered的方式让协程继续：
        yield return new WaitUntil(() => isRoundEndTriggered);

//--------------Marc添加内容-----------------------------------

        ExitPlayerTurn();
    }

    private void PlayerAttack()
    {
        Debug.Log($"角色释放技能{playerSkill}");
        if(actionPoint == 1 && playerSkill == 1003)
        {
            Debug.Log("角色行动点不足！");
            return;
        }
        actionPoint--;
        SkillManager.Instance.ReleaseSkill(playerSkill, enemies[playerTarget - 1]);
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
        Debug.Log($"角色使用道具{itemId}");
        actionPoint--;
        // 调用角色使用道具的方法


    }

    // 解释：在方法EnterPlayerTurn()与方法ExitPlayerTurn()中间进行 敌人选择，技能选择等行动
    private void PlayerAction(bool isAttack)
    {
        //使用句柄停止协程：
        StopCoroutine(roundCoroutine);

        if (isAttack)
            PlayerAttack();
        else
            PlayerUseItem();

        // // 检查敌人状态
        // 这个写法会导致迭代器失效（边遍历边移除元素）
        // foreach (var e in enemies)
        // {
        //     int count = 1;      // 用于执行一些范围伤害判定(如果有)

        //     // 进行一些判断
        //     if (e.isDead)
        //     {
        //         enemies.Remove(e);
        //         // 敌人消失动画
        //         e.DieAnimation();
        //     }

        //     count++;
        // }

        // 检查敌人状态
        List<Enemy> deadEnemies = new List<Enemy>();  // 临时列表，记录死亡的敌人

        foreach (var e in enemies)
        {
            if (e.isDead)
            {
                deadEnemies.Add(e);   // 先记下来
                e.DieAnimation();     // 触发死亡动画
            }
        }

        // 循环结束后统一移除
        foreach (var dead in deadEnemies)
        {
            enemies.Remove(dead);
        }

        deadEnemies.Clear();



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

        // 行动点检查
        if (actionPoint > 0)
        {
            Debug.Log($"当前剩余{actionPoint}个行动点!");
        }
        else
        {
            Debug.Log("当前剩余0个行动点，轮到敌方行动!");
        }

        // 排到队尾
        actionQueue.Enqueue(actionQueue.Dequeue());
    }

    // 退出角色回合
    // 按下结束键则代表角色主动结束该回合
    public void ExitPlayerTurn()
    {
        // 更新角色回合(并做出一些Buff处理)
        TurnCounter.Instance.UpdatePlayerTurn();
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
        int index = positionId - 1;
        Debug.Log("敌人发动攻击！");
        foreach(var s in enemies[index].enemySkills)
        {
            s.Use(enemies[index]);
        }
        // enemies[positionId].Attack(player);

        // 判断游戏状态
        if (player.isDie)
        {
            GameOver(false);
            return;
        }

        // 更新敌人回合(并做出一些Buff处理)
        TurnCounter.Instance.UpdateEnemyTurn(index);

        // 排到队尾
        actionQueue.Enqueue(actionQueue.Dequeue());

        // 判断下一个行动的对象
        if(actionQueue.Peek().GetType() == typeof(Player))
        {
            EnterPlayerTurn();
        }
        else
        {
            EnterEnemyTurn(index + 1);
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

        TurnCounter.Instance.ClearTurnCounter();

        // 下面接奖励结算界面


        //调整当前玩家的playerSceneIndex
        PlayerManager.Instance.playerSceneIndex = E_PlayerSceneIndex.Maze;

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
    // 玩家使用道具1
    public void UseItem1()
    {
        itemId = 1002;
        PlayerAction(false);
    }

    // 玩家使用道具2
    public void UseItem2()
    {
        itemId = 1003;
        PlayerAction(false);
    }




}
