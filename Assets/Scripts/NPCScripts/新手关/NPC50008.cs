using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC50008 : NPCBase
{
  protected override void OnComplete(int avgId)
    {
        base.OnComplete(avgId);
        Destroy(this.gameObject);
    }

    protected override void Awake()
    {
        base.Awake();
        avgId = 2045;
        GameLevelManager.Instance.avgIndexIsTriggeredDic.Add(avgId, false);
    }
}
