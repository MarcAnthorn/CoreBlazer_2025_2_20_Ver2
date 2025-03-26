using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayCasePanel : BasePanel
{
    public Button btnBack;
    public Button btnTarot;
    public Button btnItem;
    public Button btnKaidan;
    public Button btnSideQuest;


    protected override void Init()
    {
        btnBack.onClick.AddListener(()=>{
            UIManager.Instance.HidePanel<DisplayCasePanel>();
            EventHub.Instance.EventTrigger<bool>("Freeze", false);
        });

        btnTarot.onClick.AddListener(()=>{
            UIManager.Instance.ShowPanel<TarotDisplayPanel>();   
        });

        btnItem.onClick.AddListener(()=>{
            UIManager.Instance.ShowPanel<ItemDisplayPanel>();   
        });

        btnKaidan.onClick.AddListener(()=>{
            UIManager.Instance.ShowPanel<KaidanDisplayPanel>();   
        });

        btnSideQuest.onClick.AddListener(()=>{
            UIManager.Instance.ShowPanel<SideQuestDisplayPanel>();
        });
    }
}
