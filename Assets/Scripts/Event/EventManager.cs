using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using TMPro;
using UnityEditor.PackageManager;

public class EventManager : Singleton<EventManager>
{
    public Dictionary<int, Event> events = new Dictionary<int, Event>();    //�洢ĳһ�ؿ��¼�����������¼�
    public int currentEventId = 0;
    public static int eventCount = 0;

    protected override void Awake()
    {
        base.Awake();   //������ʼ��
        LoadEvents();
    }

    private void Update()
    {
        
    }

    void LoadEvents()
    {
        //���������¼�����(CSV��ʽ)��events�ֵ��У�ʹ��Assets(Application.dataPath)�µ����·��
        string path = Path.Combine(Application.dataPath, "Resources/EventData/eventDatas.json");
        if (File.Exists(path))
        {
            string[] lines = File.ReadAllLines(path);   //�ָ�ÿһ�д���lines

            for(int i = 1; i < lines.Length; i++)       //����ÿһ�У���ø��е���Ϣ
            {
                string line = lines[i];
                string[] values = line.Split(',');      //��ÿһ�а��ն��ŷָ�

                // ȷ�����㹻����  
                if (values.Length >= 5)
                {
                    Event eventData = new Event()
                    {
                        id = int.Parse(values[0]),      //����id����������  
                        name = values[1],
                        EvDescription = values[2]
                    };
                    events.Add(eventData.id, eventData);
                }
            }
        }
        else
        {
            Debug.LogWarning("�¼������ļ������ڣ�");
        }
    }

    void SaveEvents()
    {
        // �����¼����ݵ� CSV  
        string path = Path.Combine(Application.dataPath, "Resources/EventData/eventDatas.csv");
        // ����ļ������ڣ�д�������  
        if (!File.Exists(path))
        {
            File.WriteAllText(path, "id,name,date\n"); // ���������Ӧ�� EventData ������ֶ����Ӧ  
        }

        // ���� events �ֵ䣬���� CSV ��  
        List<string> lines = new List<string>();
        foreach (var kvp in events)
        {
            Event eventData = kvp.Value;
            string line = $"{eventData.id},{eventData.name},{eventData.EvDescription}";  
            lines.Add(line);
        }

        // ���µ�����ӵ� CSV �ļ�  
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
            //������

            Debug.Log(result.outcome);                  //��ӡ���¼��Ľ��
            currentEventId = result.nextEventId;        //���µ���һ���¼�
        }
    }

    public void TriggerEvent(GameObject go)                //����ɫ��OnTriggerEnter()��������ʱ����,��ȡ���¼���Ϣ
    {
        EventGO EvGO = go.GetComponent<EventGO>();
        currentEventId = EvGO.eventId;
        EventUI eventUI = new EventUI();
        //���ܻ�����eventUI��ص�����(����λ�ã���С��)
        Instantiate(eventUI);
    }

}
