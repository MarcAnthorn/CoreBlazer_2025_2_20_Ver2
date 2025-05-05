using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC20018 : NPCBase
{
    //后续节点：
    //节点20019 & 20020:
    public GameObject node20019;
    public GameObject node20020;



    //该节点同时也是一个灯塔，会亮两次；
    //直接使用两个失活的灯塔对象就行；
    //第一次亮起的灯塔对象：
    public GameObject lightHouseOne;

    //第二次亮起的灯塔对象在节点20020激活之后就会被触发；


    protected override void OnComplete(int avgId)
    {
        base.OnComplete(avgId);

        node20019.SetActive(true);
        node20020.SetActive(true);

        GameLevelManager.Instance.avgIndexIsTriggeredDic[avgId] = true;
        Destroy(this.gameObject);
    }

    protected override void Awake()
    {
        base.Awake();
        avgId = 2108;  

       
        //自己激活时，如果上一次死亡我触发过，那么直接调用OnComplete，然后将自己失活返回；
        if(GameLevelManager.Instance.avgIndexIsTriggeredDic.ContainsKey(avgId) && GameLevelManager.Instance.avgIndexIsTriggeredDic[avgId]) 
        {
            OnComplete(avgId);
            this.gameObject.SetActive(false);
            return;
        }
        
        lightHouseOne.SetActive(true);
        GameLevelManager.Instance.avgIndexIsTriggeredDic.TryAdd(avgId, false);
    }

} 