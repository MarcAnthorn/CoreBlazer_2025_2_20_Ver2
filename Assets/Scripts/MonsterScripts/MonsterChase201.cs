using System.Collections;
using System.Collections.Generic;
using System.IO;
using Cinemachine;
using UnityEngine;

public class MonsterChase201 : MonsterBase
{
    [Header("怪物201配置")]
    public float detectionRange = 100f;        // 检测范围
    public float chaseSpeed; // 追逐速度

    private float chaseDelayTime = 2;   //唤醒到追逐的延迟时间
    //虚拟相机：
    public CinemachineVirtualCamera _cam;
    private GameObject reward;


    //在Awake中调用的初始化状态的方法：InitializeStates
    protected override void InitializeStates()
    {
        Debug.LogWarning($"Monster Initialized!");
        
        chaseSpeed = 1.2f * 3 * (1 + (5 * PlayerManager.Instance.player.SPD.value - 10) / 100);       
        // 创建自定义状态
        patrolState = null;
        chaseState = new Chase201State(this);
           
        // 设置初始状态为追逐
        LeanTween.delayedCall(chaseDelayTime, ()=>{
            SwitchToChase();
        });
        
    }

    protected override void Awake() {
        base.Awake();
        // 设置怪物特殊属性
        moveSpeedBase = chaseSpeed;
        enemyId = 1015;

        _cam.Priority = 11;       
        LeanTween.delayedCall(2f, ()=>{
            _cam.Priority = 0;   
        });

        reward = Resources.Load<GameObject>(Path.Combine("MapPOIs", "RewardChaseBoss"));

    }


    //追踪回调
    protected override void OnChaseEnd()
    {
        Debug.LogWarning($"怪物{enemyId}追逐结束, 触发战斗");

        //触发战斗：
        var panel = UIManager.Instance.ShowPanel<BattlePanel>();
        panel.InitEnemyInfo(enemyId);
        panel.callback = OnComplete;
        EventHub.Instance.EventTrigger<bool>("Freeze", true);


   
    }

    protected override void OnComplete(int id)
    {
        //在最近的地块生成宝箱：
        int x, y;
        PathFindingManager.Instance.GetGridIndex(this.transform.position, out x, out y);
        Vector3 targetPosition = PathFindingManager.Instance.GetWorldPosition(x, y);

        Instantiate(reward, targetPosition, Quaternion.identity);

    
        Destroy(this.gameObject);

        

    }

    protected override void Freeze(bool _isFrozen)
    {
        base.Freeze(_isFrozen);     
    }

}




// 自定义巡逻状态
public class Patrol201State : PatrolState
{
    private MonsterChase201 monster201;

    public Patrol201State(MonsterChase201 monster)
    {
        monster201 = monster;
    }

    protected override void OnPatrolUpdate(MonsterBase monster)
    {
        
    }
}


// 自定义追逐状态
public class Chase201State : ChaseState
{
    private MonsterChase201 monster201;

    public Chase201State(MonsterChase201 monster)
    {
        monster201 = monster;
        pathUpdateInterval = 0.5f;          //路径更新的时间阈值
        minDistanceToUpdate = 1f;           //路径更新的距离阈值
    }

    protected override void OnChaseUpdate(MonsterBase monster)
    {
        base.OnChaseUpdate(monster);
        
    }
}