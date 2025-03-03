using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.Rendering.Universal;


public class PlayerController : MonoBehaviour
{
   
    [Range(0, 5)]
    public float speed;

    public Light2D spriteLight;

    private bool isLightShrinking = true;
    [Range(0, 1)]
    public float lightShrinkSpeed;
    [Range(0, 8)]
    public float initialLightScope;

    // Start is called before the first frame update
    void Start()
    {
        spriteLight.pointLightOuterRadius = initialLightScope;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
        {
            this.transform.Translate(0, Input.GetAxis("Vertical") * Time.deltaTime * speed, 0);
        }

        if(Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.A)) 
        {
             this.transform.Translate(Input.GetAxis("Horizontal") * Time.deltaTime * speed, 0, 0);
        }

        if(isLightShrinking)
        {
            spriteLight.pointLightOuterRadius -= lightShrinkSpeed * Time.deltaTime;
            if(spriteLight.pointLightOuterRadius <= 2.12f)
            {
                spriteLight.pointLightOuterRadius = 2.12f;
                TriggerLightShrinking(false);
            }
        }


    }


    private void TriggerLightShrinking(bool _isShrinking)
    {
        isLightShrinking = _isShrinking;
    }
}
