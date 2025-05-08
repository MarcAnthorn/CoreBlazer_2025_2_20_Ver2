using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class EquipmentCheckPanel : BasePanel
{
    public Image imgSelf;

    public Image imgLevelBg;
    public Button btnSkillCheck;
    public Button btnQuit;
    public Button btnEquip;
    public Button btnUnequip;

    
    public TextMeshProUGUI txtSkillName;
    public TextMeshProUGUI txtBuffDescription;
    public TextMeshProUGUI txtDescription;
    public TextMeshProUGUI txtRemainDuration;

    //当前面板显示的装备实例：
    public Equipment myEquipment;

    //装备回调函数的委托：
    public UnityAction<Equipment> equipmentCallback; 
    protected override void Init()
    {
        imgSelf.sprite = Resources.Load<Sprite>($"ArtResources/Equipment/{myEquipment.id}");

        switch((int)myEquipment.level)
        {
            case 0: //最高等级
                imgLevelBg.sprite = Resources.Load<Sprite>($"ArtResources/Equipment/Level3");
            break;  

            case 1:
                imgLevelBg.sprite = Resources.Load<Sprite>($"ArtResources/Equipment/Level2");
            break;

            case 2:
                imgLevelBg.sprite = Resources.Load<Sprite>($"ArtResources/Equipment/Level1");
            break;
        }

        equipmentCallback += equipmentCallback;

        btnSkillCheck.onClick.AddListener(()=>{
            UIManager.Instance.ShowPanel<SkillCheckPanel>().InitInfo(myEquipment.mySkill);
        });

        btnQuit.onClick.AddListener(()=>{
            UIManager.Instance.HidePanel<EquipmentCheckPanel>();
        });

        btnEquip.onClick.AddListener(()=>{
            if(EquipmentManager.Instance.NowLeftSlotsCount() < 3)
            {
                Debug.LogWarning("???????");
                
                //调用处在BattlePanel中的事件：
                EventHub.Instance.EventTrigger<Equipment>("EquipTarget", myEquipment);   
                
                //MaskEquipmentOrNot位于EquipmentPanelInventory，是「已装备」的遮罩；
                EventHub.Instance.EventTrigger("MaskEquipmentOrNot", true, myEquipment);
                myEquipment.isEquipped = true;
                UIManager.Instance.HidePanel<EquipmentCheckPanel>();

                //此处还需调用Equipment的Use方法：
                myEquipment.Equip();

                //更新玩家的属性面板：
                EventHub.Instance.EventTrigger("UpdateAllUIElements");
            }
            else
            {
                UIManager.Instance.ShowPanel<WarningPanel>().SetWarningText("精神值接近阈值! 装备数量达上限");
            }
        });

        btnUnequip.onClick.AddListener(()=>{
            //能有卸下键，说明该装备一定是处在装备中的；因此直接卸下就行；

            //取消装备的方法：
            EventHub.Instance.EventTrigger<Equipment>("UnequipTarget", myEquipment);
            EventHub.Instance.EventTrigger("MaskEquipmentOrNot", false, myEquipment);
            myEquipment.isEquipped = false;

            myEquipment.Unequip();

            //将卸下button切换成装备button：
            btnEquip.gameObject.SetActive(true);
            btnUnequip.gameObject.SetActive(false);

            //更新玩家的属性面板：
            EventHub.Instance.EventTrigger("UpdateAllUIElements");
        });
    }


    // Update is called once per frame
    void Update()
    {
        
    }


    //初始化自己信息的方法：
    public void InitInfo(Equipment _equipment)
    {
        myEquipment = _equipment;

        //初始化信息：
        txtSkillName.text = _equipment.mySkill.skillName;
        txtBuffDescription.text = _equipment.effectDescriptionText;
        txtDescription.text = _equipment.descriptionText;
        txtRemainDuration.text = $"剩余耐久度:{_equipment.currentDuration}";

        //按照当前需要显示的装备的装备情况，调整Button是装备还是卸下：
        if(_equipment.isEquipped)
        {
            btnEquip.gameObject.SetActive(false);
            btnUnequip.gameObject.SetActive(true);
        }
        else
        {
            btnEquip.gameObject.SetActive(true);
            btnUnequip.gameObject.SetActive(false);
        }
    }

    //

    //使用装备后的回调函数：
    //可能只会设计耐久度的更新之类的；Use()、isEquipped在BattlePanel中处理过了；
    public void EquipmentUsedCallback(Equipment equipment)
    {
        // //装备之后，对玩家执行效果：
        // // myEquipment.Use();

        // //同时，会调整该方法内部的标识字段：
        // myEquipment.isEquipped = true;

        //如果是我这里进行耐久度的调整，那么此处还需要进行耐久度的调整设置：
        //并且还需要UI的更新：
    }
}
