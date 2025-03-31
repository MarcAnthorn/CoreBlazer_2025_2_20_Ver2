using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class Prop                  //所有道具类的基类
{
    public string name;
    public int id;
    public PropType type;
    public bool isImmediate;
    public int[] usableScene = new int[] { 0, 0, 0 };   // 1代表可使用
    public bool resetAfterDeath;
    public bool quickEquip;
    public bool reObtain;
    public int maxLimit;                                //???感觉不用上面的reObtain也行
    public bool isPermanent;
    public float EffectiveTime;                         //表示回合：(int)(EffectiveTime/2)
    public string instruction;                          //表示使用说明
    public string description;                          //表示道具文案

    public enum PropType
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



public class Prop1 : Prop
{
    public override void Use()
    {
        Debug.Log("道具 Prop1 使用！");
    }

}

public class Prop2 : Prop
{
    public override void Use()
    {
        Debug.Log("道具 Prop2 使用！");
    }
}

public class Prop3 : Prop
{
    public override void Use()
    {
        Debug.Log("道具 Prop3 使用！");
    }
}
