using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemCheckPanel : BasePanel
{
    // private Item currentShownItem;
    public TextMeshProUGUI txtItemName;
    public TextMeshProUGUI txtItemLeftNotice;
    public TextMeshProUGUI txtItemEffectDescription;
    public TextMeshProUGUI txtItemOtherDescription;
    public Button btnUse;
    public Button btnClose;

    protected override void Init()
    {
        InitItemInfo();

        btnUse.onClick.AddListener(()=>{

        });

        btnClose.onClick.AddListener(()=>{

        });
    }

    //获取当前应该显示的Item的实例
    public void FetchItem()
    {

    }

    //初始化Item显示面板信息的方法：
    private void InitItemInfo()
    {

    }

}
