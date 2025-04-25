using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChooseBeliefButton : MonoBehaviour
{
    //当前售卖的 / 剥夺的道具名：
    public TextMeshProUGUI txtItemName;
    //道具的一句话描述（不是作用描述，是简短文案描述）
    public TextMeshProUGUI txtItemDes;
    //消耗精神值Cost文本：
    public TextMeshProUGUI txtSanityCost; 
    //售卖后的遮罩：
    public GameObject soldMaskObject;
    //图标：
    public Image imgItem;
    //当前售卖的ItemId:
    public int myItemId; 
    public Button btnSelf;

    void Awake()
    {
        btnSelf = this.GetComponent<Button>();
        EventHub.Instance.AddEventListener<int>("RefreshMask", RefreshMask);
    }

    void OnDestroy()
    {
        EventHub.Instance.RemoveEventListener<int>("RefreshMask", RefreshMask);
    }


    //初始化的方法：
    public void Init(int _itemId)
    {
        myItemId = _itemId;

        btnSelf.onClick.AddListener(()=>{
            UIManager.Instance.ShowPanel<BuyItemCheckPanel>().InitPanel(_itemId);
        });

        switch(myItemId)
        {
            //初始化相关的信息：
            case 611:
                // imgItem.sprite = Resources.Load<Sprite>();   //图标相关之后再填入；
                txtItemName.text = "丰饶降临";
                txtItemDes.text = "丰饶降临人间，万家喜得肉食。";
                txtSanityCost.text = $"消耗精神值：{20}";
            break;
            case 612:
                txtItemName.text = "繁育心神";
                txtItemDes.text = "来源于生物最原始的欲望之一。";
                txtSanityCost.text = $"消耗精神值：{20}";
            break;
            case 613:
                txtItemName.text = "虚空的祝福";
                txtItemDes.text = "未知的祝福，真的就是好事吗？";
                txtSanityCost.text = $"消耗精神值：{20}";
            break;
            case 614:
                txtItemName.text = "透视黑暗之眼";
                txtItemDes.text = "看不透黑暗也是一种幸运。";
                txtSanityCost.text = $"消耗精神值：{20}";
            break;
            case 615:
                txtItemName.text = "惊喜盒子";
                txtItemDes.text = "每次打开都有不同的惊喜。";
                txtSanityCost.text = $"消耗精神值：{20}";
            break;
            case 616:
                txtItemName.text = "祂的一撇";
                txtItemDes.text = "祂投来了一撇，不知道在打什么主意。";
                txtSanityCost.text = $"消耗精神值：{20}";
            break;
        }

        RefreshMask(_itemId);
        
    }

    private void RefreshMask(int _itemId)
    {
        Debug.Log($"当前尝试刷新的道具是：{_itemId}");
        //如果背包中存有该道具，那么就激活蒙版：
        if(ItemManager.Instance.itemList.Contains(_itemId) && _itemId == myItemId)
        {
            soldMaskObject.gameObject.SetActive(true);
        }


    }
}
