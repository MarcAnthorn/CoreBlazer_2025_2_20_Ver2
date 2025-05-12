using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCanvas : MonoBehaviour
{
    private static TestCanvas instance;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject); // 如果已有实例存在，销毁新创建的副本
            return;
        }

        instance = this;
        DontDestroyOnLoad(this.gameObject); // 只有首次实例会保留
    }
}
