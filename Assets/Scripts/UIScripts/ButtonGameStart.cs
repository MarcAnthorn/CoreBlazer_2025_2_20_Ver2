using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ButtonGameStart : MonoBehaviour
{
    public Button btnStartGame;
    void Start()
    {
        GameLevelManager.Instance.gameLevelType = E_GameLevelType.Tutorial;
        btnStartGame.onClick.AddListener(()=>{

        EventHub.Instance.EventTrigger<UnityAction>("ShowMask", ()=>{
            //播放avg，结束之后直接去新手关：
            DialogueOrderBlock ob = LoadManager.Instance.orderBlockDic[1106];
            UIManager.Instance.HidePanel<StartPanel>();
            var panel = UIManager.Instance.ShowPanel<AVGPanel>();
            panel.orderBlock = ob;
            panel.callback = OnComplete;
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
