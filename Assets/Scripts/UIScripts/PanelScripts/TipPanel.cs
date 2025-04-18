using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TipPanel : BasePanel
{
    public UnityAction<string> setTipAction;
    //TipPanel的确认逻辑交给外部确认；
    public UnityAction setOnConfirmAction;
    //TipPanel的取消逻辑也交给外部确认：
    public UnityAction setOnCancelAction;
    public TextMeshProUGUI txtTipText;
    public Button btnConfirm;
    public Button btnCancel;
    protected override void Awake()
    {
        base.Awake();
        setTipAction += SetTipText;
    }
    protected override void Init()
    {
        btnConfirm.onClick.AddListener(setOnConfirmAction);

        btnCancel.onClick.AddListener(setOnCancelAction);
    }

    private void OnDestroy()
    {
        setTipAction -= SetTipText; 
        setOnConfirmAction = null;
    }

    private void SetTipText(string tipText)
    {
        txtTipText.text = tipText;
    }

  
}
