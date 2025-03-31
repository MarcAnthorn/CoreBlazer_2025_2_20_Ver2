using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PropManager : Singleton<PropManager>
{
    private Inventory inventory;

    protected override void Awake()
    {
        base.Awake();
        inventory = new Inventory();
    }

    public void GetProp(Prop prop)
    {
        inventory.AddProp(prop);
    }

    public void UseProp(Prop prop)
    {
        if (prop.CanUseOrNot(GameLevelManager.currentEnvironment) && inventory.props.Contains(prop))
        {
            prop.Use();
            inventory.RemoveProp(prop);
            return;
        }

        Debug.LogError($"无法使用该道具：{prop.name}");
    }

    public Prop ClassifyProps(int propId)               //用于将道具分类
    {
        switch (propId)
        {
            case 101:
                return new Prop_Glim();
            case 201:
                return new Prop_Tatakai();
            case 301:
                return new Prop_LightUP();
            case 401:
                return new Prop_Alive();
            case 501:
                return new Prop_BloodMedicine();
            case 601:
                return new Prop_Tarot1();
            default:
                Debug.LogWarning($"未找到id为 {propId} 的道具");
                return null;
        }
    }

}
