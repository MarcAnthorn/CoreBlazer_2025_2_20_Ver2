using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC30003  : NPCBase
{
    public GameObject npc30004;
    protected override void OnComplete(int avgId)
    {
        base.OnComplete(avgId);
        npc30004.SetActive(true);
        Destroy(this.gameObject);
    }

   protected override void Awake()
    {
        base.Awake();
        avgId = 1103;
        GameLevelManager.Instance.avgIndexIsTriggeredDic.Add(avgId, false);
    }
}