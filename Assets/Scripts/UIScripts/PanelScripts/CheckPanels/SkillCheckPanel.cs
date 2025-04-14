using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class SkillCheckPanel : BasePanel
{
    public GameObject skillImageObject; 
    public TextMeshProUGUI txtCost;
    public TextMeshProUGUI txtDamage;
    public TextMeshProUGUI txtBuff;
    protected override void Init()
    {
       
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0) || Input.anyKeyDown)
        {
            UIManager.Instance.HidePanel<ToastPanel>();
        }
    }

}
