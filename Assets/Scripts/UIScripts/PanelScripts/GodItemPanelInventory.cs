using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GodItemPanelInventory : BasePanel
{
    public Transform itemContent;
    public List<GameObject> itemList = new List<GameObject>();
    private bool isHighLight = false;
    //当前是否还处在待选中特定Item的状态：
    public bool isStillSelecting = false;
    //这个是额外的Button：如果当前处在Item交互状态，那么点击这个Button就会离开当前交互：
    public Button btnExitInteraction; 
    protected override void Init()
    {
        //测试用生成道具：
        for(int i = 0; i < 20; i++)
        {
            GameObject go = Resources.Load<GameObject>("TestResources/ItemInventory");
            itemList.Add(Instantiate(go, itemContent, false));
        }

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
        EventHub.Instance.AddEventListener<bool>("HighLightItemsOrNot", HighLightItemsOrNot);
        EventHub.Instance.AddEventListener("ResetItem", ResetItem);
        btnExitInteraction = GameObject.Find("CancelButton").GetComponent<Button>();
    }


    private void OnDestroy()
    {
        EventHub.Instance.RemoveEventListener<bool>("HighLightItemsOrNot", HighLightItemsOrNot);
        EventHub.Instance.RemoveEventListener("ResetItem", ResetItem);
    }

    //高亮 or 取消高亮的方法：
    private void HighLightItemsOrNot(bool _isHighLight)
    {

        Debug.Log($"当前高光指令：{_isHighLight}");

        isHighLight = _isHighLight;

        //对每一个道具都进行高光 / 取消高光处理：
        foreach(var item in itemList)
        {
            //注意：实际上的脚本在我们存储的游戏对象的子对象上：
            InventoryItemLogic script = item.gameObject.GetComponentInChildren<InventoryItemLogic>();
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

  
}
