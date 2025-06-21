using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEquipment : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //测试装备：
        EquipmentManager.Instance.AddEquipment(1001, 1001, 1002, 1003, 1004);

        // foreach(var key in LoadManager.Instance.allEquipment.Keys)
        // {
        //     EquipmentManager.Instance.AddEquipment(key);
        // }

    }
}
