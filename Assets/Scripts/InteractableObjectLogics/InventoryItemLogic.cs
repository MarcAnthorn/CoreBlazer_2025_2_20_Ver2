using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class InventoryItemLogic : MonoBehaviour
{
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
    private Outline outline;

    void Awake()
    {
        btnSelf = this.GetComponent<Button>();
        outline = this.GetComponent<Outline>();
        imgSelf = this.GetComponent<Image>();
        txtSelfName = this.GetComponentInChildren<TextMeshProUGUI>();
        isOutlineActivated = false;
        EventHub.Instance.AddEventListener("ResetItem", ResetItem);

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
                UIManager.Instance.ShowPanel<ItemCheckPanel>();
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
    }

    //用于重置当前Item的方法，在GodItemPanelInventory中取消交互方法调用的时候，通过外部调用委托触发：
    private void ResetItem()
    {
        isInPreselecting = false;
    }

    public void OutlineActivated()
    {

    }



}
