using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseBossReward : MonoBehaviour
{
    private bool isTriggerLock = true;
    private GameObject txtObject;
    private Vector3 offset = new Vector3(0, 0.5f);

   
    // 随机抽取并添加3个装备（允许重复）
    public void AddItems()
    {
               
        ItemManager.Instance.AddItem(101, true, 3);
        ItemManager.Instance.AddItem(103, true, 2);
        ItemManager.Instance.AddItem(302);
        ItemManager.Instance.AddItem(303, true, 4);
        ItemManager.Instance.AddItem(401, true, 4);
        ItemManager.Instance.AddItem(402, true, 4);
        ItemManager.Instance.AddItem(403, true, 4);
        ItemManager.Instance.AddItem(501, true, 4);

    }

    private void Update() {
        if(!isTriggerLock)
        {
            if(Input.GetKeyDown(KeyCode.J))
            {
                AddItems();
            } 
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.CompareTag("Player"))
        {
            isTriggerLock = false;
            txtObject = PoolManager.Instance.SpawnFromPool("TipText");
            EventHub.Instance.EventTrigger<string, Vector3>("SetTipContent", "按下「J」交互", this.transform.position + offset);

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
