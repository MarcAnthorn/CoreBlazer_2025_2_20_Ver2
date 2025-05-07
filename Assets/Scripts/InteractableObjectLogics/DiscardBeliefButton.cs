using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DiscardBeliefButton : MonoBehaviour
{

  //当前售卖的 / 剥夺的道具名：
    public TextMeshProUGUI txtItemName;
    //道具的一句话描述（不是作用描述，是简短文案描述）
    public TextMeshProUGUI txtItemDes;
    //消耗精神值Cost文本：
    public TextMeshProUGUI txtSanityCompensation; 
    //售卖后的遮罩：
    public GameObject inactiveMaskObject;
    //图标：
    public Image imgItem;
    //当前消除的ItemId:
    public int myItemId; 
    public Button btnSelf;


    void Awake()
    {
        EventHub.Instance.AddEventListener<int>("DiscardItemCallback", RefreshMask);
    }

    void OnDestroy()
    {
        EventHub.Instance.RemoveEventListener<int>("DiscardItemCallback", RefreshMask);
    }



    //初始化的方法：
    public void Init(int _itemId)
    {
        myItemId = _itemId;

        txtSanityCompensation.color = new Color(0.3f, 0.8f, 0.3f);  // 绿色字体

        btnSelf.onClick.AddListener(()=>{
            if(ItemManager.Instance.itemList.Contains(_itemId))
                UIManager.Instance.ShowPanel<DiscardItemCheckPanel>().InitPanel(_itemId);
        });

        switch(myItemId)
        {
            //初始化相关的信息：
            case 2011:
                // imgItem.sprite = Resources.Load<Sprite>();   //图标相关之后再填入；
                txtItemName.text = "丰饶降临";
                txtItemDes.text = "丰饶降临人间，万家喜得肉食。";
                txtSanityCompensation.text = $"恢复精神值：{10}";
            break;
            case 2012:
                txtItemName.text = "繁育心神";
                txtItemDes.text = "来源于生物最原始的欲望之一。";
                txtSanityCompensation.text = $"恢复精神值：{10}";
            break;
            case 2013:
                txtItemName.text = "虚空的祝福";
                txtItemDes.text = "未知的祝福，真的就是好事吗？";
                txtSanityCompensation.text = $"恢复精神值：{10}";
            break;
            case 2014:
                txtItemName.text = "透视黑暗之眼";
                txtItemDes.text = "看不透黑暗也是一种幸运。";
                txtSanityCompensation.text = $"恢复精神值：{10}";
            break;
            case 2015:
                txtItemName.text = "惊喜盒子";
                txtItemDes.text = "每次打开都有不同的惊喜。";
                txtSanityCompensation.text = $"恢复精神值：{10}";
            break;
            case 2016:
                txtItemName.text = "祂的一撇";
                txtItemDes.text = "祂投来了一撇，不知道在打什么主意。";
                txtSanityCompensation.text = $"恢复精神值：{10}";
            break;
        }

        RefreshMask(_itemId);
        
    }

    public void RefreshMask(int _itemId)
    {
        Debug.Log($"当前尝试刷新的道具是：{_itemId}");
        //如果背包中不存有该道具，那么就激活蒙版，阻止售卖：
        if(!ItemManager.Instance.itemList.Contains(_itemId) && _itemId == myItemId)
        {
            inactiveMaskObject.gameObject.SetActive(true);
        }


    }
}
