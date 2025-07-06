using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.Events;
using TMPro;

public class ButtonGameContinue : MonoBehaviour
{
    private Button btnSelf;
    public TextMeshProUGUI txtSanity;
    public TextMeshProUGUI txtConversation;
    
    void Awake()
    {
        btnSelf = this.GetComponent<Button>();
        btnSelf.GetComponentInChildren<TextMeshProUGUI>().text = TextManager.Instance.GetText("按钮文本", "主界面", "继续游戏");
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

        btnSelf.onClick.AddListener(() =>
        {
            //从上次的存档中加载内容：


            EventHub.Instance.EventTrigger<bool>("TestClearFunction", false);

            //并且将玩家加载到对应的安全屋中：
            /*
            EventHub.Instance.EventTrigger<UnityAction>("ShowMask", () =>
            {
                UIManager.Instance.HidePanel<StartPanel>();
                LoadSceneManager.Instance.LoadSceneAsync("ShelterScene");
                UIManager.Instance.ShowPanel<MainPanel>();
            });
            */
            UIManager.Instance.HidePanel<StartPanel>();
            UIManager.Instance.ShowPanel<MainPanel>();
            SaveManager.Instance.LoadGame();
        });
    }

}
