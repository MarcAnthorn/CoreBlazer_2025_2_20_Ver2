using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC50008 : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            EventManager.Instance.TriggerEvent(2045);

            SoundEffectManager.Instance.PlaySoundEffect("与地图POI交互");
            Destroy(this.gameObject);
        }


    }
}
