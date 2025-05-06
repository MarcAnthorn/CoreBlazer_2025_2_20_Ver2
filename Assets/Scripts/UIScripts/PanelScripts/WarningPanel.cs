using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WarningPanel : BasePanel
{
    public TextMeshProUGUI txtWarning;
    public Button btnConfirm;
    protected override void Init()
    {
        btnConfirm.onClick.AddListener(()=>{
            UIManager.Instance.HidePanel<WarningPanel>();
        });
    }

    //第二参数：isFadeWithTime表示该提醒Panel是否是自动消除的；
    public void SetWarningText(string text, bool _isFadeWithTime = false)
    {
        txtWarning.text = text;
        if(_isFadeWithTime)
        {
            btnConfirm.gameObject.SetActive(false);
            //2s后自动消除：
            LeanTween.delayedCall(2f, ()=>{
                UIManager.Instance.HidePanel<WarningPanel>();
            });
            
        }
    }

}
