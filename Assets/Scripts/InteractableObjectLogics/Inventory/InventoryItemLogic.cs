using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;


//这个是可使用的Item逻辑（Item会处在背包面板中、ItemPanel中）
public class InventoryItemLogic : MonoBehaviour
{
    //当前持有的Item（存储了相关Item信息的，通过id访问LoadManager实现获取）
    public Item myItem;
    //当前持有的Item其对应的id：
    public int myItemId;


    public Image imgSelf;
    public TextMeshProUGUI txtSelfName;
    public TextMeshProUGUI txtSelfItemCount;
    //是否处在预选阶段；
    public bool isInPreselecting;
    //是否已经插入插槽；
    //只有在卸下当前道具之后才会重置；
    public bool isSelectedToSlot;
    //这个布尔标识是用于区分插入插槽和处在背包中的Item的；
    //防止使用Item的时候，插入快捷插槽和背包中的同Id的Item双重响应：
    public bool isItemInSlot;
    public bool isOutlineActivated;
    private Button btnSelf;
    public GameObject outlineObjects;
    //这个不是自己挂载的GameObject，而是自己的父对象；
    public GameObject itemObject;
    //生效遮罩的游戏对象：
    public GameObject takeEffectMaskObject;
    private Outline outline;

    void Awake()
    {
        btnSelf = this.GetComponent<Button>();
        outline = this.GetComponent<Outline>();
        imgSelf = this.GetComponent<Image>();
        txtSelfName = this.GetComponentInChildren<TextMeshProUGUI>();
        isOutlineActivated = false;

        
        EventHub.Instance.AddEventListener("ResetItem", ResetItem);
        EventHub.Instance.AddEventListener<int>("ItemUsedCallback", ItemUsedCallback);


    }
    void Start()
    {

        btnSelf.onClick.AddListener(()=>{
            //点击当前按钮会有多个不同的相应逻辑；取决于我当前所处的状态是什么；
            //如果处在高亮态（isHighLight），那么再次点击我会进入预选中（isPreSelected）；
            if(isInPreselecting)
            {
                //取消其他所有的Item的高光：
                EventHub.Instance.EventTrigger<bool>("HighLightItemsOrNot", false);
                //但是对于自己，进行特殊处理：依然高光：
                outlineObjects.SetActive(true);
                isInPreselecting = true;

                //将当前选中的Item发送到Panel中让它持有：
                EventHub.Instance.EventTrigger("BroadcastCurrentItem", itemObject);

            }
            //如果什么状态都不属于，就会进入当前item对应的展示面板；
            else
            {
                UIManager.Instance.ShowPanel<ItemCheckPanel>().InitItemInfo(myItem);
                
            }

        });

        string rootPath = Path.Combine("ArtResources", "Item",  myItem.name);
        imgSelf.sprite = Resources.Load<Sprite>(rootPath);
    }


    void Update()
    {
        if(isSelectedToSlot)
        {
            //如果当前的Item被选中了，那么就持续高光：
            outlineObjects.SetActive(true);
            //但是isHighLight不能持续置true，不然这个道具点击不会触发展示Item面板；
        }
        else if(!isInPreselecting && !isSelectedToSlot)
        {
            outlineObjects.SetActive(false);        
        }

    }

    void OnDestroy()
    {
        EventHub.Instance.RemoveEventListener("ResetItem", ResetItem);
        EventHub.Instance.RemoveEventListener<int>("ItemUsedCallback", ItemUsedCallback);
    }

    //初始化方法Item和信息Item；
    //以及初始化部分显示的内容
    public void Init(Item _item)
    {
        myItem = _item;
        myItemId = _item.id;
        //初始化名称和使用剩余次数；
        txtSelfName.text = myItem.name;
        txtSelfItemCount.text = ItemManager.Instance.itemCountDic[myItem.id].ToString();
    }

    //刷新自己UI显示的方法，在使用之后，通过Panel中的回调RemoveItemInPanel触发
    public void RefreshSelf()
    {
        //如果道具已经没了，那么就不需要处理了：
        if(!ItemManager.Instance.itemCountDic.ContainsKey(myItemId))
            return;
            
        txtSelfItemCount.text = ItemManager.Instance.itemCountDic[myItem.id].ToString();

        //如果我不是永久的，那么说明是我的生效时间到了；
        //移除蒙版：
        if(!myItem.isPermanent)
        {
            takeEffectMaskObject.SetActive(false);
        }
    }

    //用于重置当前Item的方法，在GodItemPanelInventory中取消交互方法调用的时候，通过外部调用委托触发：
    private void ResetItem()
    {
        isInPreselecting = false;
    }

    //事件：道具响应事件；
    //当一个道具使用之后，会进行该事件的调用；传入的参数是对应Item的id：
    //如果我的id符合，那么我就会执行逻辑: 这里就是执行激活黑色蒙版；表示道具生效中；
    //不需要蒙版的只有isImmediate的Item；
    private void ItemUsedCallback(int targetItemId)
    {
        //Item响应的要求有两个：
        // 1.目前广播的目标id和我的id是一样的；
        // 2.我不是处在插槽中的Item；也就是说，始终只有背包中的Item会进行响应；
        if(targetItemId == myItemId && !isItemInSlot)
        {
            //和我对应上了，说明是广播给我的；
            //那么我就执行这个逻辑：
            takeEffectMaskObject.SetActive(true);

        }

        //如果我也是这个id，但是我是插槽中的Item，那么我也需要进行对应的逻辑处理：
        if(targetItemId == myItemId && isItemInSlot)
        {
            //首先：如果我不是持续时间内生效的，那么我就需要删除我自己，然后对插槽状态进行重置：
            if(!myItem.isPermanent && myItem.EffectiveTime == 0)
            {
                EventHub.Instance.EventTrigger("ResetItem");

                //该事件在InventoryPanel中注册：
                EventHub.Instance.EventTrigger<int>("RemoveSlotItem", targetItemId);         
            }
            else if(!myItem.isPermanent)
            {
                //先加上蒙版：
                takeEffectMaskObject.SetActive(true);
                //因为我只是一个插槽中的复制品，所以只需要在结束之后处理自己的显示相关就行：
                myItem.onCompleteCallback = null;
                myItem.isInUse = true;
                //移除蒙版的处理也在RefreshItemsInPanel中；
                myItem.onCompleteCallback += () => {
                    myItem.isInUse = false;
                    
                    //同时，如果数量减少到0了，那么就会从背包中移除：
                    if(ItemManager.Instance.itemCountDic[myItem.id] == 0)
                    {
                        EventHub.Instance.EventTrigger("ResetItem");
                        //该Item对应的背包中的本体的处理中，已经包含了对于背包内容的调整；
                        //因此，此处不应该出现逻辑的重复：
                        //只是删除自己：
                        EventHub.Instance.EventTrigger<int>("RemoveSlotItem", targetItemId);               
                    }
                };
            }
        }

        //更新玩家的属性面板：
        EventHub.Instance.EventTrigger("UpdateAttributeText");
    }



}
