using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Events;


//以下是怪物基类
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
public abstract class MonsterBase : MonoBehaviour
{
   
    [Range(0, 5)]
    public float moveSpeedBase;
    public bool isMoving = false;  // 改为public以便状态类访问
    //当前怪物对应的enemyId:
    public int enemyId;
    private Animator animator;
    private SpriteRenderer sr;
    protected Vector3 startPoint;
    protected Vector3 endPoint;
    private List<Vector3> path;
    
    //巡逻状态下，存在若干巡逻点位；按照巡逻点位的顺序进行巡逻：
    public List<Vector3> patrolPoints = new List<Vector3>();
    //当前所处的巡逻点位index：
    public int patrolIndex;


    // 状态模式相关
    protected IMonsterState currentState;
    protected PatrolState patrolState;
    protected ChaseState chaseState;
    protected E_StateFlag stateFlag;

    //移动协同程序：
    protected Coroutine moveCoroutine;
    
    // 暂停相关
    private bool isPaused = false;


//-----------------------------------------------------------------------------


    protected virtual void Awake()
    {
        EventHub.Instance.AddEventListener<bool>("Freeze", Freeze);
        EventHub.Instance.AddEventListener<int>("AdjustLayer", AdjustLayer);
        EventHub.Instance.AddEventListener<Vector3>("ChangeDestination", ChangeDestination);

        
        InitializeStates();

        moveSpeedBase = 3;

        animator = this.GetComponent<Animator>();
        sr = this.GetComponent<SpriteRenderer>();

        //设置自己的Tag:
        this.gameObject.tag = "Monster";
    }

    protected void Update()
    {
        startPoint = this.transform.position;
        
        // 更新当前状态
        if (currentState != null)
        {
            currentState.UpdateState(this);
        }
    }

    protected virtual void OnDestroy()
    {
        EventHub.Instance.RemoveEventListener<bool>("Freeze", Freeze);
        EventHub.Instance.RemoveEventListener<int>("AdjustLayer", AdjustLayer);
        EventHub.Instance.RemoveEventListener<Vector3>("ChangeDestination", ChangeDestination);
    }


//-----------------------------------------------------------------------------

    // 初始化状态 - 子类需要实现
    // 子类需要设置 patrolState 和 chaseState
    // 例如：patrolState = new MyPatrolState();
    //      chaseState = new MyChaseState();

    //以及需要初始化当前的默认状态；   
    //比如：设置初始状态为巡逻
    // if (patrolState != null)
    // {
    //     TransitionToState(patrolState);
    // }
    protected abstract void InitializeStates();

    //初始化巡逻点位的方法：
    protected virtual void InitPatrolPoints(){}

    //追逐结束后的回调函数：
    protected abstract void OnChaseEnd();

    //战斗结束后的回调函数：
    protected virtual void OnComplete(int 占位){}

    // 状态转换方法
    public void TransitionToState(IMonsterState newState)
    {
        if (currentState != null)
        {
            currentState.ExitState(this);
        }
        
        currentState = newState;
        currentState.EnterState(this);
    }

    // 切换到巡逻状态
    public void SwitchToPatrol()
    {
        if (patrolState != null)
        {
            stateFlag = E_StateFlag.Patrol;
            TransitionToState(patrolState);
        }
    }

    // 切换到追逐状态
    public void SwitchToChase()
    {
        if (chaseState != null)
        {
            stateFlag = E_StateFlag.Chase;
            TransitionToState(chaseState);
        }
    }

    //变更终点 or 重新寻路时调用的方法：
    //注意：如果处在追踪状态下，那么该方法会被持续调用，用于确保玩家位置的准确性；
    public virtual void ChangeDestination(Vector3 newDestination)
    {
        endPoint = newDestination;

        // 用对象池获取path
        if (path != null)
        {
            ListPool<Vector3>.Release(path);
        }
        path = PathFindingManager.Instance.FindPath(startPoint, endPoint);
        if (path != null && path.Count > 0)
        {
            isPaused = false;
            isMoving = true;
            
            if (moveCoroutine != null)
            {
                StopCoroutine(moveCoroutine);
            }
            moveCoroutine = StartCoroutine(MoveAlongPath(path));
        }

        //if path is null, then the destination isnt reachable:
        //make it stay static:
        else
        {
            //路径不可达：终止移动；依然尝试每0.5s检测一次是否可以找到路径         
            PauseMoving();
        }
    }

    //碰撞检测：如果直接撞上玩家，则直接调用ReachDestination:
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            //终止相关内容：
            if (moveCoroutine != null)
            {
                StopCoroutine(moveCoroutine);
                moveCoroutine = null;
            }
            
            isMoving = false;
            isPaused = false;

            
            // 回收路径到对象池
            if (path != null)
            {
                ListPool<Vector3>.Release(path);
                path = null;
            }
            
            Debug.Log($"{enemyId} 移动已完全停止, 触发回调函数");

            //调用回调函数
            OnChaseEnd();
        }
    }

    //冻结时调用的方法：
    protected virtual void Freeze(bool _isFrozen)
    {
        if(this == null)
        {
            return;
        }

        Debug.LogWarning($"now state:{_isFrozen}");
        
        if (_isFrozen)
        {
            // 游戏暂停时，暂停移动
            PauseMoving();
        }
        else
        {
            // 游戏恢复时，恢复移动
            ResumeMoving();
        }
    }

    //响应layer变化的事件：
    private void AdjustLayer(int layer)
    {
        sr.sortingOrder = layer;
    }

    // 引入ListPool工具类
    public static class ListPool<T>
    {
        private static readonly Stack<List<T>> pool = new Stack<List<T>>();

        public static List<T> Get()
        {
            return pool.Count > 0 ? pool.Pop() : new List<T>();
        }

        public static void Release(List<T> toRelease)
        {
            toRelease.Clear();
            pool.Push(toRelease);
        }
    }
    
//------------------------------------移动寻路相关----------------------------------
    //怪物移动协同程序：
    private IEnumerator MoveAlongPath(List<Vector3> path)
    {
        isMoving = true;
        // 直接用path参数，不再new List
        Vector3 targetPos;
        for (int i = 0; i < path.Count; i++)
        {
            targetPos = path[i];
            while (Vector3.Distance(transform.position, targetPos) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeedBase * Time.deltaTime);
                yield return null; // 等待下一帧
            }
            // 保证最终位置对齐
            transform.position = targetPos;
            yield return null;
        }

        //到达终点：
        ReachDestination();

        // 协程结束后回收path到对象池
        if (path != null)
        {
            ListPool<Vector3>.Release(path);
        }
    }

    // 从暂停位置恢复移动的协程
    private IEnumerator MoveAlongPathFromPause(List<Vector3> path)
    {
        isMoving = true;
        Vector3 targetPos;
        
        // 从暂停位置开始，找到当前应该移动到的目标点
        Vector3 currentPos = transform.position;
        
        // 找到当前暂停位置在路径中的位置
        int startIndex = FindClosestPathIndex(currentPos, path);
        
        //继续移动：
        for (int i = startIndex; i < path.Count; i++)
        {
            targetPos = path[i];
            
            // 如果当前位置不是目标点，先移动到目标点
            if (Vector3.Distance(currentPos, targetPos) > 0.01f)
            {
                while (Vector3.Distance(transform.position, targetPos) > 0.01f)
                {
                    transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeedBase * Time.deltaTime);
                    yield return null;
                }
            }
            
            // 保证最终位置对齐
            transform.position = targetPos;
            currentPos = targetPos;
            yield return null;
        }
        
        //到达终点：
        ReachDestination();

        // 协程结束后回收path到对象池
        if (path != null)
        {
            ListPool<Vector3>.Release(path);
        }
    }

    // 找到最接近当前位置的路径点索引
    private int FindClosestPathIndex(Vector3 currentPos, List<Vector3> path)
    {
        if (path == null || path.Count == 0)
            return 0;
            
        float minDistance = float.MaxValue;
        int closestIndex = 0;
        
        for (int i = 0; i < path.Count; i++)
        {
            float distance = Vector3.Distance(currentPos, path[i]);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestIndex = i;
            }
        }
        
        return closestIndex;
    }

    // 暂停移动 - 保存当前状态以便恢复
    public virtual void PauseMoving()
    {
        if (!isPaused)
        {
            isPaused = true;
            
            if (moveCoroutine != null)
            {
                StopCoroutine(moveCoroutine);
                moveCoroutine = null;
            }
            
            isMoving = false;
            Debug.Log($"{name} 移动已暂停");
        }
    }

    // 恢复移动 - 从暂停位置继续
    public virtual void ResumeMoving()
    {
        if (isPaused && path != null && path.Count > 0)
        {
            isPaused = false;
            isMoving = true;
            
            // 重新启动移动协程，从暂停位置继续
            moveCoroutine = StartCoroutine(MoveAlongPathFromPause(path));
            
            Debug.Log($"{name} 移动已恢复");
        }
    }


    //在对象到达自己当前状态的终点后调用的方法：
    public virtual void ReachDestination()
    {
        Debug.LogWarning("Reach Destination!");
        //只会在巡逻状态触发这个函数；
        //如果是巡逻状态，那么就重新寻路；调用ChangeDestination向着下一个路径点出发
        if(stateFlag == E_StateFlag.Patrol)
        {
            //检查下一个巡逻点：
            patrolIndex = (patrolIndex + 1) % patrolPoints.Count;

            //前往下一个巡逻点：
            ChangeDestination(patrolPoints[patrolIndex]);
        }

        //如果是追踪状态，那么不管继续寻路；
        //追踪状态的终止是由BoxCollider控制；
        // else
        // {
            
        // }
    }


//------------------------------------移动寻路相关----------------------------------


//----------------------------------属性暴露------------------------------------------
    public virtual Vector3 GetCurrentPosition()
    {
        return transform.position;
    }

    public virtual bool IsMoving()
    {
        return isMoving;
    }

    public virtual bool IsPaused()
    {
        return isPaused;
    }

    public enum E_StateFlag{
        Patrol = 0,
        Chase = 1,
    }
}



// 怪物状态接口
public interface IMonsterState
{
    void EnterState(MonsterBase monster);
    void UpdateState(MonsterBase monster);
    void ExitState(MonsterBase monster);
}

// 巡逻状态
public abstract class PatrolState : IMonsterState
{
    public virtual void EnterState(MonsterBase monster)
    {
        Debug.Log($"{monster.name} 进入巡逻状态");
        //前往第一个巡逻点位：
        monster.ChangeDestination(monster.patrolPoints[0]);
        //更新巡逻index：
        monster.patrolIndex = 0;
    }


    public virtual void UpdateState(MonsterBase monster)
    {
        // 子类实现具体的巡逻逻辑
        OnPatrolUpdate(monster);
    }

    public virtual void ExitState(MonsterBase monster)
    {
        Debug.Log($"{monster.name} 退出巡逻状态");
    }

    // 巡逻状态中的Update逻辑； 对于没有巡逻状态的怪物，无实现必要；
    //因为我们的寻路是通过协同程序实现的，所以不依赖Update；
    //此处Update作用就是
    //1.更新怪物的小人朝向；
    //2.检测玩家是否进入追踪范围；
    protected virtual void OnPatrolUpdate(MonsterBase monster) { }
}

// 追逐状态
public abstract class ChaseState : IMonsterState
{
    protected Transform playerTransform;
    
    // 寻路优化参数
    protected float lastPathUpdateTime = 0f;
    protected float pathUpdateInterval = 0.5f; // 每0.5秒更新一次路径
    protected float minDistanceToUpdate = 1f;  // 玩家移动超过1米才重新寻路
    protected Vector3 lastPlayerPosition;
    protected float chaseRange = 10f;          // 追逐范围
    
    public virtual void EnterState(MonsterBase monster)
    {
        Debug.Log($"{monster.name} 进入追逐状态");
 
        // 获取玩家Transform引用
        if(playerTransform == null)
        {
            EventHub.Instance.EventTrigger<UnityAction<Transform>>("ExposePlayerTransform", (_transform) =>{
                playerTransform = _transform;
                if(playerTransform != null)
                {
                    lastPlayerPosition = playerTransform.position;
                }
            });
        }

        monster.ChangeDestination(playerTransform.position);
        lastPlayerPosition = playerTransform.position;
        lastPathUpdateTime = Time.time;

        Debug.LogWarning($"Time recorded, {lastPathUpdateTime}");

    }

    public virtual void UpdateState(MonsterBase monster)
    {
        // 子类实现具体的追逐逻辑
        OnChaseUpdate(monster);
    }

    public virtual void ExitState(MonsterBase monster)
    {
        Debug.Log($"{monster.name} 退出追逐状态");
    }

    //追逐状态中的Update逻辑；
    //因为我们的寻路是通过协同程序实现的，所以不依赖Update；
    //此处Update最大的作用就是更新怪物的小人朝向；
    protected virtual void OnChaseUpdate(MonsterBase monster)
    {

        // 获取当前玩家位置
        Vector3 currentPlayerPos = playerTransform.position;
        Vector3 monsterPos = monster.GetCurrentPosition();
        
        // 计算距离
        float distanceToPlayer = Vector3.Distance(monsterPos, currentPlayerPos);
        
        // 检查是否在追逐范围内
        if(distanceToPlayer > chaseRange)
        {
            // 超出范围，回到巡逻状态
            monster.SwitchToPatrol();
            return;
        }
        
        // 检查是否需要更新路径
        bool shouldUpdatePath = monster.IsPaused() ? false : ShouldUpdatePath(currentPlayerPos, monster);

        Debug.Log($"Chasing! shouldUpdatePath is {shouldUpdatePath}");
        
        if(shouldUpdatePath)
        {
            // 更新路径
            monster.ChangeDestination(currentPlayerPos);
            lastPlayerPosition = currentPlayerPos;

            
            lastPathUpdateTime = Time.time;
            Debug.LogWarning($"Time recorded, {lastPathUpdateTime}");
        }
    }
    
    // 判断是否需要更新路径
    protected virtual bool ShouldUpdatePath(Vector3 currentPlayerPos, MonsterBase monster)
    {
        // 1. 检查时间间隔
        //如果距离上一次更新路径的时间间隔超出了pathUpdateInterval，则执行更新
        if(Time.time - lastPathUpdateTime > pathUpdateInterval)
        {
            return true;
        }
     
        
        // // 2. 检查玩家移动距离
        // //玩家移动超过1单位，才会更新
        // float playerMoveDistance = Vector3.Distance(currentPlayerPos, lastPlayerPosition);
        // if(playerMoveDistance < minDistanceToUpdate)
        // {
        //     return false;
        // }
        
        // // 3. 检查怪物是否正在移动
        // if(!monster.IsMoving())
        // {
        //     return true; // 怪物停止时立即更新路径
        // }

        return false;
    }
  
}


