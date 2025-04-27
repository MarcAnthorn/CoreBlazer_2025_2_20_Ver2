using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuffCheckPanel : BasePanel
{
    public Image imgBuff;
    public TextMeshProUGUI txtRemainingLayerCount;
    public TextMeshProUGUI txtOverlyingLayerCount; 
    public TextMeshProUGUI txtBuffDescription;

    protected override void Init()
    {
        
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0) || Input.anyKeyDown)
        {
            UIManager.Instance.HidePanel<BuffCheckPanel>();
        }
    }

    //初始化内部显示的方法：
    public void InitPanel(BattleBuff _buff)
    {
        imgBuff.sprite = Resources.Load<Sprite>(_buff.buffIconPath);
        txtRemainingLayerCount.text = _buff.lastTurns.ToString();
        txtOverlyingLayerCount.text = _buff.GetOverlyingCount().ToString();
        txtBuffDescription.text = _buff.buffDescriptionText;
    }

}
