using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataProcessor : Singleton<DataProcessor>       //用于对数值进行处理(主要为了后续可能会有的功能考虑)
{
    protected override void Awake()
    {
        base.Awake();   //单例初始化
    }

    public bool LowerThanStandard(int test, int standard)   //低于标准
    {
        if (test < standard)
        {
            return true;
        }

        return false;
    }

    public bool UpToStandard(int test, int standard)        //达到标准
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
