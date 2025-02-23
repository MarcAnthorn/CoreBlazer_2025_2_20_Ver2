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
    public EventType type;                      //�¼�����(?�磺�����¼�/ֱ���¼�(����/)?)
    public int textLibId;                       //�¼��ı���Id
    public List<EventOption> options;           //�¼�ѡ��

    //public string GUAI_Description;             
    public Dictionary<int, KaidanText> textLib; //  !!!!!!!!!!!!!!
    public string EvDescription;                //�¼�����(?װ���¼��ı����е��¼�?)

    public int id;                              //��ǰ�¼���Ψһ��ʶ
    public bool isTrigger;                      //�Ƿ񴥷���(���ڵ����¼�)

    public enum EventType
    {
        None,       // Ĭ��ֵ  
        Action1,    // �¼�1  
        Action2,    // �¼�2  
                    // �����¼�����  
    }

}

[System.Serializable]
public class EventOption
{
    public int[] conditions = new int[4];       //��ѡ����������Ҫ��ֵ(�ֱ��Ӧ�����й�Id)
    public string OpDescription;                //ѡ���ı�(?�����������¼��ı����У����ǵ�������?)

    public int optionId;                        //��Ϊ ÿ���¼� ��ͬѡ���Ψһ��ʶ(1,2,3...)
    public bool isSeletable = false;

    public EventResult result;

    //�����¼��ı�׼     !!!!!
    public int minHP, maxHP;
    public int minSTR, maxSTR;
    public int minDEF, maxDEF;
    public int minLVL, maxLVL;
    public int minSAN, maxSAN;


    [System.Serializable]
    public class EventResult
    {
        public string outcome;      //�¼����
        //
        public Action myAction;     
        public int nextEventId;     //�����Ӧ���¼�

        public void TriggerEvent()
        {
            myAction();
        }

    }

    public bool LockOrNot()         //���ɫ���Ա仯����̬����(�ڽ�ɫ�����¼�(ShowEvent)ʱ����)  !!!!!
    {
        int HP = PlayerManager.Instance.player.HP;
        int STR = PlayerManager.Instance.player.STR;
        int DEF = PlayerManager.Instance.player.DEF;
        int LVL = PlayerManager.Instance.player.LVL;
        int SAN = PlayerManager.Instance.player.SAN;
        if (DataProcessor.Instance.LowerThanStandard(HP, minHP) &&
            DataProcessor.Instance.UpToStandard(HP, maxHP) &&
            DataProcessor.Instance.LowerThanStandard(STR, minSTR) &&
            DataProcessor.Instance.UpToStandard(STR, maxSTR) &&
            DataProcessor.Instance.LowerThanStandard(DEF, minDEF) &&
            DataProcessor.Instance.UpToStandard(DEF, maxDEF) &&
            DataProcessor.Instance.LowerThanStandard(LVL, minLVL) &&
            DataProcessor.Instance.UpToStandard(LVL, maxLVL) &&
            DataProcessor.Instance.LowerThanStandard(SAN, minSAN) &&
            DataProcessor.Instance.UpToStandard(SAN, maxSAN))
        {
            isSeletable = true;             //��������
            return true;
        }

        isSeletable = false;                //����������
        return false;
    }

}

public class KaidanText                     //�ֵ��ı�
{
    public string description;
    public int textId;
}





//[System.Serializable]
//public class EventDataContainer     //������Json���ݺ�Event���ݽ�������תվ
//{
//    public List<Event> eventDatas;
//}

