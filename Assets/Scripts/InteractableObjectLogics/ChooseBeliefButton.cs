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
    public SoldMaskLogic soldMaskScript;
    //图标：
    public Image imgItem;
    //当前售卖的ItemId:
    public int myItemId; 
    public Button btnSelf;

    //如果精神值不够，那么需要限制购买；这是一个实现这个的锁：
    private bool isSanityEnoughToBuy = true;

    void Awake()
    {
        btnSelf = this.GetComponent<Button>();
        EventHub.Instance.AddEventListener<int>("BuyItemCallback", RefreshMask);
    }

    void OnDestroy()
    {
        EventHub.Instance.RemoveEventListener<int>("BuyItemCallback", RefreshMask);
    }


    //初始化的方法：
    public void Init(int _itemId)
    {
        myItemId = _itemId;
        Item item = LoadManager.Instance.allItems[_itemId];
        string rootPath = "ArtResources/Item/" + item.name;
        imgItem.sprite = Resources.Load<Sprite>(rootPath);

        if(PlayerManager.Instance.player.SAN.value < 20)
        {
            txtSanityCost.color = new Color(0.8f, 0.3f, 0.3f);  //红色字体；
            isSanityEnoughToBuy = false;
        }

        else
        {
            txtSanityCost.color = new Color(0.3f, 0.8f, 0.3f);  // 绿色字体
            isSanityEnoughToBuy = true;
        }

        btnSelf.onClick.AddListener(()=>{
            if(isSanityEnoughToBuy)
                UIManager.Instance.ShowPanel<BuyItemCheckPanel>().InitPanel(_itemId);

            else
                UIManager.Instance.ShowPanel<WarningPanel>().SetWarningText("精神值不足，不能购买");

        });

        switch(myItemId)
        {
            //初始化相关的信息：
            case 2011:
                // imgItem.sprite = Resources.Load<Sprite>();   //图标相关之后再填入；
                txtItemName.text = "丰饶降临";
                txtItemDes.text = "丰饶降临人间，万家喜得肉食。";
                txtSanityCost.text = $"消耗精神值：{20}";
            break;
            case 2012:
                txtItemName.text = "繁育心神";
                txtItemDes.text = "来源于生物最原始的欲望之一。";
                txtSanityCost.text = $"消耗精神值：{20}";
            break;
            case 2013:
                txtItemName.text = "虚空的祝福";
                txtItemDes.text = "未知的祝福，真的就是好事吗？";
                txtSanityCost.text = $"消耗精神值：{20}";
            break;
            case 2014:
                txtItemName.text = "透视黑暗之眼";
                txtItemDes.text = "看不透黑暗也是一种幸运。";
                txtSanityCost.text = $"消耗精神值：{20}";
            break;
            case 2015:
                txtItemName.text = "惊喜盒子";
                txtItemDes.text = "每次打开都有不同的惊喜。";
                txtSanityCost.text = $"消耗精神值：{20}";
            break;
            case 2016:
                txtItemName.text = "祂的一撇";
                txtItemDes.text = "祂投来了一撇，不知道在打什么主意。";
                txtSanityCost.text = $"消耗精神值：{20}";
            break;
        }

        RefreshMask(_itemId);
        
    }

    private void RefreshMask(int _itemId)
    {
        //如果精神值不足，同时还持有我，那么就是先显示售罄：
        if(PlayerManager.Instance.player.SAN.value < 20 && ItemManager.Instance.itemList.Contains(_itemId) && _itemId == myItemId)
        {
            soldMaskScript.gameObject.SetActive(true);
            soldMaskScript.flag = 0;
        }

        //如果背包中存有该道具，那么就激活蒙版：
        else if(ItemManager.Instance.itemList.Contains(myItemId))
        {
            soldMaskScript.gameObject.SetActive(true);
            soldMaskScript.flag = 0;
        }

        //精神值不足，那么就是更新蒙版的显示内容
        else if(PlayerManager.Instance.player.SAN.value < 20)
        {
            soldMaskScript.gameObject.SetActive(true);
            soldMaskScript.flag = 1;
            soldMaskScript.SetText("精神值不足");
        }


    
    }
}
