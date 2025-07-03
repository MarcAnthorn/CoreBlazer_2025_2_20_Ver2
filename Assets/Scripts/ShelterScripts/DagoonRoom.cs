using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DagoonRoom : MonoBehaviour
{
    private bool isTriggerLock = true;
    private GameObject txtObject;
    private Vector3 offset = new Vector3(0, 0.5f);
    private E_NPCName myName = E_NPCName.达贡;

    void Awake()
    {
        if(GameLevelManager.Instance.avgIndexIsTriggeredDic.ContainsKey(2304) && GameLevelManager.Instance.avgIndexIsTriggeredDic[2304] || GameLevelManager.Instance.avgIndexIsTriggeredDic.ContainsKey(2311) && GameLevelManager.Instance.avgIndexIsTriggeredDic[2311])
        {
            DestroyDagoon();
        }

        EventHub.Instance.AddEventListener("DestroyDagoon", DestroyDagoon);
    }

    void OnDestroy()
    {
        EventHub.Instance.RemoveEventListener("DestroyDagoon", DestroyDagoon);
    }

    private void Update()
    {
        if (!isTriggerLock)
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
                int avgId = AVGDistributeManager.Instance.FetchAVGId(myName);
                UIManager.Instance.ShowPanel<AVGPanel>().InitAVG(avgId);
                EventHub.Instance.EventTrigger<bool>("Freeze", true);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.CompareTag("Player"))
        {
            isTriggerLock = false;
            txtObject = PoolManager.Instance.SpawnFromPool("TipText");
            EventHub.Instance.EventTrigger<string, Vector3>("SetTipContent", "按下「K」进入对话", this.transform.position + offset);

        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if(other.gameObject.CompareTag("Player"))
        {
            isTriggerLock = true;
            PoolManager.Instance.ReturnToPool("TipTexts", txtObject);
        }
    }

    //销毁达贡：在战斗触发 or 2311播放结束之后调用：
    private void DestroyDagoon()
    {
        Destroy(this.gameObject);
    }
}
