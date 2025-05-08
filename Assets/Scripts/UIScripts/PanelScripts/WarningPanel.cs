using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class WarningPanel : BasePanel
{
    public TextMeshProUGUI txtWarning;
    public Button btnConfirm;

    //点击确定后的回调函数：
    public UnityAction callback;

    
    protected override void Init()
    {
        btnConfirm.onClick.AddListener(()=>{
            PoolManager.Instance.ReturnToPool("WarningPanel", this.gameObject);     
            LeanTween.delayedCall(0.2f, ()=>{
                callback?.Invoke();
            });          
        });
    }

    //第二参数：isFadeWithTime表示该提醒Panel是否是自动消除的；
    //第三参数：点击确定后的回调函数：

    public void SetWarningText(string text, bool _isFadeWithTime = false, UnityAction _callback = null)
    {
        txtWarning.text = text;
        if(_isFadeWithTime)
        {
            btnConfirm.gameObject.SetActive(false);
            //1s后自动消除：
            LeanTween.delayedCall(1f, ()=>{
                UIManager.Instance.HidePanel<WarningPanel>();
            });
            
        }

        callback = _callback;

    }

}
