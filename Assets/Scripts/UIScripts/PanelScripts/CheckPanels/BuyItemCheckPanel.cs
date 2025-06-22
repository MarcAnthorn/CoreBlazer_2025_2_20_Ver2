using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuyItemCheckPanel : BasePanel
{
    //当前售卖的 / 剥夺的道具名：
    public TextMeshProUGUI txtItemName;
    //道具的一句话描述（不是作用描述，是简短文案描述）
    public TextMeshProUGUI txtItemDes;
    //道具的作用描述：
    public TextMeshProUGUI txtItemEffectDes;
    //图标：
    public Image imgItem;
     
    public Button btnBuy;
    public Button btnCancel;
    public int myItemId;
    protected override void Init()
    {
        btnBuy.onClick.AddListener(()=>{
            ItemManager.Instance.AddItem(myItemId);

            //更新购买Button处的遮罩，位于ChooseBeliefButton中：
            //以及更新对话，位于ChooseBeliefPanel中：
            //两个都被封装在BuyItemCallback这个事件key中：

            //先扣除精神值：
            PlayerManager.Instance.player.SAN.AddValue(-20);
        
            //更新UI：
            EventHub.Instance.EventTrigger("UpdateAllUIElements");
          
            //再广播：
            EventHub.Instance.EventTrigger<int>("BuyItemCallback", myItemId);

            UIManager.Instance.HidePanel<BuyItemCheckPanel>();
        });

        btnCancel.onClick.AddListener(()=>{
            UIManager.Instance.HidePanel<BuyItemCheckPanel>();
        });
    }

    public void InitPanel(int _itemId)
    {
        myItemId = _itemId;

        Item item = LoadManager.Instance.allItems[_itemId];
        string rootPath = Path.Combine("ArtResources", "Item", item.name);
        imgItem.sprite = Resources.Load<Sprite>(rootPath);
        
        switch(myItemId)
        {
            //初始化相关的信息：
            case 2011:
                // imgItem.sprite = Resources.Load<Sprite>();   //图标相关之后再填入；
                txtItemName.text = "丰饶降临";
                txtItemDes.text = "丰饶降临人间，万家喜得肉食。";
                txtItemEffectDes.text = "获得本道具后，处于装备状态的所有怪谈装备当前耐久度及其上限+1";
            break;
            case 2012:
                txtItemName.text = "繁育心神";
                txtItemDes.text = "来源于生物最原始的欲望之一。";
                txtItemEffectDes.text = "获得本道具后，处于未装备状态的随机3件装备当前耐久度及其上限+3";
            break;
            case 2013:
                txtItemName.text = "虚空的祝福";
                txtItemDes.text = "未知的祝福，真的就是好事吗？";
                txtItemEffectDes.text = "使用后：当前生命值+100，防御+10";
            break;
            case 2014:
                txtItemName.text = "透视黑暗之眼";
                txtItemDes.text = "看不透黑暗也是一种幸运。";
                txtItemEffectDes.text = "使用后：当前灯光值与生命值以及他们的上限+50。";
            break;
            case 2015:
                txtItemName.text = "惊喜盒子";
                txtItemDes.text = "每次打开都有不同的惊喜。";
                txtItemEffectDes.text = "持有该道具时：每次战斗胜利后随机获得2件怪谈道具。";
            break;
            case 2016:
                txtItemName.text = "祂的一撇";
                txtItemDes.text = "祂投来了一撇，不知道在打什么主意。";
                txtItemEffectDes.text = "持有该道具时：进入下一层关卡以及安全屋后，当前力量/防御/速度以及其上限随机一项属性+10/-10";
            break;
        }
    }


}
