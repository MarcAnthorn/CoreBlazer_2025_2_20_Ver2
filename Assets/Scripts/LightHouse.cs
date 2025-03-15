using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightHouse : MonoBehaviour
{
    private Light2D light;
    private BoxCollider2D collider;
    private bool lightLock = false;
    private void Awake()
    {
        light = this.GetComponent<Light2D>();
    }
    void Start()
    {
        lightLock = false;
        light.intensity = 1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player") && !lightLock)
        {
            LeanTween.value(gameObject, 1f, 0f, 1f)
                .setOnUpdate((float val) => {
                    light.intensity = val;
            });

            Debug.Log("LightHouse Triggered");
            PlayerController pc = collision.gameObject.GetComponent<PlayerController>();
            pc.ResumeLight();
            lightLock = true;

        }

    }
}

