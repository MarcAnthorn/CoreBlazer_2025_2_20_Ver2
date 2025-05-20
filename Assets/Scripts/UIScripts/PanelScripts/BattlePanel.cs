using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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

    public Image imgForGero;
    public Button btnPlayerInfo;
    public Button btnInventory;
    public Button btnEndThisRound;
    public TextMeshProUGUI txtLeftCost;
    public TextMeshProUGUI txtMaxCost;
    public int maxCost;
    public int currentCost;
    float lastPlayerHealthValue;
    float lastEnemyHealthValue;


     //伤害Text：
     public TextMeshProUGUI txtEnemyDamage;
     public TextMeshProUGUI txtPlayerDamage;

     

    public EquipmentSlot equipmentSlot1;
    public EquipmentSlot equipmentSlot2;
    public EquipmentSlot equipmentSlot3;
    public EquipmentSlot equipmentSlot4;
    //管理插槽的List：
    public List<EquipmentSlot> equipmentSlotList = new List<EquipmentSlot>();

     //当前处理的敌人：
     private Enemy enemy;
     private int myEnemyId;

     //当前战斗结束之后的回调函数：
     public UnityAction<int> callback;

     //五个遮罩对象：
     //对应id：0:战斗开始；1：我方回合开始 2:我方回合结束 3:敌方回合开始 4：敌方回合结束
     public GameObject maskBattleStart;
     public GameObject maskPlayerTurnStart;
     public GameObject maskPlayerTurnEnd;
     public GameObject maskEnemyTurnStart;
     public GameObject maskEnemyTurnEnd;
     protected override void Awake()
     {
          EventHub.Instance.AddEventListener<Equipment>("EquipTarget", EquipTarget);
          EventHub.Instance.AddEventListener<Equipment>("UnequipTarget", UnequipTarget);

          //更新BattlePanel UI的事件注册：     
          EventHub.Instance.AddEventListener("UpdateAllUIElements", UpdateBattlePanelUI);


          //更新伤害Ui的事件注册：
          EventHub.Instance.AddEventListener<float, bool>("UpdateDamangeText", UpdateDamangeText);
          EventHub.Instance.AddEventListener<int, bool>("UpdateDamangeTextInt", UpdateDamangeTextInt);

          EventHub.Instance.AddEventListener<int>("TriggerBattleMask", TriggerBattleMask);


     }

     private void TriggerBattleMask(int maskId)
     {
          switch (maskId)
          {
               case 0:
                    maskBattleStart.SetActive(true);
                    break;
               case 1:
                    maskPlayerTurnStart.SetActive(true);
                    break;
               case 2:
                    maskPlayerTurnEnd.SetActive(true);
                    break;
               case 3:
                    maskEnemyTurnStart.SetActive(true);
                    break;
               case 4:
                    maskEnemyTurnEnd.SetActive(true);
                    break;
                    
          }
     }
     protected override void Init()
     {
          equipmentSlot1.InitSlot(null);

          int endNumber = myEnemyId % 1000;
          int resultNumber = 10000 + endNumber;
          string path = Path.Combine("ArtResources", "战斗图片", resultNumber.ToString());

          if (myEnemyId != 1013)
               imgEnemy.sprite = Resources.Load<Sprite>(path);


          else
          {
               imgEnemy.gameObject.SetActive(false);
               imgForGero.gameObject.SetActive(true);
               imgForGero.sprite = Resources.Load<Sprite>(path);
          }

          //再冻结一次：
          EventHub.Instance.EventTrigger<bool>("Freeze", true);
          //将对应的Slot脚本加入容器：
          // equipmentSlotList.Add(equipmentSlot1);
          equipmentSlotList.Add(equipmentSlot2);
          equipmentSlotList.Add(equipmentSlot3);
          equipmentSlotList.Add(equipmentSlot4);

          btnPlayerInfo.onClick.AddListener(() =>
          {
               //此处展示玩家当前的数值：
               UIManager.Instance.ShowPanel<PlayerAttributePanel>();
          });

          btnInventory.onClick.AddListener(() =>
          {
               //展示背包面板：
               var panel = UIManager.Instance.ShowPanel<InventoryPanel>();
               panel.SetIfInBattle(true);
          });

          btnEndThisRound.onClick.AddListener(() =>
          {

               Debug.LogWarning("Triggered!");
               //触发BattleManager中的bool标识，让回合协程继续：
               BattleManager.Instance.isRoundEndTriggered = true;

          });


          //----------------测试战斗：----------------------------
          //    TestBattle();
          //----------------测试战斗：----------------------------


          //初始化战斗：
          BeginBattle();


          //先遍历所有的装备，如果有是装备中的，那么就直接装备就行：
          foreach (var equipment in EquipmentManager.Instance.equipmentList)
          {
               if (equipment.isEquipped)
               {
                    EquipTarget(equipment);
               }
          }


          //初步更新UI：
          UpdateBattlePanelUI();
     }

//     //测试用：
//     void Update()
//     {
//         if(Input.GetKeyDown(KeyCode.T))
//         {
//           EventHub.Instance.EventTrigger("GameOver", true, callback);
//         }
//     }



    //测试方法：
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
          // TurnCounter.Instance.AddEnemyBuff(buff);
          // TurnCounter.Instance.AddEnemyBuff(buff1);
          // TurnCounter.Instance.AddEnemyBuff(buff2);
          // TurnCounter.Instance.AddEnemyBuff(buff3);

          //玩家buff：
          BattleBuff buff4 = new BattleBuff_1001();
          BattleBuff buff5 = new BattleBuff_1001();

          // TurnCounter.Instance.AddPlayerBuff(buff4);
          // TurnCounter.Instance.AddPlayerBuff(buff5);

          BattleManager.Instance.BattleStart();

     }

     //初始化战斗方法：
     public void BeginBattle()
     {
          //更新保存的上次血量字段：
          lastPlayerHealthValue = 0;
          lastEnemyHealthValue = 0;

          Enemy[] enemies = new Enemy[] { enemy };

          //开始战斗：
          BattleManager.Instance.BattleInit(PlayerManager.Instance.player, enemies);
          BattleManager.Instance.BattleStart();
     }

     void OnDestroy()
     {
          EventHub.Instance.RemoveEventListener<Equipment>("EquipTarget", EquipTarget);
          EventHub.Instance.RemoveEventListener<Equipment>("UnequipTarget", UnequipTarget);

          EventHub.Instance.RemoveEventListener("UpdateAllUIElements", UpdateBattlePanelUI);

          EventHub.Instance.RemoveEventListener<float, bool>("UpdateDamangeText", UpdateDamangeText);
          EventHub.Instance.RemoveEventListener<int, bool>("UpdateDamangeTextInt", UpdateDamangeTextInt);

          EventHub.Instance.RemoveEventListener<int>("TriggerBattleMask", TriggerBattleMask);
          
          EventHub.Instance.EventTrigger("Freeze", false);
     } 


     //外部调用方法：初始化敌人信息：
     public void InitEnemyInfo(int enemyId)
     {   
          
          Enemy _enemy = LoadManager.Instance.allEnemies[enemyId];

          Debug.LogWarning($"Now encountered enemy is{_enemy.name}, id is{enemyId}");
          
          myEnemyId = enemyId;

          if(_enemy == null)
          {
               Debug.LogWarning("当前获取的敌人是null");
          }

          _enemy.HP = _enemy.HP_limit;  //将血量回满；

          // imgEnemy.sprite = Resources.Load<Sprite>(Path.Combine("Enemy", enemyId.ToString()));
          enemy = _enemy;
     }

    //广播方法：将某一个装备装备后调用；
    //注意，后续将这个方法的空闲Slot数量检查、UI面板关闭、遮罩调用全部迁移出去，到EquipmentCheckPanel中；
    //这是因为出现了 允许装上装备但是BattlePanel不在的情况（安全屋）；
     private void EquipTarget(Equipment equipment)
     {
          Debug.Log($"Try to equip: {equipment.name}");
          //找到第一个空闲的Slot：
          foreach(var slotScript in equipmentSlotList)
          {
               if(!slotScript.isSlotted)
               {
                    Debug.Log($"This is not slotted");
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
          PoolManager.Instance.SpawnFromPool("Panels/WarningPanel", GameObject.Find("Canvas").transform).gameObject.GetComponent<WarningPanel>().SetWarningText("精神值接近阈值! 装备数量达上限");
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
          Debug.Log(BattleManager.Instance.enemies.Count);

          //获取当前的敌人：
          if(BattleManager.Instance.enemies.Count == 1)
               enemy = BattleManager.Instance.enemies[0];


          if(enemy == null)
          {
               Debug.LogWarning("当前BattlePanel未获取到敌人");
          }

          //更新Sliders；

          //玩家的Slider：
          if(lastPlayerHealthValue != 0)     //当前不是第一次更新：那么使用上次记录的血量实现平滑减少；
          {
               Debug.Log($"last player hp is {lastPlayerHealthValue}, now is:{PlayerManager.Instance.player.HP.value}");
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
               Debug.Log($"last enemy hp is {lastEnemyHealthValue}, now is:{enemy.HP}");
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

          if(PlayerManager.Instance.player.HP.value <= 0) //玩家死亡
          {
               //GameOver在BattleManager中；
               Debug.LogWarning("玩家死亡！");
               EventHub.Instance.EventTrigger("GameOver", false, callback);     //callback只有获胜才会调用，内部会进判断的；
          }

          else if(enemy.HP <= 0) //敌人死亡
          {
               Debug.LogWarning("敌人死亡！");
               EventHub.Instance.EventTrigger("GameOver", true, callback);

          }
     }


     //更新伤害面板的事件
     //主要是为了避免短时间多次调用UpdateBattlePanelUI导致UI显示（主要是Slider）出错
     //如果value是-1，说明是被闪避了；第二参数表示是否是我发出的伤害，用于区分敌方和我方；
     private void UpdateDamangeText(float damageValue, bool isPlayersDamage)
     {
          //以-3为flag：如果是-4，那么就是开始之前的惯例UI更新
          if (damageValue < -3)
          {
               if (isPlayersDamage)
               {
                    txtPlayerDamage.color = Color.white;
                    txtPlayerDamage.text = $"尚未造成伤害";
               }

               else
               {
                    txtEnemyDamage.color = Color.white;
                    txtEnemyDamage.text = $"尚未造成伤害";
               }
               return;

          }
          Debug.Log("Damage Text is Updated!");
          if(isPlayersDamage)
          {
               
               if(Math.Abs(damageValue + 1) <= 0.01f){   //闪避,注意浮点型是取绝对值误差判断！
                    txtPlayerDamage.color = Color.red;
                    txtPlayerDamage.text = "你的伤害被闪避了！";
               }
               else
               {
                    txtPlayerDamage.color = Color.white;
                    txtPlayerDamage.text = $"你造成伤害：{damageValue}";
               }
          }

          else 
          {
               if(Math.Abs(damageValue + 1) <= 0.01f){   //闪避,注意浮点型是取绝对值误差判断！
                    txtEnemyDamage.color = Color.red;
                    txtEnemyDamage.text = $"敌方的伤害被闪避了！";
               }
               else
               {
                    txtEnemyDamage.color = Color.white;
                    txtEnemyDamage.text = $"敌方造成伤害：{damageValue}";
               }

          }
     }

     //整型重载：
     private void UpdateDamangeTextInt(int damageValue, bool isPlayersDamage)
     {
          Debug.Log("Damage(int)Text is Updated!");
          if(isPlayersDamage)
          {
               
               if(damageValue == -1){   //闪避
                    txtPlayerDamage.color = Color.red;
                    txtPlayerDamage.text = "你的伤害被闪避了！";
               }
               else
               {
                    txtPlayerDamage.color = Color.white;
                    txtPlayerDamage.text = $"你造成伤害：{damageValue}";
               }
          }

          else 
          {
               if(damageValue == -1){   //闪避
                    txtEnemyDamage.color = Color.red;
                    txtEnemyDamage.text = $"敌方的伤害被闪避了！";
               }
               else
               {
                    txtEnemyDamage.color = Color.white;
                    txtEnemyDamage.text = $"敌方造成伤害：{damageValue}";
               }

          }
     }

}
