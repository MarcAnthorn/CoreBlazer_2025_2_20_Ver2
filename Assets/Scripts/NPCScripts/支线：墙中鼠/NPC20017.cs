using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC20017 : NPCBase
{
    //六个20016对象，在当前对象触发之后激活：
    public List<GameObject> aftermathNodes = new List<GameObject>();


    protected override void OnComplete(int avgId)
    {
        base.OnComplete(avgId);

        foreach(var obj in aftermathNodes)
        {
            obj.gameObject.SetActive(true);
        }

        Destroy(this.gameObject);
    }

    private void Awake()
    {
        avgId = 2101;  

        //自己激活时，如果上一次死亡我触发过，那么直接调用OnComplete，然后将自己失活返回；
        if(GameLevelManager.Instance.avgIndexIsTriggeredDic[avgId]) 
        {
            OnComplete(avgId);
            this.gameObject.SetActive(false);
            return;
        }

        GameLevelManager.Instance.avgIndexIsTriggeredDic.Add(avgId, false);
    }

} 