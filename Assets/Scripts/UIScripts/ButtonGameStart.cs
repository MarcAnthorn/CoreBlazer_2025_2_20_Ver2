using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
public class ButtonGameStart : MonoBehaviour
{
    public Button btnStartGame;
    public TextMeshProUGUI txtSanity;
    public TextMeshProUGUI txtConversation;
    void Start()
    {   
        btnStartGame = this.GetComponent<Button>();
        var tmp = btnStartGame.GetComponentInChildren<TextMeshProUGUI>();
        if (tmp != null)
        {
            tmp.text = TextManager.Instance.GetText("按钮文本", "主界面", "新游戏");
        }
        GameLevelManager.Instance.gameLevelType = E_GameLevelType.Tutorial;
        btnStartGame.onClick.AddListener(()=>{

        EventHub.Instance.EventTrigger<UnityAction>("ShowMask", ()=>{
            UIManager.Instance.HidePanel<StartPanel>();
            
            UIManager.Instance.ShowPanel<AVGPanel>().InitAVG(1106, OnComplete);

        });
            
        });
    }


    public void OnComplete(int id)
    {
        PlayerManager.Instance.playerSceneIndex = E_PlayerSceneIndex.Maze;
        //失活所有需要失活的过场景不移除的对象：
        //该方法定义在TestCanvas中，该脚本挂载在Canvas上；
        EventHub.Instance.EventTrigger<bool>("TestClearFunction", false);

        //停止bgm：
        SoundEffectManager.Instance.StopMusic();
        
        //进行场景的切换：
        EventHub.Instance.EventTrigger<UnityAction>("ShowMask", ()=>{
            LoadSceneManager.Instance.LoadSceneAsync("MazeScene");
            UIManager.Instance.ShowPanel<MainPanel>();
        });
    }

}
