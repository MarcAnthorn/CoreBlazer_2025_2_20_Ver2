using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BattlePanel : BasePanel
{
    // 叠加的回合结束发光特效
    private GameObject endTurnGlowEffect;
    private Coroutine glowCoroutine;

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


     // //伤害Text：
     // public TextMeshProUGUI txtEnemyDamage;
     // public TextMeshProUGUI txtPlayerDamage;
     
     //玩家受伤Content:
     // public Transform playerHurtContent;
     // public Transform enemyHurtContent;
     //更改成：两个pivot，按照pivot的随机偏移放置伤害数字：
     public Transform playerHurtFather;
     public Transform enemyHurtFather;
     private Vector2 xOffset = new Vector2(60, 0);
     private Coroutine damageCoroutine;
     

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

     //闪避触发TMP：
     public GameObject playerMissTextObject;
     public GameObject enemyMissTextObject;

     public GameObject maskTriggerButton;
     public TMP_FontAsset defaultFont;

     // 预先缓存路径
     private static readonly string DamageNumberCritPrefix = Path.Combine("ArtResources", "DamageNumber", "DamageNumberCrit");

     private static readonly string DamageNumberCritPrefixExtra = Path.Combine("ArtResources", "DamageNumber", "DamageCritMark");
     private static readonly string DamageNumberPrefix = Path.Combine("ArtResources", "DamageNumber", "DamageNumber");
     private void SetAllTextFont()
     {
     foreach (var tmp in GetComponentsInChildren<TextMeshProUGUI>(true))
     {
          tmp.font = defaultFont;
     }
     }
     protected override void Awake()
     {


          EventHub.Instance.AddEventListener<Equipment>("EquipTarget", EquipTarget);
          EventHub.Instance.AddEventListener<Equipment>("UnequipTarget", UnequipTarget);

          //更新BattlePanel UI的事件注册：     
          EventHub.Instance.AddEventListener("UpdateAllUIElements", UpdateBattlePanelUI);
          EventHub.Instance.AddEventListener("UpdateSliders", UpdateSliders);

          EventHub.Instance.AddEventListener<bool>("MaskPlayerTriggerOrNot", MaskPlayerTriggerOrNot);

          


          //更新伤害Ui的事件注册：
          EventHub.Instance.AddEventListener<Damage, int>("ParseDamage", ParseDamage);
          EventHub.Instance.AddEventListener<int>("TriggerBattleMask", TriggerBattleMask);

          //将对应的Slot脚本加入容器：
          // equipmentSlotList.Add(equipmentSlot1);
          equipmentSlotList.Add(equipmentSlot2);
          equipmentSlotList.Add(equipmentSlot3);
          equipmentSlotList.Add(equipmentSlot4);


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

     private void MaskPlayerTriggerOrNot(bool isMasked)
     {
          maskTriggerButton.SetActive(isMasked);
     }
     protected override void Init()
     {
         // 创建回合结束发光特效对象（初始隐藏）
         if (endTurnGlowEffect == null)
         {
             var glowSprite = Resources.Load<Sprite>("ArtResources/战斗_UI/技能面板/回合结束发光");
             endTurnGlowEffect = new GameObject("EndTurnGlowEffect");
             var img = endTurnGlowEffect.AddComponent<Image>();
             img.sprite = glowSprite;
             img.raycastTarget = false;
             // 设置为按钮的第0层子物体，确保在所有子物体（包括文字）下方
             endTurnGlowEffect.transform.SetParent(btnEndThisRound.transform, false);
             endTurnGlowEffect.transform.SetSiblingIndex(0);
             // 拉伸到按钮大小
             var rect = endTurnGlowEffect.GetComponent<RectTransform>();
             var btnRect = btnEndThisRound.GetComponent<RectTransform>();
             rect.anchorMin = Vector2.zero;
             rect.anchorMax = Vector2.one;
             rect.offsetMin = Vector2.zero;
             rect.offsetMax = Vector2.zero;
             endTurnGlowEffect.SetActive(false);
         }
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

          //先遍历所有的装备，如果有是装备中的，那么就直接装备就行：
          foreach (var equipment in EquipmentManager.Instance.equipmentList)
          {
               if (equipment.isEquipped)
               {
                    EquipTarget(equipment);
               }
          }

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
               Debug.Log("Triggered!");
               //触发BattleManager中的bool标识，让回合协程继续：
               BattleManager.Instance.isRoundEndTriggered = true;
               // 手动点击后隐藏特效
               if (endTurnGlowEffect != null)
                   endTurnGlowEffect.SetActive(false);
               if (glowCoroutine != null)
               {
                   StopCoroutine(glowCoroutine);
                   glowCoroutine = null;
               }
          });


          //----------------测试战斗：----------------------------
          //    TestBattle();
          //----------------测试战斗：----------------------------
     }


     //初始化战斗方法：
     public void BeginBattle()
     {
          //更新保存的上次血量字段：
          lastPlayerHealthValue = 0;
          lastEnemyHealthValue = 0;

          //开始战斗：
          BattleManager.Instance.BattleInit(PlayerManager.Instance.player, enemy);
          BattleManager.Instance.BattleStart();
     }

     void OnDestroy()
     {
          EventHub.Instance.RemoveEventListener<Equipment>("EquipTarget", EquipTarget);
          EventHub.Instance.RemoveEventListener<Equipment>("UnequipTarget", UnequipTarget);

          EventHub.Instance.RemoveEventListener("UpdateAllUIElements", UpdateBattlePanelUI);
          EventHub.Instance.RemoveEventListener("UpdateSliders", UpdateSliders);

          EventHub.Instance.RemoveEventListener<Damage, int>("ParseDamage", ParseDamage);
   

          EventHub.Instance.RemoveEventListener<int>("TriggerBattleMask", TriggerBattleMask);

          EventHub.Instance.RemoveEventListener<bool>("MaskPlayerTriggerOrNot", MaskPlayerTriggerOrNot);
          
          // 停止所有与BattlePanel相关的Tween
          LeanTween.cancel(gameObject); // 或 LeanTween.cancelAll();

          // 停止伤害数字的协程
          if (damageCoroutine != null)
          {
               StopCoroutine(damageCoroutine);
               damageCoroutine = null;
          }

          EventHub.Instance.EventTrigger("Freeze", false);
     }


     //外部调用方法：初始化敌人信息：
     public void InitEnemyInfo(int enemyId)
     {
          enemy = LoadManager.Instance.allEnemies[enemyId];

          Debug.LogWarning($"Now encountered enemy is{enemy.name}, id is{enemyId}");

          myEnemyId = enemyId;

          enemy.HP = enemy.HP_limit;  //将血量回满；
          
          //初始化战斗：
          BeginBattle();

          //初步更新UI：
          UpdateBattlePanelUI();
     }



     //广播方法：将某一个装备装备后调用；
     //注意，后续将这个方法的空闲Slot数量检查、UI面板关闭、遮罩调用全部迁移出去，到EquipmentCheckPanel中；
     //这是因为出现了 允许装上装备但是BattlePanel不在的情况（安全屋）；
     private void EquipTarget(Equipment equipment)
     {
          Debug.Log($"Try to equip: {equipment.name}");
          //找到第一个空闲的Slot：
          foreach (var slotScript in equipmentSlotList)
          {
               if (!slotScript.isSlotted)
               {
                    Debug.LogWarning($"This is not slotted");
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
          Debug.LogWarning($"当前free slot数量：{EquipmentManager.Instance.NowLeftSlotsCount()}");
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
          Debug.Log(BattleManager.Instance.battleEnemy);
          //buff的显示更新；
          foreach(var playerBuff in TurnCounter.Instance.playerBuffs)
          {
               if(!playerBuff.isShownOnUI)
                    Instantiate<GameObject>(Resources.Load<GameObject>("BuffCheckObject"), playerBuffContainer, false);
               EventHub.Instance.EventTrigger<BattleBuff>("UpdateBuffUI", playerBuff);
          }
          foreach(var enemyBuff in enemy.buffs)
          {
               if(!enemyBuff.isShownOnUI)
                    Instantiate<GameObject>(Resources.Load<GameObject>("BuffCheckObject"), enemyBuffContainer, false);
               EventHub.Instance.EventTrigger<BattleBuff>("UpdateBuffUI", enemyBuff);
          }
          //以及剩余行动点数、最大行动点数的更新：
          txtLeftCost.text = BattleManager.Instance.actionPoint.ToString();
          Debug.Log($"当前剩余行动点数：{BattleManager.Instance.actionPoint}");
          txtMaxCost.text = BattleManager.Instance.actionPointMax.ToString();
          Debug.Log($"当前最大行动点数：{BattleManager.Instance.actionPointMax}");

          // 行动点数归零时显示发光特效
          if (BattleManager.Instance.actionPoint == 0)
          {
               if (endTurnGlowEffect != null && !endTurnGlowEffect.activeSelf)
               {
                    endTurnGlowEffect.SetActive(true);
                    if (glowCoroutine != null)
                    {
                        StopCoroutine(glowCoroutine);
                        glowCoroutine = null;
                    }
                    glowCoroutine = StartCoroutine(GlowBreathCoroutine());
               }
          }
          else
          {
               if (endTurnGlowEffect != null && endTurnGlowEffect.activeSelf)
               {
                    endTurnGlowEffect.SetActive(false);
                    if (glowCoroutine != null)
                    {
                        StopCoroutine(glowCoroutine);
                        glowCoroutine = null;
                    }
               }
          }

          if (PlayerManager.Instance.player.HP.value <= 0) //玩家死亡
          {
               Debug.LogWarning("玩家死亡！");
               EventHub.Instance.EventTrigger("GameOver", false, callback);
          }
          else if (enemy.HP <= 0) //敌人死亡
          {
               Debug.LogWarning("敌人死亡！");
               EventHub.Instance.EventTrigger("GameOver", true, callback);
          }
     }

     // 呼吸闪烁协程
     private IEnumerator GlowBreathCoroutine()
     {
         var img = endTurnGlowEffect.GetComponent<Image>();
         float t = 0f;
         while (endTurnGlowEffect.activeSelf)
         {
             t += Time.deltaTime;
             float alpha = 0.5f + 0.5f * Mathf.Sin(t * 2.5f); // 0~1之间呼吸
             var c = img.color;
             c.a = alpha;
             img.color = c;
             yield return null;
         }
     }

     //更新Slider的方法
     private void UpdateSliders()
     {

          //获取当前的敌人：
          if(BattleManager.Instance.battleEnemy != null)
               enemy = BattleManager.Instance.battleEnemy;


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


     }


     //解析伤害的方法；
     //注意区分：0表示是玩家受伤；1表示是敌人受伤：
     private void ParseDamage(Damage damage, int flag)
     {

          bool isMiss = damage.isAvoided;
          bool isCombo = damage.isCombo;
          //如果连击，伤害数量：
          int damageCount = damage.GetSize();

          Transform damageTextFather;
          GameObject missTextObject;

          if(flag == 0)
          {
               //玩家受伤
               damageTextFather = playerHurtFather;
               missTextObject = playerMissTextObject;

          }
          else
          {
               //敌人受伤
               damageTextFather = enemyHurtFather;
               missTextObject = enemyMissTextObject;
          }

          //如果闪避：
          if(isMiss)
          {
               // 假设你有唯一的missTextObject
               int missTweenId = -1;
               if (missTweenId != -1)
                    LeanTween.cancel(missTweenId);

               missTextObject.SetActive(true);
               TextMeshProUGUI tmp = missTextObject.GetComponent<TextMeshProUGUI>();
               if (tmp != null)
               {
                    Color c = tmp.color;
                    c.a = 1f;
                    tmp.color = c;

                    missTweenId = LeanTween.value(missTextObject, 1f, 0f, 1f)
                         .setOnUpdate((float val) =>
                         {
                              if (tmp == null || !tmp.gameObject.activeInHierarchy) return;
                              Color cc = tmp.color;
                              cc.a = val;
                              tmp.color = cc;
                         })
                         .setOnComplete(() =>
                         {
                              if (tmp == null || !tmp.gameObject.activeInHierarchy) return;
                              tmp.gameObject.SetActive(false);
                              missTweenId = -1;
                         }).id;
               }

               return;
          }
          
          //如果连击，使用协程来处理延迟显示：
          if(damageCount >= 1)
          {
               if(damageCoroutine != null)
                    StopCoroutine(damageCoroutine);

               damageCoroutine =  StartCoroutine(ShowComboDamageWithDelay(damage, damageTextFather));
          }



     }

     // 协程：显示连击伤害，每次伤害之间有延迟
     private IEnumerator ShowComboDamageWithDelay(Damage damage, Transform damageTextFather)
     {
          int damageCount = damage.GetSize();
          
          for(int i = 0; i < damageCount; i++)
          {
               // 每次伤害都设定偏移范围
               float offsetRange = 40f; // 可根据需要调整
               Vector3 randomOffset = new Vector3(
                    UnityEngine.Random.Range(-offsetRange, offsetRange),
                    UnityEngine.Random.Range(-offsetRange, offsetRange),
                    0
               );

               //位数统计：
               int digitCount = 0;

               StringBuilder sb = new StringBuilder();
               int damageValue = (int)damage[i].damage;

               int tempValue = damageValue;
               int divisor = 1;

               while (tempValue / divisor >= 10)
               {
                    divisor *= 10;
               }
               while (divisor > 0)
               {
                    sb.Clear();
                    if(damage[i].isCritical)
                         sb.Append(DamageNumberCritPrefix);
                    else
                         sb.Append(DamageNumberPrefix);

                    int digit = tempValue / divisor;
                    sb.Append(digit.ToString());

                    Debug.LogWarning($"now path is {sb.ToString()}");
                    //获取这个sb对应的字体资源，加入显示中：
                    GameObject textObject = PoolManager.Instance.SpawnFromPool(sb.ToString(), damageTextFather);

                    //设置随机的偏移：
                    RectTransform textRect = textObject.GetComponent<RectTransform>();
                    if (textRect != null)
                    {
                         textRect.localScale = Vector3.one; // 重置缩放，防止"越用越小"
                         textRect.anchoredPosition = Vector2.zero; // 先归零，再加偏移
                         textRect.anchoredPosition += (Vector2)randomOffset;
                         textRect.anchoredPosition += digitCount * (Vector2)xOffset;
                    }

                    digitCount++;

                    // 获取Image组件
                    Image img = textObject.GetComponent<Image>();
                    img.SetNativeSize(); // 让图片恢复原始尺寸

                    if (img != null)
                    {
                         Color c = img.color;
                         c.a = 1f;
                         img.color = c;

                         LeanTween.delayedCall(1, ()=>{
                              if (img == null || textObject == null || !textObject.activeInHierarchy) return;
                              LeanTween.value(textObject, 1f, 0f, 1f)
                                        .setOnUpdate((float val) =>
                                        {
                                             if (img == null || textObject == null || !textObject.activeInHierarchy) return;
                                             Color cc = img.color;
                                             cc.a = val;
                                             img.color = cc;
                                        })
                                        .setOnComplete(() =>
                                        {
                                             if (textObject == null || !textObject.activeInHierarchy) return;
                                             PoolManager.Instance.ReturnToPool(sb.ToString(), textObject);
                                        });
                         });

                         // 添加字体上移效果
                         LeanTween.moveLocalY(textObject, textObject.transform.localPosition.y + 15f, 1.2f).setEase(LeanTweenType.easeOutQuad);
                    }

                    tempValue %= divisor;
                    divisor /= 10;
               }

               //如果是暴击，额外加上一个图片：
               if(damage[i].isCritical)
               {
                    GameObject textObject = PoolManager.Instance.SpawnFromPool(DamageNumberCritPrefixExtra, damageTextFather);

                    //设置随机的偏移：
                    RectTransform textRect = textObject.GetComponent<RectTransform>();
                    if (textRect != null)
                    {
                         textRect.localScale = Vector3.one; // 重置缩放，防止"越用越小"
                         textRect.anchoredPosition = Vector2.zero; // 先归零，再加偏移
                         textRect.anchoredPosition += (Vector2)randomOffset;
                         textRect.anchoredPosition += digitCount * (Vector2)xOffset;
                    }

                    Image img = textObject.GetComponent<Image>();
                    img.SetNativeSize(); // 让图片恢复原始尺寸

                    if (img != null)
                    {
                         Color c = img.color;
                         c.a = 1f;
                         img.color = c;

                         LeanTween.delayedCall(1, ()=>{
                              if (img == null || textObject == null || !textObject.activeInHierarchy) return;
                              LeanTween.value(textObject, 1f, 0f, 1f)
                                        .setOnUpdate((float val) =>
                                        {
                                             if (img == null || textObject == null || !textObject.activeInHierarchy) return;
                                             Color cc = img.color;
                                             cc.a = val;
                                             img.color = cc;
                                        })
                                        .setOnComplete(() =>
                                        {
                                             if (textObject == null || !textObject.activeInHierarchy) return;
                                             PoolManager.Instance.ReturnToPool(sb.ToString(), textObject);
                                        });
                         });

                         // 添加字体上移效果
                         LeanTween.moveLocalY(textObject, textObject.transform.localPosition.y + 15f, 1.2f).setEase(LeanTweenType.easeOutQuad);
                    }
               }

               // 在每次伤害显示之间添加延迟（除了最后一次）
               if(i < damageCount - 1)
               {
                    yield return new WaitForSeconds(0.7f); // 200毫秒延迟，可以根据需要调整
               }
          }
     }


    
     

}
