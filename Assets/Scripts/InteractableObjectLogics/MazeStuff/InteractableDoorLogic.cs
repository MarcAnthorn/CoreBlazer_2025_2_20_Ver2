using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractableDoorLogic : DoorLogicBase
{
    protected override void DoorTrigger()
    {
        Destroy(this.gameObject);   //暂时使用销毁；

        ItemManager.Instance.RemoveItem(509);   //移除钥匙；
        
        //手动确保不可交互：
        isDoorInteractable = false;
    }

}
