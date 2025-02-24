using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventGO : MonoBehaviour            //挂载在游戏中表示事件的物体上
{
    public Event @event;
    public int eventId;         //表示事件的固定Id

    void Awake()
    {
        @event = EventManager.Instance.CreateEvent(eventId);
    }

    void Update()
    {
        
    }
}
