using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GlobalLightTrigger : MonoBehaviour
{
    private Light2D light2D;

    private void Awake()
    {
        light2D = this.GetComponent<Light2D>();
        EventHub.Instance.AddEventListener<bool>("TriggerLight", TriggerLight);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDestroy()
    {
        EventHub.Instance.RemoveEventListener<bool>("TriggerLight", TriggerLight);
    }

    //在进入安全屋的时候，触发的取消灯光的方法：
    private void TriggerLight(bool isOn)
    {
        light2D.enabled = isOn;
    }
}
