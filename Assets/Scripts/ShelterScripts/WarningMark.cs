using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarningMark : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // 记录初始位置
        Vector3 startPos = transform.localPosition;
        float floatDistance = 0.3f; // 浮动高度
        float floatTime = 0.6f;     // 单次浮动时间

        // LeanTween实现上下浮动（PingPong循环）
        LeanTween.moveLocalY(gameObject, startPos.y + floatDistance, floatTime)
            .setEaseInOutSine()
            .setLoopPingPong();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
