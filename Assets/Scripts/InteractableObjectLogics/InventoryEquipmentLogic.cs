using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryEquipmentLogic : MonoBehaviour
{
    public GameObject equippedMask;
    public Image imgEquipment;
    public TextMeshProUGUI txtDurationCount;
    public TextMeshProUGUI txtEquipmentName;
    public Button btnSelf;
    // Start is called before the first frame update
    void Start()
    {
        btnSelf.onClick.AddListener(()=>{
            UIManager.Instance.ShowPanel<EquipmentCheckPanel>();
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
