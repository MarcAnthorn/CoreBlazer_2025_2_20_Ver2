using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


//实现：不论如何，一个门全局只会被解锁一次；解锁后就会无限次开关
//只要isDoorInteractable == true;那么就可以无限次交互；确保这点就行；
public class ShortcutDoorLogic : DoorLogicBase
{
    //当前的门，第一次触发是开启，第二次是关闭，以此类推：
    private int triggerCount = 0;
    private SpriteRenderer sr;
    private BoxCollider2D bc;

    //当前的门的唯一id：
    //用于统计：是否触发过，如果触发过，则永远解锁：
    public int uniqueId = 0;

    protected override void Awake()
    {
        base.Awake();
        sr = GetComponent<SpriteRenderer>();
        bc = GetComponent<BoxCollider2D>();

        if(uniqueId == 0)
        {
            Debug.LogError("请为所有的Shortcut Door分配id！");
            return;
        }

        if(!GameLevelManager.Instance.doorIsUnlockedDic.TryAdd(uniqueId, false))
        {
            //如果当前的id已存在，那么尝试看是否解锁过：
            if(GameLevelManager.Instance.doorIsUnlockedDic[uniqueId])
            {
                //触发过，那么解锁：
                isDoorInteractable = true;
            }
        }
    }
    protected override void DoorTrigger()
    {
       

        triggerCount++;

        //第一次触发的时候，尝试移除钥匙：
        if(triggerCount == 1)
        {
            //如果之前解锁过，那么不移除：
            if(GameLevelManager.Instance.doorIsUnlockedDic.ContainsKey(uniqueId) && GameLevelManager.Instance.doorIsUnlockedDic[uniqueId]){}

            else{
                ItemManager.Instance.RemoveItem(509);   //移除钥匙；
            }
            

            //但是将可交互选项置true：
            //确保后续依然可以交互；
            isDoorInteractable = true;
        }

        if(triggerCount % 2 == 1)
        {
            //奇数次触发：开门
            sr.enabled = false; 
            bc.isTrigger = true;    

            //触发寻路中的更改路径flag的方法：
            PathFindingManager.Instance.ModifyGridFlag(0, this.gameObject.transform.position);

        }
        else
        {
            //偶数次触发：关门
            sr.enabled = true; 
            bc.isTrigger = false;   

            PathFindingManager.Instance.ModifyGridFlag(1, this.gameObject.transform.position); 
        }

        //最后再检测：Trigger过，那么将这这个id的door加入到doorIsUnlockedDic：
        GameLevelManager.Instance.doorIsUnlockedDic[uniqueId] = true;
    }

}
