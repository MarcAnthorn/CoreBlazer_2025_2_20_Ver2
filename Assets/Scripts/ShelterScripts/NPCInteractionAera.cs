using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCInteractionAera : MonoBehaviour
{
    private bool isTriggerLock = true;
    private GameObject txtObject;
    private Vector3 offset = new Vector3(0, 0.5f);
    
    public E_NPCName myName;

    //出现非默认对话的时候，悬浮的符号：
    public GameObject warningMarkObject;

    void Awake()
    {
        EventHub.Instance.AddEventListener("UpdateWaringMark", UpdateWaringMark);
    }

    //在Star中更新：
    void Start()
    {
        UpdateWaringMark();
    }   

    private void Update()
    {
        if (!isTriggerLock)
        {
            if (Input.GetKeyDown(KeyCode.J))
            {
                UIManager.Instance.ShowPanel<NPCInteractionPanel>().setNPCAction(myName);
                EventHub.Instance.EventTrigger<bool>("Freeze", true);
            }

            else if (Input.GetKeyDown(KeyCode.K))
            {
                int avgId = AVGDistributeManager.Instance.FetchAVGId(myName);

                //如果是格赫，那么还需要在第一次遇见的时候贡献三个AVG给三神：
                //AVGDistributionManager中确保了不会重复添加avgid:
                //迁移到了TestMazeStart中了：
                // AVGDistributeManager.Instance.ContributeAVGId(E_NPCName.奈亚拉, 2501);
                // AVGDistributeManager.Instance.ContributeAVGId(E_NPCName.优格, 2502);
                // AVGDistributeManager.Instance.ContributeAVGId(E_NPCName.莎布, 2503);
                

                UIManager.Instance.ShowPanel<AVGPanel>().InitAVG(avgId);
                EventHub.Instance.EventTrigger<bool>("Freeze", true);
            }
        }
    }

    void OnDestroy()
    {
        EventHub.Instance.RemoveEventListener("UpdateWaringMark", UpdateWaringMark);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.CompareTag("Player"))
        {
            isTriggerLock = false;
            
            // 从对象池获取提示文本对象，添加空值检查
            txtObject = PoolManager.Instance.SpawnFromPool("TipText");
            // 安全检查：确保PoolManager实例存在
            if (PoolManager.Instance == null)
            {
                Debug.LogError("[NPCInteractionAera] PoolManager实例不存在！");
                return;
            }
            
            
            if (txtObject != null)
            {
                // 设置提示文本内容和位置
                TextManager.SetContentFont(this.gameObject);
                EventHub.Instance.EventTrigger<string, Vector3>("SetTipContent", TextManager.Instance.GetText("交互提示", "信仰", "绑定"), this.transform.position + offset);
            }
            else
            {
                Debug.LogError("[NPCInteractionAera] 无法从对象池获取TipTexts对象！请检查对象池配置。");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if(other.gameObject.CompareTag("Player"))
        {
            isTriggerLock = true;
            // 安全返回对象到对象池
            if (txtObject != null && PoolManager.Instance != null)
            {
                PoolManager.Instance.ReturnToPool("TipTexts", txtObject);
                txtObject = null; // 清空引用
            }
            else if (txtObject != null)
            {
                // 如果PoolManager不存在，直接销毁对象避免内存泄漏
                Debug.LogWarning("[NPCInteractionAera] PoolManager不存在，直接销毁txtObject");
                Destroy(txtObject);
                txtObject = null;
            }
        }
    }

    //广播方法：在ContributeAVGId后统一调用
    //以防出现没有触发Start但是需要显示WarningMark的情况出现
    private void UpdateWaringMark()
    {
        //如果出现了新的对话，那么就让WaringMark激活：
        if(AVGDistributeManager.Instance.JudgeNewConversation(myName))
        {
            warningMarkObject.SetActive(true);
        }

        else 
        {
            warningMarkObject.SetActive(false);
        }
    }
}
