using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System.Linq;

public class InventoryPanel : BasePanel
{
    //是否是在战斗中打开的背包；是的话，那关闭背包不会导致解冻：
    public bool isInBattle;
    public Image imgPlayer;
    public Slider sliderHealth;
    public Slider sliderSanity;
    public Slider sliderLight;
    public Button btnGodPanelReveal;
    public Button btnCommonPanelReveal;
    public Button btnEquipmentPanelReveal;
    public Button btnQuickSlotLeft;
    public Button btnQuickSlotRight;
    public Button btnSlotLeftOff;
    public Button btnSlotRightOff;
    //用于取消插槽提示的Button：
    public Button btnCancelSlotHighLight;
    public Button btnExit;
    public Button btnSetting;
    public TextMeshProUGUI txtStrength;
    public TextMeshProUGUI txtSpeed;

    public TextMeshProUGUI txtDefense;

    public TextMeshProUGUI txtCriticalRate;

    public TextMeshProUGUI txtComboRate;

    public TextMeshProUGUI txtDodgeRate;

    public TextMeshProUGUI txtCriticalMultiplier;
    public TextMeshProUGUI txtHealth;
    public TextMeshProUGUI txtSan;
    public TextMeshProUGUI txtLight;
    

    public Transform tarotContent;
    public Transform leftSlotItemAnchor;
    public Transform rightSlotItemAnchor;
    public Transform itemPanelContainer;
    public List<GameObject> tarotList = new List<GameObject>();

    //存储三个Button Image组建的List:
    //0:装备
    //1:神明道具
    //2:普通道具
    private List<Image> buttonImageList = new List<Image>();
    private Color buttonDarkenColor = new Color(0.5f, 0.5f, 0.5f, 1f); // 变暗

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

    //当前插入插槽的两个Item (注意是GodItemPanel中的)；
    private GameObject leftSlottedOriginalItem;
    private GameObject rightSlottedOriginalItem;

    //当前插入插槽的两个Item (注意是InventoryPanel中的)；

    private GameObject leftSlottedInventoryItem;
    private GameObject rightSlottedInventoryItem;
    private InventoryItemLogic leftItemScript;
    private InventoryItemLogic rightItemScript;
    public GameObject equiptmentPanelObject;
    public GameObject godItemPanelObject;
    public GameObject commonItemPanelObject;



    



    protected override void Init()
    {
        EventHub.Instance.EventTrigger<bool>("Freeze", true);
        UpdateAttributeText();

        
        //初始化的时候，按照PlayerManager中的playerSceneIndex进行选择性显示UI：
        //如果是当前是战斗中，那么就显示装备背包栏
        // if(PlayerManager.Instance.playerSceneIndex == E_PlayerSceneIndex.Battle || PlayerManager.Instance.playerSceneIndex == E_PlayerSceneIndex.Shelter)
        // {
        //     equiptmentPanelObject.SetActive(true);
        // }

        godItemPanelObject.SetActive(true);

        buttonImageList.Add(btnEquipmentPanelReveal.GetComponent<Image>());
        buttonImageList.Add(btnGodPanelReveal.GetComponent<Image>());
        buttonImageList.Add(btnCommonPanelReveal.GetComponent<Image>());

        buttonImageList[1].color = buttonDarkenColor;

        
        //不然就是commonItemPanelObject；同时禁用让equiptmentPanelObject显示的Button：
        // else
        // {
        //     commonItemPanelObject.SetActive(true);
        //     btnEquipmentPanelReveal.gameObject.SetActive(false);
        // }


        btnExit.onClick.AddListener(()=>{
            UIManager.Instance.HidePanel<InventoryPanel>();
        });

        btnSetting.onClick.AddListener(()=>{
            UIManager.Instance.ShowPanel<SettingPanel>();
        });

        btnCommonPanelReveal.onClick.AddListener(()=>{
            commonItemPanelObject.SetActive(true);
            godItemPanelObject.SetActive(false);
            equiptmentPanelObject.SetActive(false);

            // 让该按钮变暗，其他两个恢复正常
            buttonImageList[0].color = Color.white;
            buttonImageList[1].color = Color.white;
            buttonImageList[2].color = buttonDarkenColor;

            ResetCurrentSelectedItem();
        });

        btnGodPanelReveal.onClick.AddListener(()=>{
            godItemPanelObject.SetActive(true);
            commonItemPanelObject.SetActive(false);
            equiptmentPanelObject.SetActive(false);

            // 让该按钮变暗，其他两个恢复正常
            buttonImageList[0].color = Color.white;
            buttonImageList[1].color = buttonDarkenColor;
            buttonImageList[2].color = Color.white;

            ResetCurrentSelectedItem();
        });

        btnEquipmentPanelReveal.onClick.AddListener(()=>{
            equiptmentPanelObject.SetActive(true);
            godItemPanelObject.SetActive(false);
            commonItemPanelObject.SetActive(false);

            // 让该按钮变暗，其他两个恢复正常
            buttonImageList[0].color = buttonDarkenColor;
            buttonImageList[1].color = Color.white;
            buttonImageList[2].color = Color.white;

            ResetCurrentSelectedItem();
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
                SlotItemToLeft(currentSelectedItem, currentSelectedItem.GetComponentInChildren<InventoryItemLogic>().myItem.name);
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
                SlotItemToRight(currentSelectedItem, currentSelectedItem.GetComponentInChildren<InventoryItemLogic>().myItem.name);
            }
            
        });

        btnSlotLeftOff?.onClick.AddListener(()=>{
            Destroy(leftSlottedInventoryItem);
            leftSlottedOriginalItem = null;
            isLeftSlotReadyForItem = false;

            //同时将对应的Item高光取消：
            Debug.LogWarning("你正在尝试将 isSelectedToSlot 置false");
            leftItemScript.isSelectedToSlot = false;
            leftItemScript.myItem.isSlottedToLeft = false;

            btnSlotLeftOff.gameObject.SetActive(false);


        });

        btnSlotRightOff?.onClick.AddListener(()=>{
            Destroy(rightSlottedInventoryItem);
            rightSlottedOriginalItem = null;
            isRightSlotReadyForItem = false;

            Debug.LogWarning("你正在尝试将 isSelectedToSlot 置false");
            rightItemScript.isSelectedToSlot = false;
            rightItemScript.myItem.isSlottedToRight = false;
            btnSlotRightOff.gameObject.SetActive(false);
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
        EventHub.Instance.AddEventListener<int>("RemoveSlotItem", RemoveSlotItem);
        EventHub.Instance.AddEventListener<GameObject>("BroadcastCurrentItem", BroadcastCurrentItem);
        EventHub.Instance.AddEventListener<GameObject, string>("SlotItemToLeft", SlotItemToLeft);
        EventHub.Instance.AddEventListener<GameObject, string>("SlotItemToRight", SlotItemToRight);
        EventHub.Instance.AddEventListener("ResetCurrentSelectedItem", ResetCurrentSelectedItem);


        //这是一个多播委托：存在任何对玩家属性做出调整的地方，都需要调用这个委托；
        EventHub.Instance.AddEventListener("UpdateAllUIElements", UpdateAttributeText);
    }

    private void Update() {
        
    }


    void OnDestroy()
    {
        EventHub.Instance.RemoveEventListener<int>("RemoveSlotItem", RemoveSlotItem);
        EventHub.Instance.RemoveEventListener<GameObject>("BroadcastCurrentItem", BroadcastCurrentItem);
        EventHub.Instance.RemoveEventListener<GameObject, string>("SlotItemToLeft", SlotItemToLeft);
        EventHub.Instance.RemoveEventListener<GameObject, string>("SlotItemToRight", SlotItemToRight);
        EventHub.Instance.RemoveEventListener("UpdateAllUIElements", UpdateAttributeText);
        EventHub.Instance.RemoveEventListener("ResetCurrentSelectedItem", ResetCurrentSelectedItem);

        ResetCurrentSelectedItem();

        //只有不在战斗中，才会解冻：
        if(!isInBattle){
            EventHub.Instance.EventTrigger<bool>("Freeze", false);
        }
    }

    //设置是否在战斗中：
    public void SetIfInBattle(bool _isInBattle)
    {
        isInBattle = _isInBattle;
    }


    //存在item进入preSelected之后，向InventoryPanel触发的广播方法：
    private void BroadcastCurrentItem(GameObject _currentSelectedItem)
    {
        //规定：如果触发的时候传入的是null，表示的是当前取消了选择；
        currentSelectedItem = _currentSelectedItem;
    }

    private void ResetCurrentSelectedItem()
    {
        currentSelectedItem = null;
        EventHub.Instance.EventTrigger("ResetItem");
        isLeftSlotReadyForItem = false;
        isRightSlotReadyForItem = false;
    }

    //移除左侧 / 右侧插槽Item的方法：
    private void RemoveSlotItem(int itemId)
    {
        Debug.LogWarning($"尝试删除 Slot Item, id：{itemId}");
        //这两处基本就是直接对点击「卸下」之后的逻辑复用；
        if(leftItemScript.myItemId == itemId)
        {
            leftSlottedOriginalItem = null;
            isLeftSlotReadyForItem = false;
            leftItemScript.isSelectedToSlot = false;
            leftItemScript.myItem.isSlottedToLeft = false;

            btnSlotLeftOff.gameObject.SetActive(false);
            Destroy(leftSlottedInventoryItem);
            
            
        }
        else if(rightItemScript.myItemId == itemId)
        {
            btnSlotRightOff.gameObject.SetActive(false);
            rightSlottedOriginalItem = null;
            isRightSlotReadyForItem = false; 
            rightItemScript.isSelectedToSlot = false;
            rightItemScript.myItem.isSlottedToRight = false;
            btnSlotRightOff.gameObject.SetActive(false);
            Destroy(rightSlottedInventoryItem);
        }
        else
        {
            Debug.LogWarning("当前尝试移除的SlotItem不存在");
        }
    }

    //将指定的Item复制之后，插入Slot：
    private void SlotItemToLeft(GameObject item, string itemName)
    {
        InventoryItemLogic currentScript =  item.GetComponentInChildren<InventoryItemLogic>();
        leftItemScript = currentScript;

        //如果当前的道具是神明道具，那么不可使用插槽：
        if(currentScript.myItem.type == Item.ItemType.God_Battle || currentScript.myItem.type == Item.ItemType.God_Maze)
        {
            UIManager.Instance.ShowPanel<WarningPanel>().SetWarningText("不可选择神明道具到快捷插槽中");
            EventHub.Instance.EventTrigger("ResetItem");

            //需要重新点击Slot才能再次尝试插入：
            isLeftSlotReadyForItem = false;
            return;
        }

        if(currentScript.isSelectedToSlot)
        {
            //如果currentSelectedItem是null，那么说明不是选中之后尝试重复插入，而是初始化面板的时候尝试重复更新；
            //直接return就行，避免弹出WarningPanel；
            if(currentSelectedItem == null)
                return;

            UIManager.Instance.ShowPanel<WarningPanel>().SetWarningText("不可重复选择道具到快捷插槽中");
            EventHub.Instance.EventTrigger("ResetItem");

            //需要重新点击Slot才能再次尝试插入：
            isLeftSlotReadyForItem = false;
            return;
        }
        else if(!currentScript.myItem.quickEquip)
        {
            UIManager.Instance.ShowPanel<WarningPanel>().SetWarningText($"道具「{itemName}」不可插入快捷插槽");
            EventHub.Instance.EventTrigger("ResetItem");

            //需要重新点击Slot才能再次尝试插入：
            isLeftSlotReadyForItem = false;
            return;
        }
        Debug.Log("左侧装备");
        //如果当前的Slot处在预备态，同时当前已经选中了Item(也就是currentSelectedItem不为空)
        //让leftSlottedItem指向该Item，使其被记录下来，用于之后需要的同步操作；
        leftSlottedOriginalItem = item;
        leftSlottedInventoryItem = Instantiate(item, leftSlotItemAnchor, false);

        //为了防止Awake中的随机id覆盖了Instantiate出来的Item的正确的id而设置的callbackId；
        //之后肯定不是在Awake中随机生成的，所以这段只是暂时的逻辑；
        InventoryItemLogic script =  leftSlottedInventoryItem.GetComponentInChildren<InventoryItemLogic>();

        //如果本体处在使用中，那么该复制品也需要加上蒙版：
        if(currentScript.myItem.isInUse)
        {
            script.takeEffectMaskObject.SetActive(true);
        }
        //初始化它的Item信息：
        script.Init(currentScript.myItem);
        script.isItemInSlot = true;


        RectTransform rt = leftSlottedInventoryItem.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f); 
        rt.anchorMax = new Vector2(0.5f, 0.5f); 
        rt.sizeDelta = new Vector2(100, 100);
        rt.anchoredPosition = new Vector2(0, 0);

        //处理GodItemPanel中的当前被装载道具的逻辑
        //它需要持续高亮，以及快捷道具的使用等等需要数据同步（相当于就是绑定）
        leftItemScript.isSelectedToSlot = true;

        //这个是对Item本身的字段调整，用于下次面板显示的时候，快捷插槽可以重新正确显示对应的Item
        leftItemScript.myItem.isSlottedToLeft = true;


        //成功插入之后，触发ResetItem；因为相当于是点了一次取消Button；
        EventHub.Instance.EventTrigger("ResetItem");
        //结束置空，不然进不去右侧RightSlot的逻辑：
        item = null;

        //装载之后，将「卸下」Button恢复；
        btnSlotLeftOff.gameObject.SetActive(true);
    }

    private void SlotItemToRight(GameObject item, string itemName)
    {
        InventoryItemLogic currentScript =  item.GetComponentInChildren<InventoryItemLogic>();
        rightItemScript = currentScript;

        //如果当前的道具是神明道具，那么不可使用插槽：
        if(currentScript.myItem.type == Item.ItemType.God_Battle || currentScript.myItem.type == Item.ItemType.God_Maze)
        {
            UIManager.Instance.ShowPanel<WarningPanel>().SetWarningText("不可选择神明道具到快捷插槽中");
            EventHub.Instance.EventTrigger("ResetItem");

            //需要重新点击Slot才能再次尝试插入：
            isRightSlotReadyForItem = false;
            return;
        }
        if(currentScript.isSelectedToSlot)
        {
            if(currentSelectedItem == null)
                return;

            UIManager.Instance.ShowPanel<WarningPanel>().SetWarningText("不可重复选择道具到快捷插槽中");
            EventHub.Instance.EventTrigger("ResetItem");

            //需要重新点击Slot才能再次尝试插入：
            isRightSlotReadyForItem = false;
            return;
        }
        else if(!currentScript.myItem.quickEquip)
        {
            UIManager.Instance.ShowPanel<WarningPanel>().SetWarningText($"道具「{itemName}」不可插入快捷插槽");
            EventHub.Instance.EventTrigger("ResetItem");

            //需要重新点击Slot才能再次尝试插入：
            isRightSlotReadyForItem = false;
            return;
        }
        Debug.Log("右侧装备");
        rightSlottedOriginalItem = item;

        rightSlottedInventoryItem = Instantiate(item, rightSlotItemAnchor, false);



        InventoryItemLogic script = rightSlottedInventoryItem.GetComponentInChildren<InventoryItemLogic>();

        //如果本体处在使用中，那么该复制品也需要加上蒙版：
        if(currentScript.myItem.isInUse)
        {
            script.takeEffectMaskObject.SetActive(true);
        }
        
        script.Init(currentScript.myItem);

        script.isItemInSlot = true;




        RectTransform rt = rightSlottedInventoryItem.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f); 
        rt.anchorMax = new Vector2(0.5f, 0.5f); 
        rt.sizeDelta = new Vector2(100, 100);
        rt.anchoredPosition = new Vector2(0, 0);

        rightItemScript = currentScript;
        rightItemScript.isSelectedToSlot = true;
        rightItemScript.myItem.isSlottedToRight = true;

        EventHub.Instance.EventTrigger("ResetItem");

        btnSlotRightOff.gameObject.SetActive(true);
                    
        item = null;
    }

    private void UpdateAttributeText()
    {
        txtStrength.text = $"力量：{(int)PlayerManager.Instance.player.STR.value}";
        txtSpeed.text = $"速度：{(int)PlayerManager.Instance.player.SPD.value}";

        txtDefense.text = $"防御：{(int)PlayerManager.Instance.player.DEF.value}";

        txtCriticalRate.text = $"暴击率：{(int)PlayerManager.Instance.player.CRIT_Rate.value}";

        txtComboRate.text = $"连击率：{(int)PlayerManager.Instance.player.HIT.value}";

        txtDodgeRate.text = $"闪避率：{(int)PlayerManager.Instance.player.AVO.value}";

        txtCriticalMultiplier.text = $"暴击伤害：{(int)PlayerManager.Instance.player.CRIT_DMG.value}"; 

        //更新Sliders：
        sliderHealth.value = PlayerManager.Instance.player.HP.value / PlayerManager.Instance.player.HP.value_limit;
        sliderLight.value = PlayerManager.Instance.player.LVL.value / PlayerManager.Instance.player.LVL.value_limit;
        sliderSanity.value = PlayerManager.Instance.player.SAN.value / PlayerManager.Instance.player.SAN.value_limit;

        txtHealth.text = $"{(int)PlayerManager.Instance.player.HP.value} / {(int)PlayerManager.Instance.player.HP.value_limit}";
        txtLight.text = $"{(int)PlayerManager.Instance.player.LVL.value} / {(int)PlayerManager.Instance.player.LVL.value_limit}";
        txtSan.text = $"{(int)PlayerManager.Instance.player.SAN.value} / {(int)PlayerManager.Instance.player.SAN.value_limit}";;


    }



}
