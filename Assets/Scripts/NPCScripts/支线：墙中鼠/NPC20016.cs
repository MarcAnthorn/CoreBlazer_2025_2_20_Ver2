using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC20016 : NPCBase
{
    public static List<int> availableIDs = new List<int> { 2102, 2103, 2104, 2105, 2106, 2107 };
    private static System.Random rng = new System.Random(); // 线程安全更高，可替换为 UnityEngine.Random

    //类间共享的，已触发的演出数量；4个之后，会激活20018:
    public static int triggeredCount = 0;

    //20018对象：
    public GameObject node20018;

    //宝箱：
    public GameObject reward;

    protected override void OnComplete(int avgId)
    {
        base.OnComplete(avgId);

        triggeredCount++;
        if(triggeredCount == 4)
        {
            node20018.SetActive(true);
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
        if (!IDAllocator.Instance.TryGetUniqueID(out avgId))
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


public class IDAllocator
{
    private List<int> availableIDs = new List<int> { 2102, 2103, 2104, 2105, 2106, 2107 };
    private System.Random rng = new System.Random();

    private static IDAllocator _instance;
    public static IDAllocator Instance => _instance ??= new IDAllocator();

    private IDAllocator() { }

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


