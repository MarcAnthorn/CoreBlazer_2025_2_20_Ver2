using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerAttributePanel : BasePanel
{
    public TextMeshProUGUI txtStrength;
    public TextMeshProUGUI txtSpeed;

    public TextMeshProUGUI txtDefense;

    public TextMeshProUGUI txtCriticalRate;

    public TextMeshProUGUI txtComboRate;

    public TextMeshProUGUI txtDodgeRate;

    public TextMeshProUGUI txtCriticalMultiplier;



    protected override void Init()
    {
        UpdateAttributeText();
        if(Input.anyKeyDown)
        {
            UIManager.Instance.HidePanel<PlayerAttributePanel>();
        }
    }

   
    private void UpdateAttributeText()
    {
        txtStrength.text = $"力量：{(int)PlayerManager.Instance.player.STR.value}";
        txtSpeed.text = $"速度：{(int)PlayerManager.Instance.player.SPD.value}";

        txtDefense.text = $"防御：{(int)PlayerManager.Instance.player.DEF.value}";

        txtCriticalRate.text = $"暴击率：{(int)PlayerManager.Instance.player.CRIT_Rate.value}";

        txtComboRate.text = $"连击率：{(int)PlayerManager.Instance.player.HIT.value}";

        txtDodgeRate.text = $"闪避率：{(int)PlayerManager.Instance.player.AVO.value}";

        txtCriticalMultiplier.text = $"暴击伤害：{(int)PlayerManager.Instance.player.CRIT_DMG.value}"; 

    

    }

}
