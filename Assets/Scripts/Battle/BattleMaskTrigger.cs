using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class BattleMaskTrigger : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    private int fadeInTweenId = -1;
    private int fadeOutTweenId = -1;
    private int delayCallId = -1;

    private void OnEnable()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) return;
        
        canvasGroup.alpha = 0; // 从透明开始

        // 先淡入（透明度从0到1）
        fadeInTweenId = LeanTween.alphaCanvas(canvasGroup, 1f, 0.2f).setOnComplete(() =>
        {
            if (canvasGroup == null || !gameObject.activeInHierarchy) return;
            
            // 淡入完成后，延迟1秒再淡出
            delayCallId = LeanTween.delayedCall(0.5f, () =>
            {
                if (canvasGroup == null || !gameObject.activeInHierarchy) return;
                
                // 再淡出（透明度从1到0）
                fadeOutTweenId = LeanTween.alphaCanvas(canvasGroup, 0f, 0.2f).setOnComplete(()=>
                {
                    if (gameObject != null && gameObject.activeInHierarchy)
                    {
                        this.gameObject.SetActive(false);
                    }
                }).id;
            }).id;
        }).id;
    }

    private void OnDisable()
    {
        // 取消所有正在进行的动画，防止引用已销毁的组件
        if (fadeInTweenId != -1)
        {
            LeanTween.cancel(fadeInTweenId);
            fadeInTweenId = -1;
        }
        
        if (fadeOutTweenId != -1)
        {
            LeanTween.cancel(fadeOutTweenId);
            fadeOutTweenId = -1;
        }
        
        if (delayCallId != -1)
        {
            LeanTween.cancel(delayCallId);
            delayCallId = -1;
        }
    }

    private void OnDestroy()
    {
        // 确保在对象被销毁时取消所有动画
        OnDisable();
    }
}
