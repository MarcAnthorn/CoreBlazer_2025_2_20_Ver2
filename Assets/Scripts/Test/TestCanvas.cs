using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCanvas : MonoBehaviour
{

    //测试用：失活所有不需要出现在游戏场景中的东西：
    public GameObject objToDo;

    private static TestCanvas instance;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject); // 如果已有实例存在，销毁新创建的副本
            return;
        }

        instance = this;
        EventHub.Instance.AddEventListener<bool>("TestClearFunction", TestClearFunction);
        DontDestroyOnLoad(this.gameObject); // 只有首次实例会保留
    }

    // void Update()
    // {
    //     if(Input.GetKeyDown(KeyCode.P))
    //     {
    //         UIManager.Instance.HidePanel
    //     }
    // }

    void OnDestroy()
    {
        EventHub.Instance.RemoveEventListener<bool>("TestClearFunction", TestClearFunction);
    }

    //将所有过场景不需要的东西失活的方法：测试用：
    //该方法调用在StatueRoom和PlayerController中的OnPlayerDead两个方法中，用于在切换场景的时候处理一些东西的激活失活；
    private void TestClearFunction(bool _isSetActive)
    {
        objToDo.SetActive(_isSetActive);
    }
}
