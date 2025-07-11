using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingPanel : BasePanel
{
    public Button btnExit;
    public Button btnSave;
    public Slider sliderVolumn;
    protected override void Init()
    {
        TextManager.SetContentFont(this.gameObject);
        sliderVolumn.value = SoundEffectManager.Instance.SoundEffectVolume;
        btnSave.onClick.AddListener(()=>{
            //此处进行存档；
        });

        sliderVolumn.onValueChanged.AddListener((value)=>{
            SoundEffectManager.Instance.SetSoundVolume(value);
            SoundEffectManager.Instance.SetMusicVolume(value);
        });

        btnExit.onClick.AddListener(()=>{
            UIManager.Instance.HidePanel<SettingPanel>();

            EventHub.Instance.EventTrigger<bool>("Freeze", false);
        });
    }
    void Update()
    {
        // 按下ESC键关闭面板
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UIManager.Instance.HidePanel<SettingPanel>();
            EventHub.Instance.EventTrigger<bool>("Freeze", false);
        }
    }

}
