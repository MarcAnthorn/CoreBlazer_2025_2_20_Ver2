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
    public float LMax = 100;
    [Range(0, 200)]
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
            PlayerManager.Instance.player.LVL.value = value;
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

        EventHub.Instance.AddEventListener<float>("ResumeLight", ResumeLight);

        

    }
    // Start is called before the first frame update
    void Start()
    { 
        isLightShrinking = true;
        lightShrinkingTime = 0;
        spriteLight.pointLightOuterRadius = initialLightScope;

        L = LMax;

        TriggerLightShrinking(true);

    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        //测试用，速死选项：
        if(Input.GetKeyDown(KeyCode.M)){
            EventHub.Instance.EventTrigger("OnPlayerDead");
        }



        // //测试用，速通道具分配：
        // if(Input.GetKeyDown(KeyCode.Y)){
        //     ItemManager.Instance.AddItem(104);
        // }

        // //测试用：按键锁灯光 
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
        EventHub.Instance.RemoveEventListener<float>("ResumeLight", ResumeLight);

        
        
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

        EventHub.Instance.EventTrigger("UpdateAllUIElements");

        // 更新时间计数器（限制最大9秒）
        lightShrinkingTime = Mathf.Min(lightShrinkingTime + Time.deltaTime, 9f);
        
        // 计算当前t值（0-9秒）
        float t = lightShrinkingTime;
        
        // 目标L值：LMax - t²
        float targetL = LMax - t * t;
        
        // 平滑过渡到目标值（使用Lerp或阻尼弹簧）
        L = Mathf.Lerp(L, targetL, Time.deltaTime * 5f); // 5是平滑系数，可调整
        
        // 确保不会低于目标值（防止lerp过冲）
        L = Mathf.Max(L, targetL);
        
        // 更新光照半径（优化版）
        float radiusMultiplier = 0f;
        switch (GameLevelManager.Instance.gameLevelType)
        {
            case E_GameLevelType.Tutorial:
            case E_GameLevelType.Second:
                radiusMultiplier = 0.02f / 20f;
                break;
            case E_GameLevelType.First:
                radiusMultiplier = 0.015f / 20f;
                break;
            case E_GameLevelType.Third:
                radiusMultiplier = 0.03f / 20f;
                break;
        }
        spriteLight.pointLightOuterRadius = radiusMultiplier * L * L;

        // 灯光最小保护
        if (spriteLight.pointLightOuterRadius <= 2.12f)
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

        spriteLight.pointLightOuterRadius = 2.12f;
        L = 21.2f;
        TriggerLightShrinking(false);

        isDamaging = isDamageLocked ? false : true;
        if (isDamaging)
        {
            damageCoroutine = StartCoroutine(DamageCoroutine());
            damageTime = 0;
        }
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


        //如果死亡的时候，发现是玩家的SAN归零死亡的，那么直接播放剧情，然后回到游戏主界面：
        if(PlayerManager.Instance.player.SAN.value <= 0)
        {
            UIManager.Instance.ShowPanel<AVGPanel>().InitAVG(1301, OnComplete);
            EventHub.Instance.EventTrigger<bool>("Freeze", true);
            return; //直接终止死亡的后续逻辑；
        }

        // PlayerManager.Instance.playerSceneIndex = E_PlayerSceneIndex.Maze;
        //加载安全屋的场景：
        //激活所有需要失活的过场景不移除的对象：
        //该方法定义在TestCanvas中，该脚本挂载在Canvas上；     

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


    //这个是切换场景的时候的回调函数：
    private void SwitchSceneCallback()
    {
        EventHub.Instance.EventTrigger("TestClearFunction", true);
        //重置灯光相关

        //重置血量相关
        PlayerManager.Instance.player.HP.SetValue(100);
        PlayerManager.Instance.player.LVL.SetValue(300);
        PlayerManager.Instance.player.LVL.value_limit = 300; // 确保上限也同步
        GameLevelManager.Instance.gameLevelType = E_GameLevelType.First;

        //更新UI面板：
        EventHub.Instance.EventTrigger("UpdateAllUIElements");

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

    private void SetPlayerPosition(Vector3 position)
    {
        this.transform.position = position;
    }

    public void ResumeLight(float lightDelta)
    {
        if(lightDelta < 0)
        {
            LExtra = lightDelta;
        }

        isWarningLocked = false;
        isLightShrinking = false;
        L = Math.Min(L + LExtra, LMax);
        lightShrinkingTime = Mathf.Sqrt(LMax - L);
        t = lightShrinkingTime;

        Debug.LogWarning($"灯光值L：{L}");
        Debug.LogWarning($"实际灯光值：{PlayerManager.Instance.player.LVL.value}");

        //补充灯光之后，开启灯光衰减，关闭伤害判定协程；
        isLightShrinking = true;
        if(damageCoroutine != null)
            StopCoroutine(damageCoroutine);

        isDamaging = false;
       
        float radiusMultiplier = 0f;
        switch (GameLevelManager.Instance.gameLevelType)
        {
            case E_GameLevelType.Tutorial:
            case E_GameLevelType.Second:
                radiusMultiplier = 0.02f / 20f;
                break;
            case E_GameLevelType.First:
                radiusMultiplier = 0.015f / 20f;
                break;
            case E_GameLevelType.Third:
                radiusMultiplier = 0.03f / 20f;
                break;
        }
        spriteLight.pointLightOuterRadius = radiusMultiplier * L * L;
        EventHub.Instance.EventTrigger("UpdateAllUIElements");
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
            
            Debug.Log(currentDamage);
            
            PlayerManager.Instance.player.HP.AddValue(-currentDamage);
            Debug.Log(PlayerManager.Instance.player.HP.value);

            if(PlayerManager.Instance.player.HP.value <= 0)
            {
                EventHub.Instance.EventTrigger("OnPlayerDead");
            }

            EventHub.Instance.EventTrigger("UpdateAllUIElements");

        }


    }

}


