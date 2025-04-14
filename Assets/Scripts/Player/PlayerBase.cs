using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBase : MonoBehaviour
{
    public bool isMoving = true;
    [Range(0, 5)]
    public float moveSpeedBase;
    public bool isDetectingEscape = true;

    protected virtual void Awake()
    {
        EventHub.Instance.AddEventListener<E_DetectInputType>("CloseSpecificDetectInput", CloseSpecificDetectInput);
        EventHub.Instance.AddEventListener<E_DetectInputType>("UnlockSpecificDetectInput", UnlockSpecificDetectInput);
        EventHub.Instance.AddEventListener<bool>("Freeze", Freeze);
        moveSpeedBase = 3;
        isDetectingEscape = true;
    }



    protected virtual void OnDestroy()
    {
        EventHub.Instance.RemoveEventListener<E_DetectInputType>("CloseSpecificDetectInput", CloseSpecificDetectInput);
        EventHub.Instance.RemoveEventListener<E_DetectInputType>("UnlockSpecificDetectInput", UnlockSpecificDetectInput);
        EventHub.Instance.RemoveEventListener<bool>("Freeze", Freeze);
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
                this.transform.Translate(0, Input.GetAxis("Vertical") * Time.deltaTime * moveSpeed, 0);
            }

            if(Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.A)) 
            {
                this.transform.Translate(Input.GetAxis("Horizontal") * Time.deltaTime * moveSpeed, 0, 0);
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



}

public enum E_DetectInputType
{
    Escape = 1,
    F = 2,
    Move = 3,

}
