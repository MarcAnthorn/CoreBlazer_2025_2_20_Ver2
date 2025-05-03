using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;



//这是一个测试用脚本；主要是用于测试当前UI面板是否存在问题
public class TestScript : MonoBehaviour
{
    void Start()
    {
        EventHub.Instance.EventTrigger("T");
    }
}
