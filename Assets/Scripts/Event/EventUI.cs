using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventUI : MonoBehaviour        //���ڹ������¼�UI��prefab�ϣ�ʵ����ʱ��ʾ�¼���Ϣ
{
    public Text eventDescriptionText;
    //public Text optionText;
    //public Button button;
    public Transform optionsPanel;

    private void Start()
    {
        ShowEvent(EventManager.Instance.currentEventId);
    }

    public void ShowEvent(int eventId)
    {
        if(EventManager.Instance.events.TryGetValue(eventId,out Event @event))
        {
            eventDescriptionText.text = @event.EvDescription;
            foreach(Transform option in optionsPanel)
            {
                Destroy(option.gameObject);
            }
            foreach(var option in @event.options)
            {
                GameObject optionGO = new GameObject("Option");
                bool isLock = option.LockOrNot();             //���������ж�
                optionGO.transform.SetParent(optionsPanel);
                if (isLock)
                {
                    Button button = optionGO.AddComponent<Button>();
                    button.onClick.AddListener(() => EventManager.Instance.SelectOption(@event.options.IndexOf(option)));       //��̬����¼�����
                }
                else
                {
                    //��Ӧλ�����ϴ�����ѡ���ͼƬ
                }
                Text optionText = optionGO.AddComponent<Text>();
                optionText.text = option.OpDescription;
            }
        }
    }

}
