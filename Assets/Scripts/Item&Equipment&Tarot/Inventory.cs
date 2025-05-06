using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Inventory              //背包
{
    //List存储当前背包中持有的Item种类(以Item id的形式存储)；
    //初始化背包UI的时候，需要遍历List；然后通过id访问dic，获取该道具的数量；
    public List<int> itemList = new List<int>();

    //更新：itemCountDic通过Item的id访问对应的Item还存在几个：
    public Dictionary<int, int> itemCountDic = new Dictionary<int, int>();  //供外部访问的数据结构

    public void AddItem(int id)
    {
        if(itemList.Contains(id))
        {
            //如果存在，那就是调整dic中的数量：
            //始终确保List和Dic中的元素数量是一致的；
            itemCountDic[id]++;

        }
        else
        {
            //不存在，那么就是加入List，然后初始化Dic数量为1；
            itemList.Add(id);
            itemCountDic.Add(id, 1);
        }


        // if(!itemCountDic.ContainsKey(item))
        //     itemCountDic.Add(item, 1);

        // else
        // {
        //     //道具上限的信息存储在LoadManager中的allItem中；
        //     //访问当前Item自己的maxLimit是错误的；
        //     Item infoItem = LoadManager.Instance.allItems[item.id];
        //     if (itemCountDic[item] < infoItem.maxLimit)
        //     {
        //         Debug.Log($"Item is added!, item id is{item.id}");
        //         itemCountDic[item]++;
        //         // itemList.Add(item);
        //         return;
        //     }
        //     else
        //     {
        //         Debug.LogWarning("超出堆叠上限，Item ID为：" + item.id);
        //     }
        // }


        // if (itemCountDic.ContainsKey(item))     //加这个是为了防止下面的CountOfItems[Item]空引用
        // {
        //     //道具上限的信息存储在LoadManager中的allItem中；
        //     //访问当前Item自己的maxLimit是错误的；
        //     Item infoItem = LoadManager.Instance.allItems[Item.id];
        //     if (itemCountDic[Item] < infoItem.maxLimit)
        //     {
        //         Debug.Log($"Item is added!, item id is{Item.id}");
        //         AddInDic(Item);
        //         itemList.Add(Item);
        //         return;
        //     }
        //     else
        //     {
        //         Debug.LogWarning("超出堆叠上限，Item ID为：" + Item.id);
        //     }
        // }
        // else
        // {
        //     //Items.Add(Item);
        //     AddInDic(Item);
        // }

        // Debug.LogWarning($"道具：{Item.name}已满！");
    }

    //Marc调整过：
    //从背包中移除这个Item的方法：在Item数量归零之后调用
    // public void RemoveItem(int id)
    // {   
    //     if(itemList.Contains(id))
    //     {
    //         //包含才会删除：
    //         itemList.Remove(id);
    //         //同步移除Dic中的元素：
    //         itemCountDic.Remove(id);

    //     }
       
    //     Debug.LogError($"背包中没有该道具，id为：{id}");
    // }

    // public void RemoveItem(Item Item)
    // {
    //     if (itemList.Contains(Item))           //按照List内的Item来判断
    //     {
    //         itemList.Remove(Item);
    //         RemoveInDic(Item);
    //         return;
    //     }

    //     Debug.LogError($"背包中没有该道具：{Item.name}");
    // // }

    // private void AddInDic(Item Item)
    // {
    //     if (!itemCountDic.ContainsKey(Item))
    //     {
    //         itemCountDic.Add(Item, 0);
    //     }
    //     Debug.Log("Turn to add in dic");
    //     itemCountDic[Item]++;
    // }
    // private void RemoveInDic(Item Item)
    // {
    //     if (!itemCountDic.ContainsKey(Item))
    //     {
    //         itemCountDic.Add(Item, 0);
    //         Debug.LogError($"背包中没有该道具：{Item.name}");
    //         return;
    //     }
    //     itemCountDic[Item]--;
        
    //     //如果减少到0，那么就需要进行一个消除Item的行为广播：

    // }

}
