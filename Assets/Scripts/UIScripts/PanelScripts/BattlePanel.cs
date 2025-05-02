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
    float lastPlayerHealthValue;
    float lastEnemyHealthValue;


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


          //先遍历所有的装备，如果有是装备中的，那么就直接装备就行：
          foreach(var equipment in EquipmentManager.Instance.equipmentList)
          {
               if(equipment.isEquipped)
               {
                    EquipTarget(equipment);
               }
          }


//----------------测试战斗：----------------------------
          TestBattle();
//----------------测试战斗：----------------------------
          
          //初步更新UI：
          UpdateBattlePanelUI();
     }



     private void TestBattle()
     {
          //更新保存的上次血量字段：
          lastPlayerHealthValue = 0;
          lastEnemyHealthValue = 0;

          PlayerManager.Instance.InitPlayer();
          EnemySkill[] enemySkills = new EnemySkill[] { new EnemySkill_1001(), new EnemySkill_1002() };

          Enemy enemy = new Enemy_1001(enemySkills);

          //尝试给敌人加buff：
          //中毒：
          BattleBuff buff = new BattleBuff_1001();
          BattleBuff buff1 = new BattleBuff_1001();



          //易伤：
          BattleBuff buff2 = new BattleBuff_1002();
          BattleBuff buff3 = new BattleBuff_1002();
          
          Enemy[] enemies = new Enemy[] { enemy };

          //开始战斗：
          BattleManager.Instance.BattleInit(PlayerManager.Instance.player, enemies);

          //先加敌人再加buff，不然会BattleManager.Instance.enemies越界：
          //添加buff：
          TurnCounter.Instance.AddEnemyBuff(buff);
          TurnCounter.Instance.AddEnemyBuff(buff1);
          TurnCounter.Instance.AddEnemyBuff(buff2);
          TurnCounter.Instance.AddEnemyBuff(buff3);

          //玩家buff：
          BattleBuff buff4 = new BattleBuff_1001();
          BattleBuff buff5 = new BattleBuff_1001();

          TurnCounter.Instance.AddPlayerBuff(buff4);
          TurnCounter.Instance.AddPlayerBuff(buff5);

          BattleManager.Instance.BattleStart();

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
     //在对应战斗内容（使用技能、回合结算等）结束之后就会调用：
     public void UpdateBattlePanelUI()
     {
          //获取当前的敌人：
          var list = BattleManager.Instance.enemies;
          Enemy enemy = null;
          foreach(var val in list)
          {
               enemy = val;
          }

          //更新Sliders；

          //玩家的Slider：
          if(lastPlayerHealthValue != 0)     //当前不是第一次更新：那么使用上次记录的血量实现平滑减少；
          {
               LeanTween.value(sliderPlayerHealth.gameObject, lastPlayerHealthValue / PlayerManager.Instance.player.HP.value_limit,  PlayerManager.Instance.player.HP.value /  PlayerManager.Instance.player.HP.value_limit, 0.5f)
               .setEase(LeanTweenType.easeInOutQuad)
               .setOnUpdate((float val) => 
               {
                    sliderPlayerHealth.value = val; // 设置血条的实时更新值
               });
          }
          else
               sliderPlayerHealth.value = PlayerManager.Instance.player.HP.value /  PlayerManager.Instance.player.HP.value_limit;

          lastPlayerHealthValue = PlayerManager.Instance.player.HP.value;


          //敌人的Slider：
          if(lastEnemyHealthValue != 0) 
          {
               LeanTween.value(sliderEnemyHealth.gameObject, lastEnemyHealthValue / enemy.HP_limit, enemy.HP / enemy.HP_limit, 0.5f)
               .setEase(LeanTweenType.easeInOutQuad)
               .setOnUpdate((float val) => 
               {
                    sliderEnemyHealth.value = val; // 设置血条的实时更新值
               });
          }
          else
               sliderEnemyHealth.value = enemy.HP / enemy.HP_limit;

          lastEnemyHealthValue = enemy.HP;


          //buff的显示更新；
          foreach(var playerBuff in TurnCounter.Instance.playerBuffs)
          {
               // Debug.LogWarning($"尝试更新我方buff：{playerBuff.name}");


               //此处是我方的buff；
               //如果没有显示过，那么就执行游戏对象的实例化：
               if(!playerBuff.isShownOnUI)
               {
                    Instantiate<GameObject>(Resources.Load<GameObject>("BuffCheckObject"), playerBuffContainer, false);
               }

               //执行更新的广播：位于BuffCheckerLogic中，签名：UpdateBuffUI(BattleBuff targetBuff)
               //对于新实例化的buff对象，和已存在的buff对象，都是通过这个广播实现更新的：
               EventHub.Instance.EventTrigger<BattleBuff>("UpdateBuffUI", playerBuff);

          }

          //敌方buff的更新:
          foreach(var enemyBuff in enemy.buffs)
          {
               // Debug.LogWarning($"尝试更新敌方buff：{enemyBuff.name}");


               //此处是敌方的buff：
               //如果没有显示过，那么就执行游戏对象的实例化：
               if(!enemyBuff.isShownOnUI)
               {
                    Instantiate<GameObject>(Resources.Load<GameObject>("BuffCheckObject"), enemyBuffContainer, false);
               }

               //执行更新的广播：位于BuffCheckerLogic中，签名：UpdateBuffUI(BattleBuff targetBuff)
               //对于新实例化的buff对象，和已存在的buff对象，都是通过这个广播实现更新的：
               EventHub.Instance.EventTrigger<BattleBuff>("UpdateBuffUI", enemyBuff);
          }
          
          //以及剩余行动点数、最大行动点数的更新：
          txtLeftCost.text = BattleManager.Instance.actionPoint.ToString();
          txtMaxCost.text = BattleManager.Instance.actionPointMax.ToString();
     }

}
