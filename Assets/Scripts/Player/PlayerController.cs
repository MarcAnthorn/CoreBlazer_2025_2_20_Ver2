using System;
using System.Collections;
using System.Collections.Generic;
// using System.Numerics;
using Cinemachine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;



public class PlayerController : PlayerBase
{
    public float LMax = PlayerManager.Instance.player.LVL.value_limit; // 灯光值的上限，默认为玩家的LVL上限值
    [Range(0, 500)]
    public float LExtra;
    private float _l;

    private bool isWarningLocked = false;



//----------测试用：按键锁灯光：-------------------
    private bool isLightLocked = false;


    //将L和PlayerManager.Instance.player.LVL.value绑定，互相同步；
    public float L
    {
        get => PlayerManager.Instance.player.LVL.value;
        set
        {
            PlayerManager.Instance.player.LVLValue = value;
            _l = value; // 也同步更新本地的 _l
        }
    }
    
    private float lightShrinkingTime = 0;
    private float t;

    // [Range(50, 300f)]
    // [Tooltip("每秒扣除的血量（万分比），如100就是每秒扣除百分之一")]
    // public float bleedSpeed = 100f;

    public Light2D spriteLight;

    private bool isFrozen;
    private bool isLightShrinking = true;
    private bool isDamaging = false;
    public bool isDamageLocked = false;
    public float initialLightScope = 100;
    private Coroutine damageCoroutine;
    private int initDamageValue = 1;
    private int damageTime = 0;
    private int currentDamage;



    private Coroutine lightShrinkingCoroutine;
    private bool isShrinkingCoroutineRunning = false;


    private float shrinkingTimer = 0f; // 用于累计整秒
    private float shrinkingTarget = 0f; // 本秒应该减少的总量
    private float shrinkingLeftThisSecond = 0f; // 当前帧剩余的减少量
    

    protected override void Awake()
    {
        base.Awake();

        cam.Follow = this.gameObject.transform;

        EventHub.Instance.AddEventListener("OnPlayerDead", OnPlayerDead);
        EventHub.Instance.AddEventListener<bool>("Freeze", Freeze);
        EventHub.Instance.AddEventListener<bool>("TriggerLightShrinking", TriggerLightShrinking);
        EventHub.Instance.AddEventListener<Vector3>("SetPlayerPosition", SetPlayerPosition);

        EventHub.Instance.AddEventListener<UnityAction<Transform>>("ExposePlayerTransform", ExposePlayerTransform);

        EventHub.Instance.AddEventListener("ResumeLight", ResumeLight);



    }
    // Start is called before the first frame update
    void Start()
    { 
        isLightShrinking = true;
        lightShrinkingTime = 0;
        spriteLight.pointLightOuterRadius = initialLightScope;
        L = PlayerManager.Instance.player.LVL.value_limit; // 初始化灯光值为上限
        L = LMax;

        TriggerLightShrinking(true);

    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        //测试用，速死选项：
        // if(Input.GetKeyDown(KeyCode.M)){
        //     EventHub.Instance.EventTrigger("OnPlayerDead");
        // }



        // //测试用，速通道具分配：
        // if(Input.GetKeyDown(KeyCode.Y)){
        //     ItemManager.Instance.AddItem(104);
        // }

        //测试用：按键锁灯光 
        // if(Input.GetKeyDown(KeyCode.B)){
        //     isLightLocked = !isLightLocked;
        // }
    
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if(!isLightLocked)
            LightShrinking();

        
        
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        EventHub.Instance.RemoveEventListener("OnPlayerDead", OnPlayerDead);
        EventHub.Instance.RemoveEventListener<bool>("Freeze", Freeze);
        EventHub.Instance.RemoveEventListener<bool>("TriggerLightShrinking", TriggerLightShrinking);
        EventHub.Instance.RemoveEventListener<Vector3>("SetPlayerPosition", SetPlayerPosition);

        EventHub.Instance.RemoveEventListener<UnityAction<Transform>>("ExposePlayerTransform", ExposePlayerTransform);
        EventHub.Instance.RemoveEventListener("ResumeLight", ResumeLight);

        
        
    }



    private void TriggerLightShrinking(bool _isShrinking)
    {
        isLightShrinking = _isShrinking;
    }

    //暴露当前玩家的位置信息的方法：
    private void ExposePlayerTransform(UnityAction<Transform> action)
    {
        action?.Invoke(this.gameObject.transform);
    }

    private void LightShrinking()
    {
        if (!isLightShrinking)
            return;
        LMax = PlayerManager.Instance.player.LVL.value_limit;
        EventHub.Instance.EventTrigger("UpdateAllUIElements");

        
        
        // 计算当前t值和预期L值（用于检测外部变化）
        float t = lightShrinkingTime;
        float expectedL = LMax - 3.5f * t;

        // 检测灯光值是否发生外部变化（如道具使用、灯塔触发等）
        float currentL = PlayerManager.Instance.player.LVLValue;
        
        // 如果当前灯光值与预期衰减值差异较大，说明有外部干预
        if (Mathf.Abs(currentL - expectedL) > 10f)
        {
            // 重新计算对应的时间点：根据当前L值反推时间
            float newTime = (LMax - currentL) / 3.5f;
            lightShrinkingTime = Mathf.Max(0, newTime);
            Debug.Log($"[LightShrinking] 检测到灯光值外部变化，预期值：{expectedL}，实际值：{currentL}，重置计数器: L={currentL}, 新时间={lightShrinkingTime:F2}秒");
        }

        // 更新时间计数器
        lightShrinkingTime = lightShrinkingTime + Time.deltaTime;
        
        // 直接按每秒3.5的速度衰减，不使用Lerp平滑
        // 这样可以确保精确的衰减速度和即时响应
        L -= 3.5f * Time.deltaTime; // 每帧减少 3.5 * deltaTime
        
        // 确保不会低于0
        L = Mathf.Max(L, 0f);
        
        // 更新光照半径（基于L值的平方根或线性关系）
        float baseRadius = 0f;
        float radiusMultiplier = 0f;
        switch (GameLevelManager.Instance.gameLevelType)
        {
            case E_GameLevelType.Tutorial:
            case E_GameLevelType.Second:
                baseRadius = 2.12f;  // 最小基础半径
                radiusMultiplier = 0.05f;  // L值线性系数
                break;
            case E_GameLevelType.First:
                baseRadius = 2.12f;
                radiusMultiplier = 0.05f;
                break;
            case E_GameLevelType.Third:
                baseRadius = 2.12f;
                radiusMultiplier = 0.05f;
                break;
        }
        
        // 新的光照半径计算：基础半径 + L值的线性或平方根关系
        // 选择一种计算方式：
        spriteLight.pointLightOuterRadius = baseRadius + radiusMultiplier * L; // 平方根关系，变化更平缓 
        //Debug.Log($"L: {L}, Radius: {spriteLight.pointLightOuterRadius}");
        // 或者使用线性关系：spriteLight.pointLightOuterRadius = baseRadius + radiusMultiplier * L;

        // 灯光最小保护 - 基于L值判断
        if (L <= 3f)
        {
            HandleMinimumLight();
        }
    }

    private void HandleMinimumLight()
    {
        if (!isWarningLocked)
        {
            UIManager.Instance.ShowPanel<AboveWarningPanel>().SetWarningText("灯光消散！ 正在受到黑暗侵蚀！", true);
            isWarningLocked = true;
        }

        // 设置最小光照保护值
        L = 0f; // L值设为3，触发最小保护
        spriteLight.pointLightOuterRadius = 2.12f; // 最小光照半径
        TriggerLightShrinking(false);

        isDamaging = isDamageLocked ? false : true;
        if (isDamaging)
        {
            damageCoroutine = StartCoroutine(DamageCoroutine());
            damageTime = 0;
        }
    }
    /// <summary>
    /// 设置玩家属性的方法，保持上限值不变
    /// </summary>
    /// <param name="attrType">属性类型</param>
    /// <param name="newValue">新的属性值</param>
    /// <returns>是否设置成功</returns>
    public static bool SetPlayerAttribute(AttributeType attrType, float newValue)
    {
        var player = PlayerManager.Instance?.player;
        if (player == null)
        {
            Debug.LogError("未找到PlayerManager或player实例！");
            return false;
        }

        // 使用Player类的安全方法，避免反射调用
        player.SetAttrValue(attrType, newValue);
        Debug.Log($"已设置{attrType}={newValue}");
        return true;
    }

    /// <summary>
    /// 设置玩家属性的方法，可以同时设置当前值和上限值
    /// </summary>
    /// <param name="attrType">属性类型</param>
    /// <param name="newValue">新的属性值</param>
    /// <param name="newValueLimit">新的属性上限值</param>
    /// <returns>是否设置成功</returns>
    public static bool SetPlayerAttribute(AttributeType attrType, float newValue, float newValueLimit)
    {
        var player = PlayerManager.Instance?.player;
        if (player == null)
        {
            Debug.LogError("未找到PlayerManager或player实例！");
            return false;
        }

        // 使用Player类的安全方法，避免反射调用
        player.SetAttrValueAndLimit(attrType, newValue, newValueLimit);
        Debug.Log($"已设置{attrType}={newValue}，上限={newValueLimit}");
        return true;
    }


    // private void LightShrinking()
    // {
    //     if (!isLightShrinking)
    //         return;

    //     // 更新时间计数器
    //     lightShrinkingTime += Time.deltaTime;

    //     // 每满 1 秒，计算本秒应减少的值
    //     shrinkingTimer += Time.deltaTime;
    //     if (shrinkingTimer >= 1f)
    //     {
    //         shrinkingTimer -= 1f;

    //         float t = Mathf.Min(lightShrinkingTime, 3f); // 最大 3 秒
    //         shrinkingTarget = t * t; // 每秒减少 t²
    //         shrinkingLeftThisSecond = shrinkingTarget;
    //     }

    //     // 平滑减少灯光值
    //     if (shrinkingLeftThisSecond > 0f)
    //     {
    //         // 按帧等量减少
    //         float reduceThisFrame = shrinkingTarget * (Time.deltaTime / 1f); // 在一秒内匀速减完
    //         reduceThisFrame = Mathf.Min(reduceThisFrame, shrinkingLeftThisSecond);

    //         L -= reduceThisFrame;
    //         shrinkingLeftThisSecond -= reduceThisFrame;
    //     }

    //     Debug.Log($"L is {L}");

    //     // 更新光照半径
    //     switch (GameLevelManager.Instance.gameLevelType)
    //     {
    //         case E_GameLevelType.Tutorial:
    //         case E_GameLevelType.Second:
    //             spriteLight.pointLightOuterRadius = (0.02f / 20f) * L * L;
    //             break;

    //         case E_GameLevelType.First:
    //             spriteLight.pointLightOuterRadius = (0.015f / 20f) * L * L;
    //             break;

    //         case E_GameLevelType.Third:
    //             spriteLight.pointLightOuterRadius = (0.03f / 20f) * L * L;
    //             break;
    //     }

    //     // 灯光最小保护
    //     if (spriteLight.pointLightOuterRadius <= 2.12f)
    //     {
    //         if (!isWarningLocked)
    //         {
    //             UIManager.Instance.ShowPanel<WarningPanel>().SetWarningText("灯光消散！ 正在受到黑暗侵蚀！", true);
    //             isWarningLocked = true;
    //         }

    //         spriteLight.pointLightOuterRadius = 2.12f;
    //         L = 21.2f;
    //         TriggerLightShrinking(false);

    //         isDamaging = isDamageLocked ? false : true;
    //         if (isDamaging)
    //         {
    //             damageCoroutine = StartCoroutine(DamageCoroutine());
    //             damageTime = 0;
    //         }
    //     }
    // }



    // private void LightShrinking()
    // {
    //     if(isLightShrinking)
    //     {      
    //         lightShrinkingTime += Time.deltaTime;

    //         t = lightShrinkingTime >= 3 ? 3 : lightShrinkingTime;

    //         //灯光值调整：
    //         L -= t * t;

    //         Debug.Log($"L is {L}");
    //         switch(GameLevelManager.Instance.gameLevelType)
    //         {
    //             case E_GameLevelType.Tutorial:
    //             case E_GameLevelType.Second:
    //                 spriteLight.pointLightOuterRadius = (0.02f / 20f) * L * L;  
    //             break;

    //             case E_GameLevelType.First:
    //                 spriteLight.pointLightOuterRadius = (0.015f / 20f) * L * L;
    //             break;

    //             case E_GameLevelType.Third:
    //                 spriteLight.pointLightOuterRadius = (0.03f / 20f) * L * L;  
    //             break;
    //         }


    //         //灯光照射下限值：
    //         if(spriteLight.pointLightOuterRadius <= 2.12f)
    //         {
    //             if(!isWarningLocked)    //只有恢复灯光之后才会解锁的锁
    //             {
    //                 UIManager.Instance.ShowPanel<WarningPanel>().SetWarningText($"灯光消散！ 正在受到黑暗侵蚀！", true);
    //                 isWarningLocked = true;
    //             }

    //             spriteLight.pointLightOuterRadius = 2.12f;
    //             L = 21.2f;
    //             TriggerLightShrinking(false);

    //             //考虑是否锁血：如果锁血，那么就不会开启伤害协程：

    //             isDamaging = isDamageLocked ? false : true;

    //             if(isDamaging)
    //             {
    //                 damageCoroutine = StartCoroutine(DamageCoroutine());
    //                 damageTime = 0;
    //             }
    //         }

    //     }

    // }



    // private IEnumerator LightShrinkingCoroutine()
    // {
    //     isShrinkingCoroutineRunning = true;
    //     lightShrinkingTime = 0f;

    //     while (isLightShrinking)
    //     {
    //         lightShrinkingTime += 1f;  // 每秒一次

    //         float t = Mathf.Min(lightShrinkingTime, 3f);
    //         float reduction = t * t;
    //         L -= reduction;

    //         Debug.Log($"[LightShrink] L -= {reduction}, new L: {L}");

    //         // 更新灯光半径
    //         switch (GameLevelManager.Instance.gameLevelType)
    //         {
    //             case E_GameLevelType.Tutorial:
    //             case E_GameLevelType.Second:
    //                 spriteLight.pointLightOuterRadius = (0.02f / 20f) * L * L;
    //                 break;

    //             case E_GameLevelType.First:
    //                 spriteLight.pointLightOuterRadius = (0.015f / 20f) * L * L;
    //                 break;

    //             case E_GameLevelType.Third:
    //                 spriteLight.pointLightOuterRadius = (0.03f / 20f) * L * L;
    //                 break;
    //         }

    //         // 灯光下限判断
    //         if (spriteLight.pointLightOuterRadius <= 2.12f)
    //         {
    //             if (!isWarningLocked)
    //             {
    //                 UIManager.Instance.ShowPanel<WarningPanel>().SetWarningText($"灯光消散！ 正在受到黑暗侵蚀！", true);
    //                 isWarningLocked = true;
    //             }

    //             spriteLight.pointLightOuterRadius = 2.12f;
    //             L = 21.2f;
    //             TriggerLightShrinking(false); // 会自动停协程
    //             isDamaging = isDamageLocked ? false : true;

    //             if (isDamaging)
    //             {
    //                 damageCoroutine = StartCoroutine(DamageCoroutine());
    //                 damageTime = 0;
    //             }

    //             yield break;
    //         }

    //         yield return new WaitForSeconds(1f); // 每秒一次衰减
    //     }

    //     isShrinkingCoroutineRunning = false;
    // }


    private void OnPlayerDead()
    {
        //重置伤害相关
        damageTime = 0;
        if(damageCoroutine != null)
            StopCoroutine(damageCoroutine);
        isDamaging = false;

        //音效：
        SoundEffectManager.Instance.PlaySoundEffect("死亡音效");

        //清除所有死亡清除的道具：(装备不清除)
        ItemManager.Instance.ResetItemAfterDeath();

        //清除所有可能的层级buff：
        EventHub.Instance.EventTrigger("ResetFloorDiffer");

        // 显示死亡结算界面
        ShowDeathPanel();
    }

    /// <summary>
    /// 显示死亡结算界面
    /// </summary>
    private void ShowDeathPanel()
    {
        //如果死亡的时候，发现是玩家的SAN归零死亡的，那么直接播放剧情，然后回到游戏主界面：
        if(PlayerManager.Instance.player.SAN.value <= 0)
        {
            // SAN值归零的特殊死亡处理 - 直接播放剧情
            UIManager.Instance.ShowPanel<AVGPanel>().InitAVG(1301, OnComplete);
            EventHub.Instance.EventTrigger<bool>("Freeze", true);
            return;
        }

        // 普通死亡处理 - 直接进入复活流程，不显示死亡面板
        Debug.Log("玩家死亡，直接进入复活流程");
        
        // 死亡后直接返回安全屋，复活时会显示SAN奖励面板
        EventHub.Instance.EventTrigger<UnityAction>("ShowMask", ()=>{
            EventHub.Instance.EventTrigger<bool>("Freeze", true);
            LoadSceneManager.Instance.LoadSceneAsync("ShelterScene", SwitchSceneCallback);
        });
    
    }

    //回调函数：执行结局之后，退出当前游戏到主界面；
    public void OnComplete(int id)
    {
        EventHub.Instance.EventTrigger<UnityAction>("ShowMask", ()=>{
            Debug.LogWarning("退出游戏到主界面");
            LoadSceneManager.Instance.LoadSceneAsync("StartScene");  
            EventHub.Instance.EventTrigger<bool>("Freeze", false);
                        
        });
    }

    /// <summary>
    /// 显示通关界面（已适配极简 GameOverPanel，仅显示 SAN 和关闭按钮）
    /// </summary>
    public void ShowVictoryPanel()
    {
        // 获取实际的通关 SAN 值
        float sanValue = PlayerManager.Instance.player.SAN.value;

        // UIManager.Instance.ShowPanel<GameOverPanel>();
        // GameOverPanel.ShowSANOnlyPanel(sanValue, () => {
        //     // 通关后返回主菜单或显示成就等
        //     EventHub.Instance.EventTrigger<UnityAction>("ShowMask", ()=>{
        //         Debug.LogWarning("游戏通关，返回主界面");
        //         LoadSceneManager.Instance.LoadSceneAsync("StartScene");  
        //         EventHub.Instance.EventTrigger<bool>("Freeze", false);
        //     });
        // });
    }
    //这个是切换场景的时候的回调函数：
    private void SwitchSceneCallback()
    {
        EventHub.Instance.EventTrigger("TestClearFunction", true);
        //重置灯光相关

        //重置血量相关
        PlayerManager.Instance.player.HP.SetValue(100);
        PlayerManager.Instance.player.LVLValue = 300;

        //新增条件：San增加40:
        PlayerManager.Instance.player.SAN.AddValue(40);

        //新增条件：死亡掉落10个石头
        ItemManager.Instance.AddItem(105, true, 10);

        int equipmentId = UnityEngine.Random.Range(1, 22);
        EquipmentManager.Instance.AddEquipment(equipmentId);

        //重置当前的关卡进度：
        //不论是在哪一层死亡的，都会回到第一层的安全屋；
        GameLevelManager.Instance.gameLevelType = E_GameLevelType.First;

        //更新UI面板：
        EventHub.Instance.EventTrigger("UpdateAllUIElements");

        //新手关结束不会触发：
        if(GameLevelManager.Instance.gameLevelType != E_GameLevelType.First)
            UIManager.Instance.ShowPanel<ResultPanel>();

        // // 复活时处理：自动保存游戏并显示所有储存的事件与互动过的物体
        // try
        // {
        //     // 复活时首先计算并给予SAN奖励
        //     int sanReward = 0;
        //     if (SaveManager.Instance != null)
        //     {
        //         sanReward = SaveManager.Instance.CalculateAndAwardReviveSan();
        //         Debug.Log($"复活SAN奖励计算完成，获得SAN: {sanReward}");
        //     }
        //     else
        //     {
        //         Debug.LogWarning("SaveManager.Instance 为空，无法计算复活SAN奖励");
        //     }

        //     // 优先使用ReviveDataManager
        //     var reviveManagerType = System.Type.GetType("ReviveDataManager");
        //     if (reviveManagerType != null)
        //     {
        //         var instanceProperty = reviveManagerType.GetProperty("Instance", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
        //         if (instanceProperty != null)
        //         {
        //             var instance = instanceProperty.GetValue(null);
        //             if (instance != null)
        //             {
        //                 var onPlayerReviveMethod = reviveManagerType.GetMethod("OnPlayerRevive");
        //                 if (onPlayerReviveMethod != null)
        //                 {
        //                     onPlayerReviveMethod.Invoke(instance, null);
        //                     Debug.Log("复活数据管理器调用成功");
        //                 }
        //                 else
        //                 {
        //                     Debug.LogWarning("ReviveDataManager.OnPlayerRevive 方法未找到，使用SaveManager替代");
        //                     UseSaveManagerReviveMethod();
        //                 }
        //             }
        //             else
        //             {
        //                 Debug.LogWarning("ReviveDataManager.Instance 为空，使用SaveManager替代");
        //                 UseSaveManagerReviveMethod();
        //             }
        //         }
        //         else
        //         {
        //             Debug.LogWarning("ReviveDataManager.Instance 属性未找到，使用SaveManager替代");
        //             UseSaveManagerReviveMethod();
        //         }
        //     }
        //     else
        //     {
        //         Debug.LogWarning("ReviveDataManager 类型未找到，使用SaveManager替代");
        //         UseSaveManagerReviveMethod();
        //     }

        //     // 复活后显示复活面板（无论是否获得SAN奖励都显示）
        //     // ShowReviveSanRewardPanel(sanReward);
        // }
        // catch (System.Exception e)
        // {
        //     Debug.LogError($"调用ReviveDataManager时发生错误: {e.Message}，使用SaveManager替代");
        //     UseSaveManagerReviveMethod();
        // }
    }

    /// <summary>
    /// 使用SaveManager的复活方法作为回退方案
    /// </summary>
    private void UseSaveManagerReviveMethod()
    {
     
        SaveManager.Instance.SaveGameOnReviveAndShowData(calculateSanReward: false);
        Debug.Log("使用SaveManager复活处理完成！");
        var itemList = ItemManager.Instance.itemList;

        // 临时保存要删除的项
        List<int> toRemove = new List<int>();

        foreach (var itemIndex in itemList)
        {
            Item realItem = LoadManager.Instance.allItems[itemIndex];
            if (realItem.resetAfterDeath)
            {
                toRemove.Add(itemIndex);
            }
        }

        // 遍历临时列表，安全移除
        foreach (var index in toRemove)
        {
            ItemManager.Instance.RemoveItem(index);
        }
        
    }

    // /// <summary>
    // /// 显示复活SAN奖励面板 - 紧凑版
    // /// </summary>
    // /// <param name="sanReward">获得的SAN奖励数量</param>
    // private void ShowReviveSanRewardPanel(int sanReward)
    // {
    //     try
    //     {
    //         // 使用GameOverPanel的专用复活面板方法 - 集成SaveManager数据
    //         GameOverPanel.ShowReviveSanRewardPanel(sanReward, () => {
    //             Debug.Log("复活面板已关闭，解冻玩家");
    //             // 解冻玩家，允许继续游戏
    //             EventHub.Instance?.EventTrigger<bool>("Freeze", false);
    //         });

    //         Debug.Log($"复活面板已显示：{(sanReward > 0 ? $"+{sanReward} SAN" : "无SAN奖励")}");
    //     }
    //     catch (System.Exception e)
    //     {
    //         Debug.LogError($"显示复活面板时发生错误: {e.Message}");
    //         // 发生错误时也要解冻玩家
    //         EventHub.Instance?.EventTrigger<bool>("Freeze", false);
    //     }
    // }

    // public new static void SetPlayerPosition(Vector3 position)
    // {
    //     if (PlayerManager.Instance?.PlayerTransform != null)
    //     {
    //         PlayerManager.Instance.PlayerTransform.position = position;
    //         PlayerManager.Instance.playerPosition = position;
    //         Debug.Log($"玩家位置已设置为: {position}");
    //     }
    //     else
    //     {
    //         Debug.LogWarning("PlayerTransform为空，无法设置玩家位置！");
    //     }
    // }

    public void ResumeLight()
    {
        isWarningLocked = false;
        isLightShrinking = false;
        L = Math.Min(L + LExtra, LMax);
        
        // 根据新的灯光值重新计算对应的时间点（线性衰减反推）
        // 从 L = LMax - 3.5 * t 反推 t = (LMax - L) / 3.5
        lightShrinkingTime = Mathf.Max(0, (LMax - L) / 3.5f);
        t = lightShrinkingTime;

        Debug.LogWarning($"灯光值L：{L}");
        Debug.LogWarning($"实际灯光值：{PlayerManager.Instance.player.LVL.value}");
        Debug.LogWarning($"重置计数器时间：{lightShrinkingTime:F2}秒");

        //补充灯光之后，开启灯光衰减，关闭伤害判定协程；
        isLightShrinking = true;
        if(damageCoroutine != null)
            StopCoroutine(damageCoroutine);

        isDamaging = false;
       
    }

    //冻结（解冻）方法，在UI显示等其他交互的时候，冻结Player相关的调整
    protected override void Freeze(bool _isFrozen)
    {
        base.Freeze(_isFrozen);
        
        isFrozen = _isFrozen;
        isLightShrinking = !_isFrozen;
        if(_isFrozen)
        {
            if(damageCoroutine != null)
                StopCoroutine(damageCoroutine);

            damageCoroutine = null;
        }
        else
        {
            if(damageCoroutine != null)
                StopCoroutine(damageCoroutine);
        }


    }

    
    IEnumerator DamageCoroutine()
    {
        int time = 0;
        while(true)
        {
            yield return new WaitForSeconds(1f);
            time++;
            if(isFrozen)
            {
                //使用lambda表达式，表示当!isFrozen返回true才会继续执行；
                yield return new WaitUntil( () => !isFrozen );
            }

            // damageTime = 1;
            // currentDamage = initDamageValue * (1 + damageTime) >= 20 ? 20 : initDamageValue * (1 + damageTime);

            currentDamage = 1 + time;

            
            // 检测灯光值，如果大于3则停止伤害
            float currentLightValue = PlayerManager.Instance.player.LVLValue;
            if (currentLightValue > 3f)
            {
                Debug.Log($"[DamageCoroutine] 灯光值恢复到{currentLightValue}，停止黑暗伤害");
                isDamaging = false;
                TriggerLightShrinking(true);
                yield break; // 退出协程
            }
            Debug.Log($"currentDamage: {currentDamage}");
            // 修复：使用HPValue直接设置，避免struct副本问题
            var currentHP = PlayerManager.Instance.player.HPValue;
            var newHP = Mathf.Max(0, currentHP - currentDamage); // 确保不低于0
            PlayerManager.Instance.player.HPValue = newHP;
            Debug.Log($"Player HP: {PlayerManager.Instance.player.HPValue}");

            if(PlayerManager.Instance.player.HP.value <= 0)
            {
                EventHub.Instance.EventTrigger("OnPlayerDead");
            }

            EventHub.Instance.EventTrigger("UpdateAllUIElements");

        }


    }

}


