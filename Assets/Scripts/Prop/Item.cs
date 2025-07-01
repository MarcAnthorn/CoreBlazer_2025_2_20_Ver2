using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
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
        Thread thread = new Thread(() => InOperation());
        thread.Start();
    }

    private void InOperation()
    {
        int timerIndex;
        lock (PlayerManager._lock)
        {
            timerIndex = Timers.Instance.AddTimer();
            Timers.Instance.SingleTimerStart(timerIndex);
            //在此处理持续时间内的增益效果
            PlayerManager.Instance.player.LVL.value += 20;
        }

        while (Timers.Instance.GetSingleTime(timerIndex) < EffectiveTime)
        {
            //在此阻塞，或者处理持续时间内的逻辑
        }
        lock (PlayerManager._lock)
        {
            Timers.Instance.RemoveTimer(timerIndex);
            PlayerManager.Instance.player.LVL.value -= 20;
        }
    }

}

public class Item_Tatakai : Item
{
    public override void Use()
    {
        Debug.Log($"道具 Item_Tatakai 使用！");
        PlayerManager.Instance.player.STR.value += 10;
    }

}

public class Item_LightUP : Item
{
    public override void Use()
    {
        Debug.Log($"道具 Item_LightUP 使用！");
        PlayerManager.Instance.player.LVL.value += 20;
    }
}

public class Item_Alive : Item
{
    public override void Use()
    {
        Debug.Log($"道具 Item_Alive 使用！");
        PlayerManager.Instance.player.HP.value_limit += 10;
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
