using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerManager : Singleton<PlayerManager>          //用于管理角色的事件
{
    public Player player;

    protected override void Awake()
    {
        base.Awake();

        //测试用：
        //该方法在Awake中调用，确保全局只触发一次
        InitPlayer();
    }

    //使用只读属性暴露玩家数值
    //报错，暂时先调整：（Marc调整）
    public float Health => player.HP.value;

    public float Strength => player.STR.value;

    public float Defence => player.DEF.value;
    public float Level => player.LVL.value;
    public float Sanity => player.SAN.value;

    public float Speed => player.SPD.value;

    // 暂未定义玩家灯火值
    // public int Light => player.LIT;



    public void InitPlayer()
    {
        //PlayerManager 管理的全局唯一Player实例
        player = new Player()
        {
            //报错，暂时先注释：（Marc调整）
            // HP = 100,
            // STR = 10,
            // DEF = 5,
            // LVL = 100,
            // SAN = 0,
            // SPD = 10    //测试用速度数值
        };
    }

    //通过单例的内部方法，进行玩家属性的调整
    //如：调整玩家血量：
    public void AdjustHealth(int adjustment){}
    //外部通过单例进行调整方法的调用；



}
