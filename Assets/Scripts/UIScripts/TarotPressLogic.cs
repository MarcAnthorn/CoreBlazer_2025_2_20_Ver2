using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TarotPressLogic : MonoBehaviour
{
    public Button btnSelf;

    void Awake()
    {
        btnSelf = this.gameObject.GetComponent<Button>();
    }

    void Start()
    {
        btnSelf.onClick.AddListener(()=>{
            UIManager.Instance.ShowPanel<TarotCheckPanel>();
        });
    }
}
