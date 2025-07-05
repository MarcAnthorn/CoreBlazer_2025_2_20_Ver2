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
            txtObject = PoolManager.Instance.SpawnFromPool("TipText");
            var tmpUGUI = txtObject.GetComponent<TMPro.TextMeshProUGUI>();
            var tmp3D = txtObject.GetComponent<TMPro.TextMeshPro>();
            var font = Resources.Load<TMPro.TMP_FontAsset>("Noto_Sans_SC/static/NotoSansSC-Black SDF");
            if (font != null)
            {
                if (tmpUGUI != null) tmpUGUI.font = font;
                if (tmp3D != null) tmp3D.font = font;
                Debug.Log($"字体已设置为: {font.name}");
            }
            EventHub.Instance.EventTrigger<string, Vector3>("SetTipContent", TextManager.Instance.GetText("交互提示", "信仰", "绑定"), this.transform.position + offset);

        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if(other.gameObject.CompareTag("Player"))
        {
            isTriggerLock = true;
            PoolManager.Instance.ReturnToPool("TipTexts", txtObject);
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
