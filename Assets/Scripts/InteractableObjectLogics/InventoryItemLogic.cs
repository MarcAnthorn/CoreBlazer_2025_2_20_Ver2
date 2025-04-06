using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class InventoryItemLogic : MonoBehaviour
{
    //当前持有的数据结构类Item：
    public Item myItem;
    //当前持有的Item其对应的id：
    public int myItemId;

//------------------------------------------------------------------------
    //为了防止Awake中的随机id覆盖了Instantiate出来的Item的正确的id而设置的callbackId；
    //之后肯定不是在Awake中随机生成的，所以这段只是暂时的逻辑；
    public int callbackId = -1;
//------------------------------------------------------------------------

    public Image imgSelf;
    public TextMeshProUGUI txtSelfName;
    //是否处在预选阶段；
    public bool isInPreselecting;
    //是否已经插入插槽；
    //只有在卸下当前道具之后才会重置；
    public bool isSelectedToSlot;
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

        

//------------------------------------------------------------------------
        //测试用：随机分配id：
        myItemId = Random.Range(0, 100001);

//------------------------------------------------------------------------

    }
    void Start()
    {
//----------------------测试-------------------------------------------------------
        if(callbackId != -1)
        {
            myItemId = callbackId;
        }
//----------------------测试-------------------------------------------------------
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
//----------------------测试-------------------------------------------------------
                UIManager.Instance.ShowPanel<ItemCheckPanel>().Id = myItemId;
                
            }

        });
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

    //用于重置当前Item的方法，在GodItemPanelInventory中取消交互方法调用的时候，通过外部调用委托触发：
    private void ResetItem()
    {
        isInPreselecting = false;
    }

    //事件：道具响应事件；
    //当一个道具使用之后，会进行该事件的调用；传入的参数是对应Item的id：
    //如果我的id符合，那么我就会执行某个逻辑；比如这里就是执行激活黑色蒙版；表示道具生效中；
    private void ItemUsedCallback(int targetItemId)
    {
        if(targetItemId == myItemId)
        {
            //和我对应上了，说明是广播给我的；
            //那么我就执行这个逻辑：

            //测试逻辑：加上蒙版：
            takeEffectMaskObject.SetActive(true);
        }
    }



}
