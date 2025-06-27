using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class NPCInteractionPanel : BasePanel
{
    //不管是什么NPC都公用的内容；
    public Image imgNPC;
    public Transform npcPos;
    public TextMeshProUGUI txtSanity;
    public TextMeshProUGUI txtConversation;
    

    

    //需要判断当前的NPC是什么类型的，根据这个进行信仰Button的实例化；
    public Button btnDiscardBelief;
    public Button btnChooseBelief;
    public Button btnQuit;

    //继续当前对话的Button：
    public Button btnContinue;

    //设置当前面板显示的NPC，因为没有数据结构类，所以暂时使用GameObject;
    public UnityAction<E_NPCName> setNPCAction;

    //测试用：当前交互的NPC：
    public E_NPCName currentNPCName;
    private bool isContinueButtonClicked;
    private string rootPath = "ArtResources/Shop";

    protected override void Awake()
    {
        base.Awake();
        setNPCAction += SetCurrentNPC;
        // //根据当前的NPC类型实例化btn；
        // if(currentNPCName == E_NPCName.奈亚拉 || currentNPCName == E_NPCName.优格 || currentNPCName == E_NPCName.莎布)
        // {
        //     btnChooseBelief.gameObject.SetActive(true);
        // }
        // else
        // {
        //     btnDiscardBelief.gameObject.SetActive(true);
        // }


        // EventHub.Instance.AddEventListener<UnityAction<E_NPCName>>("BroadcastCurrentInteractingNPC", BroadcastCurrentInteractingNPC);
    }
    protected override void Init()
    {
        // btnDiscardBelief.onClick.AddListener(()=>{
            
        // });

        // btnChooseBelief.onClick.AddListener(()=>{
        //     UIManager.Instance.ShowPanel<ChooseBeliefPanel>().setNPCAction(currentNPCName);
        //     UIManager.Instance.HidePanel<NPCInteractionPanel>();

        // });

        btnQuit.onClick.AddListener(()=>{
            UIManager.Instance.HidePanel<NPCInteractionPanel>();
            EventHub.Instance.EventTrigger<bool>("Freeze", false);
        });

        btnContinue.onClick.AddListener(()=>{
            isContinueButtonClicked = true;
        });

    }

    void OnDestroy()
    {
        setNPCAction -= SetCurrentNPC;

        // EventHub.Instance.RemoveEventListener<UnityAction<E_NPCName>>("BroadcastCurrentInteractingNPC", BroadcastCurrentInteractingNPC);
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
        //将当前交互的NPC
        currentNPCName = _npcName;
        string path = Path.Combine(rootPath, currentNPCName.ToString());

        Debug.Log($"当前尝试加载的NPC路径：{path}");
        imgNPC.sprite = Resources.Load<Sprite>(path);
        imgNPC.SetNativeSize();
    
        StartCoroutine(WaitForContinue(_npcName));
    }

    //测试用：
    // private void BroadcastCurrentInteractingNPC(UnityAction<E_NPCName> action)
    // {
    //     action?.Invoke(currentNPCName);
    // }

    private IEnumerator WaitForContinue(E_NPCName _npcName)  
    {

        
        switch (_npcName)
        {
            case E_NPCName.奈亚拉:
                txtConversation.text = TextManager.Instance.GetText("奈亚拉", "交互", "1");
                break;
                txtConversation.text = TextManager.Instance.GetText("奈亚拉", "交互", "1");
                break;
            case E_NPCName.优格:
                txtConversation.text = TextManager.Instance.GetText("优格", "交互", "1");
                break;
                txtConversation.text = TextManager.Instance.GetText("优格", "交互", "1");
                break;
            case E_NPCName.莎布:
                txtConversation.text = TextManager.Instance.GetText("莎布", "交互", "1");
                break;
                txtConversation.text = TextManager.Instance.GetText("莎布", "交互", "1");
                break;
            default:    //此处是格赫罗斯
                txtConversation.text = TextManager.Instance.GetText("格赫罗斯", "交互", "1");
                break;
                txtConversation.text = TextManager.Instance.GetText("格赫罗斯", "交互", "1");
                break;
        }
        isContinueButtonClicked = false;
        yield return new WaitUntil(() => isContinueButtonClicked);

        switch(_npcName){
            case E_NPCName.奈亚拉:
                txtConversation.text = TextManager.Instance.GetText("奈亚拉", "交互", "2");
                txtConversation.text = TextManager.Instance.GetText("奈亚拉", "交互", "2");
            break;
            case E_NPCName.优格:
                txtConversation.text = TextManager.Instance.GetText("优格", "交互", "2");
                txtConversation.text = TextManager.Instance.GetText("优格", "交互", "2");
            break;
            case E_NPCName.莎布:
                txtConversation.text = TextManager.Instance.GetText("莎布", "交互", "2");
                txtConversation.text = TextManager.Instance.GetText("莎布", "交互", "2");
            break;

            default:    //此处是格赫罗斯
                txtConversation.text = TextManager.Instance.GetText("格赫罗斯", "交互", "2");
                txtConversation.text = TextManager.Instance.GetText("格赫罗斯", "交互", "2");
            break;
        }

        switch(_npcName){
            case E_NPCName.奈亚拉:
            case E_NPCName.优格:
            case E_NPCName.莎布:
                isContinueButtonClicked = false;
                yield return new WaitUntil(() => isContinueButtonClicked);
            break;

            default:    //此处是格赫罗斯
                txtConversation.text = TextManager.Instance.GetText("格赫罗斯", "交互", "2");
                txtConversation.text = TextManager.Instance.GetText("格赫罗斯", "交互", "2");
            break;
        }



        

        UIManager.Instance.HidePanel<NPCInteractionPanel>();
        switch(_npcName){
            case E_NPCName.奈亚拉:
            case E_NPCName.优格:
            case E_NPCName.莎布:
                //进入购买界面：
                UIManager.Instance.ShowPanel<ChooseBeliefPanel>().InitCurrentPanel(_npcName);
            break;
            default:    //此处是格赫罗斯
                UIManager.Instance.ShowPanel<DiscardBeliefPanel>();
            break;
        }
       
        yield break;
        

    }

 
}
