using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TarotReward : MonoBehaviour
{
    int tarotId;
    void Awake()
    {
        AssignUniqueID();
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            ItemManager.Instance.AddItem(601);
            Destroy(this.gameObject);
        }


    } 
    


    void AssignUniqueID()
    {
        IDAllocator.Instance.TryGetUniqueID(out tarotId);
        Debug.Log($"{gameObject.name} 获得的ID是：{tarotId}");
    }
}

public class IDAllocatorForTarot
{
    public List<int> availableIDs = new List<int> { 
        601, 602, 603, 604, 605, 
        606, 607, 608, 609, 610,
        611, 612, 613, 614, 615,
        616, 617, 618, 619, 620 
    };
    private System.Random rng = new System.Random();

    public static IDAllocatorForTarot _instance;
    public static IDAllocatorForTarot Instance => _instance ??= new IDAllocatorForTarot();

    private IDAllocatorForTarot() { }

    public bool TryGetUniqueID(out int id)
    {
        if (availableIDs.Count == 0)
        {
            //如果耗尽，那就开始随机给id：
            id = Random.Range(601, 621);

            return true;
        }

        int index = rng.Next(availableIDs.Count);
        id = availableIDs[index];
        availableIDs.RemoveAt(index);
        return true;
    }
}
