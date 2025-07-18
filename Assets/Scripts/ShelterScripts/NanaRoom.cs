using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NanaRoom : MonoBehaviour
{
    private bool isTriggerLock = true;
    private GameObject txtObject;
    private Vector3 offset = new Vector3(0, 0.5f);

    private E_NPCName myName = E_NPCName.奈亚拉;

    private void Update()
    {
        if (!isTriggerLock)
        {
            if (Input.GetKeyDown(KeyCode.J))
            {
                UIManager.Instance.ShowPanel<NPCInteractionPanel>().setNPCAction(myName);
                EventHub.Instance.EventTrigger<bool>("Freeze", true);
            }

            else if (Input.GetKeyDown(KeyCode.K))
            {
                int avgId = AVGDistributeManager.Instance.FetchAVGId(myName);

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
