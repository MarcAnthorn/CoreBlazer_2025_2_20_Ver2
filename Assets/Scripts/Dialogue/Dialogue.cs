using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialogue               //�ı��Ի���
{
    public int libId;               //�ı���Id
    public int TextId;              //�ı�Id
    public bool isKwaidan;          //�ı����ͣ���̸�ı�(true)/�Ի��ı�(false)
    public string text;             //�ı�����
    public int nextId;              //��һ���ı���Id
    public int illustrationId;      //����Id
    public int bgId;                //����Id

    //public int id;                  //��ǰ�ı���Ψһ��ʶ
    public List<Option> options = new List<Option>();

    [System.Serializable]
    public class Option                 //�Ի�ѡ����(����)
    {
        public string text;
        public int nextId;              //���ݲ�ͬѡ���Ӧ��صĺ����ı�(�磺����/if��)
        public EventType eventType;     //�¼����ͣ�����ִ�е��¼�  

        // �����¼�����  
        public enum FollowUpType
        {
            None,           // Ĭ��ֵ  
            FollowUp1,      // ����1  
            FollowUp2,      // ����2  
                            // ������������  
        }
    }
}
