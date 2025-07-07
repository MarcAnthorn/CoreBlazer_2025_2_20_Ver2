using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyDetectorLogic : MonoBehaviour
{
    private bool isTriggerLock = true;
    public DoorLogicBase door;    
    private Vector3 offset = new Vector3(0, 0.5f);
    private GameObject txtObject;

    //当前对应的门是否可以交互：
    private bool isDoorInteractable => door.isDoorInteractable;
     


    private void Update() {
        if(!isTriggerLock && Input.GetKeyDown(KeyCode.F))
        {
            door.actionDoorTrigger();
        }
    }


    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.CompareTag("Player"))
        {
            //要想门可以交互，需要满足门自己的判断：
            if(ItemManager.Instance.CheckIfItemExist(509) || isDoorInteractable)  //存在钥匙
            {
                isTriggerLock = false;
                txtObject = PoolManager.Instance.SpawnFromPool("TipText");
                EventHub.Instance.EventTrigger<string, Vector3>("SetTipContent", "按下「F」和门交互", this.transform.position + offset);
            }

        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if(txtObject != null && other.gameObject.CompareTag("Player"))
        {
            isTriggerLock = true;
            PoolManager.Instance.ReturnToPool("TipTexts", txtObject);
        }
    }
}
