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
    public CinemachineVirtualCamera cam;
    private Coroutine damageCoroutine;
    private int initDamageValue = 1;
    private int damageTime = 0;
    private int currentDamage;

    protected override void Awake()
    {
        base.Awake();

        cam = GameObject.FindGameObjectWithTag("VirtualCamera").GetComponent<CinemachineVirtualCamera>();
        cam.Follow = this.gameObject.transform;

        EventHub.Instance.AddEventListener("OnPlayerDead", OnPlayerDead);
        EventHub.Instance.AddEventListener<bool>("Freeze", Freeze);
        EventHub.Instance.AddEventListener<bool>("TriggerLightShrinking", TriggerLightShrinking);
        EventHub.Instance.AddEventListener<Vector3>("SetPlayerPosition", SetPlayerPosition);

    }
    // Start is called before the first frame update
    void Start()
    { 
        isLightShrinking = true;
        lightShrinkingTime = 0;
        spriteLight.pointLightOuterRadius = initialLightScope;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        LightShrinking();
        
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        EventHub.Instance.RemoveEventListener("OnPlayerDead", OnPlayerDead);
        EventHub.Instance.RemoveEventListener<bool>("Freeze", Freeze);
        EventHub.Instance.RemoveEventListener<bool>("TriggerLightShrinking", TriggerLightShrinking);
        EventHub.Instance.RemoveEventListener<Vector3>("SetPlayerPosition", SetPlayerPosition);
        
    }



    private void TriggerLightShrinking(bool _isShrinking)
    {
        isLightShrinking = _isShrinking;
    }
    private void LightShrinking()
    {
        if(isLightShrinking)
        {
            lightShrinkingTime += Time.deltaTime;

            t = lightShrinkingTime >= 10 ? 10 : lightShrinkingTime;

            //灯光值调整：
            L = LMax - t * t;
            switch(GameLevelManager.Instance.gameLevelType)
            {
                case E_GameLevelType.Tutorial:
                case E_GameLevelType.Second:
                    spriteLight.pointLightOuterRadius = (0.015f / 20f) * L * L;
                break;

                case E_GameLevelType.First:
                    spriteLight.pointLightOuterRadius = (0.02f / 20f) * L * L;  
                break;

                case E_GameLevelType.Third:
                    spriteLight.pointLightOuterRadius = (0.03f / 20f) * L * L;  
                break;
            }


            //灯光照射下限值：
            if(spriteLight.pointLightOuterRadius <= 2.12f)
            {
                spriteLight.pointLightOuterRadius = 2.12f;
                L = 21.2f;
                TriggerLightShrinking(false);

                //考虑是否锁血：如果锁血，那么就不会开启伤害协程：

                isDamaging = isDamageLocked ? false : true;

                if(isDamaging)
                {
                    damageCoroutine = StartCoroutine(DamageCoroutine());
                    damageTime = 0;
                }
            }

        }

    }
    private void OnPlayerDead()
    {
        // //重置位置
        // ResetPosition();

        //角色死亡之后，重置一些内容之后，直接传送回对应的安全屋：
        

        //重置伤害相关
        damageTime = 0;
        if(damageCoroutine != null)
            StopCoroutine(damageCoroutine);
        isDamaging = false;

        // PlayerManager.Instance.playerSceneIndex = E_PlayerSceneIndex.Maze;
        //加载安全屋的场景：
        //激活所有需要失活的过场景不移除的对象：
        //该方法定义在TestCanvas中，该脚本挂载在Canvas上；        
        LoadSceneManager.Instance.LoadSceneAsync("ShelterScene", SwitchSceneCallback);


    }

    //这个是切换场景的时候的回调函数：
    private void SwitchSceneCallback()
    {
        EventHub.Instance.EventTrigger("TestClearFunction", true);
        //重置灯光相关

        //重置血量相关
        PlayerManager.Instance.player.HP.SetValue(100);
        PlayerManager.Instance.player.LVL.value = 300;
    }

    private void SetPlayerPosition(Vector3 position)
    {
        this.transform.position = position;
    }

    public void ResumeLight()
    {
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
            if(isDamaging)
                damageCoroutine = StartCoroutine(DamageCoroutine());
        }


    }

    
    IEnumerator DamageCoroutine()
    {
        while(true)
        {
            yield return new WaitForSeconds(5f);

            if(isFrozen)
            {
                //使用lambda表达式，表示当!isFrozen返回true才会继续执行；
                yield return new WaitUntil( () => !isFrozen );
            }

            damageTime += 1;
            currentDamage = initDamageValue * (1 + damageTime) >= 20 ? 20 : initDamageValue * (1 + damageTime);
            
            Debug.Log(currentDamage);
            
            PlayerManager.Instance.player.HP.AddValue(-currentDamage);
            Debug.Log(PlayerManager.Instance.player.HP.value);

            if(PlayerManager.Instance.player.HP.value <= 0)
            {
                EventHub.Instance.EventTrigger("OnPlayerDead");
            }

        }


    }

}


