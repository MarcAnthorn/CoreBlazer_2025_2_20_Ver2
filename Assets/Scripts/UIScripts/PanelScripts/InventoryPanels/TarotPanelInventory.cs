using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TarotPanelInventory : MonoBehaviour
{
    public Transform tarotContainer;
    public List<InventoryTarotLogic> itemScriptList = new List<InventoryTarotLogic>();
    public List<GameObject> itemObjectList = new List<GameObject>();
    void Start()
    {
        RefreshPanel();
    }

    private void RefreshPanel()
    {
     
        itemScriptList.Clear();

        foreach (GameObject obj in itemObjectList)
        {
            Destroy(obj);
        }

        itemObjectList.Clear();

        foreach(int itemId in ItemManager.Instance.itemList)
        {
            if(itemId / 100 != 6)
                continue;

            //只有Tarot道具初始化：
            //id % 100 == 6
            //通过item ID 去获取这个Item的固定信息（如Item的类型）
            Item infoItem = LoadManager.Instance.allItems[itemId];
            GameObject nowItem = null;
            InventoryTarotLogic script = null;

            nowItem = Instantiate(Resources.Load<GameObject>("TestResources/TarotInventory"), tarotContainer, false);
            script = nowItem.GetComponentInChildren<InventoryTarotLogic>();
            script.Init(infoItem);

            itemScriptList.Add(script);
            itemObjectList.Add(nowItem);
        }
    }
}
