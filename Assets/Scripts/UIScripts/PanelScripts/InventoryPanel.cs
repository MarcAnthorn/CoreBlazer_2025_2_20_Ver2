using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class InventoryPanel : BasePanel
{
    public Image imgPlayer;
    public Slider sliderHealth;
    public Slider sliderSanity;
    public Slider sliderLight;
    public Button btnGodPanelReveal;
    public Button btnCommonPanelReveal;
    public Button btnQuickSlotLeft;
    public Button btnQuickSlotRight;
    public Button btnSlotLeftOff;
    public Button btnSlotRightOff;
    //用于取消插槽提示的Button：
    public Button btnCancelSlotHighLight;
    public TextMeshProUGUI txtStrength;
    public TextMeshProUGUI txtSpeed;

    public TextMeshProUGUI txtDefense;

    public TextMeshProUGUI txtCriticalRate;

    public TextMeshProUGUI txtComboRate;

    public TextMeshProUGUI txtDodgeRate;

    public TextMeshProUGUI txtCriticalMultiplier;
    public Transform tarotContent;
    public Transform leftSlotItemAnchor;
    public Transform rightSlotItemAnchor;
    public List<GameObject> tarotList = new List<GameObject>();

    //当前的快速插槽（左和右）是否处在等待插入道具的状态：
    //如果是，那么下次点击对应的插槽就会把当前处在isPreselected状态的Item实例化一份到插槽；
    //注意：始终只会有一个Slot处在预备态；两个插槽不能同时处在预备态；
    private bool isLeftSlotReadyForItem = false;
    private bool isRightSlotReadyForItem = false;
    //这个变量用于当前选中的item和slot之间的交流；
    //如果存在item进入预备态（preSelected），那么这个item就会向InventoryPanel广播；
    //让这个bool更换到true；
    //这样再次点击slot就会将这个item加入slot；
    private GameObject currentSelectedItem = null;

    //当前插入插槽的两个Item；
    private GameObject leftSlottedItem;
    private GameObject rightSlottedItem;


    



    protected override void Init()
    {
        // UIManager.Instance.ShowPanel<GodItemPanelInventory>();
        btnCommonPanelReveal.onClick.AddListener(()=>{

        });

        btnGodPanelReveal.onClick.AddListener(()=>{

        });

        btnQuickSlotLeft.onClick.AddListener(()=>{
            Debug.Log("左侧快捷插槽触发");
            if(!isLeftSlotReadyForItem)
            {
                //先重置item状态：
                EventHub.Instance.EventTrigger("ResetItem");

                EventHub.Instance.EventTrigger<bool>("HighLightItemsOrNot", true);

                isLeftSlotReadyForItem = true;
                isRightSlotReadyForItem = false;
            }
            else if(currentSelectedItem != null)
            {
                Debug.Log("左侧装备");
                //如果当前的Slot处在预备态，同时当前已经选中了Item(也就是currentSelectedItem不为空)
                //将当前持有的item的信息同步到leftSlottedItem中；
                leftSlottedItem = Instantiate(currentSelectedItem, leftSlotItemAnchor, false);
                RectTransform rt = leftSlottedItem.GetComponent<RectTransform>();
                rt.anchorMin = new Vector2(0.5f, 0.5f); 
                rt.anchorMax = new Vector2(0.5f, 0.5f); 
                rt.sizeDelta = new Vector2(100, 100);
                rt.anchoredPosition = new Vector2(0, 0);
                //立刻置空：
                currentSelectedItem = null;
            }

        });

        btnQuickSlotRight.onClick.AddListener(()=>{
            Debug.Log("右侧快捷插槽触发");
            if(!isRightSlotReadyForItem)
            {
                //先重置item状态：
                EventHub.Instance.EventTrigger("ResetItem");
                EventHub.Instance.EventTrigger<bool>("HighLightItemsOrNot", true);

                isLeftSlotReadyForItem = false;
                isRightSlotReadyForItem = true;
            }
            else if(currentSelectedItem != null)
            {
                Debug.Log("右侧装备");
                rightSlottedItem = Instantiate(currentSelectedItem, rightSlotItemAnchor, false);
                RectTransform rt = rightSlottedItem.GetComponent<RectTransform>();
                rt.anchorMin = new Vector2(0.5f, 0.5f); 
                rt.anchorMax = new Vector2(0.5f, 0.5f); 
                rt.sizeDelta = new Vector2(100, 100);
                rt.anchoredPosition = new Vector2(0, 0);
                currentSelectedItem = null;
            }
            
        });

        btnSlotLeftOff?.onClick.AddListener(()=>{

        });

        btnSlotRightOff?.onClick.AddListener(()=>{

        });

        btnCancelSlotHighLight.onClick.AddListener(()=>{
            Debug.Log("道具栏高亮取消");
            //先重置item状态：
            EventHub.Instance.EventTrigger("ResetItem");

            EventHub.Instance.EventTrigger<bool>("HighLightItemsOrNot", false);

            //重置两个Slot的状态：
            isLeftSlotReadyForItem = false;
            isRightSlotReadyForItem = false;

        });
    }

    protected override void Awake()
    {
        base.Awake();
        EventHub.Instance.AddEventListener<GameObject>("BroadcastCurrentItem", BroadcastCurrentItem);
    }

    void OnDestroy()
    {
         EventHub.Instance.RemoveEventListener<GameObject>("BroadcastCurrentItem", BroadcastCurrentItem);
    }


    //存在item进入preSelected之后，向InventoryPanel触发的广播方法：
    private void BroadcastCurrentItem(GameObject _currentSelectedItem)
    {
        //规定：如果触发的时候传入的是null，表示的是当前取消了选择；
        currentSelectedItem = _currentSelectedItem;


    }


}
