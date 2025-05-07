using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC20017 : NPCBase
{
    //六个20016对象，在当前对象触发之后激活：
    public List<GameObject> aftermathNodes = new List<GameObject>();


    protected override void OnComplete(int avgId)
    {
        base.OnComplete(avgId);

        foreach(var obj in aftermathNodes)
        {
            obj.gameObject.SetActive(true);
        }

        GameLevelManager.Instance.avgIndexIsTriggeredDic[avgId] = true;

        Debug.Log($"avgId:{avgId}, 当前触发情况：{GameLevelManager.Instance.avgIndexIsTriggeredDic[avgId]}");

        Destroy(this.gameObject);
    }

    protected override void Awake()
    {
        base.Awake();
        avgId = 2101;  

        GameLevelManager.Instance.DebugAVGInfo();

        //复原20016的List：
        var list = IDAllocator.Instance.availableIDs;
        if(!list.Contains(2102))
            IDAllocator.Instance.availableIDs.Add(2102);
        if(!list.Contains(2103))
            IDAllocator.Instance.availableIDs.Add(2103);
        if(!list.Contains(2104))
            IDAllocator.Instance.availableIDs.Add(2104);
        if(!list.Contains(2105))
            IDAllocator.Instance.availableIDs.Add(2105);
        if(!list.Contains(2106))
            IDAllocator.Instance.availableIDs.Add(2106);
        if(!list.Contains(2107))
            IDAllocator.Instance.availableIDs.Add(2107);
        NPC20016.triggeredCount = 0;

        //自己激活时，如果上一次死亡我触发过，那么直接调用OnComplete，然后将自己失活返回；
        if(GameLevelManager.Instance.avgIndexIsTriggeredDic.ContainsKey(avgId) && GameLevelManager.Instance.avgIndexIsTriggeredDic[avgId]) 
        {
            Debug.LogWarning($"AVG 已跳过，id为：{avgId}");

            OnComplete(avgId);
            this.gameObject.SetActive(false);
            return;
        }

       GameLevelManager.Instance.avgIndexIsTriggeredDic.TryAdd(avgId, false);
    }

} 