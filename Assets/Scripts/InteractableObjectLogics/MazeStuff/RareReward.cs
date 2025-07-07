using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RareReward : MonoBehaviour
{
    private List<WeightedItem> items;
    public int result;

    void Start()
    {
        // 初始化数据（按你的需求设置）
        items = new List<WeightedItem>
        {
            new WeightedItem(0.1f, 502),
            new WeightedItem(0.1f, 503),
            new WeightedItem(0.1f, 504),
            new WeightedItem(0.1f, 505),
            new WeightedItem(0.1f, 506),
            new WeightedItem(0.1f, 507),
            new WeightedItem(0.1f, 508),

            new WeightedItem(0.1f, 1001),
            new WeightedItem(0.1f, 1002),
            new WeightedItem(0.1f, 1003),
        };

        result = GetRandomID();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            // SoundEffectManager.Instance.PlaySoundEffect("与地图POI交互");

            //按权重分发道具：
            if(result != 1001 && result != 1002 && result != 1003)  //不是装备，道具；
                ItemManager.Instance.AddItem(result);
            else    //装备：
                EquipmentManager.Instance.AddEquipment(result);

            Destroy(this.gameObject);
        }
    }


    int GetRandomID()
    {
        float roll = Random.value; // [0,1)
        float cumulative = 0f;

        foreach (var item in items)
        {
            cumulative += item.Probability;
            if (roll < cumulative)
                return item.ID;
        }

        // 理论上不会到达这里
        return items[items.Count - 1].ID;
    }
}


