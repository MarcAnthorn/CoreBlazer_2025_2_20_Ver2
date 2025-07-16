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
        PlayerManager.Instance.player.AddAttrValue(AttributeType.LVL, 20);

        //分配：101
        ItemManager.Instance.AddItem(101);

        PoolManager.Instance.SpawnFromPool("Panels/WarningPanel", UIManager.Instance.CanvasTransform).gameObject.GetComponent<WarningPanel>().SetWarningText($"按下ESC键打开背包");
        
        Destroy(this.gameObject);
        
    } 


    protected override void Awake()
    {
        base.Awake();
        avgId = 1102;
    }
    
}
