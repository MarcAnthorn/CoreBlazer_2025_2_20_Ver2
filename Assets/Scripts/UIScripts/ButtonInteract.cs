using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

[RequireComponent(typeof(EventTrigger))]
public class ButtonInteract : MonoBehaviour
{
    private EventTrigger trigger;
    private EventTrigger.Entry entryEnter = new EventTrigger.Entry();
    private EventTrigger.Entry entryExit = new EventTrigger.Entry();

    private Vector3 transformVector = new Vector3(1.1f, 1.1f, 1.1f);
    private Vector3 originalVector = new Vector3(1f, 1f, 1f);

    private Button btnSelf;

    private void Awake()
    {
        EnterExitTransform();
        ClickTransform();
    }


    //鼠标移除按钮范围之后的动态效果
    private void EnterExitTransform()
    {
        trigger = this.GetComponent<EventTrigger>();
        entryEnter.eventID = EventTriggerType.PointerEnter;
        entryEnter.callback.AddListener((data) =>
        {
            SoundEffectManager.Instance.PlaySoundEffect("ButtonEntry");
            transform.LeanScale(transformVector, 0.2f);
        });
        trigger.triggers.Add(entryEnter);

        entryExit.eventID = EventTriggerType.PointerExit;
        entryExit.callback.AddListener((data) =>
        {
            transform.LeanScale(originalVector, 0.2f);
        });
        trigger.triggers.Add(entryExit);
    }


    //点击按钮之后的动态反馈
    private void ClickTransform()
    {
      
        btnSelf = this.GetComponent<Button>();
        btnSelf.onClick.AddListener(() =>
        {
            Debug.Log("Clicked!");

            SoundEffectManager.Instance.PlaySoundEffect("ButtonClickConfirm");
            if (!btnSelf.gameObject.name.Equals("GameStartButton") && !btnSelf.gameObject.name.Equals("BackButton"))
            {
                btnSelf.interactable = false;
                LeanTween.delayedCall(0.3f, () => {
                    btnSelf.interactable = true;
                });
            }
            transform.LeanScale(originalVector, 0.13f).setLoopPingPong(1).setOnComplete(() =>
            {
                //使用事件中心进行事件分发；
                switch (this.gameObject.name)
                {
                    case "SettingButton":
                        SoundEffectManager.Instance.PlaySoundEffect("PanelRevealVer1");
                        UIManager.Instance.ShowPanel<SettingPanel>();
                        break;
                    case "QuitButton":
                        SoundEffectManager.Instance.PlaySoundEffect("PanelRevealVer1");
                        var tip = UIManager.Instance.ShowPanel<TipPanel>();
                        tip.SetTipText("是否退出游戏？");
                        tip.setOnCancelAction = OnTipCancel;
                        tip.setOnConfirmAction = OnTipComfirm;
                        break;
                    case "AboutButton":
                        SoundEffectManager.Instance.PlaySoundEffect("PanelRevealVer1");
                        UIManager.Instance.ShowPanel<WarningPanel>().SetWarningText("制作人员：\n策划：咕咕咕 王润霖 \n被窝猫 朔风闽江\n程序：Ancheryy Marc 核爆蛋\n美术：年丰巷凩工坊 冻鱼不列 洛提亚\n感谢游玩！", Color.white);
                        break;
                    default:
                        break;
                }

            });

        });
    }

    //退出的事件注册：
    public void OnTipComfirm()
    {
        Application.Quit();
    }

    public void OnTipCancel()
    {
        UIManager.Instance.HidePanel<TipPanel>();
    }




}
