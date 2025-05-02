using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainPanel : BasePanel
{
    public Button btnSetting;
    public Button btnQuit;
    protected override void Init()
    {
        btnSetting.onClick.AddListener(()=>{
            UIManager.Instance.ShowPanel<SettingPanel>();

            //冻结玩家：
            EventHub.Instance.EventTrigger<bool>("Freeze", true);
        });

        btnQuit.onClick.AddListener(()=>{

            EventHub.Instance.EventTrigger<bool>("Freeze", true);


            //显示是否退出游戏的Tip:
            UIManager.Instance.ShowPanel<TipPanel>().setOnConfirmAction += () => {
                //退出游戏：
                Debug.LogWarning("已存档并且退出游戏");


            };
        });
    }
}
