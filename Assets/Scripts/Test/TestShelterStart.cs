using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TestShelterStart : MonoBehaviour
{
    void Awake()
    {
        // //关闭灯光：
        // EventHub.Instance.EventTrigger("TriggerLight", false);
    }
    
    void Start()
    {
        // 重置所有IDAllocator
        IDAllocator.Instance.Reset();
        IDAllocatorFor30016.Instance.Reset();
        IDAllocatorFor40016.Instance.Reset();
        IDAllocatorForTarot.Instance.Reset();

        //回复血量：
        PlayerManager.Instance.player.HP.value = PlayerManager.Instance.player.HP.value_limit;
        EventHub.Instance.EventTrigger("UpdateAllUIElements");

        UIManager.Instance.ShowPanel<MainPanel>();
        
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
                UIManager.Instance.ShowPanel<AVGPanel>().InitAVG(1108);
                EventHub.Instance.EventTrigger<bool>("Freeze", true);
            }

            //完成第一关，并且和第一关/大地图的格赫罗斯交互后。返回安全屋才能触发。（两个条件同时满足）
            else if(GameLevelManager.Instance.gameLevelType == E_GameLevelType.Second && !GameLevelManager.Instance.avgShelterIsTriggered[E_GameLevelType.Second] && GameLevelManager.Instance.avgIndexIsTriggeredDic.ContainsKey(1112) && GameLevelManager.Instance.avgIndexIsTriggeredDic[1112])
            {
                GameLevelManager.Instance.avgShelterIsTriggered[E_GameLevelType.Second] = true;
                UIManager.Instance.ShowPanel<AVGPanel>().InitAVG(1113);
                EventHub.Instance.EventTrigger<bool>("Freeze", true);
            }

            else if(GameLevelManager.Instance.gameLevelType == E_GameLevelType.Third && !GameLevelManager.Instance.avgShelterIsTriggered[E_GameLevelType.Third])
            {
                GameLevelManager.Instance.avgShelterIsTriggered[E_GameLevelType.Third] = true;
                UIManager.Instance.ShowPanel<AVGPanel>().InitAVG(1118);
                EventHub.Instance.EventTrigger<bool>("Freeze", true);
            }


        });


        //自动存档：
        SaveManager.Instance.SaveGame();
    }

}
