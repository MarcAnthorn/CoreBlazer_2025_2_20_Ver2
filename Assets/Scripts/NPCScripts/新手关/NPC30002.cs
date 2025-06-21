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

        //分配：101
        ItemManager.Instance.AddItem(101);
        
        Destroy(this.gameObject);
        
    } 


    protected override void Awake()
    {
        base.Awake();
        avgId = 1102;
    }
    
}
