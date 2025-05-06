using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC30017 : NPCBase
{
    protected override void OnComplete(int avgId)
    {
        base.OnComplete(avgId);
        GameLevelManager.Instance.avgIndexIsTriggeredDic[avgId] = true;
        Destroy(this.gameObject);
    }

    protected override void Awake()
    {
        base.Awake();
        avgId = 3101;  

        
        //自己激活时，如果上一次死亡我触发过，那么直接调用OnComplete，然后将自己失活返回；
        if(GameLevelManager.Instance.avgIndexIsTriggeredDic.ContainsKey(avgId) && GameLevelManager.Instance.avgIndexIsTriggeredDic[avgId]) 
        {
            OnComplete(avgId);
            this.gameObject.SetActive(false);
            return;
        }
        
       GameLevelManager.Instance.avgIndexIsTriggeredDic.TryAdd(avgId, false);
    }

} 
