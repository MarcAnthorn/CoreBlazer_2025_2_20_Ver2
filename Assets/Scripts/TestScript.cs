using System.Collections;
using System.Collections.Generic;
using UnityEngine;



//这是一个测试用脚本；主要是用于测试当前UI面板是否存在问题
public class TestScript : MonoBehaviour
{
    //测试：点击空格，立刻跳过当前对话
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            TextDisplayManager.Instance.DisplayTextImmediately();
        }
    }

   
}
