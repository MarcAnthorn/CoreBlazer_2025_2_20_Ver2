using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ChaseBattleBase : MonoBehaviour
{
    //当前点位的追逐怪对象：
    public GameObject chaser;
    //是否触发过：
    private bool isTriggered = false;
    //玩家进入触发点，触发追逐
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(!isTriggered && collision.gameObject.CompareTag("Player"))
        {
            //生成追逐怪
            LeanTween.delayedCall(1, ()=>{
                chaser.gameObject.SetActive(true);
            });
    
            isTriggered = true;
        }
    }
}
