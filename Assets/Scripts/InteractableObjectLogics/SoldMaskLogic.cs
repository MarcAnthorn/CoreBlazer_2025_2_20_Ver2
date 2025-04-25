using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoldMaskLogic : MonoBehaviour
{
    private Button btnSelf;

    void Awake()
    {
        btnSelf = this.GetComponent<Button>();
    }
    void Start()
    {
        btnSelf.onClick.AddListener(()=>{
            UIManager.Instance.ShowPanel<WarningPanel>().SetWarningText("道具已持有，不可重复购买");
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
