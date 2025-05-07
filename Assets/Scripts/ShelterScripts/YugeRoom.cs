using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YugeRoom : MonoBehaviour
{
    private bool isTriggerLock = true;
    private GameObject txtObject;
    private Vector3 offset = new Vector3(0, 0.5f);

    private void Update() {
        if(!isTriggerLock)
        {
            if(Input.GetKeyDown(KeyCode.J))
            {
                UIManager.Instance.ShowPanel<NPCInteractionPanel>().setNPCAction(E_NPCName.优格);
                EventHub.Instance.EventTrigger<bool>("Freeze", true);
            }

            else if(Input.GetKeyDown(KeyCode.K))
            {
                int avgId = 0;
                switch((int)GameLevelManager.Instance.gameLevelType)
                {
                    case 0:
                        avgId = 1110;
                    break;

                    case 1:
                        avgId = 1115;
                    break;

                    case 2:
                        avgId = 1120;
                    break;

                }

                DialogueOrderBlock ob = LoadManager.Instance.orderBlockDic[avgId];
                var panel = UIManager.Instance.ShowPanel<AVGPanel>();
                panel.orderBlock = ob;
                EventHub.Instance.EventTrigger<bool>("Freeze", true);
            }

        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.CompareTag("Player"))
        {
            isTriggerLock = false;
            txtObject = PoolManager.Instance.SpawnFromPool("TipText");
            EventHub.Instance.EventTrigger<string, Vector3>("SetTipContent", "按下「J」进入信仰绑定界面\n按下「K」进入对话", this.transform.position + offset);

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
