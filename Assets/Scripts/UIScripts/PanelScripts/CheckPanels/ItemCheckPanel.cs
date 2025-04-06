using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemCheckPanel : BasePanel
{
    //当前道具检查面板持有的道具是谁：
    private Item currentShownItem;

//-----------测试用：将持有的Item替换为ItemId，用于测试响应逻辑：-----------
    public int Id;
//------------------------------------------------------------------

    public TextMeshProUGUI txtItemName;
    public TextMeshProUGUI txtItemCount;
    public TextMeshProUGUI txtItemEffectDescription;
    public TextMeshProUGUI txtItemOtherDescription;
    public Button btnUse;
    public Button btnClose;

    protected override void Init()
    {
        InitItemInfo();

        btnUse.onClick.AddListener(()=>{
            Debug.Log($"Item is used,id{Id}");

//------------这里本需要检查道具剩余量，但是我们是测试广播行为的；因此暂时不管---------------------------------
            EventHub.Instance.EventTrigger<int>("ItemUsedCallback", Id);

        });

        btnClose.onClick.AddListener(()=>{
            UIManager.Instance.HidePanel<ItemCheckPanel>();
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
