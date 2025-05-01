using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestNPCFor1106AVG  : NPCBase
{
    protected override void OnComplete(int avgId)
    {
        base.OnComplete(avgId);
        Destroy(this.gameObject);
        LoadSceneManager.Instance.LoadSceneAsync("ShelterScene");
    }

    void Awake()
    {
        avgId = 1106;
        GameLevelManager.Instance.avgIndexIsTriggeredDic.Add(avgId, false);
    }


}
