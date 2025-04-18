using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC30005 : NPCBase
{
    protected override void OnComplete(int avgId)
    {
        base.OnComplete(avgId);
        Destroy(this.gameObject);
        LoadSceneManager.Instance.LoadSceneAsync("ShelterScene");
    }

    void Awake()
    {
        avgId = 1105;
        GameLevelManager.Instance.avgIndexIsTriggeredDic.Add(avgId, false);
    }


}
