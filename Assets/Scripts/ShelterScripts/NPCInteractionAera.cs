using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCInteractionAera : MonoBehaviour
{
    private bool isTriggerLock = true;
    private GameObject txtObject;
    private Vector3 offset = new Vector3(0, 0.5f);
    
    public E_NPCName myName;

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
}
