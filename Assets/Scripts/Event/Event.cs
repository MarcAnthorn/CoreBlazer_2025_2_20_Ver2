using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]           //��������һ��������ݿ������л�
public class Event : MonoBehaviour
{
    public int id;                              //ÿ����ͬ�¼���Ψһ��ʶ
    public string EvDescription;                //�¼�����
    public List<EventOption> options;           //�¼�ѡ��
    public List<EventOption.EventResult> results;
    //public Dictionary<EventOption, EventOption.EventResult> results;

}

[System.Serializable]
public class EventOption
{
    public string OpDescription;
    public int eventId;             //�����¼���Id
    public int optionId;            //��Ϊ ÿ���¼� ��ͬѡ���Ψһ��ʶ(1,2,3...)

    public Event nextEventId;

    private bool isSeletable = false;

    //�����¼��ı�׼
    public int minHP, maxHP;
    public int minSTR, maxSTR;
    public int minDEF, maxDEF;
    public int minLVL, maxLVL;
    public int minSAN, maxSAN;


    [System.Serializable]
    public class EventResult
    {
        public string outcome;      //�¼����
        public int nextEventId;     //�����Ӧ���¼�
    }

    public bool LockOrNot()         //���ɫ���Ա仯����̬����(�ڽ�ɫ�����¼�(ShowEvent)ʱ����)
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



[System.Serializable]
public class EventDataContainer     //������Json���ݺ�Event���ݽ�������תվ
{
    public List<Event> eventDatas;
}

