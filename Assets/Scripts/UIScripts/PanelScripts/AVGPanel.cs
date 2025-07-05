using System.Collections;
using System.Collections.Generic;
using System.IO;
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
    
    public Image imgBackground;
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
    private bool isTimerDone = false;

    //当前AVG演出是否结束：
    //入股结束，下一次点击就会退出AVG交互：
    private bool isAvgOver = false;
    private bool isErased = false;

    //当前处理的选项是否被封锁：
    private bool isOptionLocked = false;
    //当前处理的选项是否可以goto：
    //需要满足：1.ifKey达到count；2.存在goto指令
    private bool isGotoLocked = true;

    public string currentBackgroundName;
    public static int replaceTriggerCount;
    public string bgmName;
    //加载的路径：
    private string rootPath = Path.Combine("ArtResources", "AVG");
    
    [Range(0, 1)]
    public float intervalTime = 1f;
    //当前对话场景中包含的所有NPC（以NPC名字的形式存储）
    public Dictionary<string, GameObject> currentNPCDic = new Dictionary<string, GameObject>();
    //当前所有可能的立绘的RGB颜色字典，用于恢复NPC立绘的亮度：
    public Dictionary<string, Color> orginalColorDic = new Dictionary<string, Color>();
    //用于调暗NPC立绘的亮度：
    public Dictionary<string, Color> darkenColorDic = new Dictionary<string, Color>();
    //当前播放的循环音效：
    public Dictionary<string, AudioSource> audioDic = new Dictionary<string, AudioSource>();

    //用于处理if赋值、判断的字典：
    public Dictionary<int, int> ifKeyCountDic = new Dictionary<int, int>();

    public List<GameObject> optionList = new List<GameObject>();

    //当前进行的协程句柄：
    private Coroutine dialogueCoroutine;

    //avg callback:
    public UnityAction<int> callback = null;

    public GameObject npcObjectTarget1;
    public GameObject npcObjectTarget2;
    public GameObject npcObjectTarget3;

    public Queue<GameObject> npcObjectQueue;

    protected override void Awake()
    {
        replaceTriggerCount = 0;
        base.Awake();
        EventHub.Instance.AddEventListener<DialogueOrderBlock>("BroadcastCurrentOrderBlock", BroadcastCurrentOrderBlock);
        EventHub.Instance.AddEventListener<int, DialogueOrder>("ChoiceIsMade", ChoiceIsMade);

        EventHub.Instance.AddEventListener<UnityAction<int>>("ReplaceCallback", ReplaceCallback);

        

        npcObjectQueue = new Queue<GameObject>();
        npcObjectQueue.Enqueue(npcObjectTarget1);
        npcObjectQueue.Enqueue(npcObjectTarget2);
        npcObjectQueue.Enqueue(npcObjectTarget3);

    }

    void Update()
    {
        if(isAvgOver)
        {
            Debug.Log($"AVG演出已关闭，关闭的AVG演出ID：{orderBlock.rootId}");
            isAvgOver = false;  //确保只进入一次这个if判断；
            //将玩家解冻：
            EventHub.Instance.EventTrigger<bool>("Freeze", false);
            UIManager.Instance.HidePanel<AVGPanel>();
        }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.LogWarning("Try to setting");
            UIManager.Instance.ShowPanel<SettingPanel>();
        }
    }

    void OnDestroy()
    {
        EventHub.Instance.RemoveEventListener<DialogueOrderBlock>("BroadcastCurrentOrderBlock", BroadcastCurrentOrderBlock);
        EventHub.Instance.RemoveEventListener<int, DialogueOrder>("ChoiceIsMade", ChoiceIsMade);

        EventHub.Instance.RemoveEventListener<UnityAction<int>>("ReplaceCallback", ReplaceCallback);
    }

    
    protected override void Init()
    {
        isAvgOver = false;

        EventHub.Instance.EventTrigger<UnityAction>("HideMask", null);

        //冻结玩家
        EventHub.Instance.EventTrigger<bool>("Freeze", true);

        btnContinue.onClick.AddListener(()=>{
            isContinueButtonClicked = true;
        });
        ExecuteOrder();
    }

    /// <summary>
    /// 初始化AVG唯一方法；
    /// 在使用UIManager.Instance.ShowPanel<AVGPanel>显示AVG面板之后，可以获取到AVGPanel脚本，
    /// 通过AVGPanel可以调用InitAVG，从而初始化必要的AVG内容；
    /// </summary>
    /// <param name="dialogueBlockId">初始化的AVG唯一id，参考策划给定的AVG触发需求，传入对应的id就行</param>
    /// <param name="callback">AVG播放结束之后自动调用的回调；默认是null，表示当前AVG无后续结果需要处理；参数是整型，表示对应AVG的id，是否使用该id取决于是否在回调中有使用id的需求；绝大多数情况下，该参数只传不用</param>
    public void InitAVG(int dialogueBlockId, UnityAction<int> callback = null)
    {
        this.orderBlock =  LoadManager.Instance.orderBlockDic[dialogueBlockId];
        this.callback = callback;
    }

    //执行指令的方法；是一个协同程序
    private void ExecuteOrder()
    {
        //当前演出的id由触碰的NPC事件决定；外部会给的；

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
            isErased = false;
            isTimerDone = false;
            isOptionLocked = false;
            isGotoLocked = true;


            //处理IF指令：
            int ifKey = currentOrder.ifKey;
            int targetCount = currentOrder.ifKeyTriggeredCount;
            //首先要是分支选项才行，因此ifKey != 0
            if (ifKey != 0)
            {
                //如果当前的指令不是选项但是存在分支策略，抛出警告：
                if (type != E_OrderType.Option)
                    Debug.LogWarning($"当前尝试对一个非选项执行分支处理，id为：{currentOrder.orderId}");

                isOptionLocked = true;  //先置true，符合条件再置false
                if (ifKeyCountDic.ContainsKey(ifKey) && ifKeyCountDic[ifKey] >= targetCount)
                {
                    //符合条件：
                    isOptionLocked = false;

                    //如果此时选项还存在goto指令，则解放goto：
                    if (currentOrder.gotoRootId != 0)
                    {
                        isGotoLocked = false;
                    }
                    else
                    {
                        Debug.LogWarning($"当前选项存在if，但是不存在分支，id为：{currentOrder.orderId}");
                    }
            
                }
                
            }

            //还存在一个情况，ifKey是0，但是当前依然是存在有效的goto：
            else if (currentOrder.gotoRootId != 0)
            {
                //解锁 isGotoLocked 和 isOptionLocked
                isGotoLocked = false;
                isOptionLocked = false;
            }

            //处理当前的指令贡献：
            int contributeKey = currentOrder.contributeKey;
            if (currentOrder.contributeKey != 0)
            {
                if (ifKeyCountDic.ContainsKey(contributeKey))
                    ifKeyCountDic[contributeKey]++;
                else
                    ifKeyCountDic.Add(contributeKey, 1);
            }
            
            

            //当前指令是：普通指令 / 选项后对话指令
            if (type == E_OrderType.Common)
            {
                LeanTween.delayedCall(0.5f, () =>
                {
                    isTimerDone = true;
                });
                //处理NPC消失的逻辑：
                //直接复用npcName这个变量：
                string npcName = currentOrder.disappearNPCName.ToString();
                if (currentOrder.disappearNPCName != "0")
                {
                    EraseNPC(npcName);
                }

                //处理背景：
                currentBackgroundName = currentOrder.backgroundName;
                if (currentBackgroundName != "0")
                {
                    string spritePath = Path.Combine(rootPath, currentBackgroundName);

                    imgBackground.sprite = Resources.Load<Sprite>(spritePath);
                    imgBackground.SetNativeSize();
                }


                //处理音效：
                if (currentOrder.audioClipStartName != "0" || currentOrder.audioClipEndName != "0")
                {
                    //如果两个都不是0，并且两个相等，那么就是瞬时的一次性音效：
                    if (currentOrder.audioClipStartName == currentOrder.audioClipEndName)
                    {
                        SoundEffectManager.Instance.PlaySoundEffect(currentOrder.audioClipStartName);
                    }

                    //如果开始播放的不是0，并且当前行不终止，那么就是循环音效：
                    else if (currentOrder.audioClipStartName != "0")
                    {
                        //播放循环音效，同时加入字典，便于之后停止：
                        SoundEffectManager.Instance.PlaySoundEffect(currentOrder.audioClipStartName, true, (audio) =>
                        {
                            audioDic.Add(currentOrder.audioClipStartName, audio);
                        });

                    }

                    //如果结束播放不是0，那么就是终止之前的循环音效：
                    else if (currentOrder.audioClipEndName != "0")
                    {
                        SoundEffectManager.Instance.StopAllSoundEffect(audioDic[currentOrder.audioClipEndName]);
                    }
                }

                //处理出现的NPC
                //这个需要分情况，
                //如果是NPC已经在场景中存在，那么就是移动位置
                //如果不存在，才是浮现的效果出现在对应的位置；         
                npcName = currentOrder.showUpNPCName.ToString();

                //当前要显示的NPC名称，用于加载npc美术资源；
                string loadName;

                bool isDiff = false;
                if (currentOrder.diffNPCName == "0")
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

                if (currentOrder.showUpNPCName != "0")
                {
                    //处理当前的位置：
                    switch (currentOrder.positionId)
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
                    if (currentNPCDic.ContainsKey(npcName))
                    {
                        //如果包含,并且不是差分指令，那么就是重置位置:
                        if (!isDiff)
                            MoveNPC(npcName, currentTargetPos);

                        else
                        {
                            //差分：获取当前NPC位置；
                            //并且将差分立绘加载到当前位置，并且替换currentNPCDic中的value:

                            //修改：不要加载prefab，直接将美术资源加载出来，附上去就行：
                            string path = Path.Combine(rootPath, loadName);
                            Sprite newSprite = Resources.Load<Sprite>(path);

                            Image nowImage = currentNPCDic[npcName].GetComponent<Image>();
                            nowImage.sprite = newSprite;
                            nowImage.SetNativeSize();
                        }
                    }

                    //如果名字不是？？？，那么就是加载NPC：
                    else if (npcName != "???")
                    {
                        //不包含，就是加入Dictionary，同时让NPC出现在对应的位置
                        InitNPC(npcName, loadName, currentTargetPos);
                    }
                }

                //如果存在差分指令，那么就是处理差分：
                //将currentNPCDic中的NPCGameObject替换成当前的立绘：

                npcName = currentOrder.conversationNPCName.ToString();
                //处理NPC立绘动效的逻辑：
                if (currentOrder.effectId != 0 && currentOrder.conversationNPCName != "0")
                {
                    NPCEffect(npcName, currentOrder.effectId);
                }

                //处理对话者逻辑：
                //注意：如果没有对话需要处理，说明是过程动画，给一个固定的时间间隔，然后就会继续处理下一个order；
                if (currentOrder.conversationNPCName != "0")
                {
                    ConverseWithNPC(npcName, currentOrder.orderText);

                    Debug.Log($"当前对话对象：{npcName}");

                    //等待玩家点击后再进行：
                    isContinueButtonClicked = false;    //等待；
                    yield return new WaitUntil(() => isContinueButtonClicked && isTimerDone);
                    isContinueButtonClicked = false;    //等待；
                }

                else if (currentOrder.conversationNPCName == "0" && currentOrder.orderText != "0")
                {
                    ConverseWithNPC("", currentOrder.orderText);
                    //等待玩家点击后再进行：
                    isContinueButtonClicked = false;    //等待；
                    yield return new WaitUntil(() => isContinueButtonClicked && isTimerDone);
                    isContinueButtonClicked = false;    //等待；
                }

                //如果对话者名字为空，同时无对话文本，那么就是过场order（即：处理人物出现 / 消失等等的order）
                else
                {
                    //等待一定时间就继续：先设置等设定的时间；
                    yield return new WaitForSeconds(intervalTime);
                }

                //更新当前order（只有Common的指令才是顺序的， Option & Break都不是严格顺序的）
                if (currentOrder.nextOrderId == -1)
                {
                    Debug.LogWarning("演出已终止");
                    //trigger the callback:
                    callback?.Invoke(orderBlock.rootId);

                    //将锁开启，下一次点击就会关闭avg；
                    isAvgOver = true;

                    //尝试更新：
                    EventHub.Instance.EventTrigger("UpdateWaringMark");
                    break;
                }

                //此处：如果存在满足了GOTO条件的情况（满足IF的触发的次数累计到count次）
                //那么需要替换掉下一个过程的orderBlock，并且注意不要清除当前AVG中的相关资源（包括背景等等）；
                currentOrder = orderBlock.orderDic[currentOrder.nextOrderId];
            }

            //当前指令是：选项类型
            else if (type == E_OrderType.Option)
            {
                //如果选项是封锁的，那么直接就是不生成选项：
                if(!isOptionLocked)
                {
                    //如果是选项类型，那么就会一直执行指令；直到中断指令出现
                    //处理当前的orderId对应的选项内容：
                    GameObject option = Instantiate(Resources.Load<GameObject>("DialogueOptionButton"), optionContainer, false);
                    option.SetActive(false);
                    DialogueOptionBtn script = option.GetComponent<DialogueOptionBtn>();

                    //由于If列的出现，导致选项是否被封锁，需要先判断并且告知该选项：
                    //并且，如果goto解锁了，那么需要告知该选项是分支选项；
                    //isGotoLocked如果是false，那么_isBranchChoice就是true
                    script.Init(currentOrder, isOptionLocked, !isGotoLocked);
                    optionList.Add(option);
                }
                

                //更新当前的order：
                //其orderId就是当前的Option的orderId + 1:
                //前提是下一行不是中断指令
                Debug.LogWarning(currentOrder.nextLineOrderId);
                if (currentOrder.nextLineOrderId / 1000 != 3)
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
    private void InitNPC(string keyName, string loadName, Transform targetPos)
    {
        Debug.Log($"当前出现的NPC是:{keyName}");
        GameObject npc = npcObjectQueue.Dequeue();


        npc.transform.SetParent(targetPos, false);
        npc.transform.localPosition = new Vector3(0, 0, 140);
        
        //加载对应的资源到SpriteRenderer上：
        Image img = npc.GetComponent<Image>();

        string path = Path.Combine(rootPath, loadName);

        //当前尝试加载的路径：
        Debug.Log($"当前尝试加载的路径{path}");

        img.sprite = Resources.Load<Sprite>(path);
        img.SetNativeSize();

        currentNPCDic.Add(keyName, npc);

        //处理其渐显的逻辑：
        Color color = img.color;
        if(!orginalColorDic.ContainsKey(keyName))
        {
            color.a = 255;
            // color.r = 255;
            // color.g = 255;
            // color.b = 255;
            orginalColorDic.Add(keyName, color);
        }

        color.a = 0;
        img.color = color;


        float moveDuration = 0.4f;
        LeanTween.alpha(img.rectTransform, 1, moveDuration);

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

        Image img = nowRemoveNPC.gameObject.GetComponent<Image>();
        float moveDuration = 0.2f;
        LeanTween.alpha(img.rectTransform, 0, moveDuration)
            .setOnComplete(()=>{
                npcObjectQueue.Enqueue(nowRemoveNPC);
                isErased = true;
            });     
    }

    //处理NPC对话逻辑：
    private void ConverseWithNPC(string name, string conversation)
    {
        //然后在对话框中显示对话内容，并且调整对话者的名字：
        txtConversation.text = conversation;
        txtConverseNPCName.text = name;

        //如果是？？？那么一定就不是显示在场景上的对象；直接返回，避免访问不存在的key：
        if(name == "???")   
            return;

        foreach(var key in currentNPCDic.Keys)
        {
            if(key != name)
            {
                //如果不是当前对话的NPC，则调暗Image；
                DarkenImage(key);
            }
            else
            {
                //如果是当前对话的NPC，则调整Image维正常亮度；
                LightenImage(key);
            }
        }

    }

    //处理NPC的Image暗化的逻辑：参数是NPC名字
    //用于在对话的时候，将所有不是当前对话的Image调暗；
    private void DarkenImage(string name)
    {
        Debug.LogWarning($"{name} is darken!");

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
    private void LightenImage(string name)
    {
        Image targetImage = currentNPCDic[name].GetComponent<Image>();
        targetImage.color = orginalColorDic[name];  // 应用原先的的颜色
    }

    private void NPCEffect(string npcName, int effectId)
    {
        if(!currentNPCDic.ContainsKey(npcName))
            return;
            
        GameObject npc = currentNPCDic[npcName];
        switch(effectId)
        {
            case 1:
                //抖动
                ShakeUIObject(npcName, npc);
            break;
            case 2:
                //镜像
                Vector3 scale = npc.transform.localScale;
                scale.x *= -1;   // 反转X轴
                npc.transform.localScale = scale;
            break;
        }
    }

    public void ShakeUIObject(string npcName, GameObject uiObject, float duration = 0.2f, float magnitude = 15f)
    {
        if (uiObject == null) return;

        RectTransform rectTransform = uiObject.GetComponent<RectTransform>();
        if (rectTransform == null) return;

        Vector3 originalPos = rectTransform.anchoredPosition;

        // 使用 LeanTween 来实现 DOShake 效果
        LeanTween.value(uiObject, 0f, 1f, duration).setOnUpdate((float t) =>
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;
            rectTransform.anchoredPosition = originalPos + new Vector3(x, y);
        }).setOnComplete(() =>
        {
            rectTransform.anchoredPosition = originalPos;
            if(!isErased && currentOrder.disappearNPCName != "0")
            {
                //如果erase没有执行成功，那么就再执行一次：
                EraseNPC(npcName);
                isErased = true;
            }
        });
    }

    //方法：外部调用，清理avg的回调，并且替代成新的回调：
    //UnityAction<int> newCallback的int参数无意义，只是为了匹配avg的原先的callback
    private void ReplaceCallback(UnityAction<int> newCallback)
    {
        callback = newCallback;
        // if (callback != null)
        // {
        //     callback = null;
        //     callback = newCallback;
        // }
    }

    //方法：外部调用，通过广播当前需要显示的对话的orderBlock，执行对话：
    private void BroadcastCurrentOrderBlock(DialogueOrderBlock _orderBlock)
    {
        orderBlock = _orderBlock;
    }

    //方法；DialogueOptionBtn中调用：更新布尔变量，让对话继续：
    //规定：如果flag给定的是-1，说明当前被选中的选项，是分支选项；需要根据goto的内容进行avg的跳转；
    //如果flag是1，那么就是正常的继续
    private void ChoiceIsMade(int flag, DialogueOrder nowOptionOrder)
    {
        isChoiceMade = true;
        //注意：如果是-1，那么更新orderBlock和currentOrder:
        if (flag == -1)
        {
            orderBlock = LoadManager.Instance.orderBlockDic[nowOptionOrder.gotoRootId];
            currentOrder = orderBlock.orderDic[1001];


            //手动插入2304的战斗事件：
            if(nowOptionOrder.gotoRootId == 2304)
            {
                ReplaceCallback((id) => {
                    //2304的回调：触发战斗：
                    GameLevelManager.Instance.avgIndexIsTriggeredDic.TryAdd(2304, true);

                    //销毁交流达贡：
                    EventHub.Instance.EventTrigger("DestroyDagoon");
                    //生成战斗模式的达贡：
                    var dagoon = Resources.Load<GameObject>("NPC/战斗点达贡");
                    Instantiate(dagoon, PlayerManager.Instance.PlayerTransform.position, Quaternion.identity);    
                    
                });
            }

            //手动插入2311的事件回调：
            if(nowOptionOrder.gotoRootId == 2311)
            {
                ReplaceCallback((id) => {
                    //2311的回调：触发奖励，添加AVG，并且销毁达贡：
                    //销毁交流达贡：
                    EventHub.Instance.EventTrigger("DestroyDagoon");
                    
                    //添加AVG：
                    AVGDistributeManager.Instance.ContributeAVGId(E_NPCName.奈亚拉, 2408);
                    AVGDistributeManager.Instance.ContributeAVGId(E_NPCName.优格, 2409);
                    AVGDistributeManager.Instance.ContributeAVGId(E_NPCName.莎布, 2410);
                    
                    
                });
            }

        }
        else
        {
            //更新当前需要处理的选项：
            currentOrder = orderBlock.orderDic[nowOptionOrder.nextOrderId];
        }

        //清空当前显示的所有Options，并且清空List：
        optionContainerGameObject.SetActive(false);
        foreach (var option in optionList)
        {
            Destroy(option);
        }
        optionList.Clear();
    }


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

    //if指定的key：
    public int ifKey;
    //if指定的key的触发次数;如果不满足，会封锁当前的optionButton；
    public int ifKeyTriggeredCount;
    //当前语句的贡献ifKey：如果有贡献，那么字典中，ifKey对应的value + 1:
    public int contributeKey;
    //goto的演出表rootid：
    public int gotoRootId;
    

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
    public string showUpNPCName;
    //当前的差分NPC（如果有差分的话），如果有（比方说优格疑惑），那么此处就是优格；
    public string diffNPCName;
    //当前要消失的NPC名称
    public string disappearNPCName;
    //当前对话的NPC名称
    public string conversationNPCName;

    //当前需要展示的背景名（也是背景资源相对路径）
    public string backgroundName;
    //开始播放的Audioclip资源名；
    public string audioClipStartName;
    //结束的音效资源名
    public string audioClipEndName;

    //包含了选项文本和对话文本两个东西的string：
    public string orderText;

}