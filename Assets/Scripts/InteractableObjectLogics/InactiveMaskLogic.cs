using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InactiveMaskLogic : MonoBehaviour
{
    private Button btnSelf;

    void Awake()
    {
        btnSelf = this.GetComponent<Button>();
    }
    void Start()
    {
        btnSelf.onClick.AddListener(()=>{
            UIManager.Instance.ShowPanel<WarningPanel>().SetWarningText("当前未持有该道具");
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
