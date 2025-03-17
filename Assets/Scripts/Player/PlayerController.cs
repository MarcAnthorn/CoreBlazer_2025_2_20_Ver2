using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Cinemachine;
using UnityEngine;
using UnityEngine.Rendering.Universal;


public class PlayerController : MonoBehaviour
{
    public float l1;
    public float l2;
    [Range(0, 5)]
    public float moveSpeed;

    [Range(100, 300)]
    public float L0 = 300;
    [Range(0, 200)]
    public float LExtra;
    private float L2, L5, L;
    private float time = 0;
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

    public bool isMoving = true;
    private bool isLightShrinking = true;
    private bool isDamaging = false;

    public float initialLightScope = 100;
    public CinemachineVirtualCamera cam;
    private Coroutine damageCoroutine;
    private int initDamageValue = 1;
    private int damageTime = 0;
    private int currentDamage;

    private void Awake()
    {
        cam = GameObject.FindGameObjectWithTag("VirtualCamera").GetComponent<CinemachineVirtualCamera>();
        cam.Follow = this.gameObject.transform;

        EventHub.Instance.AddEventListener("OnPlayerDead", OnPlayerDead);
        EventHub.Instance.AddEventListener<bool>("Freeze", Freeze);

    }
    // Start is called before the first frame update
    void Start()
    { 
          
        time = 0;
        L2 = L0 * (1 - 0.2f * 2);             
        L5 = L2 * (1 - 0.2f * 2); 

        // L2 = L0 * (1 - 0.2f * 2);             
        // L5 = L2 * Mathf.Exp(-0.8f * (5 - 2)); 
        spriteLight.pointLightOuterRadius = initialLightScope;
    }

    // Update is called once per frame
    void Update()
    {
    }

    void FixedUpdate()
    {
        
        ControlPlayerMove();
        LightShrinking();
        
    }

    void OnDestroy()
    {
        EventHub.Instance.RemoveEventListener("OnPlayerDead", OnPlayerDead);
        EventHub.Instance.RemoveEventListener<bool>("Freeze", Freeze);
    }

    private void TriggerLightShrinking(bool _isShrinking)
    {
        isLightShrinking = _isShrinking;
    }
    private void LightShrinking()
    {
    //     if(isLightShrinking)
    //     {
    //         time += Time.deltaTime;
    //         if (time >= 0 && time <= 2)
    //         {
    //             // L = stageOneSpeed * L0 * (1 - 0.1f * time);
    //             L =  L0 * (1 - 0.1f * time);
    //         }
    //         else if (time > 2 && time <= 5)
    //         {
    //             // L = stageTwoSpeed * L2 * Mathf.Exp(-0.6f * (time - 2));
    //             L =  L2 * Mathf.Exp(-0.6f * (time - 2));
    //         }
    //         else if (time > 5)
    //         {
    //             // L = stageThreeSpeed * L5 / (1 + 0.5f * (time - 5));
    //             L = L5 / (1 + 0.5f * (time - 5));
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
            time += Time.deltaTime;
            if (time >= 0 && time <= 2)
            {
                if(time >= 1.96 && time <= 2.02)
                {
                    l1 = L0 * (1 - 0.1f * time);
                }
                // L = stageOneSpeed * L0 * (1 - 0.1f * time);
                L =  L0 * (1 - 0.1f * time);
            }
            else if (time > 2 && time <= 5)
            {
                if(time >= 4.96 && time <= 5.02)
                {
                    l2 = L0 * (1 - 0.1f * time);
                }
                // L = stageTwoSpeed * L2 * Mathf.Exp(-0.6f * (time - 2));
                L =  L2 * (1 - 0.1f * time);
            }
            else if (time > 5)
            {
                // L = stageThreeSpeed * L5 / (1 + 0.5f * (time - 5));
                L = L5 / (1 + 0.5f * (time - 5));
            }

            spriteLight.pointLightOuterRadius = L / 10;

            if(spriteLight.pointLightOuterRadius <= 2.12f)
            {
                spriteLight.pointLightOuterRadius = 2.12f;
                L = 21.2f;
                TriggerLightShrinking(false);
                damageCoroutine = StartCoroutine(DamageCoroutine());
                damageTime = 0;
            }

        }
    
    }

    private void ControlPlayerMove()
    {
        if(isMoving)
        {
            if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
            {
                this.transform.Translate(0, Input.GetAxis("Vertical") * Time.deltaTime * moveSpeed, 0);
            }

            if(Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.A)) 
            {
                this.transform.Translate(Input.GetAxis("Horizontal") * Time.deltaTime * moveSpeed, 0, 0);
            }
        }
    }

    private void OnPlayerDead()
    {
        ResetPosition();
        L = L0;
        time = 0;
        isLightShrinking = true;
        StopCoroutine(damageCoroutine);
        PlayerManager.Instance.player.HP.value = 100;
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
            // 第一阶段：L = L0 * (1 - 0.1f * time)
            time = (1 - L / L0) / 0.1f;
        }
        else if (L > 78 && L <= 115)  
        {
            // 第二阶段：L = L2 * (1 - 0.1f * time)
            time = (1 - L / L2) / 0.1f;
        }
        else  
        {
            // 第三阶段：L = L5 / (1 + 0.5f * (time - 5))
            time = 5 + (L5 / L - 1) / 0.5f;
        }

        //补充灯光之后，开启灯光衰减，关闭伤害判定协程；
        isLightShrinking = true;
        StopCoroutine(damageCoroutine);
       
    }

    //冻结（解冻）方法，在UI显示等其他交互的时候，冻结Player相关的调整
    private void Freeze(bool _isFrozen)
    {
        isMoving = !_isFrozen;
        isDamaging = !_isFrozen;
        isLightShrinking = !_isFrozen;

    }

    
    IEnumerator DamageCoroutine()
    {
        while(true)
        {
            yield return new WaitForSeconds(1);

            damageTime += 1;
            currentDamage = initDamageValue * (1 + damageTime);
            PlayerManager.Instance.player.HP.ChangeValue(-currentDamage);
            Debug.Log(PlayerManager.Instance.player.HP.value);

            if(PlayerManager.Instance.player.HP.value <= 0)
            {
                EventHub.Instance.EventTrigger("OnPlayerDead");
            }

        }


    }

}


