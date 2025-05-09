#define DEBUGTEST

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using TMPro;
using Unity.VisualScripting;
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
    }

   
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
        Debug.LogWarning($"Event triggered! id is{eventId}");
        currentEventId = eventId;

        //玩家触碰POI后，显示事件面板GameMainPanel:
        //面板生成之后会立刻获取当前的事件并且加载内容
        UIManager.Instance.ShowPanel<GameMainPanel>();
    }


    //用于向外部广播事件的方法，使外部获取当前的事件实例(Marc添加)
    public Event BroadcastEvent()
    {
        Debug.LogWarning($"Try to broadcast event, id is{currentEventId}");

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

    // 按 grade 分组的可用 libId 列表
    private static Dictionary<int, List<int>> availableLibIds = new Dictionary<int, List<int>>()
    {
        { 1, new List<int> { 2001, 2002, 2003, 2008, 2012, 2014, 2016, 2019, 2024, 2025, 2026, 2029, 2032, 2033, 2036 } },
        { 2, new List<int> { 2005, 2006, 2007, 2010, 2015, 2017, 2023, 2031, 2037, 2038, 2039, 2041, 2043, 2044 } },
        { 3, new List<int> { 2004, 2009, 2011, 2013, 2018, 2020, 2021, 2022, 2027, 2028, 2030, 2034, 2035, 2040, 2042 } }
    };

    // 随机获取一个 libId，并从列表中移除（防止重复分配）
    public int GetRandomLibId(int grade)
    {
        if (!availableLibIds.ContainsKey(grade) || availableLibIds[grade].Count == 0)
        {
            Debug.LogError($"No available libId for grade {grade}!");
            return -1; // 返回 -1 表示失败
        }

        var list = availableLibIds[grade];
        int randomIndex = UnityEngine.Random.Range(0, list.Count);
        int selectedLibId = list[randomIndex];
        list.RemoveAt(randomIndex); // 移除已使用的 libId

        return selectedLibId;
    }

    // 重置所有 libId（如果需要重新分配）
    public void ResetAllLibIds()
    {
        availableLibIds[1] = new List<int> { 2001, 2002, 2003, 2008, 2012, 2014, 2016, 2019, 2024, 2025, 2026, 2029, 2032, 2033, 2036 };
        availableLibIds[2] = new List<int> { 2005, 2006, 2007, 2010, 2015, 2017, 2023, 2031, 2037, 2038, 2039, 2041, 2043, 2044 };
        availableLibIds[3] = new List<int> { 2004, 2009, 2011, 2013, 2018, 2020, 2021, 2022, 2027, 2028, 2030, 2034, 2035, 2040, 2042 };
    }

}
