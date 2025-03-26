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
    //是否处在选中快捷插槽之后的高光状态；
    public bool isHighLight;
    //是否已经插入插槽；
    public bool isSelectedToSlot;
    private Button btnSelf;
    private Outline outlineSelf;

    void Awake()
    {
        btnSelf = this.GetComponent<Button>();
        outlineSelf = this.GetComponent<Outline>();
        imgSelf = this.GetComponent<Image>();
        txtSelfName = this.GetComponentInChildren<TextMeshProUGUI>();

        EventHub.Instance.AddEventListener("ResetItem", ResetItem);

    }
    void Start()
    {
        btnSelf.onClick.AddListener(()=>{
            //点击当前按钮会有多个不同的相应逻辑；取决于我当前所处的状态是什么；
            //如果处在高亮态（isHighLight），那么再次点击我会进入预选中（isPreSelected）；
            if(isHighLight)
            {
                //取消其他所有的Item的高光：
                EventHub.Instance.EventTrigger<bool>("HighLightItemsOrNot", false);
                //但是对于自己，进行特殊处理：依然高光：
                outlineSelf.enabled = true;
                isHighLight = true;

                //将当前选中的Item发送到Panel中让它持有：
                EventHub.Instance.EventTrigger("BroadcastCurrentItem", this.gameObject);

            }
            //如果什么状态都不属于，就会进入当前item对应的展示面板；
            else
            {

            }

        });
    }

    void OnDestroy()
    {
        EventHub.Instance.RemoveEventListener("ResetItem", ResetItem);
    }

    //用于重置当前Item的方法，在GodItemPanelInventory中取消交互方法调用的时候，通过外部调用委托触发：
    private void ResetItem()
    {
        isHighLight = false;
    }


}
