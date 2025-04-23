using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class SkillCheckPanel : BasePanel
{
    //显示的Skill Image：
    public Image imgSkill; 
    public TextMeshProUGUI txtCost;
    public TextMeshProUGUI txtDamage;
    public TextMeshProUGUI txtBuff;
    protected override void Init()
    {
       
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0) || Input.anyKeyDown)
        {
            UIManager.Instance.HidePanel<SkillCheckPanel>();
        }
    }

    //初始化UI的方法：
    //因为该面板不是执行Skill的地方，所以不需要持有对应的Skill（id or 实例）
    //只会进行UI信息的更新：
    public void InitInfo(Skill _skill)
    {   
        //更新图标：
        string path = Path.Combine("ArtResources", _skill.skillIconPath);
        imgSkill.sprite = Resources.Load<Sprite>(path);

        //更新相关的文本：
        txtCost.text = _skill.skillCost.ToString();
        txtBuff.text = _skill.skillBuffText;
        txtDamage.text = _skill.skillDamageText;
    }

}
