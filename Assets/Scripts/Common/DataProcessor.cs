using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataProcessor : Singleton<DataProcessor>       //���ڶ���ֵ���д���(��ҪΪ�˺������ܻ��еĹ��ܿ���)
{
    protected override void Awake()
    {
        base.Awake();   //������ʼ��
    }

    public bool LowerThanStandard(int test, int standard)   //���ڱ�׼
    {
        if (test < standard)
        {
            return true;
        }

        return false;
    }

    public bool UpToStandard(int test, int standard)        //�ﵽ��׼
    {
        if (test >= standard)
        {
            return true;
        }

        return false;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
