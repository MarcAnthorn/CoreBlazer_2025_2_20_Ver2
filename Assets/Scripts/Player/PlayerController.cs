using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Cinemachine;
using UnityEngine;
using UnityEngine.Rendering.Universal;


public class PlayerController : PlayerBase
{
    public float l1;
    public float l2;

    [Range(100, 300)]
    public float L0 = 300;
    [Range(0, 200)]
    public float LExtra;
    private float L2, L5;
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
    [Range(0, 1f)]
    public float stageOneSpeed;
    [Range(0, 1f)]
    public float stageTwoSpeed;
    [Range(0, 1f)]
    public float stageThreeSpeed;

    [Range(50, 300f)]
    [Tooltip("每秒扣除的血量（万分比），如100就是每秒扣除百分之一")]
    public float bleedSpeed = 100f;

    public Light2D spriteLight;

    private bool isFrozen;
    private bool isLightShrinking = true;
    private bool isDamaging = false;
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

    }
    // Start is called before the first frame update
    void Start()
    { 
        isLightShrinking = true;
        lightShrinkingTime = 0;
        L2 = L0 * (1 - 0.2f * 2);             
        L5 = L2 * (1 - 0.2f * 2); 
        // L2 = L0 * (1 - 0.2f * 2);             
        // L5 = L2 * Mathf.Exp(-0.8f * (5 - 2)); 
        spriteLight.pointLightOuterRadius = initialLightScope;
    }

    // Update is called once per frame

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
    }

    private void TriggerLightShrinking(bool _isShrinking)
    {
        isLightShrinking = _isShrinking;
    }
    private void LightShrinking()
    {
    //     if(isLightShrinking)
    //     {
    //         lightShrinkingTime += Time.deltaTime;
    //         if (lightShrinkingTime >= 0 && lightShrinkingTime <= 2)
    //         {
    //             // L = stageOneSpeed * L0 * (1 - 0.1f * lightShrinkingTime);
    //             L =  L0 * (1 - 0.1f * lightShrinkingTime);
    //         }
    //         else if (lightShrinkingTime > 2 && lightShrinkingTime <= 5)
    //         {
    //             // L = stageTwoSpeed * L2 * Mathf.Exp(-0.6f * (lightShrinkingTime - 2));
    //             L =  L2 * Mathf.Exp(-0.6f * (lightShrinkingTime - 2));
    //         }
    //         else if (lightShrinkingTime > 5)
    //         {
    //             // L = stageThreeSpeed * L5 / (1 + 0.5f * (lightShrinkingTime - 5));
    //             L = L5 / (1 + 0.5f * (lightShrinkingTime - 5));
    //         }

    //         spriteLight.pointLightOuterRadius = L / 10;

    //         if(spriteLight.pointLightOuterRadius <= 2.12f)
    //         {
    //             spriteLight.pointLightOuterRadius = 2.12f;
    //             TriggerLightShrinking(false);
                
    //         }
        
    //     }

        if(isLightShrinking)
        {
            lightShrinkingTime += Time.deltaTime;
            if (lightShrinkingTime >= 0 && lightShrinkingTime <= 2)
            {
                if(lightShrinkingTime >= 1.96 && lightShrinkingTime <= 2.02)
                {
                    L0 = L0 * (1 - 0.1f * lightShrinkingTime);
                }
                // L = stageOneSpeed * L0 * (1 - 0.1f * lightShrinkingTime);
                L =  L0 * (1 - 0.1f * lightShrinkingTime);
            }
            else if (lightShrinkingTime > 2 && lightShrinkingTime <= 5)
            {
                if(lightShrinkingTime >= 4.96 && lightShrinkingTime <= 5.02)
                {
                    L2 = L0 * (1 - 0.1f * lightShrinkingTime);
                }
                // L = stageTwoSpeed * L2 * Mathf.Exp(-0.6f * (lightShrinkingTime - 2));
                L =  L2 * (1 - 0.1f * lightShrinkingTime);
            }
            else if (lightShrinkingTime > 5)
            {
                // L = stageThreeSpeed * L5 / (1 + 0.5f * (lightShrinkingTime - 5));
                L = L5 / (1 + 0.5f * (lightShrinkingTime - 5));
            }

            spriteLight.pointLightOuterRadius = L / 10;

            if(spriteLight.pointLightOuterRadius <= 2.12f)
            {
                spriteLight.pointLightOuterRadius = 2.12f;
                L = 21.2f;
                TriggerLightShrinking(false);
                isDamaging = true;
                damageCoroutine = StartCoroutine(DamageCoroutine());
                damageTime = 0;
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
        PlayerManager.Instance.player.HP.value = 100;
        PlayerManager.Instance.player.LVL.value = 300;
    }

    private void ResetPosition()
    {
        this.transform.position = PlayerManager.Instance.initPosition;
    }

    public void ResumeLight()
    {
        isLightShrinking = false;
        L = L + LExtra;
        if (L > 115)  
        {
            // 第一阶段：L = L0 * (1 - 0.1f * lightShrinkingTime)
            lightShrinkingTime = (1 - L / L0) / 0.1f;
        }
        else if (L > 78 && L <= 115)  
        {
            // 第二阶段：L = L2 * (1 - 0.1f * lightShrinkingTime)
            lightShrinkingTime = (1 - L / L2) / 0.1f;
        }
        else  
        {
            // 第三阶段：L = L5 / (1 + 0.5f * (lightShrinkingTime - 5))
            lightShrinkingTime = 5 + (L5 / L - 1) / 0.5f;
        }

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
            yield return new WaitForSeconds(1);

            if(isFrozen)
            {
                //使用lambda表达式，表示当!isFrozen返回true才会继续执行；
                yield return new WaitUntil( () => !isFrozen );
            }

            damageTime += 1;
            currentDamage = initDamageValue * (1 + damageTime) >= 20 ? 20 : initDamageValue * (1 + damageTime);
            
            PlayerManager.Instance.player.HP.ChangeValue(-currentDamage);
            Debug.Log(PlayerManager.Instance.player.HP.value);

            if(PlayerManager.Instance.player.HP.value <= 0)
            {
                EventHub.Instance.EventTrigger("OnPlayerDead");
            }

        }


    }

}


