using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory              //背包
{
    public List<Item> Items = new List<Item>();
    public Dictionary<Item, int> CountOfItems = new Dictionary<Item, int>();//供外部访问的数据结构

    public void AddItem(Item Item)
    {
        if (CountOfItems.ContainsKey(Item))     //加这个是为了防止下面的CountOfItems[Item]空引用
        {
            if (CountOfItems[Item] < Item.maxLimit)
            {
                AddInDic(Item);
                Items.Add(Item);
                return;
            }
        }
        else
        {
            //Items.Add(Item);
            AddInDic(Item);
        }

        Debug.LogWarning($"道具：{Item.name}已满！");
    }
    public void RemoveItem(Item Item)
    {
        if (Items.Contains(Item))           //按照List内的Item来判断
        {
            Items.Remove(Item);
            RemoveInDic(Item);
            return;
        }

        Debug.LogError($"背包中没有该道具：{Item.name}");
    }

    private void AddInDic(Item Item)
    {
        if (!CountOfItems.ContainsKey(Item))
        {
            CountOfItems.Add(Item, 0);
        }
        CountOfItems[Item]++;
    }
    private void RemoveInDic(Item Item)
    {
        if (!CountOfItems.ContainsKey(Item))
        {
            CountOfItems.Add(Item, 0);
            Debug.LogError($"背包中没有该道具：{Item.name}");
            return;
        }
        CountOfItems[Item]--;
    }

}
