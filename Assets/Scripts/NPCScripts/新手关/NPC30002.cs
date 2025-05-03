using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC30002  : NPCBase
{
    public GameObject door10010;
    protected override void OnComplete(int avgId)
    {
        base.OnComplete(avgId);
        Destroy(door10010);
        PlayerManager.Instance.player.LVL.value += 20;
        Destroy(this.gameObject);
        
    } 

    void Awake()
    {
        avgId = 1102;

    }
}
