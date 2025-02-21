using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    public Button btnItemInteract;
    private int itemID;
    private int itemType;

    public Item(int _id, int _type)
    {

    }

    void Start()
    {
        btnItemInteract.onClick.AddListener(()=>{

        });
    }
}
