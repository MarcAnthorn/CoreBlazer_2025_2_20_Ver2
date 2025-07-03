using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Events;


//怪物101，初始即追踪，无巡逻状态
public class MonsterChase101 : MonsterBase
{
    [Header("怪物101配置")]
    public float detectionRange = 100f;        // 检测范围
    public float chaseSpeed; // 追逐速度

    private float chaseDelayTime = 2;   //唤醒到追逐的延迟时间
    //虚拟相机：
    public CinemachineVirtualCamera _cam;


    //在Awake中调用的初始化状态的方法：InitializeStates
    protected override void InitializeStates()
    {
        Debug.LogWarning($"Monster Initialized!");
        
        chaseSpeed = 1.2f * 3 * (1 + (5 * PlayerManager.Instance.player.SPD.value - 10) / 100);       
        // 创建自定义状态
        patrolState = null;
        chaseState = new Chase101State(this);
           
        // 设置初始状态为追逐
        LeanTween.delayedCall(chaseDelayTime, ()=>{
            SwitchToChase();
        });
        
    }

    protected override void Awake() {
        base.Awake();
        // 设置怪物特殊属性
        moveSpeedBase = chaseSpeed;
        enemyId = 1001;

        _cam.Priority = 11;       
        LeanTween.delayedCall(2f, ()=>{
            _cam.Priority = 0;   
        });


    }


    //追踪回调
    protected override void OnChaseEnd()
    {
        Debug.LogWarning($"怪物{enemyId}追逐结束, 触发战斗");

        // 触发战斗：
        //  var panel = UIManager.Instance.ShowPanel<BattlePanel>();
        //     panel.InitEnemyInfo(enemyId);
        //     panel.callback = OnComplete;
        //     EventHub.Instance.EventTrigger<bool>("Freeze", true);

        //暂时先是触发回去的内容：
        OnComplete();
   
    }

    protected override void OnComplete()
    {
        base.OnComplete();    
        UIManager.Instance.ShowPanel<AVGPanel>().InitAVG(1106, OnAVGComplete);

    }

    private void OnAVGComplete(int id)
    {
        Destroy(this.gameObject);
        GameLevelManager.Instance.gameLevelType = E_GameLevelType.First;
        LoadSceneManager.Instance.LoadSceneAsync("ShelterScene");
        EventHub.Instance.EventTrigger<bool>("Freeze", false);
    }



    //无巡逻怪物，无需以下内容
    // protected override void InitPatrolPoints()
    // {
    //     //此处可以存储巡逻点
        
    //     //PatrolState会在进入State的时候初始化巡逻index；此处不需要
    //     // patrolIndex = 0;
    // }


    // 在Inspector中可视化巡逻点
    // private void OnDrawGizmosSelected()
    // {
    //     if (patrolPoints != null && patrolPoints.Count > 0)
    //     {
    //         Gizmos.color = Color.yellow;
            
    //         // 绘制巡逻点
    //         for (int i = 0; i < patrolPoints.Count; i++)
    //         {
    //             Gizmos.DrawWireSphere(patrolPoints[i], 0.5f);
                
    //             // 绘制巡逻路径
    //             if (i < patrolPoints.Count - 1)
    //             {
    //                 Gizmos.DrawLine(patrolPoints[i], patrolPoints[i + 1]);
    //             }
    //             else
    //             {
    //                 Gizmos.DrawLine(patrolPoints[i], patrolPoints[0]);
    //             }
    //         }
            
    //         // 绘制检测范围
    //         Gizmos.color = Color.red;
    //         Gizmos.DrawWireSphere(transform.position, detectionRange);
    //     }
    // }
}


// 自定义巡逻状态
public class Patrol101State : PatrolState
{
    private MonsterChase101 monster101;

    public Patrol101State(MonsterChase101 monster)
    {
        monster101 = monster;
    }

    protected override void OnPatrolUpdate(MonsterBase monster)
    {
        
    }
}


// 自定义追逐状态
public class Chase101State : ChaseState
{
    private MonsterChase101 monster101;

    public Chase101State(MonsterChase101 monster)
    {
        monster101 = monster;
        pathUpdateInterval = 0.5f;          //路径更新的时间阈值
        minDistanceToUpdate = 1f;           //路径更新的距离阈值
    }

    protected override void OnChaseUpdate(MonsterBase monster)
    {
        base.OnChaseUpdate(monster);
        
    }
}