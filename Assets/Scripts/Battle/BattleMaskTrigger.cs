using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class BattleMaskTrigger : MonoBehaviour
{
    private CanvasGroup canvasGroup;

    private void OnEnable()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0; // 从透明开始

        // 先淡入（透明度从0到1）
        LeanTween.alphaCanvas(canvasGroup, 1f, 0.2f).setOnComplete(() =>
        {
            // 淡入完成后，延迟1秒再淡出
            LeanTween.delayedCall(0.5f, () =>
            {
                // 再淡出（透明度从1到0）
                LeanTween.alphaCanvas(canvasGroup, 0f, 0.2f).setOnComplete(()=>
                {
                    this.gameObject.SetActive(false);
                });
            });
        });
    }
}
