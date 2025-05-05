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


        //同时：将原地的宝箱激活：
        reward.SetActive(true);

        Destroy(this.gameObject);
    }

    private void Awake()
    {
        AssignUniqueID();
    }

    void AssignUniqueID()
    {
        if (availableIDs.Count == 0)
        {
            Debug.LogError("没有可用的ID分配给对象: " + gameObject.name);
            return;
        }

        int index = rng.Next(availableIDs.Count);   // 随机选一个
        avgId = availableIDs[index];
        availableIDs.RemoveAt(index);               // 从列表中移除，确保唯一

        // if(GameLevelManager.Instance.avgIndexIsTriggeredDic[avgId])
        // {
        //     OnComplete(avgId);
        //     this.gameObject.SetActive(false);
        //     return;
        // }

        Debug.Log($"{gameObject.name} 获得的ID是：{avgId}");
        // GameLevelManager.Instance.avgIndexIsTriggeredDic.Add(avgId, false);
    }

}


