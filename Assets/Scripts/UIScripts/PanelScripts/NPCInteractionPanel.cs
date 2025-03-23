using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class NPCInteractionPanel : BasePanel
{
    //不管是什么NPC都公用的内容；
    public Image imgNPC;
    public TextMeshProUGUI txtSanity;
    public TextMeshProUGUI txtConversation;

    //需要判断当前的NPC是什么类型的，根据这个进行信仰Button的实例化；
    public Button btnDiscardBelief;
    public Button btnChooseBelief;

    //设置当前面板显示的NPC，因为没有数据结构类，所以暂时使用GameObject;
    public UnityAction<string> setNPCAction;

    protected override void Awake()
    {
        base.Awake();
        setNPCAction += SetCurrentNPC;
        //根据当前的NPC类型实例化btn；
    }
    protected override void Init()
    {
        btnDiscardBelief?.onClick.AddListener(()=>{

        });

        btnChooseBelief?.onClick.AddListener(()=>{

        });
    }

    void OnDestroy()
    {
        setNPCAction -= SetCurrentNPC;
    }

    protected void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            UIManager.Instance.HidePanel<NPCInteractionPanel>();
            EventHub.Instance.EventTrigger("Freeze", false);
        }
    }

    private void SetCurrentNPC(string _npcName)
    {
        Debug.Log($"NPC交互面板已显示，显示NPC为：{_npcName}");
    }

 
}
