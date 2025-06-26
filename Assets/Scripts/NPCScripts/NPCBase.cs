using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class NPCBase : MonoBehaviour
{

    //当前NPC对应的avg演出id：
    public int avgId;
    public bool isTriggerLock = true;
    public UnityAction avgCallback = null;


    protected virtual void Update() {

    }

    protected void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.CompareTag("Player"))
        {
            UIManager.Instance.ShowPanel<AVGPanel>().InitAVG(avgId, OnComplete);
        }
    }


    //抽象方法：在交互完成之后的回调函数：
    protected virtual void OnComplete(int avgId)
    {
        isTriggerLock = true;
    }

    protected virtual void Awake()
    {
        
    }

}
