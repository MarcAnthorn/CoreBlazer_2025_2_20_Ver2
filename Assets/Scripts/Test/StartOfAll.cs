using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StartOfAll : MonoBehaviour
{


    // Start is called before the first frame update
    void Start()
    {
        
        SoundEffectManager.Instance.PlayMusic("第一关BGM");

        //防止面板残留：
        UIManager.Instance.HidePanel<MainPanel>();

        UIManager.Instance.ShowPanel<StartPanel>();
    
    }

    // Update is called once per frame
    void Update()
    {

    }
}
