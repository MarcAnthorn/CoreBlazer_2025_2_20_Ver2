using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventGO : MonoBehaviour            //挂载在游戏中表示事件的物体上
{
    public Event @event;
    public int eventId;         //表示事件的固定Id(在Unity内进行修改)(如果是随机生成事件的POI则不需要这个字段)

    void Awake()        //开局就定下了本关卡内的所有POI对应事件
    {
        @event = ExtractEvent();
        if(@event == null)
        {
            Debug.LogError("当前无事件抽取");
        }
        GameLevelManager.Instance.events.Add(++GameLevelManager.Instance.eventNum, @event);
        Debug.Log(@event.eventId);


    }

    void Update()
    {
        
    }

    private Event ExtractEvent()        //按照权重抽取事件
    {
        float totalWeight = 0f;
        float randomValue = 0f;
        foreach (float weight in EventManager.Instance.weights.Values)
        {
            totalWeight += weight;
        }
        randomValue = Random.Range(0f, totalWeight);      //随机生成
        foreach(KeyValuePair<int, float> pair in EventManager.Instance.weights)
        {
            if (GameLevelManager.Instance.events.ContainsKey(pair.Key))
            {
                continue;
            }
            randomValue -= pair.Value;
            if (randomValue <= 0)
            {
                return EventManager.Instance.CreateStartEvent(pair.Key);
            }
        }

        return null;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            EventManager.Instance.TriggerEvent(eventId);
            Destroy(this.gameObject);
        }
    }


}
