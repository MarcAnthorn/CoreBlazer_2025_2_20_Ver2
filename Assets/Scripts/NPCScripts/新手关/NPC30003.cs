using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class NPC30003  : NPCBase
{
    public GameObject npc30004;
    public CinemachineVirtualCamera cameraForDoor;  //默认初始优先级是0；不争夺主相机
    public GameObject doorLightSource;
    protected override void OnComplete(int avgId)
    {
        base.OnComplete(avgId);
        npc30004.SetActive(true);

        //激活光源
        doorLightSource.gameObject.SetActive(true);

        cameraForDoor.Priority = 11;        //超越主相机优先级10；切换相机
        EventHub.Instance.EventTrigger<bool>("Freeze", true);   //冻结玩家；
        LeanTween.delayedCall(2f, ()=>{
            cameraForDoor.Priority = 0;
            EventHub.Instance.EventTrigger<bool>("Freeze", false);   //解冻玩家；
            UIManager.Instance.ShowPanel<WarningPanel>().SetWarningText("【回头看看吧】");
        });
       
            

        Destroy(this.gameObject);
    }

   protected override void Awake()
    {
        base.Awake();
        avgId = 1103;
        GameLevelManager.Instance.avgIndexIsTriggeredDic.Add(avgId, false);
    }
}