using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC30004 : NPCBase
{
    public GameObject door10012;
    protected override void OnComplete(int avgId)
    {
        base.OnComplete(avgId);
        Destroy(door10012);
        Destroy(this.gameObject);
    }

    void Awake()
    {
        avgId = 1104;
        GameLevelManager.Instance.avgIndexIsTriggeredDic.Add(avgId, false);
    }
}
