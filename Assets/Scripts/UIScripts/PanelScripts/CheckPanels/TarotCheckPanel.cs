using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TarotCheckPanel : BasePanel
{
    public Image imgCurrentTarot;
    public TextMeshProUGUI txtCurrentDescription;
    public Button btnExit;
    protected override void Init()
    {
        btnExit.onClick.AddListener(()=>{
            UIManager.Instance.HidePanel<TarotCheckPanel>();
        });
    }

    public void InitItemInfo(Item _item)
    {
        string rootPath = Path.Combine("ArtResources", "Tarot", _item.id.ToString());
        imgCurrentTarot.sprite = Resources.Load<Sprite>(rootPath);
        txtCurrentDescription.text = _item.description;
    }

}
