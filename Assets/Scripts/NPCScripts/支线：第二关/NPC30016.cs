using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC30016 : NPCBase
{
  
 private static System.Random rng = new System.Random(); // 线程安全更高，可替换为 UnityEngine.Random

    //类间共享的，已触发的演出数量；4个之后，会激活30018:
    public static int triggeredCount = 0;

    //30018对象：
    public GameObject node30018;

    //宝箱：20022
    public GameObject reward;

    protected override void OnComplete(int avgId)
    {
        base.OnComplete(avgId);

        triggeredCount++;
        if(triggeredCount == 4)
        {
            node30018.SetActive(true);
        }

        //激活之后，将标识记为true:
        GameLevelManager.Instance.avgIndexIsTriggeredDic[avgId] = true;
        Debug.Log($"avgId:{avgId}, 当前触发情况：{GameLevelManager.Instance.avgIndexIsTriggeredDic[avgId]}");

        //同时：将原地的宝箱激活：
        reward.SetActive(true);

        Destroy(this.gameObject);
    }

    protected override void Awake()
    {
        base.Awake();
        AssignUniqueID();
    }

    void AssignUniqueID()
    {
        if (!IDAllocatorFor30016.Instance.TryGetUniqueID(out avgId))
        {
            Debug.LogError("没有可用的ID分配给对象: " + gameObject.name);
            return;
        }

        // 检查是否已经触发过
        if (GameLevelManager.Instance.avgIndexIsTriggeredDic.TryGetValue(avgId, out bool triggered) && triggered)
        {
            Debug.LogWarning($"AVG 已跳过，id为：{avgId}");

            OnComplete(avgId);
            this.gameObject.SetActive(false);
            return;
        }

        Debug.Log($"{gameObject.name} 获得的ID是：{avgId}");
        GameLevelManager.Instance.avgIndexIsTriggeredDic.TryAdd(avgId, false);
    }

}


public class IDAllocatorFor30016
{
    public List<int> availableIDs = new List<int> { 2202, 2203, 2204, 2205, 2206, 2207 };
    private System.Random rng = new System.Random();

    public static IDAllocatorFor30016 _instance;
    public static IDAllocatorFor30016 Instance => _instance ??= new IDAllocatorFor30016();

    private IDAllocatorFor30016() { }

    public bool TryGetUniqueID(out int id)
    {
        if (availableIDs.Count == 0)
        {
            id = -1;
            return false;
        }

        int index = rng.Next(availableIDs.Count);
        id = availableIDs[index];
        availableIDs.RemoveAt(index);
        return true;
    }
}
