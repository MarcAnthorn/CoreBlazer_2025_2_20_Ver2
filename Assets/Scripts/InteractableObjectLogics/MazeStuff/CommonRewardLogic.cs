using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonRewardLogic : MonoBehaviour
{
    private List<WeightedItem> items;
    public int result;

    void Start()
    {
        // 初始化数据（按你的需求设置）
        items = new List<WeightedItem>
        {
            new WeightedItem(0.3f, 501),
            new WeightedItem(0.2f, 303),
            new WeightedItem(0.1f, 402),
            new WeightedItem(0.1f, 401),
            new WeightedItem(0.1f, 103),
            new WeightedItem(0.1f, 102),
            new WeightedItem(0.1f, 101),
        };

        result = GetRandomID();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            // SoundEffectManager.Instance.PlaySoundEffect("与地图POI交互");

            //按权重分发道具：
            int count = result == 501 || result == 303 ? 2 : 1;
            ItemManager.Instance.AddItem(result, true, count);

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

public class WeightedItem
{
    public int ID;
    public float Probability;

    public WeightedItem(float probability, int id)
    {
        Probability = probability;
        ID = id;
    }
}
