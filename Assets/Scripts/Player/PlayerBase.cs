using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBase : MonoBehaviour
{
    public bool isMoving = true;
    [Range(0, 5)]
    public float moveSpeedBase;
    public bool isDetectingEscape = true;
    private Animator animator;
    private SpriteRenderer sr;

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
    }



    protected virtual void OnDestroy()
    {
        EventHub.Instance.RemoveEventListener<E_DetectInputType>("CloseSpecificDetectInput", CloseSpecificDetectInput);
        EventHub.Instance.RemoveEventListener<E_DetectInputType>("UnlockSpecificDetectInput", UnlockSpecificDetectInput);
        EventHub.Instance.RemoveEventListener<bool>("Freeze", Freeze);
        EventHub.Instance.RemoveEventListener<int>("AdjustLayer", AdjustLayer);
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

      


    protected void ControlPlayerMove()
    {
        if(isMoving)
        {
            float moveSpeed = moveSpeedBase * (1 + (5 * PlayerManager.Instance.player.SPD.value - 10) / 100);
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
        }
    }

    protected virtual void Freeze(bool _isFrozen)
    {
        if(this == null)
        {
            return;
        }
        isMoving = !_isFrozen;

        if(_isFrozen)
        {
            TimeManager.Instance.SuspendAllTimers();
        }
        else
        {
            TimeManager.Instance.StartAllTimers();
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
    private void AdjustLayer(int delta)
    {
        sr.sortingOrder += delta;
    }



}

public enum E_DetectInputType
{
    Escape = 1,
    F = 2,
    Move = 3,

}
