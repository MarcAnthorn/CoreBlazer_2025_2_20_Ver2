using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TarotCheckPanel : BasePanel
{
    public Image imgCurrentTarot;
    public TextMeshProUGUI txtCurrentDescription;
    public Button btnExit;
    protected override void Init()
    {
        btnExit.onClick.AddListener(()=>{
            UIManager.Instance.HidePanel<TarotCheckPanel>();
        });
    }


}
