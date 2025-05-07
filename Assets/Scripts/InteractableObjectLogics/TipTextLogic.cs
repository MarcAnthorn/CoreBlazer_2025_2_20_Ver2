using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TipTextLogic : MonoBehaviour
{
    private TextMeshPro txtTip;

    private void OnEnable()
    {
        txtTip = this.GetComponent<TextMeshPro>();
        EventHub.Instance.AddEventListener<string, Vector3>("SetTipContent", SetTipContent);
    }

    private void OnDisable() {
        EventHub.Instance.RemoveEventListener<string, Vector3>("SetTipContent", SetTipContent);
    }


    //外部调用的，初始化当前提示内容的tip:
    private void SetTipContent(string tipText, Vector3 _position)
    {
        txtTip.text = tipText;
        this.transform.position = _position;
    }

}
