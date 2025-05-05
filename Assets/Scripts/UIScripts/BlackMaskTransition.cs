using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BlackMaskTransition : MonoBehaviour
{
    private Image blackMask;
    void Awake()
    {
        blackMask = this.GetComponent<Image>();

        EventHub.Instance.AddEventListener<UnityAction>("ShowMask", ShowMask);
        EventHub.Instance.AddEventListener<UnityAction>("HideMask", HideMask);

    }

    void OnDestroy()
    {
        EventHub.Instance.RemoveEventListener<UnityAction>("ShowMask", ShowMask);
        EventHub.Instance.RemoveEventListener<UnityAction>("HideMask", HideMask);
    }

    private void ShowMask(UnityAction callback)
    {
        Debug.Log("Mask Revealed!");
        Color color = blackMask.color;
        color.a = 0;
        blackMask.color = color;
        LeanTween.value(blackMask.gameObject, 0f, 1f, 1f)
            .setOnUpdate((float val) =>
            {
                Color color = blackMask.color;
                color.a = val;
                blackMask.color = color;
            }).setOnComplete(()=>{
                callback?.Invoke();
            });
    }

    private void HideMask(UnityAction callback)
    {
        Debug.Log("Mask Hidden!");
        Color color = blackMask.color;
        color.a = 1;
        blackMask.color = color;
        LeanTween.value(blackMask.gameObject, 1f, 0f, 1f)
            .setOnUpdate((float val) =>
            {
                Color color = blackMask.color;
                color.a = val;
                blackMask.color = color;
            }).setOnComplete(()=>{
                callback?.Invoke();
            });
    }
}
