using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerManager : Singleton<PlayerManager>          //用于管理角色的事件
{
    public Player player;
    public Vector3 initPosition;

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
            HP = new playerAttribute(1) { value = 100},  
            // 生命值 (Health Point)，id = 1，初始值 100

            STR = new playerAttribute(2) { value = 10 },           
            // 力量 (Strength)，id = 2，初始值 10

            DEF = new playerAttribute(3) { value = 5 },           
            // 防御 (Defense)，id = 3，初始值 5

            LVL = new playerAttribute(4) { value = 1 },            
            // 灯光值 (Light Value)，id = 4，初始值 1

            SAN = new playerAttribute(5) { value = 40 },           
            // SAN 值 (Sanity)，id = 5，初始值 40

            SPD = new playerAttribute(6) { value = 10 },          
            // 速度 (Speed)，id = 6，初始值 10

            CRIT_Rate = new playerAttribute(7) { value = 0.1f, type = 1 }, 
            // 暴击率 (Critical Hit Rate)，id = 7，初始值 10%

            CRIT_DMG = new playerAttribute(8),                    
            // 暴击伤害 (Critical Damage)，id = 8，未初始化

            HIT = new playerAttribute(9),                          
            // 连击 (Hit)，id = 9，未初始化

            AVO = new playerAttribute(10) { value = 0.3f, type = 1 }, 
            // 闪避值 (AVO)，id = 10，初始值 30%

            bag = new Dictionary<int, Item>()  // 初始化物品栏
        };
    }




}
