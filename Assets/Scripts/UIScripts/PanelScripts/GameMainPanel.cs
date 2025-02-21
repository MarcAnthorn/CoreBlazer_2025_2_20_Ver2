using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameMainPanel : BasePanel
{
    
    private List<Button> optionList; 
    public Slider sliderHealth;
    public Slider sliderSanity;
    public Slider sliderLight;

    public TextMeshProUGUI txtHealth;
    public TextMeshProUGUI txtSanity;
    public TextMeshProUGUI txtLight;
    public TextMeshProUGUI txtEventDescription;
    public TextMeshProUGUI txtRiddleTip; 
    public Button btnToGodItem;
    public Button btnToCommonItem;
    public Button btnSetting;
    public Button btnQuit;
    public RectTransform rtOptionsContainer;

    public Transform rightSection;

    //当前的事件对象
    private Event currentEvent;

    protected override void Init()
    {
        //默认显示的是神明道具面板；
        UIManager.Instance.ShowPanel<GodItemPanel>().transform.SetParent(rightSection, false);
        btnToGodItem.onClick.AddListener(()=>{
            UIManager.Instance.ShowPanel<GodItemPanel>().transform.SetParent(rightSection, false);
            UIManager.Instance.HidePanel<CommonItemPanel>();
        });

        btnToCommonItem.onClick.AddListener(()=>{
            UIManager.Instance.ShowPanel<CommonItemPanel>().transform.SetParent(rightSection, false);
            UIManager.Instance.HidePanel<GodItemPanel>();
        });

        EventHub.Instance.AddEventListener("EventFetcher", EventFetcher);
    }

    private void OnDestroy()
    {
        EventHub.Instance.RemoveEventListener("EventFetcher", EventFetcher);
    }

    //从外部动态获取当前面板需要显示的事件信息
    public void EventFetcher()
    {

    }




   
}
