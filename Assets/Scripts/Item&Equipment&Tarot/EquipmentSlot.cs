using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class EquipmentSlot : MonoBehaviour
{
    //当前的Slot是否已经装备了：
    public bool isSlotted = false;

    

    //相关UI信息：
    public Image imgEquipment;
    public Image imgSkill;
    public Button btnSkillUse;
    //该装备技能消耗的行动点：
    public TextMeshProUGUI txtCost;
    public TextMeshProUGUI txtSkillDamage;
    public TextMeshProUGUI txtSkillName;

    public TextMeshProUGUI txtDuration;
    public Equipment myEquipment;
    public Skill mySkill;

    public GameObject unusedMask;


    void Awake()
    {
        EventHub.Instance.AddEventListener("UpdateAllSkillDamageText", UpdateAllSkillDamageText);
        //先注册，再广播，不然第一次使用不消耗耐久；
        EventHub.Instance.AddEventListener<Equipment>("UpdateEquipmentUI", UpdateEquipmentUI);

        //先默认失活：
        btnSkillUse.gameObject.SetActive(false);
    }

    void Start()
    {
        UpdateEquipmentUI(myEquipment);
        btnSkillUse.onClick.AddListener(()=>{
            // EventHub.Instance.EventTrigger("BroadcastNowEquipment", myEquipment);
            SkillManager.Instance.BroadcastNowEquipment(myEquipment);
            //释放技能：
            mySkill.Use();

        });
    }

    void OnDestroy()
    {
        EventHub.Instance.RemoveEventListener("UpdateAllSkillDamageText", UpdateAllSkillDamageText);
        EventHub.Instance.RemoveEventListener<Equipment>("UpdateEquipmentUI", UpdateEquipmentUI);
        
    }
    
    



    //初始化该Slot的方法：传入对应的Equipment实例就行：
    public void InitSlot(Equipment _equipment){

        //如果是null，那么表示是默认技能槽；
        if(_equipment == null)
        {
            //装备默认技能：
            isSlotted = true;

            //激活Button：
            btnSkillUse.gameObject.SetActive(true);
        
            mySkill = LoadManager.Instance.allSkills[1001];
            imgSkill.sprite = Resources.Load<Sprite>(Path.Combine("ArtResources", mySkill.skillIconPath));
            txtCost.text = $"消耗行动点：{mySkill.skillCost}";
            txtSkillDamage.text = mySkill.SkillDamage.ToString();
            txtSkillName.text = mySkill.skillName;
            return;


        }
        //将当前的Slot处理为已装备；
        isSlotted = true;

        //取消遮罩：
        unusedMask.gameObject.SetActive(false);

        //激活Button：
        btnSkillUse.gameObject.SetActive(true);
        
        myEquipment = _equipment;

        //更新耐久：
        txtDuration.text = $"耐久:{myEquipment.currentDuration}/{myEquipment.maxDuration}";

        mySkill = _equipment.mySkill;
        //初始化相关的信息：
        imgEquipment.sprite = Resources.Load<Sprite>(Path.Combine("ArtResources", _equipment.iconPath));
        imgSkill.sprite = Resources.Load<Sprite>(Path.Combine("ArtResources", mySkill.skillIconPath));

        txtCost.text = $"消耗行动点：{mySkill.skillCost}";


        //当前的技能对应的可造成的伤害；
        //注意：当玩家的属性更新了，该伤害数值会发生变化；因此需要及时更新UI：
        //这个更新，会通过广播的方式实现：
        txtSkillDamage.text = mySkill.SkillDamage.ToString();
        txtSkillName.text = mySkill.skillName;


        return;
    }

    //取消装备的方法：
    public void UnequipMyself()
    {
        isSlotted = false;

        //加载封锁的美术资源；
        unusedMask.gameObject.SetActive(true);

        myEquipment.Unequip();

        //处理相关的sprite:
        // imgEquipment.sprite
        // imgSkill.sprite
        
        txtCost.text = "";
        txtSkillDamage.text = "";

    }


    //玩家属性变化之后，更新UI显示的广播：
    //该广播应该在装备装备 / 道具使用后调用；因为只有这个时候该UI组件还存活；
    private void UpdateAllSkillDamageText()
    {
        txtSkillDamage.text = mySkill.SkillDamage.ToString();
        
    }


    private void UpdateEquipmentUI(Equipment target)
    {
        if(txtDuration != null && myEquipment != null && target == myEquipment)
        {
            //主要就是更新耐久的UI：
            txtDuration.text = $"耐久:{myEquipment.currentDuration}/{myEquipment.maxDuration}";
        }
    }
}
