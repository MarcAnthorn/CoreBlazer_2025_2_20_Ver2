using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventGO : MonoBehaviour            //��������Ϸ�б�ʾ�¼���������
{
    public Event @event;
    public int eventId;         //��ʾ�¼��Ĺ̶�Id

    void Awake()
    {
        @event = EventManager.Instance.CreateEvent(eventId);
    }

    void Update()
    {
        
    }
}
