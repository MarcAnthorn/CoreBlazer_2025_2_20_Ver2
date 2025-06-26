using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShelterObjectFishJar : MonoBehaviour
{
    private bool isTriggerLock = true;
    private GameObject txtObject;
    private Vector3 offset = new Vector3(0, 0.5f);

    private void Update() {
        if(!isTriggerLock)
        {
            if(Input.GetKeyDown(KeyCode.J))
            {
                int avgId = 0;
                switch((int)GameLevelManager.Instance.gameLevelType)
                {
                    case 1:
                        avgId = 3101;
                    break;

                    case 2:
                        avgId = 3201;
                    break;

                    case 3:
                        avgId = 3301;
                    break;

                }

                // DialogueOrderBlock ob = LoadManager.Instance.orderBlockDic[avgId];
                // var panel = UIManager.Instance.ShowPanel<AVGPanel>();
                // panel.orderBlock = ob;
                UIManager.Instance.ShowPanel<AVGPanel>().InitAVG(avgId);
                EventHub.Instance.EventTrigger<bool>("Freeze", true);
            }
        }
    }


    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.CompareTag("Player"))
        {
            isTriggerLock = false;
            txtObject = PoolManager.Instance.SpawnFromPool("TipText");
            EventHub.Instance.EventTrigger<string, Vector3>("SetTipContent", "按下「J」进入交互", this.transform.position + offset);

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
