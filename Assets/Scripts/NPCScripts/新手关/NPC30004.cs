using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC30004 : NPCBase
{
    public GameObject door10012;
    public GameObject monster1001;
    protected override void OnComplete(int avgId)
    {
        base.OnComplete(avgId);
        Destroy(door10012);
        Destroy(this.gameObject);

        //触发怪物追逐：
        monster1001.gameObject.SetActive(true);
        
    }

    protected override void Awake()
    {
        base.Awake();
        avgId = 1104;
        GameLevelManager.Instance.avgIndexIsTriggeredDic.Add(avgId, false);
    }
}
