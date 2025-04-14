using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommonItemPanel : ItemPanel
{

    public Transform itemContent;
    public List<InventoryItemLogic> itemScriptList = new List<InventoryItemLogic>();
    public List<GameObject> itemObjectList = new List<GameObject>();
    protected override void Init()
    {       
        RefreshItem();
    }
    protected override void Awake()
    {
        base.Awake();

        RefreshItem();

        //这个广播定义在Awake中，避免失活之后无法接受广播：
        EventHub.Instance.AddEventListener<int>("RefreshItemsInPanel", RefreshItemsInPanel);
    }

    void OnDestroy()
    {
        EventHub.Instance.RemoveEventListener<int>("RefreshItemsInPanel", RefreshItemsInPanel);
    }


    //整理面板的方法，在初始化和使用立刻生效的道具后调用；
    protected override void RefreshItem()
    {
        //清空List：
        itemScriptList.Clear();
        foreach(int itemId in ItemManager.Instance.itemList)
        {
            //只有神明道具初始化：
            //ItemType == 1 or 2
            //通过item ID 去获取这个Item的固定信息（如Item的类型）
            Item infoItem = LoadManager.Instance.allItems[itemId];

            //只有事件内道具显示
            if(infoItem.type == Item.ItemType.Normal)
            {
                if(ItemManager.Instance.itemCountDic[infoItem.id] == 0 && !infoItem.isInUse)
                    continue;

                GameObject nowItem = Instantiate(Resources.Load<GameObject>("TestResources/ItemInventory"), itemContent, false);
                InventoryItemLogic script = nowItem.GetComponentInChildren<InventoryItemLogic>();
                script.Init(infoItem);

                //更新面板的时候，如果Item已经在使用中，那么加上蒙版：
                if(infoItem.isInUse)
                {
                    script.takeEffectMaskObject.SetActive(true);
                }

                itemScriptList.Add(script);
                itemObjectList.Add(nowItem);
            }
        }
    }

    //广播的方法：更新UI：
    //参数int，表示的是需要刷新的目标ItemId；
    private void RefreshItemsInPanel(int targetId)
    {
        InventoryItemLogic temp = null;
        //先找到你：
        foreach(var script in itemScriptList)
        {
            if(script.myItem.id == targetId)
            {
                temp = script;
            }   
        }

        //如果找不到，那么就终止：
        if(temp == null)
            return;

        //刷新UI：
        temp.RefreshSelf();
        //如果为0，执行移除：
        //如果道具已经没了，那么就不需要处理了：
        if(!ItemManager.Instance.itemCountDic.ContainsKey(targetId))
        {            
            Destroy(temp.gameObject);
            itemScriptList.Remove(temp);
        }

        //整理UI：
        foreach (GameObject obj in itemObjectList)
        {
            Destroy(obj);
        }

        RefreshItem();
    }

}

