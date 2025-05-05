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
            
            //初始化玩家的SceneIndex:
            PlayerManager.Instance.playerSceneIndex = E_PlayerSceneIndex.Shelter;

        });
    }

}
