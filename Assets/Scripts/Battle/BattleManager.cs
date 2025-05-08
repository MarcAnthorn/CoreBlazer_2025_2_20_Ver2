using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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

//----------------角色最大行动点（Marc添加）--------------------------------
    public int actionPointMax;

    //是否点击结束回合的bool标识：
    public bool isRoundEndTriggered = false;

    //协程句柄：
    private Coroutine roundCoroutine = null;


    protected override void Awake()
    {
        base.Awake();

        EventHub.Instance.AddEventListener<bool, UnityAction<int>>("GameOver", GameOver);
    }


    protected override void OnDestroy()
    {
        base.OnDestroy();
        EventHub.Instance.RemoveEventListener<bool, UnityAction<int>>("GameOver", GameOver);
    }

    // 初始化战斗
    // 在战斗开始之前调用；
    public void BattleInit(Player player, params Enemy[] enemies)
    {
        this.player = player;
        for (int i = 0; i < enemies.Length; i++)
        {
            // enemies[i].positionId = i;
            this.enemies.Add(enemies[i]);
        }

        // Debug.Log($"current enemy count:{this.enemies.Count}");

        // 初始化回合计数器
        TurnCounter.Instance.InitTurnCounter(enemies);
        roundCoroutine = null;

 //----------------------初始化最大行动点是3:(Marc)-----------------------------------
        actionPointMax = 3;
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

//----------报错注释：（Marc）---------------------------------------------------------------
        //TestBattle更改为Mono,该方法ViewActionQueue被设置为static;
        TestBattle.ViewActionQueue(actionQueue);

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
    
//----------------更改（Marc）：------------------------------------------------
        // actionPoint = 3;
        actionPoint = actionPointMax;
        EventHub.Instance.EventTrigger("UpdateAllUIElements");

        roundCoroutine = StartCoroutine(InPlayerTurn());
    }

    // 表示现在正处于角色阶段
    IEnumerator InPlayerTurn()
    {
//----------------协程变动：（原先的逻辑有问题）------------------------------------------------

        Debug.Log("等待玩家行动...");
        isRoundEndTriggered = false;
        
        //外部通过触发isRoundEndTriggered的方式让协程继续：
        yield return new WaitUntil(() => isRoundEndTriggered);

        Debug.LogWarning("now exit player's turn");

//--------------Marc添加内容---------------------------------------------------

        ExitPlayerTurn();
    }

    private void PlayerAttack()
    {
        Debug.Log($"角色释放技能{playerSkill}");
        // 将技能点交给 ReleaseSkill 方法处理
        SkillManager.Instance.ReleaseSkill(ref actionPoint, playerSkill, player, enemies[playerTarget - 1]);

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
//----------------使用句柄停止协程：（Marc）--------------------------------
        //if(roundCoroutine != null)
        //    StopCoroutine(roundCoroutine);

        if (isAttack)
            PlayerAttack();
        else
            PlayerUseItem();




        // 检查敌人状态
        List<Enemy> deadEnemies = new List<Enemy>();  // 临时列表，记录死亡的敌人

        foreach (var e in enemies)
        {
            if (e.isDead)
            {
                BattleBuff buff;
                if (e.ContainsBuff<BattleBuff_1022>(out buff))  // 对“守护”进行特判
                {
                    buff.overlyingCount -= 1;
                    e.buffs.Remove(buff);
                    buff.OnEffect(1);
                }
                else
                {
                    deadEnemies.Add(e);   // 先记下来
                    e.DieAnimation();     // 触发死亡动画
                }
            }
        }

        // 循环结束后统一移除
        foreach (var dead in deadEnemies)
        {
            enemies.Remove(dead);
        }

        deadEnemies.Clear();

//--------------------------------------------------------------------------------


        // // 判断游戏状态
        // if(enemies.Count == 0)
        // {
        //     GameOver(true);
        //     return;
        // }


        // 更新行动队列
        UpdateActionQueue();

        // 更新敌人位置
        // for (int i = 0; i < enemies.Count; i++)
        // {
        //     enemies[i].positionId = i + 1;
        // }

        // 行动点检查
        if (actionPoint >= 0)
        {
            Debug.Log($"当前剩余{actionPoint}个行动点!");
            EventHub.Instance.EventTrigger("UpdateAllUIElements");
            
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
        Debug.Log("Exit player's turn");
        // 更新角色回合(并做出一些Buff处理)
        TurnCounter.Instance.UpdatePlayerTurn();
        
        // 排到队尾
        actionQueue.Enqueue(actionQueue.Dequeue());

        // 判断游戏状态
        // 在DamageCalculation中，存在player的BeHurt方法；该方法会进行一次是否死亡的判断；
        // if (player.isDie)
        // {
        //     BattleBuff buff;
        //     if (TurnCounter.Instance.ContainsBuff<BattleBuff_1022>(out buff))  // 对“守护”进行特判
        //     {
        //         buff.overlyingCount += 1;
        //         TurnCounter.Instance.playerBuffs.Remove(buff);
        //         buff.OnEffect(0);
        //     }
        //     else
        //     {
        //         GameOver(false);
        //         return;
        //     }
        // }

        //在进入地方回合之前，延迟一段时间，等buff结算的UI效果结束：
        LeanTween.delayedCall(1f, ()=>{
            EnterEnemyTurn(1);
        });
        
    }

    // 进入敌人回合
    public void EnterEnemyTurn(int positionId)
    {
        int index = positionId - 1;
        Debug.Log("敌人发动攻击！");

        //使用协程替换原先的技能无间隔释放：
        //改成一定时间间隔释放技能：
        StartCoroutine(EnemyAttack(enemies[index], index));


        // // 判断游戏状态
        // if (player.isDie)
        // {
        //     GameOver(false);
        //     return;
        // }
    }

    //敌方释放技能的协程：
    private IEnumerator EnemyAttack(Enemy enemy, int index)
    {
        foreach(var s in enemy.enemySkills)
        {
            s.Use(enemy);

            //敌方释放技能的Tip：
            UIManager.Instance.ShowPanel<WarningPanel>().SetWarningText($"「{enemy.name}」释放了技能「{s.skillName}」", true);

            yield return new WaitForSeconds(2f);    //假设设定为2s执行一次进攻；
        }

        // 更新敌人回合(并做出一些Buff处理)
        TurnCounter.Instance.UpdateEnemyTurn(index);

        // 排到队尾
        actionQueue.Enqueue(actionQueue.Dequeue());

        EnterPlayerTurn();

        // 判断下一个行动的对象
        //if (actionQueue.Peek().GetType() == typeof(Player))
        //{
        //    EnterPlayerTurn();
        //}
        //else
        //{
        //    EnterEnemyTurn(index + 1);
        //}
    }



    // 游戏结束
    //第二参数是游戏结束之后的回调函数；只有获胜之后才会触发：
    private void GameOver(bool isWin, UnityAction<int> callback)
    {
        if (isWin)
        {
            UIManager.Instance.ShowPanel<WarningPanel>().SetWarningText($"击败敌人「{enemies[0].name}」，战斗胜利", false, ()=>{
                UIManager.Instance.HidePanel<BattlePanel>();

                //如果需要触发战斗的后续奖励，在这里触发；
                callback?.Invoke(enemies[0].id);
            });
        }
        else
        {
            UIManager.Instance.ShowPanel<WarningPanel>().SetWarningText($"战斗失败", false, ()=>{
                UIManager.Instance.HidePanel<BattlePanel>();
            });
        }

        TurnCounter.Instance.ClearTurnCounter();

        // 下面接奖励结算界面


//----------------调整当前玩家的playerSceneIndex（Marc添加）------------------------------------------------
        PlayerManager.Instance.playerSceneIndex = E_PlayerSceneIndex.Maze;
    }

    // 下面是要与Button进行绑定的释放不同技能的方法
    // 玩家选择技能 格斗
    public void SelectSkill_1001()
    {
        playerSkill = 1001;
        PlayerAction(true);
    }

    // 玩家选择技能 毒针
    public void SelectSkill_1002()
    {
        playerSkill = 1002;
        PlayerAction(true);
    }

    // 玩家选择技能  新月之辉
    public void SelectSkill_1003()
    {
        playerSkill = 1003;
        PlayerAction(true);
    }

    // 玩家选择技能  心火
    public void SelectSkill_1004()
    {
        playerSkill = 1004;
        PlayerAction(true);
    }

    // 玩家选择技能  破势击
    public void SelectSkill_1005()
    {
        playerSkill = 1005;
        PlayerAction(true);
    }

    // 玩家选择技能  缚心铎声
    public void SelectSkill_1006()
    {
        playerSkill = 1006;
        PlayerAction(true);
    }

    // 玩家选择技能  落日
    public void SelectSkill_1007()
    {
        playerSkill = 1007;
        PlayerAction(true);
    }

    // 玩家选择技能  湖中女的复仇
    public void SelectSkill_1008()
    {
        playerSkill = 1008;
        PlayerAction(true);
    }

    // 玩家选择技能  魔音灌耳
    public void SelectSkill_1009()
    {
        playerSkill = 1009;
        PlayerAction(true);
    }

    // 玩家选择技能  伤口污染
    public void SelectSkill_1010()
    {
        playerSkill = 1010;
        PlayerAction(true);
    }

    // 玩家选择技能  鼠群意志
    public void SelectSkill_1020()
    {
        playerSkill = 1020;
        PlayerAction(true);
    }

    // 玩家选择技能  最后一次守护
    public void SelectSkill_1023()
    {
        playerSkill = 1023;
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
