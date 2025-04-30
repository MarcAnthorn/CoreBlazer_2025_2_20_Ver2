using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


//展示用Item：
public class ItemLogic : MonoBehaviour
{
    
    //当前持有的Item（存储了相关Item信息的，通过id访问LoadManager实现获取）
    public Item myItem;
    //当前持有的Item其对应的id：
    public int myItemId;


    public Image imgSelf;
    public TextMeshProUGUI txtSelfName;

    public bool isOutlineActivated;
    private Button btnSelf;

    //这个不是自己挂载的GameObject，而是自己的父对象；
    public GameObject itemObject;
    private Outline outline;

    void Awake()
    {
        btnSelf = this.GetComponent<Button>();
        outline = this.GetComponent<Outline>();
        imgSelf = this.GetComponent<Image>();
        txtSelfName = this.GetComponentInChildren<TextMeshProUGUI>();
        isOutlineActivated = false;


    }
    void Start()
    {

        btnSelf.onClick.AddListener(()=>{
            UIManager.Instance.ShowPanel<ItemCheckPanel>().InitItemInfo(myItem, false);
        });

        string rootPath = "ArtResources/Item/" + myItem.name;
        imgSelf.sprite = Resources.Load<Sprite>(rootPath);
    }

    //初始化方法Item和信息Item；
    //以及初始化部分显示的内容
    public void Init(Item _item)
    {
        myItem = _item;
        myItemId = _item.id;
        //初始化名称和使用剩余次数；
        txtSelfName.text = myItem.name;
    }


}
