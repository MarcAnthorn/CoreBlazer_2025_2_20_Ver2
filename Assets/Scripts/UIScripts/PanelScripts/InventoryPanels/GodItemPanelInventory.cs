using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GodItemPanelInventory : BasePanel
{
    public Transform itemContent;
    public List<InventoryItemLogic> itemScriptList = new List<InventoryItemLogic>();
    public List<GameObject> itemObjectList = new List<GameObject>();
    private bool isHighLight = false;
    //当前是否还处在待选中特定Item的状态：
    public bool isStillSelecting = false;
    //这个是额外的Button：如果当前处在Item交互状态，那么点击这个Button就会离开当前交互：
    public Button btnExitInteraction; 
    protected override void Init()
    {

        btnExitInteraction.onClick.AddListener(()=>{
            //首先是所有的Item都要取消高亮：
            HighLightItemsOrNot(false);

            //重置Item的状态：
            EventHub.Instance.EventTrigger("ResetItem");
            
           
        });
    }

    protected override void Awake()
    {
        base.Awake();
        btnExitInteraction = GameObject.Find("CancelButton").GetComponent<Button>();

        RefreshPanel();

        //这个广播定义在Awake中，避免失活之后无法接受广播：
        EventHub.Instance.AddEventListener<int>("RefreshItemsInPanel", RefreshItemsInPanel);
    }

    void OnEnable()
    {
        EventHub.Instance.AddEventListener<bool>("HighLightItemsOrNot", HighLightItemsOrNot);
        EventHub.Instance.AddEventListener("ResetItem", ResetItem);

        //测试用生成道具：
        //根据当前背包中所有的道具进行筛选，将ItemType为1 & 2的加入到自己的面板中
        
    }

    void OnDisable()
    {
        EventHub.Instance.RemoveEventListener<bool>("HighLightItemsOrNot", HighLightItemsOrNot);
        EventHub.Instance.RemoveEventListener("ResetItem", ResetItem);
    }

    void OnDestroy()
    {
        EventHub.Instance.RemoveEventListener<int>("RefreshItemsInPanel", RefreshItemsInPanel);
    }

    //高亮 or 取消高亮的方法：
    private void HighLightItemsOrNot(bool _isHighLight)
    {

        Debug.Log($"当前高光指令：{_isHighLight}");

        isHighLight = _isHighLight;

        //对每一个道具都进行高光 / 取消高光处理：
        foreach(var script in itemScriptList)
        {
            //注意：实际上的脚本在我们存储的游戏对象的子对象上：
            script.outlineObjects.SetActive(_isHighLight);
            if(!isStillSelecting)
            {
                script.isInPreselecting = _isHighLight;
            }

        }
        
        //最后更新isStillSelecting（先更新的话会出错， if(!isStillSelecting)语句进不去）
        //当前处在选择状态；选择状态之后「取消选择」& 「成功插入插槽」之后才会重置；这些都在ResetItem方法中；
        //ResetItem处在本脚本和InventoryItemLogic脚本中，是一个多播；
        isStillSelecting = true;


    }

    private void ResetItem()
    {
        isStillSelecting = false;
    }

    //整理面板的方法，在初始化和使用立刻生效的道具后调用；
    private void RefreshPanel()
    {
        //清空List：
        itemScriptList.Clear();
        
        foreach (GameObject obj in itemObjectList)
        {
            Destroy(obj);
        }

        itemObjectList.Clear();

        Debug.LogWarning("Refreshed");

        foreach(int itemId in ItemManager.Instance.itemList)
        {
            //只有神明道具初始化：
            //ItemType == 1 or 2
            //通过item ID 去获取这个Item的固定信息（如Item的类型）
            Item infoItem = LoadManager.Instance.allItems[itemId];
            GameObject nowItem = null;
            InventoryItemLogic script = null;


            //安全屋，显示所有的神明道具：
            if(PlayerManager.Instance.playerSceneIndex == E_PlayerSceneIndex.Shelter && (infoItem.type == Item.ItemType.God_Battle || infoItem.type == Item.ItemType.God_Maze))
            {
                Debug.LogWarning("Refreshed Type 1");
                if(ItemManager.Instance.itemCountDic[infoItem.id] == 0 && !infoItem.isInUse)
                    continue;

                nowItem = Instantiate(Resources.Load<GameObject>("TestResources/ItemInventory"), itemContent, false);
                script = nowItem.GetComponentInChildren<InventoryItemLogic>();
                script.Init(infoItem);

                //更新面板的时候，如果Item已经在使用中，那么加上蒙版：
                if(infoItem.isInUse)
                {
                    script.takeEffectMaskObject.SetActive(true);
                }

                itemScriptList.Add(script);
                itemObjectList.Add(nowItem);
            }

            else 
            {
                //处在战斗，才会初始化神明战斗道具：
                if((infoItem.type == Item.ItemType.God_Battle || infoItem.type == Item.ItemType.God_Maze) && infoItem.usableScene[1]  == 1 && PlayerManager.Instance.playerSceneIndex == E_PlayerSceneIndex.Battle)
                {
                     Debug.LogWarning("Refreshed Type 2");
                    if(ItemManager.Instance.itemCountDic[infoItem.id] == 0 && !infoItem.isInUse)
                        continue;

                    nowItem = Instantiate(Resources.Load<GameObject>("TestResources/ItemInventory"), itemContent, false);
                    script = nowItem.GetComponentInChildren<InventoryItemLogic>();
                    script.Init(infoItem);

                    //更新面板的时候，如果Item已经在使用中，那么加上蒙版：
                    if(infoItem.isInUse)
                    {
                        script.takeEffectMaskObject.SetActive(true);
                    }

                    itemScriptList.Add(script);
                    itemObjectList.Add(nowItem);
                }

                //不然就是初始化迷宫道具：
                // else if((infoItem.type == Item.ItemType.God_Battle || infoItem.type == Item.ItemType.God_Maze) && infoItem.usableScene[2]  == 1 && PlayerManager.Instance.playerSceneIndex == E_PlayerSceneIndex.Maze)
                else if((infoItem.type == Item.ItemType.God_Battle || infoItem.type == Item.ItemType.God_Maze) && PlayerManager.Instance.playerSceneIndex == E_PlayerSceneIndex.Maze)
                {
                     Debug.LogWarning("Refreshed Type 3");
                    if(ItemManager.Instance.itemCountDic[infoItem.id] == 0 && !infoItem.isInUse)
                        continue;

                    nowItem = Instantiate(Resources.Load<GameObject>("TestResources/ItemInventory"), itemContent, false);
                    script = nowItem.GetComponentInChildren<InventoryItemLogic>();
                    script.Init(infoItem);

                    //更新面板的时候，如果Item已经在使用中，那么加上蒙版：
                    if(infoItem.isInUse)
                    {
                        script.takeEffectMaskObject.SetActive(true);
                    }

                    //如果该Item是快捷装备中，那么就需要调用InventoryPanel中的方法，进行插槽
                    if(infoItem.isSlottedToLeft)
                    {
                        EventHub.Instance.EventTrigger<GameObject, string>("SlotItemToLeft", nowItem, script.myItem.name);
                    }
                    else if(infoItem.isSlottedToRight)
                    {
                        EventHub.Instance.EventTrigger<GameObject, string>("SlotItemToRight", nowItem, script.myItem.name);
                    }

                    itemScriptList.Add(script);
                    itemObjectList.Add(nowItem);
                }

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

        itemObjectList.Clear();

        RefreshPanel();


    }

  
}
