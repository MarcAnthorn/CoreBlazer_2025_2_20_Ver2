using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstFloorDiffer : MonoBehaviour
{
    
    private bool isInAera;
    private bool isTriggered = false;
    void Awake()
    {
        EventHub.Instance.AddEventListener("ResetFloorDiffer", ResetFloorDiffer);
    }

    void OnDestroy()
    {
        EventHub.Instance.RemoveEventListener("ResetFloorDiffer", ResetFloorDiffer);
    }

    void Update()
    {
        if(isInAera && !isTriggered && ItemManager.Instance.CheckIfItemExist(514))
        {
            isTriggered = true;
            EventHub.Instance.EventTrigger("Item514OnEffect");
        }
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(!isInAera && collision.CompareTag("Player"))
        {
            isInAera = true;
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            isInAera = true;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if(isInAera)
        {
            isInAera = false;
            EventHub.Instance.EventTrigger("Item514OnEffectOff");
            isTriggered = false;
        }
    }

    //供死亡 / 返回安全屋调用的方法：
    private void ResetFloorDiffer()
    {
        isTriggered = false;
        isInAera = false;
        EventHub.Instance.EventTrigger("Item514OnEffectOff");
    }
}
