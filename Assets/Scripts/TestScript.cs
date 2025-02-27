using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



//这是一个测试用脚本；主要是用于测试当前UI面板是否存在问题
public class TestScript : MonoBehaviour
{
    public Button btnTest;
    //测试：点击空格，立刻跳过当前对话
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            TextDisplayManager.Instance.DisplayTextImmediately();
        }
    }

    void Start()
    {
        // btnTest.onClick.AddListener(()=>{
        //     UIManager.Instance.ShowPanel<GameMainPanel>();
        // });
        UIManager.Instance.ShowPanel<GameMainPanel>();
    }


}
