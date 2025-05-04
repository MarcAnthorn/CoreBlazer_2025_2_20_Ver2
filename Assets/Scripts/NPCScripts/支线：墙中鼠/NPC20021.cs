using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC20021 : NPCBase
{
    //后续节点：
    public GameObject reward;
   


    protected override void OnComplete(int avgId)
    {
        base.OnComplete(avgId);
        reward.SetActive(true);
        Destroy(this.gameObject);
    }

    private void Awake()
    {
        avgId = 2111;  

        
        if(GameLevelManager.Instance.avgIndexIsTriggeredDic[avgId])
        {
            OnComplete(avgId);
            this.gameObject.SetActive(false);
            return;
        }
        
        GameLevelManager.Instance.avgIndexIsTriggeredDic.Add(avgId, false);
    }

} 