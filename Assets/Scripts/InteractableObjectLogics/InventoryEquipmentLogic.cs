using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryEquipmentLogic : MonoBehaviour
{
    public GameObject equippedMask;
    public Image imgEquipment;
    public TextMeshProUGUI txtDurationCount;
    public TextMeshProUGUI txtEquipmentName;
    public Button btnSelf;

    //当前持有的装备是：
    public Equipment myEquipment;
    // Start is called before the first frame update
    void Awake()
    {
        //此处的事件注册好像和 EquipmentPanelInventory 中的 MaskEquipmentOrNot 逻辑重复了；
        // EventHub.Instance.AddEventListener<Equipment>("EquipmentUsedCallback", EquipmentUsedCallback);
        EventHub.Instance.AddEventListener<Equipment>("UpdateEquipmentUI", UpdateEquipmentUI);
        
    }
    void Start()
    {
        btnSelf.onClick.AddListener(()=>{
            UIManager.Instance.ShowPanel<EquipmentCheckPanel>().InitInfo(myEquipment);
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDestroy()
    {
        // EventHub.Instance.RemoveEventListener<Equipment>("EquipmentUsedCallback", EquipmentUsedCallback);
        EventHub.Instance.RemoveEventListener<Equipment>("UpdateEquipmentUI", UpdateEquipmentUI);
    }

    //初始化当前的背包装备内容的方法，供外部使用：
    public void Init(Equipment _equipment)
    {
        myEquipment = _equipment;
        imgEquipment.sprite = Resources.Load<Sprite>($"ArtResources/Equipment/{myEquipment.id}");

        Debug.Log($"Equipment path is {$"ArtResources/Equipment/{myEquipment.id}"}");
        txtDurationCount.text = $"{myEquipment.currentDuration}/{myEquipment.maxDuration}";
        txtEquipmentName.text = myEquipment.name;
    }

    //事件：装备使用后的响应事件；
    //当一个道具使用之后，会进行该事件的调用；传入的参数是对应装备的实例：
    //如果我的实例符合，那么我就会执行逻辑: 这里就是执行激活「装备中」蒙版；表示道具生效中；
    // private void EquipmentUsedCallback(Equipment target)
    // {
    //     // 目前广播的目标和我的实例是一样的；
    //     if(target == myEquipment)
    //     {
    //         //和我对应上了，说明是广播给我的；
    //         //那么我就执行这个逻辑：
    //         equippedMask.SetActive(true);

    //     }

    //     //更新玩家的属性面板：
    //     EventHub.Instance.EventTrigger("UpdateAttributeText");
    // }

    //更新UI的事件：
    private void UpdateEquipmentUI(Equipment target)
    {
        if(target == myEquipment)
        {
            //主要就是更新耐久的UI：
            txtDurationCount.text = $"{myEquipment.currentDuration}/{myEquipment.maxDuration}";
        }
    }
}
