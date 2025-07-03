using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightHouse : MonoBehaviour
{
    private Light2D light2D;
    // private BoxCollider2D collider;
    private bool lightLock = false;
    private void Awake()
    {
        light2D = this.GetComponent<Light2D>();
        EventHub.Instance.AddEventListener("OnPlayerDead", OnPlayerDead);
    }
    void Start()
    {
        lightLock = false;
        light2D.intensity = 1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDestroy()
    {
        EventHub.Instance.RemoveEventListener("OnPlayerDead", OnPlayerDead);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player") && !lightLock)
        {
            LeanTween.value(gameObject, 1f, 0f, 1f)
                .setOnUpdate((float val) => {
                    light2D.intensity = val;
            });
            
            EventHub.Instance.EventTrigger("ResumeLight");
            lightLock = true;

            //灯光回复，尝试触发音效：
            SoundEffectManager.Instance.PlaySoundEffect("接触灯塔");

        }

    }

    private void OnPlayerDead()
    {
        lightLock = false;
        light2D.intensity = 1;
        Debug.LogWarning("LightHouse Dead Triggered");
    }
}

