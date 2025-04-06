using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Unity.Mathematics;
using UnityEngine;

[System.Serializable]
public abstract class Item                  //所有道具类的基类
{
    public string name;
    public int id;
    public ItemType type;
    public bool isImmediate;
    public int useTimes;
    public int[] usableScene = new int[] { 0, 0, 0 };   // 1代表可使用
    public bool resetAfterDeath;
    public bool quickEquip;
    public bool reObtain;
    public int maxLimit;                                //???感觉不用上面的reObtain也行
    public bool isPermanent;
    public float EffectiveTime;                         //表示回合时：(int)(EffectiveTime/2)
    public string instruction;                          //表示使用说明
    public string description;                          //表示道具文案
    public Buff buff;

    public enum ItemType
    {
        None = 0,
        God_Maze = 1,
        God_Battle = 2,
        Maze = 3,
        Battle = 4,
        Normal = 5,
        Tarot = 6               //塔罗牌
    }

    public bool CanUseOrNot(int sceneId)        // 1-事件选择 2-战斗场景 3-迷宫内
    {
        if (usableScene[sceneId - 1] == 1)
        {
            return true;
        }

        return false;
    }

    public abstract void Use();

}



public class Item_101 : Item
{
    public override void Use()
    {
        Debug.Log($"道具 灯火助燃剂 使用！");
        //可视范围扩大5格
        int timerIndex;
        timerIndex = Timers.Instance.AddTimer(8f, () => OnStart(), () => OnComplete());
    }

    private void OnStart()
    {
        PlayerManager.Instance.PlayerAttributeChange(AttributeType.LVL, +20f);
    }

    private void OnComplete()
    {
        PlayerManager.Instance.PlayerAttributeChange(AttributeType.LVL, -20f);
    }

}

public class Item_102 : Item
{
    public override void Use()
    {
        Debug.Log($"道具 封存的灯火 使用！");
        //获得后，后续每次额外获得2灯光值
        BuffManager.Instance.AddBuff(BuffType.LVL_Change, +2f);     //代表加成
    }

}

public class Item_103 : Item
{
    public override void Use()
    {
        Debug.Log($"道具 大力出奇迹 使用！");
        //使用后靠近特殊墙壁可砸碎
        int timerIndex;
        timerIndex = Timers.Instance.AddTimer(8f, () => OnStart(), () => OnComplete());
    }

    int index;
    private void OnStart()
    {
        index = BuffManager.Instance.AddBuff(BuffType.DamageWall, DamageWall);
    }

    private void OnComplete()
    {
        BuffManager.Instance.RemoveBuff(index);
    }

    public Action DamageWall;                   //由Marc将实现方法写入其中

}

public class Item_104 : Item
{
    public override void Use()
    {
        Debug.Log($"道具 传送门 使用！");
        //使用后有50%概率直接到达该层迷宫终点，也有50%概率回到起点
        OnUse();
    }

    private void OnUse()
    {
        int randomNum = UnityEngine.Random.Range(0, 2);
        if (randomNum == 0)
        {
            //到达该层迷宫终点

        }
        else if(randomNum == 1)
        {
            //回到起点

        }
    }

}

public class Item_BloodMedicine : Item
{
    public override void Use()
    {
        Debug.Log($"道具 Item_BloodMedicine 使用！");
        PlayerManager.Instance.player.HP.value += 5;
    }
}

public class Item_Tarot1 : Item
{
    public override void Use()
    {
        Debug.Log($"道具 Item_Tarot 使用！");
        PlayerManager.Instance.player.LVL.value += 2;
    }
}
