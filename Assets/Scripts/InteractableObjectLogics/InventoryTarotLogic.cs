using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class InventoryTarotLogic : MonoBehaviour
{
    
    //当前持有的Item（存储了相关Item信息的，通过id访问LoadManager实现获取）
    public Item myItem;
    //当前持有的Item其对应的id：
    public int myItemId;

    public Image imgSelf;
    public TextMeshProUGUI txtSelfName;
   
    private Button btnSelf;
    public GameObject outlineObjects;

    void Awake()
    {
        btnSelf = this.GetComponent<Button>();
        imgSelf = this.GetComponent<Image>();
        txtSelfName = this.GetComponentInChildren<TextMeshProUGUI>();

    }
    void Start()
    {

        btnSelf.onClick.AddListener(()=>{
            UIManager.Instance.ShowPanel<TarotCheckPanel>().InitItemInfo(myItem);

        });

    }


    void OnDestroy()
    {
       
    }

    //初始化方法Item和信息Item；
    //以及初始化部分显示的内容
    public void Init(Item _item)
    {
        myItem = _item;
        myItemId = _item.id;
        txtSelfName.text = myItem.name;

        string rootPath = Path.Combine("ArtResources", "Tarot", _item.id.ToString());
        imgSelf.sprite = Resources.Load<Sprite>(rootPath);
    }

    // //事件：道具响应事件；
    // //当一个道具使用之后，会进行该事件的调用；传入的参数是对应Item的id：
    // //如果我的id符合，那么我就会执行逻辑: 这里就是执行激活黑色蒙版；表示道具生效中；
    // //不需要蒙版的只有isImmediate的Item；
    // private void ItemUsedCallback(int targetItemId)
    // {
    //     //Item响应的要求有两个：
    //     // 1.目前广播的目标id和我的id是一样的；
    //     // 2.我不是处在插槽中的Item；也就是说，始终只有背包中的Item会进行响应；
    //     if(targetItemId == myItemId && !isItemInSlot)
    //     {
    //         //和我对应上了，说明是广播给我的；
    //         //那么我就执行这个逻辑：
    //         takeEffectMaskObject.SetActive(true);

    //     }

    //     //如果我也是这个id，但是我是插槽中的Item，那么我也需要进行对应的逻辑处理：
    //     if(targetItemId == myItemId && isItemInSlot)
    //     {
    //         //首先：如果我不是持续时间内生效的，那么我就需要删除我自己，然后对插槽状态进行重置：
    //         if(!myItem.isPermanent && myItem.EffectiveTime == 0)
    //         {
    //             EventHub.Instance.EventTrigger("ResetItem");

    //             //该事件在InventoryPanel中注册：
    //             EventHub.Instance.EventTrigger<int>("RemoveSlotItem", targetItemId);         
    //         }
    //         else if(!myItem.isPermanent)
    //         {
    //             //先加上蒙版：
    //             takeEffectMaskObject.SetActive(true);
    //             //因为我只是一个插槽中的复制品，所以只需要在结束之后处理自己的显示相关就行：
    //             myItem.onCompleteCallback = null;
    //             myItem.isInUse = true;
    //             //移除蒙版的处理也在RefreshItemsInPanel中；
    //             myItem.onCompleteCallback += () => {
    //                 myItem.isInUse = false;
                    
    //                 //同时，如果数量减少到0了，那么就会从背包中移除：
    //                 if(ItemManager.Instance.itemCountDic[myItem.id] == 0)
    //                 {
    //                     EventHub.Instance.EventTrigger("ResetItem");
    //                     //该Item对应的背包中的本体的处理中，已经包含了对于背包内容的调整；
    //                     //因此，此处不应该出现逻辑的重复：
    //                     //只是删除自己：
    //                     EventHub.Instance.EventTrigger<int>("RemoveSlotItem", targetItemId);               
    //                 }
    //             };
    //         }
    //     }

    //     //更新玩家的属性面板：
    //     EventHub.Instance.EventTrigger("UpdateAttributeText");
    // }


}
