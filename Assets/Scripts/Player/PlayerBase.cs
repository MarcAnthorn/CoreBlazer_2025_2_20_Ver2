using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class PlayerBase : MonoBehaviour
{
    public bool isMoving = true;
    [Range(0, 5)]
    public float moveSpeedBase;
    public float moveSpeed;
    public bool isDetectingEscape = true;
    private Animator animator;
    private SpriteRenderer sr;

    public CinemachineVirtualCamera cam;
    
    // 异步同步相关
    private Coroutine positionSyncCoroutine;
    private Vector3 lastSyncedPosition;
    private float syncInterval = 0.1f; // 每0.1秒同步一次

    protected virtual void Awake()
    {
        EventHub.Instance.AddEventListener<E_DetectInputType>("CloseSpecificDetectInput", CloseSpecificDetectInput);
        EventHub.Instance.AddEventListener<E_DetectInputType>("UnlockSpecificDetectInput", UnlockSpecificDetectInput);
        EventHub.Instance.AddEventListener<bool>("Freeze", Freeze);
        EventHub.Instance.AddEventListener<int>("AdjustLayer", AdjustLayer);

        moveSpeedBase = 3;
        isDetectingEscape = true;

        animator = this.GetComponent<Animator>();
        sr = this.GetComponent<SpriteRenderer>();


        cam = GameObject.FindGameObjectWithTag("VirtualCamera").GetComponent<CinemachineVirtualCamera>();
        
        // 启动异步位置同步
        StartPositionSync();
    }
    
    // 启动异步位置同步协程
    private void StartPositionSync()
    {
        if (positionSyncCoroutine != null)
        {
            StopCoroutine(positionSyncCoroutine);
        }
        positionSyncCoroutine = StartCoroutine(AsyncPositionSync());
    }
    
    // 异步位置同步协程
    private IEnumerator AsyncPositionSync()
    {
        while (true)
        {
            yield return new WaitForSeconds(syncInterval);
            PlayerManager.Instance.player.OverFlowDector(AttributeType.LVL);

            // 只有当位置发生变化时才同步
            if (Vector3.Distance(transform.position, lastSyncedPosition) > 0.01f)
            {
                PlayerManager.Instance.playerPosition = transform.position;
                lastSyncedPosition = transform.position;
            }
        }
    }



    protected virtual void OnDestroy()
    {
        EventHub.Instance.RemoveEventListener<E_DetectInputType>("CloseSpecificDetectInput", CloseSpecificDetectInput);
        EventHub.Instance.RemoveEventListener<E_DetectInputType>("UnlockSpecificDetectInput", UnlockSpecificDetectInput);
        EventHub.Instance.RemoveEventListener<bool>("Freeze", Freeze);
        EventHub.Instance.RemoveEventListener<int>("AdjustLayer", AdjustLayer);
        
        // 停止异步位置同步协程
        if (positionSyncCoroutine != null)
        {
            StopCoroutine(positionSyncCoroutine);
        }
    }

    protected virtual void FixedUpdate() 
    {
        ControlPlayerMove();
    }

    protected virtual void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape) && isDetectingEscape)
        {
            Debug.Log("Inventory Panel is Revealed");
            UIManager.Instance.ShowPanel<InventoryPanel>();
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            TextDisplayManager.Instance.DisplayTextImmediately();
        }
    }

      
public static void SetPlayerPosition(Vector3 pos)
{
    if (PlayerManager.Instance.PlayerTransform != null)
    {
        PlayerManager.Instance.PlayerTransform.position = pos;
        PlayerManager.Instance.playerPosition = pos;
        Debug.Log($"玩家位置已设置为: {pos}");
    }
    else
    {
        Debug.LogWarning("PlayerTransform为空，无法设置玩家位置！");
    }
}

    // 立即同步当前位置（用于重要操作）
    public void ForceSyncPosition()
    {
        PlayerManager.Instance.playerPosition = transform.position;
        lastSyncedPosition = transform.position;
    }

    protected void ControlPlayerMove()
    {
        if(isMoving)
        {
            moveSpeed = moveSpeedBase * (1 + (5 * PlayerManager.Instance.player.SPD.value - 10) / 100);
            if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
            {
                animator.SetBool("IsIdle", false);

                this.transform.Translate(0, Input.GetAxis("Vertical") * Time.deltaTime * moveSpeed, 0);

                if(Input.GetKey(KeyCode.W))
                {
                    animator.SetTrigger("WalkBackTrigger");
                }

                else if(Input.GetKey(KeyCode.S))
                {
                    animator.SetTrigger("WalkFrontTrigger");
                }
            }

            else if(Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.A)) 
            {
                animator.SetBool("IsIdle", false);

                this.transform.Translate(Input.GetAxis("Horizontal") * Time.deltaTime * moveSpeed, 0, 0);

                if(Input.GetKey(KeyCode.D))
                {
                    animator.SetTrigger("WalkRightTrigger");
                }

                else if(Input.GetKey(KeyCode.A))
                {
                    animator.SetTrigger("WalkLeftTrigger");
                }
            }

            else if(!Input.anyKeyDown)
            {
                animator.SetBool("IsIdle", true);
            }
            
            // 位置同步已改为异步协程处理
        }
    }

    protected virtual void Freeze(bool _isFrozen)
    {
        if(this == null)
        {
            return;
        }

        Debug.Log($"now state:{_isFrozen}");
        
        isMoving = !_isFrozen;

        if(_isFrozen)
        {
            TimeManager.Instance.SuspendAllTimers();
            // 冻结时停止位置同步
            if (positionSyncCoroutine != null)
            {
                StopCoroutine(positionSyncCoroutine);
                positionSyncCoroutine = null;
            }
        }
        else
        {
            TimeManager.Instance.StartAllTimers();
            // 解冻时重新启动位置同步
            StartPositionSync();
        }

        animator.SetBool("IsIdle", true);
    }

    private void CloseSpecificDetectInput(E_DetectInputType type)
    {
        switch(type)
        {
            case E_DetectInputType.Escape:
                isDetectingEscape = false;
            break;
        }
    }

    private void UnlockSpecificDetectInput(E_DetectInputType type)
    {
        switch(type)
        {
            case E_DetectInputType.Escape:
                isDetectingEscape = true;
            break;
        }
    }

    //响应layer变化的事件：
    private void AdjustLayer(int layer)
    {
        sr.sortingOrder = layer;
    }



}

public enum E_DetectInputType
{
    Escape = 1,
    F = 2,
    Move = 3,

}
