using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.Events;

public class ButtonGameContinue : MonoBehaviour
{
    private Button btnSelf;
    void Awake()
    {
        btnSelf = this.GetComponent<Button>();
    }
    void Start()
    {
        // 判断是否存在存档文件
        string savePath = Application.persistentDataPath + "/player_save.json";
        if (!File.Exists(savePath))
        {       
            if (btnSelf != null)
                btnSelf.interactable = false;
        }

        btnSelf.onClick.AddListener(() => {
            //从上次的存档中加载内容：
            SaveManager.Instance.LoadGame();

            EventHub.Instance.EventTrigger<bool>("TestClearFunction", false);
            
            //并且将玩家加载到对应的安全屋中：
            EventHub.Instance.EventTrigger<UnityAction>("ShowMask", ()=>{
                UIManager.Instance.HidePanel<StartPanel>();
                LoadSceneManager.Instance.LoadSceneAsync("ShelterScene");
                UIManager.Instance.ShowPanel<MainPanel>();
            });
        });
    }

}
