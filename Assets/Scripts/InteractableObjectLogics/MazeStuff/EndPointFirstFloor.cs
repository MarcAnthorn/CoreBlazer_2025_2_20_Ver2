using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndPointFirstFloor : MonoBehaviour
{
    private bool isTriggerLock = true;
    private GameObject txtObject;
    private Vector3 offset = new Vector3(0, 0.5f);

    void Awake()
    {
        //如果gameLevelType >= 2,说明之前这个传送门使用过；
        //直接销毁： 
        if((int)GameLevelManager.Instance.gameLevelType >= 2)
            Destroy(this.gameObject);
    }

    private void Update()
    {
        if (!isTriggerLock)
        {
            if (Input.GetKeyDown(KeyCode.J))
            {
                SoundEffectManager.Instance.StopMusic();
                //清除所有可能的层级buff：
                EventHub.Instance.EventTrigger("ResetFloorDiffer");
                //存储当前的位置：
                GameLevelManager.Instance.lastTeleportPoint = this.transform.position;

                EventHub.Instance.EventTrigger<bool>("Freeze", true);
                GameLevelManager.Instance.gameLevelType = E_GameLevelType.Second;
                LoadSceneManager.Instance.LoadSceneAsync("ShelterScene");
                
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.CompareTag("Player"))
        {
            isTriggerLock = false;
            txtObject = PoolManager.Instance.SpawnFromPool("TipText");
            EventHub.Instance.EventTrigger<string, Vector3>("SetTipContent", "按下「J」返回安全屋", this.transform.position + offset);

        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if(other.gameObject.CompareTag("Player"))
        {
            isTriggerLock = true;
            PoolManager.Instance.ReturnToPool("TipTexts", txtObject);
        }
    }



}
