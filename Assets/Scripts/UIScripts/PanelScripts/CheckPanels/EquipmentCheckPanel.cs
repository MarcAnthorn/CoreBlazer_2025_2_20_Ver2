using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentCheckPanel : BasePanel
{
    public GameObject equipmentImageObject;
    public Button btnSkillCheck;
    public Button btnQuit;
    public Button btnEquip;
    public Button btnUnequip;
    public TextMeshProUGUI txtBuffDescription;
    public TextMeshProUGUI txtDescription;
    public TextMeshProUGUI txtRemainDuration;
    protected override void Init()
    {
        btnSkillCheck.onClick.AddListener(()=>{
            UIManager.Instance.ShowPanel<SkillCheckPanel>();
        });

        btnQuit.onClick.AddListener(()=>{
            UIManager.Instance.HidePanel<EquipmentCheckPanel>();
        });

        btnEquip.onClick.AddListener(()=>{
           
        });

        btnUnequip.onClick.AddListener(()=>{
            
        });
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
