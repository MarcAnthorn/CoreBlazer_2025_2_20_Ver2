using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentPanelInventory : BasePanel
{
    public Transform equipmentContainer;
    public List<GameObject> equipmentList = new List<GameObject>();

    public List<InventoryEquipmentLogic> equipmentScriptList = new List<InventoryEquipmentLogic>();

    protected override void Awake()
    {
        base.Awake();

        //更新当前的道具：

        //注册装备 / 取消装备之后的回调事件：
        EventHub.Instance.AddEventListener<bool, Equipment>("MaskEquipmentOrNot", MaskEquipmentOrNot);

    }
    protected override void Init()
    {
        //刷新道具面板：
        RefreshPanel();
    }



    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDestroy()
    {
        EventHub.Instance.RemoveEventListener<bool, Equipment>("MaskEquipmentOrNot", MaskEquipmentOrNot);
    }

    //更新当前的UI中的显示的装备：
    private void RefreshPanel()
    {
        //清空List：
        equipmentList.Clear();

        //从管理器中先将当前持有的装备逐个取出，进行显示：
        foreach(Equipment equipment in EquipmentManager.Instance.equipmentList)
        {
            Debug.Log("装备加载");
            //如果装备耐久归零，直接跳过这个初始化面板的流程，避免被显示；（如果这个归零的装备没有被及时移除的话）
            if(EquipmentManager.Instance.equipmentDurationDic[equipment] == 0 )
                continue;

            //取出一个装备游戏对象，用于当前装备实例的显示：
            GameObject nowEquipment = Instantiate(Resources.Load<GameObject>("TestResources/EquipmentInventory"), equipmentContainer, false);

            InventoryEquipmentLogic script = nowEquipment.GetComponentInChildren<InventoryEquipmentLogic>();
            script.Init(equipment);

            //更新面板的时候，如果equipment已经在使用中，那么加上蒙版：
            if(equipment.isEquipped)
            {
                script.equippedMask.SetActive(true);
            }

            //不知道之后会不会用上实例List，先保留：
            // itemScriptList.Add(script);

            equipmentList.Add(nowEquipment);
            equipmentScriptList.Add(script);
        }
    }


    //当装备之后，调用的回调函数
    //用于对指定的装备的进行UI上的更新：
    private void MaskEquipmentOrNot(bool isTargetMasked, Equipment targetEquipment)
    {
        //遍历当前容器的所有装备
        //如果发现是目标容器，对其的遮罩进行添加 / 去除 的操作：
        foreach(var script in equipmentScriptList)
        {
            if(script.myEquipment == targetEquipment)
            {
                //对其进行遮罩操作：
                script.equippedMask.gameObject.SetActive(isTargetMasked);
            }
        }
    }
}
