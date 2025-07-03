using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class DoorLogicBase : MonoBehaviour
{

    public UnityAction actionDoorTrigger;

    //当前对应的门是否可以交互：
    public bool isDoorInteractable = false;
    

    protected virtual void Awake()
    {
        actionDoorTrigger += DoorTrigger;
    }

    protected abstract void DoorTrigger();
}
