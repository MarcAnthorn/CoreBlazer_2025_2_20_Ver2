using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class NPCInteractionPanel : BasePanel
{
    //不管是什么NPC都公用的内容；
    public GameObject npcObj;
    public Transform npcPos;
    public TextMeshProUGUI txtSanity;
    public TextMeshProUGUI txtConversation;

    //需要判断当前的NPC是什么类型的，根据这个进行信仰Button的实例化；
    public Button btnDiscardBelief;
    public Button btnChooseBelief;
    public Button btnQuit;

    //设置当前面板显示的NPC，因为没有数据结构类，所以暂时使用GameObject;
    public UnityAction<E_NPCName> setNPCAction;

    //测试用：当前交互的NPC：
    public E_NPCName currentNPCName;

    protected override void Awake()
    {
        base.Awake();
        setNPCAction += SetCurrentNPC;
        //根据当前的NPC类型实例化btn；
        if(currentNPCName == E_NPCName.奈亚拉 || currentNPCName == E_NPCName.优格 || currentNPCName == E_NPCName.纱布)
        {
            btnChooseBelief.gameObject.SetActive(true);
        }
        else
        {
            btnDiscardBelief.gameObject.SetActive(true);
        }


        EventHub.Instance.AddEventListener<UnityAction<E_NPCName>>("BroadcastCurrentInteractingNPC", BroadcastCurrentInteractingNPC);
    }
    protected override void Init()
    {
        btnDiscardBelief.onClick.AddListener(()=>{
            
        });

        btnChooseBelief.onClick.AddListener(()=>{
            UIManager.Instance.ShowPanel<ChooseBeliefPanel>().setNPCAction(currentNPCName);
            UIManager.Instance.HidePanel<NPCInteractionPanel>();

        });

        btnQuit.onClick.AddListener(()=>{
            UIManager.Instance.HidePanel<NPCInteractionPanel>();
            EventHub.Instance.EventTrigger<bool>("Freeze", false);
        });
    }

    void OnDestroy()
    {
        setNPCAction -= SetCurrentNPC;

        EventHub.Instance.RemoveEventListener<UnityAction<E_NPCName>>("BroadcastCurrentInteractingNPC", BroadcastCurrentInteractingNPC);
    }

    protected void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            UIManager.Instance.HidePanel<NPCInteractionPanel>();
            EventHub.Instance.EventTrigger("Freeze", false);
        }
    }

    private void SetCurrentNPC(E_NPCName _npcName)
    {
        Debug.Log($"NPC交互面板已显示，显示NPC为：{_npcName.ToString()}");
        //将当前交互的NPC
        currentNPCName = _npcName;
        string path = currentNPCName.ToString();
        npcObj = Instantiate(Resources.Load<GameObject>("NPC/" + path), npcPos, false);
    }

    //测试用：
    private void BroadcastCurrentInteractingNPC(UnityAction<E_NPCName> action)
    {
        action?.Invoke(currentNPCName);
    }

 
}
