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

    public void SetWarningText(string text)
    {
        txtWarning.text = text;
    }

}
