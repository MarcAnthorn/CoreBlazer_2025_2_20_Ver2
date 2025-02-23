using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using TMPro;
using UnityEditor.PackageManager;

public class EventManager : Singleton<EventManager>
{
    public Dictionary<int, Event> events = new Dictionary<int, Event>();    //存储某一关卡事件库里的所有事件
    public int currentEventId = 0;
    public static int eventCount = 0;

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
        //加载已有事件数据(CSV格式)到events字典中，使用Assets(Application.dataPath)下的相对路径
        string path = Path.Combine(Application.dataPath, "Resources/EventData/eventDatas.json");
        if (File.Exists(path))
        {
            string[] lines = File.ReadAllLines(path);   //分割每一行存入lines

            for(int i = 1; i < lines.Length; i++)       //遍历每一行，获得各列的信息
            {
                string line = lines[i];
                string[] values = line.Split(',');      //将每一行按照逗号分割

                // 确保有足够的列  
                if (values.Length >= 5)
                {
                    Event eventData = new Event()
                    {
                        id = int.Parse(values[0]),      //假设id是整型类型  
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

    public void SelectOption(int optionIndex)           //1,2,3
    {
        foreach(var option in events[currentEventId].options)
        {
            if (optionIndex == option.optionId)
            {

            }
        }

        if (events[currentEventId].options.Contains(optionIndex))
        {
            EventOption.EventResult result = currentEvent.results[currentEventId];
            //处理结果

            Debug.Log(result.outcome);                  //打印该事件的结果
            currentEventId = result.nextEventId;        //更新到下一个事件
        }
    }

    public void TriggerEvent(GameObject go)                //当角色的OnTriggerEnter()方法发生时调用,获取该事件信息
    {
        EventGO EvGO = go.GetComponent<EventGO>();
        currentEventId = EvGO.eventId;
        EventUI eventUI = new EventUI();
        //可能会设置eventUI相关的数据(比如位置，大小等)
        Instantiate(eventUI);
    }

}
