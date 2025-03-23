using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PropManager : Singleton<PropManager>
{
    private Inventory inventory;

    public void Awake()
    {
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

    public Prop ClassilyProps(int propId)               //用于将道具分类
    {
        switch (propId)
        {
            case 1:
                return new Prop1();
            case 2:
                return new Prop2();
            case 3:
                return new Prop3();
            default:
                Debug.LogWarning($"未找到id为 {propId} 的道具");
                return null;
        }
    }

}
