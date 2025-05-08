using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossReward : MonoBehaviour
{
    private bool isTriggerLock = true;
    private GameObject txtObject;
    private Vector3 offset = new Vector3(0, 0.5f);

    // 装备ID数组
    private int[] equipmentIds = 
    {
        1001, 1002, 1003, 1004, 1005, 1006, 1007, 1008, 1009, 1010,
        1011, 1012, 1013, 1014, 1015, 1016, 1017, 1018, 1020, 1021, 1022
    };

    // 随机抽取并添加3个装备（允许重复）
    public void AddRandomEquipments()
    {
        // 随机选择3个ID（可能重复）
        int id1 = equipmentIds[Random.Range(0, equipmentIds.Length)];
        int id2 = equipmentIds[Random.Range(0, equipmentIds.Length)];
        int id3 = equipmentIds[Random.Range(0, equipmentIds.Length)];

        // 添加装备
        EquipmentManager.Instance.AddEquipment(id1, id2, id3);
        
    }

    private void Update() {
        if(!isTriggerLock)
        {
            if(Input.GetKeyDown(KeyCode.J))
            {
                AddRandomEquipments();
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
