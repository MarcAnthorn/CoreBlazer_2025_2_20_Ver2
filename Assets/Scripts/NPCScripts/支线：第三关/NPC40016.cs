using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC40016 : NPCBase
{
  
    private static System.Random rng = new System.Random(); // 线程安全更高，可替换为 UnityEngine.Random

    //宝箱：20022
    public GameObject reward;

    protected override void OnComplete(int avgId)
    {
        base.OnComplete(avgId);

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
        if (!IDAllocatorFor40016.Instance.TryGetUniqueID(out avgId))
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


public class IDAllocatorFor40016
{
    public List<int> availableIDs = new List<int> { 1201, 1202, 1203, 1204, 1205};
    private System.Random rng = new System.Random();

    public static IDAllocatorFor40016 _instance;
    public static IDAllocatorFor40016 Instance => _instance ??= new IDAllocatorFor40016();

    private IDAllocatorFor40016() { }

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