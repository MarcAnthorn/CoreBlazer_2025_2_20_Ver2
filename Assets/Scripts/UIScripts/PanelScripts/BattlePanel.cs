using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BattlePanel : BasePanel
{

    public Slider sliderEnemyHealth;
    public Slider sliderPlayerHealth;

    public Transform enemyBuffContainer;
    public Transform playerBuffContainer;
    public Image imgEnemy;
    public Button btnPlayerInfo;
    public Button btnInventory;
    public Button btnEndThisRound;
    public TextMeshProUGUI txtLeftCost;
    public TextMeshProUGUI txtMaxCost;
    public int maxCost;
    public int currentCost;

    public EquipmentSlot equipmentSlot1;
    public EquipmentSlot equipmentSlot2;
    public EquipmentSlot equipmentSlot3;
    public EquipmentSlot equipmentSlot4;
    //管理插槽的List：
    public List<EquipmentSlot> equipmentSlotList = new List<EquipmentSlot>();

     protected override void Awake()
     {
          EventHub.Instance.AddEventListener<Equipment>("EquipTarget", EquipTarget);
          EventHub.Instance.AddEventListener<Equipment>("UnequipTarget", UnequipTarget);

          //更新BattlePanel UI的事件注册：     
          EventHub.Instance.AddEventListener("UpdateAllUIElements", UpdateBattlePanelUI);
     }
     protected override void Init()
     {
          //将对应的Slot脚本加入容器：
          equipmentSlotList.Add(equipmentSlot1);
          equipmentSlotList.Add(equipmentSlot2);
          equipmentSlotList.Add(equipmentSlot3);
          equipmentSlotList.Add(equipmentSlot4);

          btnPlayerInfo.onClick.AddListener(()=>{
               //此处展示玩家当前的数值：
          });

          btnInventory.onClick.AddListener(()=>{
               //展示背包面板：
               UIManager.Instance.ShowPanel<InventoryPanel>();
          });

          btnEndThisRound.onClick.AddListener(()=>{
               
               //触发BattleManager中的bool标识，让回合协程继续：
               BattleManager.Instance.isRoundEndTriggered = true;
               
          });
     }

     void OnDestroy()
     {
          EventHub.Instance.RemoveEventListener<Equipment>("EquipTarget", EquipTarget);
          EventHub.Instance.RemoveEventListener<Equipment>("UnequipTarget", UnequipTarget);

          EventHub.Instance.RemoveEventListener("UpdateAllUIElements", UpdateBattlePanelUI);
     }

    //广播方法：将某一个装备装备后调用；
    //注意，后续将这个方法的空闲Slot数量检查、UI面板关闭、遮罩调用全部迁移出去，到EquipmentCheckPanel中；
    //这是因为出现了 允许装上装备但是BattlePanel不在的情况（安全屋）；
     private void EquipTarget(Equipment equipment)
     {
          //找到第一个空闲的Slot：
          foreach(var slotScript in equipmentSlotList)
          {
               if(!slotScript.isSlotted)
               {
                    //将该装备装入Slot：
                    slotScript.InitSlot(equipment);

                    //调用UI更新委托：
                    EventHub.Instance.EventTrigger("UpdateAllUIElements");

//------------已迁移到EquipmentCheckPanel---------------------------------------------------------------
                    //调整该装备的内部字段：
                    // equipment.isEquipped = true;

                    //此处还需调用Equipment的Use方法：
                    //myEquipment.Use();

                    //成功就关闭当前的检查面板：
                    // UIManager.Instance.HidePanel<EquipmentCheckPanel>();

                    //调用装备后的回调；位于EquipmentPanelInventory；
                    //将这部分的逻辑迁移到BattlePanel外部，防止在一些场景中（如安全屋中）
                    //允许装上装备但是BattlePanel不在的情况；
                    // EventHub.Instance.EventTrigger("MaskEquipmentOrNot", true, equipment);
//------------已迁移到EquipmentCheckPanel---------------------------------------------------------------
                    //找到就return：
                    return;
               }
          }
          

          //如果没找到空位，进行弹窗的弹出：
          UIManager.Instance.ShowPanel<WarningPanel>().SetWarningText("精神值接近阈值! 装备数量达上限");
     }

     //广播方法：将某一个装备卸下
     private void UnequipTarget(Equipment equipment)
     {
          //找到匹配的的Slot：
          foreach(var slotScript in equipmentSlotList)
          {
               if(slotScript.myEquipment == equipment)
               {
                    //调用Slot中的卸下方法，更新UI：
                    slotScript.UnequipMyself();

                    //调用UI更新委托：
                    EventHub.Instance.EventTrigger("UpdateAllUIElements");
                    
                   

//------------已迁移到EquipmentCheckPanel---------------------------------------------------------------
                    //调用取消装备后的回调；位于EquipmentPanelInventory；
                    //将这部分的逻辑迁移到BattlePanel外部，防止在一些场景中（如安全屋中）
                    //允许装上装备但是BattlePanel不在的情况；
                    // myEquipment.Unuse();
                    // equipment.isEquipped = false;
                    // UIManager.Instance.HidePanel<EquipmentCheckPanel>();
                    // EventHub.Instance.EventTrigger("MaskEquipmentOrNot", false, equipment);
//------------已迁移到EquipmentCheckPanel---------------------------------------------------------------

                    //找到就return：
                    return;
               }
          }

          //按理说不会找不到的，以防万一：
          Debug.LogWarning("未找到对应可以卸下的装备");
     }


     //BattlePanel的UI更新委托，会加入UpdateAllUIElements中；
     public void UpdateBattlePanelUI()
     {
          //更新Sliders；
          sliderPlayerHealth.value = PlayerManager.Instance.player.HP.value /  PlayerManager.Instance.player.HP.value_limit;

          //敌人的Slider暂时不知道如何更新：

          //可能还有buff的显示更新；
          
          //以及剩余行动点数、最大行动点数的更新：
          // txtLeftCost.text = ;
          // txtMaxCost.text = ;
     }

}
