using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBase : MonoBehaviour
{
    public bool isMoving = true;
    [Range(0, 5)]
    public float moveSpeed;

    protected virtual void Awake()
    {
        EventHub.Instance.AddEventListener<bool>("Freeze", Freeze);
    }

    protected virtual void OnDestroy()
    {
        EventHub.Instance.RemoveEventListener<bool>("Freeze", Freeze);
    }

    protected virtual void FixedUpdate() 
    {
        ControlPlayerMove();
    }

    protected virtual void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            UIManager.Instance.ShowPanel<InventoryPanel>();
        }
    }


    protected void ControlPlayerMove()
    {
        if(isMoving)
        {
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

}
