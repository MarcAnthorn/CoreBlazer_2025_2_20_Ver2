using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SoldMaskLogic : MonoBehaviour
{
    private Button btnSelf;

    public TextMeshProUGUI txtSelf;
    //flag, == 0 -> "道具已持有，不可重复购买"； == 1 -> "精神值不足，无法购买";
    public int flag = 0;

    void Awake()
    {
        btnSelf = this.GetComponent<Button>();
    }
    void Start()
    {
        btnSelf.onClick.AddListener(()=>{
            if(flag == 0)
            {
                UIManager.Instance.ShowPanel<WarningPanel>().SetWarningText("道具已持有，不可重复购买");
            }
            else
            {
                UIManager.Instance.ShowPanel<WarningPanel>().SetWarningText("精神值不足，无法购买");
            }
            

        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetText(string _text)
    {
        txtSelf.text = _text;
    }
}
