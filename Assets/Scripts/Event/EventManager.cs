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

public class EventManager : Singleton<EventManager>
{
    //存储某一关卡事件库里的所有事件，用来为本关卡提供可调用的Event数据(用eventId来查找)
    public Dictionary<int, Event> allEvents;
    public int currentEventId = 0;
    public static int eventCount = 0;

    protected override void Awake()
    {
        base.Awake();   //单例初始化
        LoadEvents(0);  //！！！！！测试用！！！！！ 加载一些关卡未开始时候的信息
    }

    private void Update()
    {

    }

    void LoadEvents(int libIndex)           //在关卡初始化时调用(根据传入的库Id来加载对应库中的文本)
    {
        allEvents = new Dictionary<int, Event>();
        //加载已有事件数据(CSV格式)到events字典中，使用Assets(Application.dataPath)下的相对路径
        string path = Path.Combine(Application.dataPath, "Resources/EventData/eventDatas.csv");

        if (File.Exists(path))
        {
            string[] lines = File.ReadAllLines(path);       //分割每一行存入lines

            for (int i = 3; i < lines.Length; i++)          //从第四行开始遍历每一行，获得各列的信息
            {
                string line = lines[i];
                string[] values = line.Split(',');          //将每一列按照逗号分割

                if (int.Parse(values[0]) == libIndex && values.Length >= 5)
                {
                    Event eventData = new Event()
                    {
                        libId = int.Parse(values[0]),                                       //A列
                        eventId = int.Parse(values[1]),                                     //B列
                        type = (Event.MyEventType)int.Parse(values[2]),                     //C列
                    };
                    for (int j = 0; j < 3; j++)     //var option in eventData.options
                    {
                        eventData.options[j].conditionId = int.Parse(values[3 + j * 5]);    //D列
                        eventData.options[j].minCondition = int.Parse(values[4 + j * 5]);   //E列 左
                        eventData.options[j].maxCondition = int.Parse(values[5 + j * 5]);   //E列 右
                        eventData.options[j].OpDescription = values[6 + j * 5];             //F列
                        eventData.options[j].NextId = int.Parse(values[7 + j * 5]);         //G列
                    }

                    LoadKaidanTexts(eventData.libId, eventData);        //加载事件对应的怪诞文本
                    allEvents.Add(eventData.eventId, eventData);
                }

            }

        }
        else
        {
            Debug.LogWarning("事件数据文件不存在！");
        }
    }
    public void LoadKaidanTexts(int libIndex, Event @event)     //用在Event加载的时候
    {
        @event.textLib = new Dictionary<int, KaidanText>();
        string path = Path.Combine(Application.dataPath, "Resources/DialogueData/DialogueDatas.csv");

        if (File.Exists(path))
        {
            string[] lines = File.ReadAllLines(path);       //分割每一行存入lines

            for (int i = 3; i < lines.Length; i++)          //从第四行开始遍历每一行，获得各列的信息
            {
                string line = lines[i];
                string[] values = line.Split(',');          //将每一列按照逗号分割

                if (int.Parse(values[0]) == libIndex && values.Length >= 4 && int.Parse(values[2]) == 0)
                {
                    KaidanText kaidanText = new KaidanText();
                    kaidanText.eventId = int.Parse(values[0]);
                    kaidanText.textId = int.Parse(values[1]);
                    kaidanText.text = values[3];
                    kaidanText.nextId = int.Parse(values[4]);
                    kaidanText.illustrationId = int.Parse(values[5]);
                    kaidanText.bgId = int.Parse(values[6]);

                    @event.textLib.Add(kaidanText.textId, kaidanText);
                }

            }
        }
        else
        {
            Debug.LogError("怪诞文本文件未找到。");
        }
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
        foreach (var kvp in allEvents)
        {
            Event eventData = kvp.Value;
            string line = $"{eventData.eventId}, name, EvDescription";
            lines.Add(line);
        }

        // 将新的行添加到 CSV 文件  
        File.WriteAllLines(path, lines);
    }

    public void SelectOption(int optionIndex)           //1,2,3
    {
        foreach (var option in allEvents[currentEventId].options)
        {
            if (optionIndex == option.optionId)
            {
                option.result.TriggerBtlEvent();           //调用触发的(战斗)事件(事件的注册与删除逻辑由Marc来`完成)
            }
        }

        //if (allEvents[currentEventId].options.Contains(optionIndex))
        //{
        //    EventOption.EventResult result = currentEvent.results[currentEventId];
        //    //处理结果

        //    Debug.Log(result.outcome);                  //打印该事件的结果
        //    currentEventId = result.nextEventId;        //更新到下一个事件
        //}
    }

    public void TriggerEvent(GameObject go)                //当角色的OnTriggerEnter()方法发生时调用,获取该事件信息
    {
        EventGO EvGO = go.GetComponent<EventGO>();
        currentEventId = EvGO.eventId;
        EventUI eventUI = new EventUI();
        //可能会设置eventUI相关的数据(比如位置，大小等)
        Instantiate(eventUI);
    }


    //用于向外部广播事件的方法，使外部获取当前的事件实例(Marc添加)
    public Event BroadcastEvent()
    {
        if (allEvents.ContainsKey(currentEventId))
        {
            return allEvents[currentEventId];
        }

        Debug.LogError("当前尝试从EventManager中获取的事件不存在");
        return null;
    }

    public Event CreateEvent(int eventId)           //创建事件
    {
        foreach (var _event in allEvents)
        {
            if (_event.Value.eventId == eventId)
            {
                Event tempEvent = _event.Value;
                Event @event = new Event()
                {
                    libId = tempEvent.libId,
                    eventId = tempEvent.eventId,
                    type = tempEvent.type,
                    options = tempEvent.options,
                };

                return @event;
            }
        }

        Debug.LogError("无法创建一个本关卡内不存在的事件");
        return null;
    }

    private void OnOptionSelected(int optionId)
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

}
