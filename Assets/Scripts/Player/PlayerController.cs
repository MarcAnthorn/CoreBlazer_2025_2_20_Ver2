using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Cinemachine;
using UnityEngine;
using UnityEngine.Rendering.Universal;


public class PlayerController : MonoBehaviour
{
   
    [Range(0, 5)]
    public float moveSpeed;
    public bool isMoving = true;
    public float L0 = 300;
    public float L2, L5, L;
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

    private bool isLightShrinking = true;
    private bool isDamaging = false;

    public float initialLightScope = 100;
    public CinemachineVirtualCamera cam;

    private void Awake()
    {
        cam = GameObject.FindGameObjectWithTag("VirtualCamera").GetComponent<CinemachineVirtualCamera>();
        cam.Follow = this.gameObject.transform;

        EventHub.Instance.AddEventListener("OnPlayerDead", OnPlayerDead);

    }
    // Start is called before the first frame update
    void Start()
    { 
          
        time = 0;
        L0 = 300;

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
        Damage();
        LightShrinking();
        
    }

    void OnDestroy()
    {
        EventHub.Instance.RemoveEventListener("OnPlayerDead", OnPlayerDead);
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
                // L = stageOneSpeed * L0 * (1 - 0.1f * time);
                L =  L0 * (1 - 0.1f * time);
            }
            else if (time > 2 && time <= 5)
            {
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
                TriggerLightShrinking(false);
                isDamaging = true;

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

    private void Damage()
    {   
        if(isDamaging)
        {
            PlayerManager.Instance.player.HP.ChangeValue(-bleedSpeed / 50);
            Debug.Log(PlayerManager.Instance.player.HP.value);
            if(PlayerManager.Instance.player.HP.value <= 0)
            {
                EventHub.Instance.EventTrigger("OnPlayerDead");
            }
        }
    }

    private void OnPlayerDead()
    {
        ResetPosition();
        L = L0;
        time = 0;
        isLightShrinking = true;
        isDamaging = false;
        PlayerManager.Instance.player.HP.value = 100;
        Debug.LogWarning("PlayerController Dead Triggered");
    }

    private void ResetPosition()
    {
        this.transform.position = PlayerManager.Instance.initPosition;
    }

    public void ResumeLight()
    {
        LeanTween.value(gameObject, L, 200, 1f)
                .setOnUpdate((float val) => {
                    L = val;
        });
        time = 0;
        isLightShrinking = true;
        isDamaging = false;
    }
}


