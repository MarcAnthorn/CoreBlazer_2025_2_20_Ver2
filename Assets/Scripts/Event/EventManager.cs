#define DEBUGTEST

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using TMPro;
using UnityEditor.PackageManager;
using Unity.VisualScripting;
using static UnityEditor.Progress;
using System.Linq;
using static Event;
using System.Text;
using System.Diagnostics;
using Debug = UnityEngine.Debug;


public class EventManager : Singleton<EventManager>
{
    public Dictionary<int, float> weights;                          //表示事件对应的权重
    public int currentEventId = 0;
    public static int eventCount = 0;

    protected override void Awake()
    {
        base.Awake();   //单例初始化
        //LoadEvents();  //！！！！！测试用！！！！！ 加载一些关卡未开始时候的信息
        DebugTest1();
    }

    //暂定不需要
    //public void SelectOption(int optionIndex)           //1,2,3
    //{
    //    foreach (var option in optionEvents[currentEventId].options)
    //    {
    //        if (optionIndex == option.optionId)
    //        {
    //            option.result.TriggerBtlEvent();           //调用触发的(战斗)事件(事件的注册与删除逻辑由Marc来`完成)
    //        }
    //    }

    //    //if (allEvents[currentEventId].options.Contains(optionIndex))
    //    //{
    //    //    EventOption.EventResult result = currentEvent.results[currentEventId];
    //    //    //处理结果

    //    //    Debug.Log(result.outcome);                  //打印该事件的结果
    //    //    currentEventId = result.nextEventId;        //更新到下一个事件
    //    //}
    //}
    void SaveEvents()
    {
        // 保存事件数据到 CSV  
        string path = Path.Combine(Application.dataPath, "Resources/EventData/eventDatas.csv");
        // 如果文件不存在，写入标题行  
        if (!File.Exists(path))
        {
            File.WriteAllText(path, "id,name,date\n"); // 这里的列名应与 EventData 对象的字段相对应  
        }

        // 遍历 events 字典，构建 CSV 行  
        List<string> lines = new List<string>();
        foreach (var kvp in LoadManager.Instance.startEvents)
        {
            Event eventData = kvp.Value;
            string line = $"{eventData.eventId}, name, EvDescription";
            lines.Add(line);
        }

        // 将新的行添加到 CSV 文件  
        File.WriteAllLines(path, lines);
    }
    public void TriggerEvent(int eventId)                //当角色的OnTriggerEnter()方法发生时调用,获取该事件信息
    {
        currentEventId = eventId;

        //玩家触碰POI后，显示事件面板GameMainPanel:
        //面板生成之后会立刻获取当前的事件并且加载内容
        UIManager.Instance.ShowPanel<GameMainPanel>();
    }


    //用于向外部广播事件的方法，使外部获取当前的事件实例(Marc添加)
    public Event BroadcastEvent()
    {
        if (LoadManager.Instance.startEvents.ContainsKey(currentEventId))
        {
            return LoadManager.Instance.startEvents[currentEventId];
        }
        else if (LoadManager.Instance.optionEvents.ContainsKey(currentEventId))
        {
            return LoadManager.Instance.optionEvents[currentEventId];
        }

        Debug.LogError("当前尝试从EventManager中获取的事件不存在");
        return null;
    }

    public Event CreateStartEvent(int eventId)           //给外部调用，创建事件
    {
        foreach (var _event in LoadManager.Instance.startEvents)
        {
            if (_event.Value.eventId == eventId)
            {
                Event tempEvent = _event.Value;
                Event @event = new Event()
                {
                    libId = tempEvent.libId,
                    eventId = tempEvent.eventId,
                    eventType = tempEvent.eventType,
                    options = tempEvent.options,
                };

                return @event;
            }
        }

        Debug.LogError("无法创建一个本关卡内不存在的事件");
        return null;
    }
    public Event DeleteStartEvent(int eventId)           //给外部调用，删除事件
    {
        foreach (var _event in LoadManager.Instance.startEvents)
        {
            if (_event.Value.eventId == eventId)
            {
                Event tempEvent = _event.Value;
                LoadManager.Instance.startEvents[_event.Key] = null;       //GC系统自动清除new的对象
                LoadManager.Instance.startEvents.Remove(_event.Key);       //移除键值对

                return tempEvent;
            }
        }

        Debug.LogError("无法删除一个本关卡内不存在的事件");
        return null;
    }

    public void OnOptionSelected(int optionId)
    {
        // 执行事件  
        ExecuteEvent(Event.MyEventType.None);

        // 显示下一个对话  
        //ShowDialogue(nextId);
    }

    private void ExecuteEvent(Event.MyEventType eventType)
    {
        switch (eventType)
        {
            case Event.MyEventType.Action1:
                // 执行事件1  
                break;
            case Event.MyEventType.Action2:
                // 执行事件2  
                break;
            default:
                break;
        }
    }

    // [Conditional("DEBUGTEST")]          //便于调试
    public void DebugTest1()             //用于测试事件数据读取
    {
        // foreach (var _event in startEvents)
        // {
        //     Event @event = _event.Value;
        //     Debug.Log($"事件库id：{@event.libId}, 事件id：{@event.eventId}, 事件类别：{@event.eventType}");
        //     Debug.Log($"事件选项数量：{@event.options.Count}");
        //     for (int i = 0; i < 3; i++)
        //     {
        //         Debug.Log($"选项{@event.options[i].optionId}信息==>条件属性id：{@event.options[i].conditionId}, 属性min：{@event.options[i].minCondition}, 属性max：{@event.options[i].maxCondition}, 道具id：{@event.options[i].itemId}");
        //     }
        //     @event.ReadKaidanTextFrom(@event.textLib[@event.firstTextId]);
        //     Debug.Log("==========================================");

        // }
    }

}
