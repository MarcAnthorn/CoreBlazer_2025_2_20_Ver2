using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventGO : MonoBehaviour            //挂载在游戏中表示事件的物体上
{
    public Event @event;
    public int eventId;         //表示事件的固定Id(在Unity内进行修改)

    void Awake()
    {
        @event = EventManager.Instance.CreateEvent(eventId);        //随游戏对象的初始化而创建并绑定
        GameLevelManager.Instance.events.Add(++GameLevelManager.Instance.eventNum, @event);
    }

    void Update()
    {
        
    }
}
