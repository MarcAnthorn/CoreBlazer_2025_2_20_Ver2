using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using TMPro;

public class EventManager : Singleton<EventManager>
{
    public Dictionary<int, Event> events = new Dictionary<int, Event>();
    public int currentEventId = 0;

    protected override void Awake()
    {
        base.Awake();   //单例初始化
        LoadEvents();
    }

    private void Update()
    {
        
    }

    void LoadEvents()
    {
        //加载已有事件数据(CSV格式)到events字典中，使用Assets下的相对路径
        string path = Path.Combine(Application.dataPath, "Resources/EventData/eventDatas.json");
        if (File.Exists(path))
        {
            string[] lines = File.ReadAllLines(path);

            for(int i = 1; i < lines.Length; i++)       //遍历每一行，获得各列的信息
            {
                string line = lines[i];
                string[] values = line.Split(',');

                // 确保有足够的列  
                if (values.Length >= 3)
                {
                    Event eventData = new Event()
                    {
                        id = int.Parse(values[0]), // 假设id是字符串类型  
                        name = values[1],
                        EvDescription = values[2]
                    };
                    events.Add(eventData.id, eventData);
                }
            }
        }
        else
        {
            Debug.LogWarning("事件数据文件不存在！");
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
        foreach (var kvp in events)
        {
            Event eventData = kvp.Value;
            string line = $"{eventData.id},{eventData.name},{eventData.EvDescription}";  
            lines.Add(line);
        }

        // 将新的行添加到 CSV 文件  
        File.WriteAllLines(path, lines);
    }

    public void SelectOption(int optionIndex)
    {
        if (events.TryGetValue(currentEventId, out Event currentEvent))
        {
            EventOption.EventResult result = currentEvent.results[currentEventId];
            //处理结果

            Debug.Log(result.outcome);              //打印该事件的结果
            currentEventId = result.nextEventId;    //更新到下一个事件
        }
    }

    //可能需要实现一个向外部广播当前事件对象的方法：
    //可以需要将EventUI脚本中的ShowEvent迁移过来
    public Event BroadcastEvent()
    {     
        if(events.ContainsKey(currentEventId))
        {
            return events[currentEventId];
        }
        
        Debug.LogError("当前尝试获取的事件id不存在，请检查是否正确；报错脚本：EventManager");
        return null;
        
    }
    
}
