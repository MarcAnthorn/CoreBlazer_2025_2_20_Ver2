using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event20030 :MonoBehaviour
{
    public Event myEvent;
    public int eventId;         //表示事件的固定Id(在Unity内进行修改)(如果是随机生成事件的POI则不需要这个字段)

    void Awake()        //开局就定下了本关卡内的所有POI对应事件
    {
    }

    void Start()
    {
        ExtractEvent();
        Debug.Log($"Event is distributed, id is:{eventId}");
        GameLevelManager.Instance.events.Add(++GameLevelManager.Instance.eventNum, myEvent);



    }
    void Update()
    {
        
    }

    private void ExtractEvent()        //按照权重抽取事件
    {
        //事件库id*10 + 1就是事件id；如：2001 * 10 + 1 == 20011;就是起始事件id；
        eventId = EventManager.Instance.GetRandomLibId(3) * 10 + 1;

        //将起始事件对象抽出来；
        myEvent = EventManager.Instance.CreateStartEvent(eventId);

        // float totalWeight = 0f;
        // float randomValue = 0f;
        // foreach (float weight in EventManager.Instance.weights.Values)
        // {
        //     totalWeight += weight;
        // }
        // randomValue = Random.Range(0f, totalWeight);      //随机生成
        // foreach(KeyValuePair<int, float> pair in EventManager.Instance.weights)
        // {
        //     if (GameLevelManager.Instance.events.ContainsKey(pair.Key))
        //     {
        //         continue;
        //     }
        //     randomValue -= pair.Value;
        //     if (randomValue <= 0)
        //     {
        //         // Debug.Log($"尝试创建事件（EventGO）, 此时randowValue == {randomValue}");
        //         return EventManager.Instance.CreateStartEvent(pair.Key);

        //     }
        // }
        // return null;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            EventManager.Instance.TriggerEvent(eventId);

            myEvent.isTrigger = true;

            SoundEffectManager.Instance.PlaySoundEffect("与地图POI交互");
            GameLevelManager.Instance.eventList.Add(new ResultEvent("事件" + myEvent.eventId, 3, 1));

            Destroy(this.gameObject);
        }


    }
}