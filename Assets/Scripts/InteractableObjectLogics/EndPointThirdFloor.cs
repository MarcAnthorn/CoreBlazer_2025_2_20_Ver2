using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndPointThirdFloor : MonoBehaviour
{
    //播放结局：
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            SoundEffectManager.Instance.StopMusic();
            // //更新index，传送回安全屋：
            // GameLevelManager.Instance.gameLevelType = E_GameLevelType.Second;

            // Destroy(this.gameObject);
            // LoadSceneManager.Instance.LoadSceneAsync("ShelterScene");


            Debug.LogWarning("游戏结束，播放结局");
        }
    }
}
