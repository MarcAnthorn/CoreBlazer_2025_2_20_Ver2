using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeWall : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        gameObject.tag = "FakeWall";
        EventHub.Instance.AddEventListener<bool>("LockBreakerOrNot", LockBreakerOrNot);
    }

    public bool isBreakerLocked = true;

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Collider entered!");

        if(!isBreakerLocked && collision.gameObject.CompareTag("WallBreaker"))
        {
            //如果解锁了，执行破墙：
            this.gameObject.SetActive(false);
            UIManager.Instance.ShowPanel<WarningPanel>().SetWarningText($"使用道具「几何拳套」破除了虚假的墙壁！");

            //执行广播：消除当前生效的道具：在Item_103中
            EventHub.Instance.EventTrigger("UsedCallback");
        }
    }


    void OnDestroy()
    {
        EventHub.Instance.RemoveEventListener<bool>("LockBreakerOrNot", LockBreakerOrNot);
    }

    
    private void LockBreakerOrNot(bool isLocked)
    {
        isBreakerLocked = isLocked;
    }
}
