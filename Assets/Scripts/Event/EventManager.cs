﻿#define DEBUGTEST

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
    //存储某一关卡事件库里的所有可能出现的事件，用来为本关卡提供可调用的Event数据(用eventId来查找)
    public Dictionary<int, Event> startEvents;
    public Dictionary<int, Event> optionEvents;
    public Dictionary<int, float> weights;                          //表示事件对应的权重
    public Dictionary<int, Event.EventResult> eventResults;         //表示所有事件的所有结果(性能优化)
    public int currentEventId = 0;
    public static int eventCount = 0;

    protected override void Awake()
    {
        base.Awake();   //单例初始化
        LoadEvents();  //！！！！！测试用！！！！！ 加载一些关卡未开始时候的信息
        DebugTest1();
    }

    private void Start()
    {

    }

    private void Update()
    {

    }

    public void LoadEvents()           //在关卡初始化时调用(根据传入的库Id来加载对应库中的文本)
    {
        startEvents = new Dictionary<int, Event>();
        optionEvents = new Dictionary<int, Event>();
        weights = new Dictionary<int, float>();
        //加载已有事件数据(CSV格式)到events字典中，使用Assets(Application.dataPath)下的相对路径
        string path = Path.Combine(Application.dataPath, "Resources/EventData/EventCSV/Test1_CSV.csv");
        int libIndex = 2001;

        if (File.Exists(path))
        {
            LoadEventResult();                                              //加载所有事件的所有结果
            string[] lines = File.ReadAllLines(path, Encoding.UTF8);        //分割每一行存入lines

            for (int i = 5; i < lines.Length; i++)          //从第四行开始遍历每一行，获得各列的信息
            {
                string line = lines[i];
                string[] values = line.Split(',');          //将每一列按照逗号分割

                if (int.Parse(values[0]) == libIndex && values.Length >= 5)
                {
                    Event eventData = new Event()
                    {
                        libId = int.Parse(values[0]),                                       //A列
                        eventId = int.Parse(values[1]),                                     //B列
                        eventType = (Event.MyEventType)int.Parse(values[2]),                //C列
                        grade = int.Parse(values[4])                                        //E列
                    };
                    int result_id = int.Parse(values[3]);                       //D列
                    if(result_id != 0)
                    {
                        eventData.hasResult = true;                         //表示有结果可以访问
                        eventData.result = GetEventResult(result_id);       //读入事件的对应结果
                    }

                    for (int j = 0; j < 3; j++)     //var option in eventData.options
                    {
                        EventOption option = new EventOption();
                        option.optionId = j;
                        option.conditionId = int.Parse(values[5 + j * 6]);                  //F列
                        option.minCondition = int.Parse(values[6 + j * 6]);                 //G列
                        option.maxCondition = int.Parse(values[7 + j * 6]);                 //H列
                        option.itemId = int.Parse(values[8 + j * 6]);                       //I列
                        option.OpDescription = values[9 + j * 6];                           //J列
                        option.NextId = int.Parse(values[10 + j * 6]);                      //K列
                        eventData.options.Add(option);
                    }

                    LoadKaidanTexts(eventData.eventId, eventData);        //加载事件对应的怪诞文本
                    if (int.Parse(values[1]) / 10 == libIndex && int.Parse(values[1]) % 10 == 1)
                    {
                        startEvents.Add(eventData.eventId, eventData);      //起始事件
                        weights.Add(eventData.eventId, 1.0f);               //加入起始事件的权重（等权重）
                    }
                    else
                    {
                        optionEvents.Add(eventData.eventId, eventData);     //选项(后续)事件
                    }

                }

                if (i + 1 <= 87)
                {
                    line = lines[i + 1];
                    int charToFind = line.IndexOf(",");
                    string firstValue = line.Substring(0, charToFind);
                    if (int.Parse(firstValue) != libIndex)          //如果下一行的事件库Id变化，则++
                        libIndex++;
                }

            }

        }
        else
        {
            Debug.LogWarning("事件数据文件不存在！");
        }
    }
    public Event.EventResult GetEventResult(int resultId)
    {
        foreach(KeyValuePair<int, Event.EventResult> pair in eventResults)
        {
            if(pair.Key == resultId)
            {
                return pair.Value;
            }
        }

        return null;
    }
    public void LoadEventResult()        //一般来说不用外部调用
    {
        eventResults = new Dictionary<int, EventResult>();

        string path = Path.Combine(Application.dataPath, "Resources/EventData/EventResult/EventResults1.csv");
        if (File.Exists(path))
        {
            string[] lines = File.ReadAllLines(path, Encoding.UTF8);       //分割每一行存入lines

            for (int i = 4; i < lines.Length; i++)
            {
                string line = lines[i];
                string[] values = line.Split(",");

                if (values.Length > 4 /*&& int.Parse(values[0]) == resultId*/)
                {
                    Event.EventResult @eventResult = new Event.EventResult();
                    @eventResult.resultId = int.Parse(values[0]);      //涉及到覆盖，所以要重新再设置一次
                    @eventResult.spId = int.Parse(values[1]);

                    @eventResult.change_HP = float.Parse(values[2]);
                    @eventResult.change_HP_rate = float.Parse(values[3]);

                    @eventResult.change_STR = float.Parse(values[4]);
                    @eventResult.change_STR_rate = float.Parse(values[5]);

                    @eventResult.change_DEF = float.Parse(values[6]);
                    @eventResult.change_DEF_rate = float.Parse(values[7]);

                    @eventResult.change_LVL = float.Parse(values[8]);
                    @eventResult.change_LVL_rate = float.Parse(values[9]);

                    @eventResult.change_SAN = float.Parse(values[10]);
                    @eventResult.change_SAN_rate = float.Parse(values[11]);

                    @eventResult.change_SPD = float.Parse(values[12]);
                    @eventResult.change_SPD_rate = float.Parse(values[13]);

                    @eventResult.change_CRIT_Rate = float.Parse(values[14]);
                    @eventResult.change_CRIT_Rate_rate = float.Parse(values[15]);

                    @eventResult.change_CRIT_DMG = float.Parse(values[16]);
                    @eventResult.change_CRIT_DMG_rate = float.Parse(values[17]);

                    @eventResult.change_HIT = float.Parse(values[18]);
                    @eventResult.change_HIT_rate = float.Parse(values[19]);

                    @eventResult.change_AVO = float.Parse(values[20]);
                    @eventResult.change_AVO_rate = float.Parse(values[21]);

                    eventResults.Add(@eventResult.resultId, @eventResult);
                }

            }

        }

    }
    public void LoadKaidanTexts(int eventIndex, Event @event)     //用在Event加载的时候
    {
        @event.textLib = new Dictionary<int, KaidanText>();
        string path = Path.Combine(Application.dataPath, "Resources/DialogueData/KaidanTest.csv");

        if (File.Exists(path))
        {
            string[] lines = File.ReadAllLines(path, Encoding.UTF8);       //分割每一行存入lines

            for (int i = 1; i < lines.Length; i++)          //从第四行开始遍历每一行，获得各列的信息
            {
                string line = lines[i];
                string[] values = line.Split(',');          //将每一列按照逗号分割
                if (i == 1)
                {
                    @event.firstTextId = int.Parse(values[1]);
                }

                if (int.Parse(values[0]) == eventIndex && values.Length >= 4/* && int.Parse(values[2]) == 0*/)
                {
                    KaidanText kaidanText = new KaidanText();
                    kaidanText.eventId = int.Parse(values[0]);
                    kaidanText.textId = int.Parse(values[1]);
                    if (int.Parse(values[2]) == 1)
                    {
                        kaidanText.isKwaidan = false;
                    }
                    else if (int.Parse(values[2]) == 0)
                    {
                        kaidanText.isKwaidan = true;
                    }
                    kaidanText.text = values[3];
                    kaidanText.nextId = int.Parse(values[4]);
                    kaidanText.illustrationId = int.Parse(values[5]);
                    kaidanText.bgId = int.Parse(values[6]);

                    @event.evDescription += values[3];
                    @event.textLib.Add(kaidanText.textId, kaidanText);
                }

            }
        }
        else
        {
            Debug.LogError("怪诞文本文件未找到。事件Id：" + @event.eventId);
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
        foreach (var kvp in startEvents)
        {
            Event eventData = kvp.Value;
            string line = $"{eventData.eventId}, name, EvDescription";
            lines.Add(line);
        }

        // 将新的行添加到 CSV 文件  
        File.WriteAllLines(path, lines);
    }


    //暂定不需要
    public void SelectOption(int optionIndex)           //1,2,3
    {
        foreach (var option in optionEvents[currentEventId].options)
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
        if (startEvents.ContainsKey(currentEventId))
        {
            return startEvents[currentEventId];
        }
        else if (optionEvents.ContainsKey(currentEventId))
        {
            return optionEvents[currentEventId];
        }

        Debug.LogError("当前尝试从EventManager中获取的事件不存在");
        return null;
    }

    public Event CreateStartEvent(int eventId)           //给外部调用，创建事件
    {
        foreach (var _event in startEvents)
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
        foreach (var _event in startEvents)
        {
            if (_event.Value.eventId == eventId)
            {
                Event tempEvent = _event.Value;
                startEvents[_event.Key] = null;       //GC系统自动清除new的对象
                startEvents.Remove(_event.Key);       //移除键值对

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
