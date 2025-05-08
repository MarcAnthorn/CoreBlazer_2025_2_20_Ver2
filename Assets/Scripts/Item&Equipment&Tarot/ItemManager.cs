using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using JetBrains.Annotations;
using UnityEngine;

public class ItemManager : Singleton<ItemManager>
{
    //List存储当前背包中持有的Item种类(以Item id的形式存储)；
    //初始化背包UI的时候，需要遍历List；然后通过id访问dic，获取该道具的数量；
    public List<int> itemList = new List<int>();

    public bool isAdded;

    //更新：itemCountDic通过Item的id访问对应的Item还存在几个：
    public Dictionary<int, int> itemCountDic = new Dictionary<int, int>();  //供外部访问的数据结构

    public void AddItem(int id, int count = 1)
    {
        Item nowItem = LoadManager.Instance.allItems[id];
        if(itemList.Contains(id))
        {
            //首先：判断是不是塔罗牌：
            if(id / 100 == 6)
            {
                //如果是，那么就是重复添加塔罗牌，直接不添加，而是加精神值；
                PlayerManager.Instance.player.SAN.value += 5;
                UIManager.Instance.ShowPanel<WarningPanel>().SetWarningText($"当前塔罗牌「{nowItem.name}」已收集，精神值 + 5", true);
                return;
                
            }
            else
            {
                //如果不是塔罗牌,并且道具存在，那就是调整dic中的数量：
                //注意：先判断是否要超出道具叠加上限：如果是，那么直接return：
                if(itemCountDic[id]  == nowItem.maxLimit)
                {
                    UIManager.Instance.ShowPanel<WarningPanel>().SetWarningText($"当前道具「{nowItem.name}」已达到最大可叠加上限{nowItem.maxLimit}");
                    return;
                }

                //始终确保List和Dic中的元素数量是一致的；
                for(int i = 0; i < count; i++)
                {
                    itemCountDic[id]++;
                }
            }

        }
        else
        {
            //不存在，那么就是加入List，然后初始化Dic数量为1；
            itemList.Add(id);
            itemCountDic.Add(id, 0);
            for(int i = 0; i < count; i++)
            {
                itemCountDic[id]++;
            }
        }

        //如果这个道具是持有就使用，那么直接使用：
        if(LoadManager.Instance.allItems[id].isImmediate)
        {
            Item item = LoadManager.Instance.allItems[id];
            item.Use();
            item.isInUse = true;
        }

        //弹出获取的道具：
        if(id / 100 == 6){
            UIManager.Instance.ShowPanel<WarningPanel>().SetWarningText($"获得塔罗牌「{nowItem.name}」", true);
        }
        else
        {
            UIManager.Instance.ShowPanel<WarningPanel>().SetWarningText($"获得道具「{nowItem.name}」* {count}", true);
        }

    }

    //重载：一次加多个道具：
    public void AddItem(params int[] ids)
    {
         StringBuilder sb = new StringBuilder();
        foreach(var id in ids)
        {
            int count = 1;  //默认只加一个；
            Item nowItem = LoadManager.Instance.allItems[id];
            if(itemList.Contains(id))
            {
                //首先：判断是不是塔罗牌：
                if(id / 100 == 6)
                {
                    //如果是，那么就是重复添加塔罗牌，直接不添加，而是加精神值；
                    PlayerManager.Instance.player.SAN.value += 5;
                    UIManager.Instance.ShowPanel<WarningPanel>().SetWarningText($"当前塔罗牌「{nowItem.name}」已收集，精神值 + 5", true);
                    return;
                    
                }
                else
                {
                    //如果不是塔罗牌,并且道具存在，那就是调整dic中的数量：
                    //注意：先判断是否要超出道具叠加上限：如果是，那么直接return：
                    if(itemCountDic[id]  == nowItem.maxLimit)
                    {
                        UIManager.Instance.ShowPanel<WarningPanel>().SetWarningText($"当前道具「{nowItem.name}」已达到最大可叠加上限{nowItem.maxLimit}");
                        return;
                    }

                    //始终确保List和Dic中的元素数量是一致的；
                    for(int i = 0; i < count; i++)
                    {
                        itemCountDic[id]++;
                    }
                }

            }
            else
            {
                //不存在，那么就是加入List，然后初始化Dic数量为1；
                itemList.Add(id);
                itemCountDic.Add(id, 0);
                for(int i = 0; i < count; i++)
                {
                    itemCountDic[id]++;
                }
            }

            //如果这个道具是持有就使用，那么直接使用：
            if(LoadManager.Instance.allItems[id].isImmediate)
            {
                Item item = LoadManager.Instance.allItems[id];
                item.Use();
                item.isInUse = true;
            }

            //弹出获取的道具：
            if(id / 100 == 6){
                sb.Append($"获得塔罗牌「{nowItem.name}」\n");
                
            }
            else
            {
                sb.Append($"获得道具「{nowItem.name}」* {count}\n");
            }
        }

        UIManager.Instance.ShowPanel<WarningPanel>().SetWarningText(sb.ToString(), true);
        
    }


    public void RemoveItem(int id)
    {   
        if(itemList.Contains(id))
        {
            //包含才会删除：
            itemList.Remove(id);
            //同步移除Dic中的元素：
            itemCountDic.Remove(id);

        }
       
        else
        {
            Debug.LogError($"背包中没有该道具，id为：{id}");
        }
    }

    // //重载：传入的是Item的ID，内部进行实例化对应道具的操作；
    // public void AddItem(int itemId)
    // {
    //     inventory.AddItem(itemId);
    // }


    //bool返回值表示是否使用成功：
    //这个方法会处理所有使用上的逻辑；
    public bool UseItem(Item item)
    {
        //首先判断是否存在Item、当前场景是否可用，以及Item是否已经生效：
        if(itemList.Contains(item.id) && item.CanUseOrNot((int)PlayerManager.Instance.playerSceneIndex) && itemCountDic[item.id] > 0 && !item.isInUse)
        {
            //使用：这个只是处理了数值上等等的影响；
            item.Use();

            //接下来处理背包数据结构中的相关内容：
            itemCountDic[item.id]--;

            //1.1被动永久生效，直到死亡清除效果的：item.isImmediate是获得后被动生效；
            //1.2主动生效，直到死亡的：
            if(item.isImmediate || (!item.isImmediate && item.isPermanent))
            {
                Debug.Log("被动生效");
                item.isInUse = true;
            }

            //2.如果使用的是主动使用、立刻生效的，那么除了Use()、调整itemCountDic以及判断是否清零之外啥也不需要：
            else if(!item.isPermanent && item.EffectiveTime == 0)
            {
                Debug.Log("该道具立刻生效，属性加成");
                //没有时间限制的，直接执行刷新：
                //不需要调整isInUse，因为这个变量控制显示UI的时候是否需要加上蒙版；有蒙版就无法使用了；
                //即刻生效的一次性道具不需要这个蒙版；
                if(itemCountDic[item.id] == 0)
                {
                    //移除了之后，下次UI更新就不会出现它了
                    RemoveItem(item.id);
                }
            }
            //3.如果是会在生效时间内生效的，那么会以回调的方式，在时间结束之后刷新；
            else if(!item.isPermanent)
            {
                Debug.Log($"该道具立刻生效，生效时长：{item.EffectiveTime}");
                //先清理，再为Item的回调onCompleteCallback初始化
                item.onCompleteCallback = null;

                item.isInUse = true;
                //移除蒙版的处理也在RefreshItemsInPanel中；
                item.onCompleteCallback += () => {
                    item.isInUse = false;
                    
                    //同时，如果数量减少到0了，那么就会从背包中移除：
                    if(itemCountDic[item.id] == 0)
                    {
                        //移除了之后，下次UI更新就不会出现它了
                        RemoveItem(item.id);
                    }
                };
            }

            //更新UI：使用的时候UI一定是在的；
            EventHub.Instance.EventTrigger<int>("RefreshItemsInPanel", item.id); 
            return true;
        }
        else if(item.isInUse)
        {
            UIManager.Instance.ShowPanel<WarningPanel>().SetWarningText("该道具使用中，不可重复使用");
        }
        else if(!item.CanUseOrNot((int)PlayerManager.Instance.playerSceneIndex))
        {
            
            UIManager.Instance.ShowPanel<WarningPanel>().SetWarningText("当前场景下不可使用该道具");
            // Debug.LogWarning($"当前尝试使用的道具不可使用，道具id：{item.id},该道具的可使用场景是:{item.usableScene[0]}, {item.usableScene[1]}, {item.usableScene[2]}");
        }
        return false;
    }


    public Item ClassifyItems(int ItemId)               //用于将道具分类
    {
        switch (ItemId)
        {
            case 101:
                return new Item_101();
            case 102:
                return new Item_102();
            case 103:
                return new Item_103();
            case 104:
                return new Item_104();
            case 201:
                return new Item_201();
            case 202:
                return new Item_202();
            case 203:
                return new Item_203();
            case 204:
                return new Item_204();
            case 205:
                return new Item_205();
            case 206:
                return new Item_206();
            case 207:
                return new Item_207();
            case 208:
                return new Item_208();
            case 301:
                return new Item_301();
            case 302:
                return new Item_302();
            case 303:
                return new Item_303();
            case 401:
                return new Item_401();
            case 402:
                return new Item_402();
            case 403:
                return new Item_403();
            case 404:
                return new Item_404();
            case 501:
                return new Item_501();
            case 502:
                return new Item_502();
            case 503:
                return new Item_503();
            case 504:
                return new Item_504();
            case 505:
                return new Item_505();
            case 506:
                return new Item_506();
            case 507:
                return new Item_507();
            case 508:
                return new Item_508();

        //塔罗牌：
             case 601:
                return new Item_601();
            case 602:
                return new Item_602();
            case 603:
                return new Item_603();
            case 604:
                return new Item_604();
            case 605:
                return new Item_605();
            case 606:
                return new Item_606();
            case 607:
                return new Item_607();
            case 608:
                return new Item_608();
            case 609:
                return new Item_609();
            case 610:
                return new Item_610();
            case 611:
                return new Item_611();
            case 612:
                return new Item_612();
            case 613:
                return new Item_613();
            case 614:
                return new Item_614();
            case 615:
                return new Item_615();
            case 616:
                return new Item_616();
            case 617:
                return new Item_617();
            case 618:
                return new Item_618();
            case 619:
                return new Item_619();
            case 620:
                return new Item_620();

        //新增商店交易道具：
            case 2011:
                return new Item_2011();
            case 2012:
                return new Item_2012();
            case 2013:
                return new Item_2013();
            case 2014:
                return new Item_2014();
            case 2015:
                return new Item_2015();
            case 2016:
                return new Item_2016();

            default:
                Debug.LogWarning($"未找到id为 {ItemId} 的道具");
                return null;
        }
    }

}
