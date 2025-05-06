using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEquipment : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //测试装备：
        EquipmentManager.Instance.AddEquipment(1001);
        EquipmentManager.Instance.AddEquipment(1001);
        EquipmentManager.Instance.AddEquipment(1002);
        EquipmentManager.Instance.AddEquipment(1003);
        EquipmentManager.Instance.AddEquipment(1004);

    }

    // Update is called once per frame
    // void Update()
    // {
    //     if(Input.GetKeyDown(KeyCode.O))
    //     {
    //         UIManager.Instance.ShowPanel<BattlePanel>();
    //     }
    // }
}
