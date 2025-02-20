using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : Singleton<PlayerManager>          //用于管理角色的事件
{
    public Player player;

    protected override void Awake()
    {
        base.Awake();
    }

    public void InitPlayer()
    {
        player = new Player()
        {
            HP = 100,
            STR = 10,
            DEF = 5,
            LVL = 100,
            SAN = 0
        };
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
