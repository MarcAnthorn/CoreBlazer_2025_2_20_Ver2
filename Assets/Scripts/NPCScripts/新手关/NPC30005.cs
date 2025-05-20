using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC30005 : NPCBase
{
    protected override void OnComplete(int avgId)
    {
        base.OnComplete(avgId);
        Destroy(this.gameObject);
        GameLevelManager.Instance.gameLevelType = E_GameLevelType.First;
        LoadSceneManager.Instance.LoadSceneAsync("ShelterScene");
    }

    protected override void Awake()
    {
        base.Awake();
        avgId = 1105;
        GameLevelManager.Instance.avgIndexIsTriggeredDic.Add(avgId, false);
    }


}
