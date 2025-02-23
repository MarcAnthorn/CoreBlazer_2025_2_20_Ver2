using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]           //��������һ��������ݿ������л�
public class Event          //
{
    public int libId;                           //�¼���Id(����ͬ�ؿ���ѡ)   ************************
    public int eventId;                         //�¼�Id(��Ӧһ���¼��ı���)
    public EventType type;                      //�¼�����(?�磺�����¼�/ս���¼�/ֱ���¼�(�磺����)?)

    private List<EventOption> options;          //�¼�ѡ��
    private Dictionary<int, KaidanText> textLib;//�¼��ı��⣬ǰ���int�������ı���Id��ʶ���ı������ã�����ȡ�ı�
    private string EvDescription;               //�¼�����(װ���¼��ı����е��¼������ã�װ��ʵ����ʾ���ı�)

    public int id;                              //��ǰ�¼���Ψһ��ʶ
    public bool isTrigger;                      //�Ƿ񴥷���(���ڵ����¼�)

    public enum EventType
    {
        None,       // Ĭ��ֵ  
        Action1,    // �¼�1  
        Action2,    // �¼�2  
                    // �����¼�����  
    }

    [System.Serializable]
    public class KaidanText                     //�ֵ��ı�
    {
        public string description;
        public int textId;
    }

    public List<EventOption> GetOptions()           //�¼�ѡ��
    {
        return this.options;
    }
    public void SetOptions(int optionId, EventOption @eventOption)
    {
        options[optionId] = @eventOption;
    }

    public Dictionary<int, KaidanText> GetTextLib() //�¼��ı���
    {
        return textLib;
    }
    public void SetTextLib(int textId, KaidanText text)
    {
        textLib[textId] = text;
    }

    public string GetEvDescription()                //�¼�����(��ȡ�¼��ı���)
    {
        return EvDescription;
    }
    public void SetEvDescription(string description)
    {
        this.EvDescription = description;
    }

}

[System.Serializable]
public class EventOption
{
    public int conditionId;                     //��ѡ����������Id
    public int minCondition;                    //��С ��ѡ����������Ҫ��ֵ(�ֱ��Ӧһ���й�Id)
    public int maxCondition;                    //��� ��ѡ����������Ҫ��ֵ(�ֱ��Ӧһ���й�Id)
    public string OpDescription;                //ѡ���ı�(?�����������¼��ı����У����ǵ�������?)

    public int optionId;                        //��Ϊ ÿ���¼� ��ͬѡ���Ψһ��ʶ(1,2,3...)
    public bool isSeletable = false;

    public EventResult result;

    //�����¼��ı�׼     !!!!!
    public int minSAN, maxSAN;
    public int minSTR, maxSTR;
    public int minSPD, maxSPD;


    [System.Serializable]
    public class EventResult
    {
        public string outcome;                  //�¼����
        //
        public Action myAction;                 //ѡ��ѡ��֮��Ĵ���
        public BattleEvent battleEvent;         //�����Ӧ��ս���¼�

        public void TriggerEvent()
        {
            myAction();
        }

    }

    public bool LockOrNot(Player player)         //���ɫ���Ա仯����̬����(�ڽ�ɫ�����¼�(ShowEvent)ʱ����)  !!!!!
    {
        int SAN = player.SAN;       //��ɫ��ǰ����ֵ
        int STR = player.STR;       //��ɫ��ǰ����
        int SPD = player.SPD;       //��ɫ��ǰ�ٶ�
        if (optionId == 1 && minSAN < SAN && maxSAN > SAN ||
            optionId == 2 && minSTR < STR && maxSTR > STR ||
            optionId == 3 && minSPD < SPD && maxSPD > SPD)
        {
            isSeletable = true;             //��������
            return true;
        }

        isSeletable = false;                //����������
        return false;
    }

}

[System.Serializable]
public class BattleEvent : Event
{
    
}




//[System.Serializable]
//public class EventDataContainer     //������Json���ݺ�Event���ݽ�������תվ
//{
//    public List<Event> eventDatas;
//}

