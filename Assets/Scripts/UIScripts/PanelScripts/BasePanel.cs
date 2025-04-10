using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


//面板基类，所有UI Panel继承自该类
[RequireComponent(typeof(CanvasGroup))]
public abstract class BasePanel : MonoBehaviour
{
    protected CanvasGroup canvasGroup;

    [SerializeField]
    [Tooltip("Time for panel to fade, only positive values are supportive")]
    private float fadingTime = 0.4f;

    //面板隐藏之后的回调函数
    private UnityAction callBackHide;

    //Awake在脚本启用之前就可以被调用
    //如果这里使用Start函数，那么在脚本被启用之前，canvasGroup都不会成功引用；
    //因此，在初始化一些内容的时候，我们优先考虑使用Awake函数实现初始化，防止出现空引用；
    protected virtual void Awake()
    {
        canvasGroup = this.GetComponent<CanvasGroup>();
    }

    protected virtual void Start()
    {
        Init();
    }

    protected abstract void Init();

    public virtual void ShowMe()
    {
        if (canvasGroup == null)
            Debug.LogError("Canvas Group为空");
        canvasGroup.alpha = 0;
        canvasGroup.LeanAlpha(1, fadingTime);


    }
    public virtual void HideMe(UnityAction callBack)
    {
        //canvasGroup.alpha = 1;
        canvasGroup.LeanAlpha(0, fadingTime).setOnComplete(() =>
        {
            callBack?.Invoke();
        });
    }

}
