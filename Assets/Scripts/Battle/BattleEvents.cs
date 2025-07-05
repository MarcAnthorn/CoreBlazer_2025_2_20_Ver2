using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// 战斗事件类：
// 1.为触发的具体 UnityAction 提供注册和注销的方法
// 2.为战斗内循环提供方便调用的方法接口 EventTrigger()
public class BattleEvents : Singleton<BattleEvents>
{
    private Dictionary<string, UnityAction> events;

    protected override void Awake()
    {
        base.Awake();
        events = new Dictionary<string, UnityAction>();
    }

    public void Subscribe(string eventName, UnityAction action)
    {
        if (!events.ContainsKey(eventName))
        {
            events.Add(eventName, action);
        }
    }

    public void Unsubscribe(string eventName)
    {
        if (!events.ContainsKey(eventName))
        {
            events.Remove(eventName);
        }
    }

    public void EventTrigger(string eventName)
    {
        if (events.ContainsKey(eventName))
        {
            events[eventName].Invoke();
        }
    }
}
