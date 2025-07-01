using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ItemManager : Singleton<ItemManager>
{
    private Inventory inventory;

    protected override void Awake()
    {
        base.Awake();
        inventory = new Inventory();
    }

    public void GetItem(Item Item)
    {
        inventory.AddItem(Item);
    }

    public void UseItem(Item Item)
    {
        if (Item.CanUseOrNot(GameLevelManager.currentEnvironment) && inventory.Items.Contains(Item))
        {
            Item.Use();
            inventory.RemoveItem(Item);
            return;
        }

        Debug.LogError($"无法使用该道具：{Item.name}");
    }

    public Item ClassifyItems(int ItemId)               //用于将道具分类
    {
        switch (ItemId)
        {
            case 101:
                return new Item_101();
            case 201:
                return new Item_Tatakai();
            case 301:
                return new Item_LightUP();
            case 401:
                return new Item_Alive();
            case 501:
                return new Item_BloodMedicine();
            case 601:
                return new Item_Tarot1();
            default:
                Debug.LogWarning($"未找到id为 {ItemId} 的道具");
                return null;
        }
    }

}
