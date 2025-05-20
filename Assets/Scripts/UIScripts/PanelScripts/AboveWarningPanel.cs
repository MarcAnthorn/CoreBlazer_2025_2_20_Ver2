using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class AboveWarningPanel : BasePanel
{
    
    public TextMeshProUGUI txtWarning;

    //点击确定后的回调函数：
    public UnityAction callback;

    
    protected override void Init()
    {
    }

    //第二参数：isFadeWithTime表示该提醒Panel是否是自动消除的；
    //第三参数：点击确定后的回调函数：

    public void SetWarningText(string text, bool _isFadeWithTime = true, UnityAction _callback = null)
    {
        if(_isFadeWithTime)
        {
            //1s后自动消除：
            LeanTween.delayedCall(1f, ()=>{
                UIManager.Instance.HidePanel<AboveWarningPanel>();
            });
            
        }

        callback = _callback;

    }
}
