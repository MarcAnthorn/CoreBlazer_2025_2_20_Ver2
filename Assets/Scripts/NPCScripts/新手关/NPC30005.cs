using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC30005 : NPCBase
{
    public GameObject monster1001;
    protected override void OnComplete(int avgId)
    {
        base.OnComplete(avgId);
        Destroy(this.gameObject);
        monster1001.gameObject.SetActive(true);        
    }

    protected override void Awake()
    {
        base.Awake();
        avgId = 1105;
        GameLevelManager.Instance.avgIndexIsTriggeredDic.Add(avgId, false);
    }


}
