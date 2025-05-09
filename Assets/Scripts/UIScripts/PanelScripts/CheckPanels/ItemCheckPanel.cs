using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemCheckPanel : BasePanel
{
    //当前道具检查面板持有的道具是谁：
    private Item myItem;
    //持有的Item的ItemId
    public Image imgSelf;
    public int currentItemId;

    public TextMeshProUGUI txtItemName;
    public TextMeshProUGUI txtItemCount;
    public TextMeshProUGUI txtItemEffectDescription;
    public TextMeshProUGUI txtItemOtherDescription;
    public Button btnUse;
    public Button btnClose;

    //展示用的时候取消的Button;默认是失活的；
    public Button btnCloseWhenDisplay;
    

    protected override void Init()
    {
        btnUse.onClick.AddListener(()=>{
            //使用道具的唯一接口：UseItem；
            //将所有的判断、使用逻辑，以及使用后回调全部在里面处理
            //如果使用失败，那么弹出使用失败的弹框；
            //如果使用成功，则关闭当前的ItemCheckPanel:
            if(ItemManager.Instance.UseItem(myItem))
            {
                UIManager.Instance.HidePanel<ItemCheckPanel>();

                //这个方法在InventoryItemLogic中： 
                EventHub.Instance.EventTrigger<int>("ItemUsedCallback", currentItemId);
                UIManager.Instance.ShowPanel<ToastPanel>().SetItemResult(myItem);

                //调用UI更新委托：
                EventHub.Instance.EventTrigger("UpdateAllUIElements");

            }
        
        });

        btnClose.onClick.AddListener(()=>{
            UIManager.Instance.HidePanel<ItemCheckPanel>();
        });

        btnCloseWhenDisplay.onClick.AddListener(()=>{
            UIManager.Instance.HidePanel<ItemCheckPanel>();
        });

        string rootPath = Path.Combine("ArtResources", "Item", myItem.name);
        imgSelf.sprite = Resources.Load<Sprite>(rootPath);
    }

    //初始化Item显示面板信息的方法：
    public void InitItemInfo(Item _item, bool _isToUse = true)
    {
        myItem = _item;
        currentItemId = myItem.id;
        txtItemName.text = myItem.name;
        txtItemCount.text = $"持有道具数量:{ItemManager.Instance.itemCountDic[currentItemId]}";
        txtItemEffectDescription.text = myItem.instruction;
        txtItemOtherDescription.text = myItem.description;

        //如果不是用于使用的，而是展览的，那么进行额外的处理：
        if(!_isToUse)
        {
            btnUse.gameObject.SetActive(false);
            btnClose.gameObject.SetActive(false);
            txtItemCount.gameObject.SetActive(false);
            
            btnCloseWhenDisplay.gameObject.SetActive(true);
            
        }

    }

}
