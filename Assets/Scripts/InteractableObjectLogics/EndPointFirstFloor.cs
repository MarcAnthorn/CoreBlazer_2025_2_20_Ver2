using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndPointFirstFloor : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            SoundEffectManager.Instance.StopMusic();
            //更新index，传送回安全屋：
            GameLevelManager.Instance.gameLevelType = E_GameLevelType.Second;

            Destroy(this.gameObject);
            LoadSceneManager.Instance.LoadSceneAsync("ShelterScene");
        }
    }
}
