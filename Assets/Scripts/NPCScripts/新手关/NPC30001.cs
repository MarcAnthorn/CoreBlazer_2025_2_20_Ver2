using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC30001 : NPCBase
{
    protected override void OnComplete(int avgId)
    {
        base.OnComplete(avgId);
        Destroy(this.gameObject);
    }


    protected override void Awake()
    {
        base.Awake();
        avgId = 1101;
        GameLevelManager.Instance.avgIndexIsTriggeredDic.Add(avgId, false);
    }




}
