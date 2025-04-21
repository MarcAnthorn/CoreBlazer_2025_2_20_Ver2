using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class AVGPanel : BasePanel
{
    public Transform defaultPos;
    public Transform leftPos;
    public Transform midPos;
    public Transform rightPos;
    public Transform optionContainer;
    public Transform imgPos;
     //最新从表中读取出来的位置（可能是出现的位置，可能是目标移动的位置）
    private Transform currentTargetPos;
    public GameObject optionContainerGameObject;
    public TextMeshProUGUI txtConversation;
    public TextMeshProUGUI txtConverseNPCName;
   
    
    public GameObject imgBackground;
    public Button btnContinue;

    //当前正在处理的指令：
    public DialogueOrder currentOrder;
    //当前实例持有的OrderBlock
    public DialogueOrderBlock orderBlock;
    //文本继续按钮是否点击
    private bool isContinueButtonClicked = false;
    //选项是否有选择结束（在选项脚本中进行广播）
    private bool isChoiceMade = false;
    //是否准备进行选项的处理（在监测到下一行是中断指令的时候）
    private bool isReadyToUpdateOptions = false;

    //当前AVG演出是否结束：
    //入股结束，下一次点击就会退出AVG交互：
    private bool isAvgOver = false;

    public string currentBackgroundName;
    public string bgmName;
    [Range(0, 1)]
    public float intervalTime = 1f;
    //当前对话场景中包含的所有NPC（以NPC名字的形式存储）
    public Dictionary<string, GameObject> currentNPCDic = new Dictionary<string, GameObject>();
    //当前所有可能的立绘的RGB颜色字典，用于恢复NPC立绘的亮度：
    public Dictionary<string, Color> orginalColorDic = new Dictionary<string, Color>();
    //用于调暗NPC立绘的亮度：
    public Dictionary<string, Color> darkenColorDic = new Dictionary<string, Color>();

    public List<GameObject> optionList = new List<GameObject>();

    //当前进行的协程句柄：
    private Coroutine dialogueCoroutine;

    //avg callback:
    public UnityAction<int> callback = null;

    protected override void Awake()
    {
        base.Awake();
        EventHub.Instance.AddEventListener<DialogueOrderBlock>("BroadcastCurrentOrderBlock", BroadcastCurrentOrderBlock);
        EventHub.Instance.AddEventListener<int>("ChoiceIsMade", ChoiceIsMade);
    }

    void Update()
    {
        if(isAvgOver)
        {
            if(Input.GetMouseButtonDown(0))
            {
                Debug.Log($"AVG演出已关闭，关闭的AVG演出ID：{orderBlock.rootId}");
                //将玩家解冻：
                EventHub.Instance.EventTrigger<bool>("Freeze", false);
                UIManager.Instance.HidePanel<AVGPanel>();
            }
        }
    }

    void OnDestroy()
    {
        EventHub.Instance.RemoveEventListener<DialogueOrderBlock>("BroadcastCurrentOrderBlock", BroadcastCurrentOrderBlock);
        EventHub.Instance.RemoveEventListener<int>("ChoiceIsMade", ChoiceIsMade);
    }

    protected override void Init()
    {
        isAvgOver = false;

        //冻结玩家
        EventHub.Instance.EventTrigger<bool>("Freeze", true);

        btnContinue.onClick.AddListener(()=>{
            isContinueButtonClicked = true;
        });
        ExecuteOrder();
    }

    //执行指令的方法；是一个协同程序
    private void ExecuteOrder()
    {
        //此处应该会有向我广播当前的orderBlock的逻辑；
        //也就是初始化当前需要处理的avg orderBlock 的逻辑；



        //首个指令必定是1001：
        DialogueOrder first = orderBlock.orderDic[1001];
        //开启协程：
        dialogueCoroutine = StartCoroutine(ProcessOrder(first));
        



    }

    //接收首指令，并且顺序处理指令
    private IEnumerator ProcessOrder(DialogueOrder firstOrder)
    {
        currentOrder = firstOrder;
        while(true)
        {    
            E_OrderType type = currentOrder.orderType;

            Debug.Log($"当前指令代号：{currentOrder.orderId}");
            //当前指令是：普通指令 / 选项后对话指令
            if(type == E_OrderType.Common)
            {
                //处理背景：
                currentBackgroundName = currentOrder.backgroundName;
                if(currentBackgroundName != "0")
                {
                    imgBackground = Resources.Load<GameObject>("AVG/" + currentBackgroundName);
                    Instantiate(imgBackground, imgPos, false);
                    Debug.Log($"背景资源加载，资源名：{currentBackgroundName}");
                }

                //处理出现的NPC
                //这个需要分情况，
                //如果是NPC已经在场景中存在，那么就是移动位置
                //如果不存在，才是浮现的效果出现在对应的位置；

                
                
                string npcName = currentOrder.showUpNPCName.ToString();
                string loadName;

                bool isDiff = false;
                if(currentOrder.diffNPCName == E_NPCName.None)
                {
                    //如果差分栏是0，那么npcName就是showUpNPCName；
                    loadName = currentOrder.showUpNPCName.ToString();
                }
                else
                {
                    //如果差分栏不是0，那么npcName就是diffNPCName；对应要加载的资源就是showUpNPCName;
                    loadName = currentOrder.diffNPCName.ToString();
                    isDiff = true;
                }
                
                if(currentOrder.showUpNPCName != E_NPCName.None)
                {
                    //处理当前的位置：
                    switch(currentOrder.positionId)
                    {
                        case 0:
                            currentTargetPos = defaultPos;
                        break;
                        case 1:
                            currentTargetPos = leftPos;
                        break;
                        case 2:
                            currentTargetPos = midPos;
                        break;
                        case 3:
                            currentTargetPos = rightPos;
                        break;
                        default:
                            Debug.LogWarning("当前获取的位置id不存在，错误位置：AVGPanel");
                        break;
                    }

                    //处理NPC的出现（或者是位置重置）
                    if(currentNPCDic.ContainsKey(npcName))
                    {
                        //如果包含,并且不是差分指令，那么就是重置位置:
                        if(!isDiff)
                            MoveNPC(npcName, currentTargetPos);

                        else 
                        {
                            //差分：获取当前NPC位置；
                            //并且将差分立绘加载到当前位置，并且替换currentNPCDic中的value:

                            //修改：不要加载prefab，直接将美术资源加载出来，附上去就行：
                            Sprite newSprite = Resources.Load<Sprite>("NPC/AllDifferences" + loadName);

                            currentNPCDic[npcName].GetComponent<SpriteRenderer>().sprite = newSprite;
                        }
                    }
                    else
                    {
                        //不包含，就是加入Dictionary，同时让NPC出现在对应的位置
                        InitNPC(loadName, currentTargetPos);
                    }
                }

                //如果存在差分指令，那么就是处理差分：
                //将currentNPCDic中的NPCGameObject替换成当前的立绘：

                //处理NPC消失的逻辑：
                //直接复用npcName这个变量：
                npcName = currentOrder.disappearNPCName.ToString();
                if(currentOrder.disappearNPCName != E_NPCName.None)
                {
                    Debug.Log($"Erased, name{npcName}");
                    EraseNPC(npcName);
                }


                npcName = currentOrder.conversationNPCName.ToString();
                //处理NPC立绘动效的逻辑：
                if(currentOrder.effectId != 0 && currentOrder.conversationNPCName != E_NPCName.None)
                {
                    NPCEffect(npcName, currentOrder.effectId);
                }

                //处理对话者逻辑：
                //注意：如果没有对话需要处理，说明是过程动画，给一个固定的时间间隔，然后就会继续处理下一个order；
                if(currentOrder.conversationNPCName != E_NPCName.None)
                {
                    ConverseWithNPC(npcName, currentOrder.orderText);

                    //等待玩家点击后再进行：
                    isContinueButtonClicked = false;    //等待；
                    yield return new WaitUntil(() => isContinueButtonClicked);
                }

                else if(currentOrder.conversationNPCName == E_NPCName.None && currentOrder.orderText != "0")
                {
                    ConverseWithNPC("", currentOrder.orderText);
                    //等待玩家点击后再进行：
                    isContinueButtonClicked = false;    //等待；
                    yield return new WaitUntil(() => isContinueButtonClicked);
                }

                //如果对话者名字为空，同时无对话文本，那么就是过场order（即：处理人物出现 / 消失等等的order）
                else 
                {
                    //等待一定时间就继续：先设置等设定的时间；
                    yield return new WaitForSeconds(intervalTime);
                }

                //更新当前order（只有Common的指令才是顺序的， Option & Break都不是严格顺序的）
                if(currentOrder.nextOrderId == -1)
                {
                    Debug.LogWarning("演出已终止");
                    //trigger the callback:
                    callback?.Invoke(orderBlock.rootId);
                    //将锁开启，下一次点击就会关闭avg；
                    isAvgOver = true;
                    break;
                }
                currentOrder = orderBlock.orderDic[currentOrder.nextOrderId];
            }

            //当前指令是：选项类型
            else if(type == E_OrderType.Option)
            {
                //如果是选项类型，那么就会一直执行指令；直到中断指令出现
                //处理当前的orderId对应的选项内容：
                GameObject option = Instantiate(Resources.Load<GameObject>("DialogueOptionButton"), optionContainer, false);
                option.SetActive(false);
                DialogueOptionBtn script = option.GetComponent<DialogueOptionBtn>();
                script.Init(currentOrder);
                optionList.Add(option);

                //更新当前的order：
                //其orderId就是当前的Option的orderId + 1:
                //前提是下一行不是中断指令
                if(currentOrder.nextLineOrderId / 1000 != 3)
                {
                    currentOrder = orderBlock.orderDic[currentOrder.orderId + 1];
                }
                //如果下一行是中断指令，那么预备在下一个循环中处理所有的选项；
                else
                {
                    isReadyToUpdateOptions = true;
                }
            }
            
            //当前类型是：中断指令（也就是准备好了要进行选项的显示和更新了）
            if(isReadyToUpdateOptions)
            {
                Debug.Log("此处是中断指令");
                isReadyToUpdateOptions = false;    //更新一次就重置；

                //中断指令的出现，说明之前处理的是选项；因此需要处理选项；
                //值得注意的是，Common指令之后一定不会出现中断指令；

                //处理选项：
                //将选项全部激活：
                optionContainerGameObject.SetActive(true);
                foreach(var option in optionList)
                {
                    option.SetActive(true);
                }

                //继续处理的条件是：等待选项的选择：
                //首先先终止；
                isChoiceMade = false;
                yield return new WaitUntil(() => isChoiceMade); //只有选项点击选择之后才会继续；

                //本来应该是在继续之后先更新当前的指令：
                //但是我们将指令更新的逻辑迁移到了ChoiceMade方法中，也就是选项点击之后就会立刻更新：


            }
        }
    }



    //移动NPC的方法：传入的参数是目标移动位置：
    private void MoveNPC(string name, Transform targetPos)
    {
        float moveDuration = 1f;
        LeanTween.move(currentNPCDic[name], targetPos, moveDuration)
            .setEase(LeanTweenType.easeInOutQuad);
    }

    //初始化NPC的方法：出现并且将其放置在对应的位置：
    private void InitNPC(string name, Transform targetPos)
    {
        Debug.Log($"当前出现的NPC是:{name}");
        GameObject npc = Instantiate(Resources.Load<GameObject>("NPC/" + name), targetPos, false);
        currentNPCDic.Add(name, npc);

        //处理其渐显的逻辑：
        Image npcImage = npc.gameObject.GetComponent<Image>();
        Color color = npcImage.color;
        if(!orginalColorDic.ContainsKey(name))
        {
            orginalColorDic.Add(name, color);
        }

        color.a = 0;
        npcImage.color = color;


        float moveDuration = 0.4f;
        LeanTween.alpha(npcImage.rectTransform, 1, moveDuration);

    }

    //处理NPC消失的方法：
    private void EraseNPC(string name)
    {
        Debug.Log($"当前消除的NPC是:{name}");
        if(!currentNPCDic.ContainsKey(name))
        {
            Debug.LogWarning($"当前要移除的NPC不存在，NPC名：{name}");
            return;
        }

        //从字典中移除：
        GameObject nowRemoveNPC = currentNPCDic[name];
        currentNPCDic.Remove(name);

        Image npcImage = nowRemoveNPC.gameObject.GetComponent<Image>();
        float moveDuration = 0.2f;
        LeanTween.alpha(npcImage.rectTransform, 0, moveDuration)
            .setOnComplete(()=>{
                Destroy(nowRemoveNPC);
            });     
    }

    //处理NPC对话逻辑：
    private void ConverseWithNPC(string name, string conversation)
    {
        foreach(var key in currentNPCDic.Keys)
        {
            if(key != name)
            {
                //如果不是当前对话的NPC，则调暗Image；
                DarkenNPCImage(key);
            }
            else
            {
                //如果是当前对话的NPC，则调整Image维正常亮度；
                LightenNPCImage(key);
            }
        }

        //然后在对话框中显示对话内容，并且调整对话者的名字：
        txtConversation.text = conversation;
        txtConverseNPCName.text = name;

    }

    //处理NPC的Image暗化的逻辑：参数是NPC名字
    //用于在对话的时候，将所有不是当前对话的Image调暗；
    private void DarkenNPCImage(string name)
    {
        Image targetImage = currentNPCDic[name].GetComponent<Image>();
        if(!darkenColorDic.ContainsKey(name))
        {
            //如果没有调暗过，那么就调暗颜色后加入dic：
            float factor = 0.4f;
            Color currentColor = targetImage.color;
            currentColor.r *= factor;       // 调整红色通道
            currentColor.g *= factor;       // 调整绿色通道
            currentColor.b *= factor;       // 调整蓝色通道

            darkenColorDic.Add(name, currentColor);

        }
        targetImage.color = darkenColorDic[name];  // 应用新的颜色
    }

    //恢复亮度的方法：
    private void LightenNPCImage(string name)
    {
        Image targetImage = currentNPCDic[name].GetComponent<Image>();
        targetImage.color = orginalColorDic[name];  // 应用原先的的颜色
    }

    private void NPCEffect(string npcName, int effectId)
    {
        GameObject npc = currentNPCDic[npcName];
        switch(effectId)
        {
            case 1:
                //抖动
                Debug.LogWarning("抖动");
            break;
            case 2:
                //镜像
                Debug.LogWarning("镜像");
            break;
        }
    }


    //方法：外部调用，通过广播当前需要显示的对话的orderBlock，执行对话：
    private void BroadcastCurrentOrderBlock(DialogueOrderBlock _orderBlock)
    {
        orderBlock = _orderBlock;
    }   

    //方法；DialogueOptionBtn中调用：更新布尔变量，让对话继续：
    private void ChoiceIsMade(int nextOrderId)
    {
        isChoiceMade = true;
        //更新当前需要处理的选项：
        currentOrder = orderBlock.orderDic[nextOrderId];

        //清空当前显示的所有Options，并且清空List：
        optionContainerGameObject.SetActive(false);
        foreach(var option in optionList)
        {
            Destroy(option);   
        }
        optionList.Clear();
    }


}


public enum E_NPCName{
    None = -1,
    奈亚拉 = 0,
    优格 = 1,
    纱布 = 2,  
    妇人 = 3,
    施耐德太太 = 4,
}

public enum E_OrderType{
    Common = 0,
    Option = 1,
    Break = 2,

}

public class DialogueOrder
{
    //所有的成员，如果没有对应的内容，赋予默认值（string给null, 枚举给None）
    //当前指令所属的演出id；
    public int rootId;
    //当前指令的id
    public int orderId;
    //位置的flag；
    public int positionId;
    //人物效果的flag：1是抖动，2是镜像；
    public int effectId;
    //下一个指令的索引id；
    public int nextOrderId;

    //这个是在策划表中，当前指令行的下一行的指令的orderId；
    //规范是 只有当前指令行是option类型的时候，才需要辛苦你为这个字段进行赋值；如2001赋值的就是2002；2002赋值的就是3001；
    //其他类型的指令行不需要为其赋值；
    //作用是用于确定中断指令的位置的（如果有问题可以直接问我）
    public int nextLineOrderId;

    //当前指令的类型：
    //读取的时候，千位数对应：1 -> Common; 2 -> Option; 3 -> Break
    public E_OrderType orderType;
    
    //当前要展示的NPC名称（枚举类）
    public E_NPCName showUpNPCName;
    //当前的差分NPC（如果有差分的话），如果有（比方说优格疑惑），那么此处就是优格；
    public E_NPCName diffNPCName;
    //当前要消失的NPC名称
    public E_NPCName disappearNPCName;
    //当前对话的NPC名称
    public E_NPCName conversationNPCName;

    //当前需要展示的背景名（也是背景资源相对路径）
    public string backgroundName;
    //开始播放的Audioclip资源名；
    public string audioClipStartName;
    //结束的音效资源名
    public string audioClipEndName;

    //包含了选项文本和对话文本两个东西的string：
    public string orderText;

}