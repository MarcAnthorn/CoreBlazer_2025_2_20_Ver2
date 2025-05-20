using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainPanel : BasePanel
{
    public Button btnSetting;
    public Button btnQuit;
    public TextMeshProUGUI txtSan;
    public Image imgHp;
    public Image imgLight; 
    private float maxHpWidth;
    private float maxLightWidth;
    protected override void Init()
    {
        UpdateAttributeUI();
        btnSetting.onClick.AddListener(()=>{
            UIManager.Instance.ShowPanel<SettingPanel>();

            //冻结玩家：
            EventHub.Instance.EventTrigger<bool>("Freeze", true);
        });

        btnQuit.onClick.AddListener(()=>{

            EventHub.Instance.EventTrigger<bool>("Freeze", true);


            //显示是否退出游戏的Tip:
            var tipPanel = UIManager.Instance.ShowPanel<TipPanel>();
            tipPanel.SetTipText("是否退出游戏？");
        
            tipPanel.setOnConfirmAction += () =>
            {
                //退出游戏：
                Application.Quit();                
            };

            tipPanel.setOnCancelAction +=  () => {
                UIManager.Instance.HidePanel<TipPanel>();
                EventHub.Instance.EventTrigger<bool>("Freeze", false);
            };
        });
    }


    protected override void Awake()
    {
        base.Awake();
        EventHub.Instance.AddEventListener("UpdateAllUIElements", UpdateAttributeUI);

        maxHpWidth = imgHp.rectTransform.rect.width;
        maxLightWidth = imgLight.rectTransform.rect.width;

    }

    void OnDestroy()
    {
        EventHub.Instance.RemoveEventListener("UpdateAllUIElements", UpdateAttributeUI);
    }

    private void UpdateAttributeUI()
    {
        var player = PlayerManager.Instance.player;
        txtSan.text = ((int)player.SAN.value).ToString();

        float hpRatio = Mathf.Clamp01(player.HP.value / player.HP.value_limit);
        RectTransform rt = imgHp.rectTransform;
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, maxHpWidth * hpRatio);


        float lightRatio = Mathf.Clamp01(player.LVL.value / player.LVL.value_limit);
        RectTransform rt_ = imgLight.rectTransform;
        rt_.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, maxLightWidth * lightRatio);
    }
}
