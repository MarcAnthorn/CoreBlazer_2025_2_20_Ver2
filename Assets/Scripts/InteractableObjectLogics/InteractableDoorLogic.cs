using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractableDoorLogic : MonoBehaviour
{
    public UnityAction actionDoorTrigger;
    public GameObject monster1001;

    void Awake()
    {
        actionDoorTrigger += DoorTrigger;
    }

    private void DoorTrigger()
    {
        //触发怪物追逐：
        monster1001.gameObject.SetActive(true);
        Destroy(this.gameObject);   //暂时使用销毁；
    }
}
