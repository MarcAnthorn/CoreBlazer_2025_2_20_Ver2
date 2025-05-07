using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TestShelterStart : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        EventHub.Instance.EventTrigger<UnityAction>("HideMask", ()=>{
            SoundEffectManager.Instance.PlayMusic("安全屋循环BGM");
            EventHub.Instance.EventTrigger<bool>("Freeze", false);
            //关闭当前的背包面板（防止使用传送道具之后面板残留）
            UIManager.Instance.HidePanel<InventoryPanel>();
            
            //初始化玩家的SceneIndex:
            PlayerManager.Instance.playerSceneIndex = E_PlayerSceneIndex.Shelter;


            //针对不同的目标场景，播放不同的avg：
            if(GameLevelManager.Instance.gameLevelType == E_GameLevelType.First && !GameLevelManager.Instance.avgShelterIsTriggered[E_GameLevelType.First])
            {
                GameLevelManager.Instance.avgShelterIsTriggered[E_GameLevelType.First] = true;
                DialogueOrderBlock ob = LoadManager.Instance.orderBlockDic[1108];
                var panel = UIManager.Instance.ShowPanel<AVGPanel>();
                panel.orderBlock = ob;
                EventHub.Instance.EventTrigger<bool>("Freeze", true);
            }

            else if(GameLevelManager.Instance.gameLevelType == E_GameLevelType.Second && !GameLevelManager.Instance.avgShelterIsTriggered[E_GameLevelType.Second])
            {
                GameLevelManager.Instance.avgShelterIsTriggered[E_GameLevelType.Second] = true;
                DialogueOrderBlock ob = LoadManager.Instance.orderBlockDic[1113];
                var panel = UIManager.Instance.ShowPanel<AVGPanel>();
                panel.orderBlock = ob;
                EventHub.Instance.EventTrigger<bool>("Freeze", true);
            }

            else if(GameLevelManager.Instance.gameLevelType == E_GameLevelType.Third && !GameLevelManager.Instance.avgShelterIsTriggered[E_GameLevelType.Third])
            {
                GameLevelManager.Instance.avgShelterIsTriggered[E_GameLevelType.Third] = true;
                DialogueOrderBlock ob = LoadManager.Instance.orderBlockDic[1118];
                var panel = UIManager.Instance.ShowPanel<AVGPanel>();
                panel.orderBlock = ob;
                EventHub.Instance.EventTrigger<bool>("Freeze", true);
            }


        });
    }

}
