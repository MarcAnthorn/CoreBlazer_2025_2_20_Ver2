using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KaidanDisplayPanel : BasePanel
{
    public Button btnLastPage;
    public Button btnNextPage;
    public Button btnBack;   
    public List<GameObject> kaidanList = new List<GameObject>();
    protected override void Init()
    {
        btnLastPage.onClick.AddListener(()=>{

        });

        btnNextPage.onClick.AddListener(()=>{

        });

        btnBack.onClick.AddListener(()=>{
            UIManager.Instance.HidePanel<KaidanDisplayPanel>();
        });
    }

  

}
