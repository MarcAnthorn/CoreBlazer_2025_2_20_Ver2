using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC20025: NPCBase
{
    //奖励：20024
    public GameObject reward;

    //激活20018的后继同位置的节点
    public GameObject node20018Next;


    //第二次亮起的灯塔对象在节点20020激活之后就会被触发；


    protected override void OnComplete(int avgId)
    {
        base.OnComplete(avgId);
        reward.SetActive(true);
        node20018Next.SetActive(true);

        Destroy(this.gameObject);
    }

    private void Awake()
    {
        avgId = 2110;  
        
        // if(GameLevelManager.Instance.avgIndexIsTriggeredDic[avgId])
        // {
        //     OnComplete(avgId);
        //     this.gameObject.SetActive(false);
        //     return;
        // }

        // GameLevelManager.Instance.avgIndexIsTriggeredDic.Add(avgId, false);
    }

} 