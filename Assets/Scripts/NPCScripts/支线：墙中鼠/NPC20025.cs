using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC20025: NPCBase
{
    //奖励：20024
    public GameObject reward;

    //激活20018的后继同位置的节点
    public GameObject node20018Next;


    //第二次亮起的灯塔对象在节点20020激活之后就会被触发；


    protected override void OnComplete(int avgId)
    {
        base.OnComplete(avgId);
        reward.SetActive(true);
        node20018Next.SetActive(true);

        GameLevelManager.Instance.avgIndexIsTriggeredDic[avgId] = true;
        
        Destroy(this.gameObject);
    }

    protected override void Awake()
    {
        base.Awake();
        avgId = 2110;  
        
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