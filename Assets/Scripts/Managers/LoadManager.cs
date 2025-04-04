using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using static Event;

public class LoadManager : Singleton<LoadManager>
{
    public Dictionary<int, Dialogue> dialogueDictionary;       // 对话文本库
    public Dictionary<int, Event> startEvents;
    public Dictionary<int, Event> optionEvents;
    public Dictionary<int, Event.EventResult> eventResults;         //表示所有事件的所有结果(性能优化)
    public Dictionary<int, Item> allItems;
    
    //指令字典，管理的是 按照演出id区分的DialogueOrderBlock；
    //DialogueOrderBlock在 Dialogue文件夹下；
    //int -> rootId;
    public Dictionary<int, DialogueOrderBlock> orderBlockDic = new Dictionary<int, DialogueOrderBlock>();

    public int[,] map1Index;
    public MapElement[,] map1;


    protected override void Awake()
    {
        base.Awake();   //单例初始化

        //测试：加载资源
        LoadResources();
    }

    public void LoadResources()
    {
        LoadMapElements(1);
        InitMap1Elements(1);
        LoadAVGDialogues();
        //LoadDialogues(0);
        LoadEvents();
        // LoadItems();
    }




    private void LoadDialogues(int libIndex)
    {
        // DialogueOrderBlock block1 = new DialogueOrderBlock();
        // DialogueOrder currentOrder  = new DialogueOrder();



        // block1.orderDic.Add(1001, currentOrder);


        // DialogueOrderBlock block2 = new DialogueOrderBlock();
        // block2.orderDic.Add(1001, currentOrder);





        dialogueDictionary = new Dictionary<int, Dialogue>();
        string path = Path.Combine(Application.dataPath, "Resources/DialogueData/DialogueDatas.csv");

        if (File.Exists(path))
        {
            string[] lines = File.ReadAllLines(path);       //分割每一行存入lines

            for (int i = 1; i < lines.Length; i++)          //从第四行开始遍历每一行，获得各列的信息
            {
                string line = lines[i];
                string[] values = line.Split(',');          //将每一列按照逗号分割

                if (int.Parse(values[0]) == libIndex && values.Length >= 4 && int.Parse(values[2]) == 1)
                {
                    Dialogue dialogue = new Dialogue();
                    dialogue.eventId = int.Parse(values[0]);
                    dialogue.textId = int.Parse(values[1]);
                    dialogue.text = values[3];
                    dialogue.nextId = int.Parse(values[4]);
                    dialogue.illustrationId = int.Parse(values[5]);
                    dialogue.bgId = int.Parse(values[6]);

                    dialogueDictionary.Add(dialogue.textId, dialogue);
                }
                else if (values.Length <= 4)
                {
                    break;
                }

            }

        }
        else
        {
            Debug.LogError("对话文件未找到。");
        }
    }
    private void LoadItems()
    {
        allItems = new Dictionary<int, Item>();
        string path = Path.Combine(Application.dataPath, "Resources/ItemData/AllItems.csv");

        if (File.Exists(path))
        {
            string[] lines = File.ReadAllLines(path);
            for (int i = 1; i < lines.Length; i++)
            {
                string line = lines[i];
                string[] values = line.Split(',');

                if (values.Length > 3)
                {
                    //此处进行道具分类
                    Item Item = ItemManager.Instance.ClassifyItems(int.Parse(values[1]));
                    Item.name = values[0];                              //A列
                    Item.id = int.Parse(values[1]);                     //B列
                    Item.type = (Item.ItemType)int.Parse(values[2]);    //C列

                    if (values[3] == "1")                               //D列
                        Item.isImmediate = true;
                    else
                        Item.isImmediate = false;

                    Item.useTimes = int.Parse(values[4]);               //E列
                    char[] envs = values[5].ToCharArray();              //F列
                    for (int j = 0; j < 3; j++)
                        Item.usableScene[j] = (int)envs[j];

                    if (values[6] == "0")                               //G列
                        Item.resetAfterDeath = true;
                    else
                        Item.resetAfterDeath = false;

                    if (values[7] == "0")                               //H列
                        Item.quickEquip = true;
                    else
                        Item.quickEquip = false;

                    if (values[8] == "0")                               //I列
                        Item.reObtain = true;
                    else
                        Item.reObtain = false;

                    Item.maxLimit = int.Parse(values[9]);               //J列

                    if (values[10] == "0")                              //K列
                        Item.isPermanent = true;
                    else
                        Item.isPermanent = false;

                    Item.EffectiveTime = float.Parse(values[11]);       //L列
                    Item.instruction = values[12];                      //M列
                    Item.description = values[13];                      //N列

                    allItems.Add(Item.id, Item);
                }
                else
                {
                    Debug.LogError("道具表格文件未找到");
                }
            }
        }

    }

    private void InitMap1Elements(int mapId)                     //!!先设定一个地图，之后再想办法拓展!!
    {
        map1 = new MapElement[41, 41];
        for (int i = 0; i < map1Index.GetLength(0); i++)        //map1Index.GetLength(0) ==> 行数
        {
            for (int j = 0; j < map1Index.GetLength(1); j++)     //map1Index.GetLength(1) ==> 列数
            {
                map1[i, j] = MapManager.Instance.CreateMapElement(map1Index[i, j]);
            }
        }

    }
    private void LoadMapElements(int mapId)
    {
        map1Index = new int[41, 41];
        for (int i = 0; i < MapManager.Instance[mapId].row; i++)              //根据地图长宽来进行打印
        {
            for (int j = 0; j < MapManager.Instance[mapId].colume; j++)         //初始化地图上所有的地块Id
            {
                map1Index[i, j] = -1;
            }
        }
        string path = null;
        if (mapId >= 1 && mapId <= 3)
        {
            path = Path.Combine(Application.dataPath, $"Resources/MapDatas/Map{mapId}.csv");    //命名规范！！！

        }
        else
        {
            Debug.LogError($"不存在Id为 {mapId} 的地图");
            return;
        }

        if (File.Exists(path))
        {
            string[] lines = File.ReadAllLines(path);       //分割每一行存入lines

            for (int i = 0; i < lines.Length; i++)          //从第四行开始遍历每一行，获得各列的信息
            {
                string line = lines[i];
                string[] values = line.Split(',');          //将每一列按照逗号分割

                if (values.Length >= 4)
                {
                    for (int j = 0; j < MapManager.Instance[mapId].colume; j++)
                    {
                        if (mapId == 1)
                        {
                            map1Index[i, j] = int.Parse(values[j]);
                        }
                        else if (mapId == 2)
                        {

                        }
                        else if (mapId == 3)
                        {

                        }
                        //else
                        //{
                        //    Debug.LogError($"地图Id为 {mapId} 的地图不存在");
                        //}
                    }
                }
                else if (values.Length <= 4)    //遇到空行主动退出
                {
                    break;
                }

            }
        }
        else
        {
            Debug.LogError($"找不到Id为 {mapId} 的地图");          //表示没有在路径中找到该文件
        }
    }

    private void LoadEvents()           //在关卡初始化时调用(根据传入的库Id来加载对应库中的文本)
    {
        startEvents = new Dictionary<int, Event>();
        optionEvents = new Dictionary<int, Event>();
        EventManager.Instance.weights = new Dictionary<int, float>();
        //加载已有事件数据(CSV格式)到events字典中，使用Assets(Application.dataPath)下的相对路径
        string path = Path.Combine(Application.dataPath, "Resources/EventData/EventCSV/Test1_CSV.csv");
        int libIndex = 2001;

        if (File.Exists(path))
        {
            LoadEventResult();                                              //加载所有事件的所有结果
            string[] lines = File.ReadAllLines(path, Encoding.UTF8);        //分割每一行存入lines

            for (int i = 3; i < lines.Length; i++)          //从第四行开始遍历每一行，获得各列的信息
            {
                string line = lines[i];
                string[] values = line.Split(',');          //将每一列按照逗号分割

                if (int.Parse(values[0]) == libIndex && values.Length >= 5)
                {
                    Event eventData = new Event()
                    {
                        libId = int.Parse(values[0]),                                       //A列
                        eventId = int.Parse(values[1]),                                     //B列
                        eventType = (Event.MyEventType)int.Parse(values[2]),                //C列
                        grade = int.Parse(values[4])                                        //E列
                    };
                    int result_id = int.Parse(values[3]);                       //D列
                    if (result_id != 0)
                    {
                        eventData.hasResult = true;                         //表示有结果可以访问
                        eventData.result = GetEventResult(result_id);       //读入事件的对应结果
                    }

                    for (int j = 0; j < 3; j++)     //var option in eventData.options
                    {
                        EventOption option = new EventOption();
                        option.optionId = j;
                        option.conditionId = int.Parse(values[5 + j * 6]);                  //F列
                        option.minCondition = int.Parse(values[6 + j * 6]);                 //G列
                        option.maxCondition = int.Parse(values[7 + j * 6]);                 //H列
                        option.itemId = int.Parse(values[8 + j * 6]);                       //I列
                        option.OpDescription = values[9 + j * 6];                           //J列
                        option.NextId = int.Parse(values[10 + j * 6]);                      //K列
                        eventData.options.Add(option);
                    }

                    LoadKaidanTexts(eventData.eventId, eventData);        //加载事件对应的怪诞文本
                    if (int.Parse(values[1]) / 10 == libIndex && int.Parse(values[1]) % 10 == 1)
                    {
                        startEvents.Add(eventData.eventId, eventData);      //起始事件
                        EventManager.Instance.weights.Add(eventData.eventId, 1.0f);               //加入起始事件的权重（等权重）
                    }
                    else
                    {
                        optionEvents.Add(eventData.eventId, eventData);     //选项(后续)事件
                    }

                }

                if (i + 1 <= lines.Length - 1)      // 85 = 当前Test1_CSV表格的最后一行行号 - 1
                {
                    line = lines[i + 1];
                    int charToFind = line.IndexOf(",");
                    string firstValue = line.Substring(0, charToFind);
                    if (int.Parse(firstValue) != libIndex)          //如果下一行的事件库Id变化，则++
                        libIndex++;
                }
                //else
                //{
                //    Debug.LogWarning($"结束，当前 i 的值为：{i}");
                //}

            }

        }
        else
        {
            Debug.LogWarning("事件数据文件不存在！");
        }
    }
    private Event.EventResult GetEventResult(int resultId)
    {
        foreach (KeyValuePair<int, Event.EventResult> pair in eventResults)
        {
            if (pair.Key == resultId)
            {
                return pair.Value;
            }
        }

        return null;
    }
    private void LoadEventResult()        //一般来说不用外部调用
    {
        eventResults = new Dictionary<int, EventResult>();

        string path = Path.Combine(Application.dataPath, "Resources/EventData/EventResult/EventResults1.csv");
        if (File.Exists(path))
        {
            string[] lines = File.ReadAllLines(path, Encoding.UTF8);       //分割每一行存入lines

            for (int i = 4; i < lines.Length; i++)
            {
                string line = lines[i];
                string[] values = line.Split(",");

                if (values.Length > 4 /*&& int.Parse(values[0]) == resultId*/)
                {
                    Event.EventResult @eventResult = new Event.EventResult();
                    @eventResult.resultId = int.Parse(values[0]);      //涉及到覆盖，所以要重新再设置一次
                    @eventResult.spId = int.Parse(values[1]);

                    @eventResult.change_HP = float.Parse(values[2]);
                    @eventResult.change_HP_rate = float.Parse(values[3]);

                    @eventResult.change_STR = float.Parse(values[4]);
                    @eventResult.change_STR_rate = float.Parse(values[5]);

                    @eventResult.change_DEF = float.Parse(values[6]);
                    @eventResult.change_DEF_rate = float.Parse(values[7]);

                    @eventResult.change_LVL = float.Parse(values[8]);
                    @eventResult.change_LVL_rate = float.Parse(values[9]);

                    @eventResult.change_SAN = float.Parse(values[10]);
                    @eventResult.change_SAN_rate = float.Parse(values[11]);

                    @eventResult.change_SPD = float.Parse(values[12]);
                    @eventResult.change_SPD_rate = float.Parse(values[13]);

                    @eventResult.change_CRIT_Rate = float.Parse(values[14]);
                    @eventResult.change_CRIT_Rate_rate = float.Parse(values[15]);

                    @eventResult.change_CRIT_DMG = float.Parse(values[16]);
                    @eventResult.change_CRIT_DMG_rate = float.Parse(values[17]);

                    @eventResult.change_HIT = float.Parse(values[18]);
                    @eventResult.change_HIT_rate = float.Parse(values[19]);

                    @eventResult.change_AVO = float.Parse(values[20]);
                    @eventResult.change_AVO_rate = float.Parse(values[21]);

                    eventResults.Add(@eventResult.resultId, @eventResult);
                }

            }

        }

    }
    private void LoadKaidanTexts(int eventIndex, Event @event)     //用在Event加载的时候
    {
        @event.textLib = new Dictionary<int, KaidanText>();
        string path = Path.Combine(Application.dataPath, "Resources/DialogueData/KaidanTest.csv");

        if (File.Exists(path))
        {
            string[] lines = File.ReadAllLines(path, Encoding.UTF8);       //分割每一行存入lines

            for (int i = 1; i < lines.Length; i++)          //从第四行开始遍历每一行，获得各列的信息
            {
                string line = lines[i];
                string[] values = line.Split(',');          //将每一列按照逗号分割
                if (i == 1)
                {
                    @event.firstTextId = int.Parse(values[1]);
                }

                if (int.Parse(values[0]) == eventIndex && values.Length >= 4/* && int.Parse(values[2]) == 0*/)
                {
                    KaidanText kaidanText = new KaidanText();
                    kaidanText.eventId = int.Parse(values[0]);
                    kaidanText.textId = int.Parse(values[1]);
                    if (int.Parse(values[2]) == 1)
                    {
                        kaidanText.isKwaidan = false;
                    }
                    else if (int.Parse(values[2]) == 0)
                    {
                        kaidanText.isKwaidan = true;
                    }
                    kaidanText.text = values[3];
                    kaidanText.nextId = int.Parse(values[4]);
                    kaidanText.illustrationId = int.Parse(values[5]);
                    kaidanText.bgId = int.Parse(values[6]);

                    @event.evDescription += values[3];
                    @event.textLib.Add(kaidanText.textId, kaidanText);
                }

            }
        }
        else
        {
            Debug.LogError("怪诞文本文件未找到。事件Id：" + @event.eventId);
        }
    }

    private void LoadAVGDialogues()
    {
        Debug.Log("开始加载AVG内容");
        string path = Path.Combine(Application.dataPath, "Resources/DialogueData/AVGDialogues.csv");
        int showIndex = 1;
        DialogueOrderBlock tempBlock = new DialogueOrderBlock();

        if (File.Exists(path))
        {
            string[] lines = File.ReadAllLines(path, Encoding.UTF8);
            DialogueOrder dialogue = null;

            for (int i = 1; i < lines.Length; i++)
            {
                string line = lines[i];
                string[] values = line.Split(',');          //将每一列按照逗号分割
                string nextLine = null;
                string[] nextValues = null;
                if (i + 1 != lines.Length)
                {
                    nextLine = lines[i + 1];
                    nextValues = nextLine.Split(",");
                }

                if (int.Parse(values[0]) == showIndex && values.Length >= 5)
                {
                    dialogue = new DialogueOrder();
                    dialogue.rootId = int.Parse(values[0]);                 //A列
                    dialogue.orderId = int.Parse(values[1]);                //B列
                    dialogue.backgroundName = values[2];                    //C列

                    if (values[3] == "奈亚拉")                               //D列
                        dialogue.showUpNPCName = E_NPCName.奈亚拉;
                    else if (values[3] == "优格")
                        dialogue.showUpNPCName = E_NPCName.优格;
                    else if (values[3] == "纱布")
                        dialogue.showUpNPCName = E_NPCName.纱布;
                    else
                        dialogue.showUpNPCName = E_NPCName.None; 

                    dialogue.positionId = int.Parse(values[4]);             //E列

                    if (values[5] == "奈亚拉")                               //F列
                        dialogue.disappearNPCName = E_NPCName.奈亚拉;
                    else if (values[5] == "优格")
                        dialogue.disappearNPCName = E_NPCName.优格;
                    else if (values[5] == "纱布")
                        dialogue.disappearNPCName = E_NPCName.纱布;
                    else
                        dialogue.disappearNPCName = E_NPCName.None; 

                    dialogue.effectId = int.Parse(values[6]);               //G列

                    if (values[7] == "奈亚拉")                               //H列
                        dialogue.conversationNPCName = E_NPCName.奈亚拉;
                    else if (values[7] == "优格")
                        dialogue.conversationNPCName = E_NPCName.优格;
                    else if (values[7] == "纱布")
                        dialogue.conversationNPCName = E_NPCName.纱布;
                    else
                        dialogue.conversationNPCName = E_NPCName.None;    

                    dialogue.orderText = values[8];                         //I列
                    dialogue.nextOrderId = int.Parse(values[9]);            //J列
                    dialogue.audioClipStartName = values[10];               //K列
                    dialogue.audioClipEndName = values[11];                 //L列

                    if ((int.Parse(values[1]) / 1000) == 1)         //进行分类
                        dialogue.orderType = E_OrderType.Common;
                    else if ((int.Parse(values[1]) / 1000) == 2)
                        dialogue.orderType = E_OrderType.Option;
                    else if ((int.Parse(values[1]) / 1000) == 3)
                        dialogue.orderType = E_OrderType.Break;

                    //检测是否需要对nextLineOrderId赋值
                    if (nextLine != null && dialogue.orderType == E_OrderType.Option)   //可加处理细节
                    {
                        dialogue.nextLineOrderId = int.Parse(nextValues[1]);
                        nextLine = null;
                    }

                    // tempBlock.orderDic.Add(dialogue.rootId, dialogue);
                    tempBlock.orderDic.Add(dialogue.orderId, dialogue);

                }

                if (i + 1 <= lines.Length - 1)
                {
                    line = lines[i + 1];
                    int charToFind = line.IndexOf(",");
                    string firstValue = line.Substring(0, charToFind);
                    if (int.Parse(firstValue) != showIndex)          //代表下一行开始是新的演出id
                    {
                        Debug.Log("???");
                        orderBlockDic.Add(showIndex, tempBlock);
                        tempBlock = new DialogueOrderBlock();
                        showIndex++;
                    }
                }

                if(i + 1 == lines.Length)
                {
                    Debug.Log("orderBlock已添加2");
                    orderBlockDic.Add(showIndex, tempBlock);
                }

            }

        }
        else
        {
            Debug.LogWarning("事件数据文件不存在！");
        }
    }

}
