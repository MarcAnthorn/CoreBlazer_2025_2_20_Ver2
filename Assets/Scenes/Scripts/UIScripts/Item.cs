using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//测试用Item数据结构类，可以删除或者是被替代
public class Item : MonoBehaviour
{
    public Button btnItemInteract;
    private int itemID;
    private int itemType;
    private bool isUsable = false;

    public Item(int _id, int _type, bool _isUsable)
    {

    }

    void Start()
    {
        btnItemInteract.onClick.AddListener(()=>{

        });
    }
}
