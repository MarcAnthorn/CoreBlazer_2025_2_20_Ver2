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

    //装备管理容器，存储了所有可能的装备种类（和Item实现基本一致）
    //用于存储策划表中的基本装备信息；
    //类似的，Classify方法在EquipmentManager中；

    //这个是正式版的语句：public Dictionary<int, Equipment> allEquipment 
    public Dictionary<int, Equipment> allEquipment = new Dictionary<int, Equipment>();

    //技能管理容器，存储了所有可能的技能种类（和Item一致）
    //类似的，Classify方法也在EquipmentManager中；
    public Dictionary<int, Skill> allSkills = new Dictionary<int, Skill>();
    
    //指令字典，管理的是 按照演出id区分的DialogueOrderBlock；
    //DialogueOrderBlock在 Dialogue文件夹下；
    //int -> rootId;
    public Dictionary<int, DialogueOrderBlock> orderBlockDic = new Dictionary<int, DialogueOrderBlock>();


    //教学关：
    public int[,] mapTutorialFloorIndex =  new int[21, 21];
    public MapElement[,] mapTutorialFloor;


    //第一层：
    public int[,] mapFirstFloorIndex =  new int[31, 31];
    public MapElement[,] mapFirstFloor;

    //第二层
    public int[,] mapSecondFloorIndex = new int[41, 41];
    public MapElement[,] mapSecondFloor;


    //第三层：
    public int[,] mapThirdFloorIndex = new int[43, 55];
    public MapElement[,] mapThirdFloor;


    protected override void Awake()
    {
        base.Awake();
        //测试：加载资源
        LoadResources();
    }

    public void LoadResources()
    {
     
        LoadMaps();



//--------------测试--------------------------------------------

        //加载所有的avg：
        LoadAllAvgs();

        //先技能后装备，因为装备需要LoadManager中的技能实例；
        // 初始化 allSkills（假设是 Dictionary<int, Skill>）
        // Dictionary<int, Skill> allSkills = new Dictionary<int, Skill>();

        // 手动实例化并添加所有技能
        Skill_1001 skill1001 = new Skill_1001();
        Skill_1002 skill1002 = new Skill_1002();
        Skill_1003 skill1003 = new Skill_1003();
        Skill_1004 skill1004 = new Skill_1004();
        Skill_1005 skill1005 = new Skill_1005();
        Skill_1006 skill1006 = new Skill_1006();
        Skill_1007 skill1007 = new Skill_1007();
        Skill_1008 skill1008 = new Skill_1008();
        Skill_1009 skill1009 = new Skill_1009();
        Skill_1010 skill1010 = new Skill_1010();
        Skill_1020 skill1020 = new Skill_1020();
        Skill_1023 skill1023 = new Skill_1023();

        // 添加到字典
        allSkills.Add(1001, skill1001);
        allSkills.Add(1002, skill1002);
        allSkills.Add(1003, skill1003);
        allSkills.Add(1004, skill1004);
        allSkills.Add(1005, skill1005);
        allSkills.Add(1006, skill1006);
        allSkills.Add(1007, skill1007);
        allSkills.Add(1008, skill1008);
        allSkills.Add(1009, skill1009);
        allSkills.Add(1010, skill1010);
        allSkills.Add(1020, skill1020);
        allSkills.Add(1023, skill1023);

        foreach(var key in LoadManager.Instance.allSkills.Keys)
        {
            Debug.Log($"key:{key},value : {LoadManager.Instance.allSkills[key].skillName }");
        }

        //测试：装备：
        // 初始化所有装备
        allEquipment.Add(1001, new Equipment_1001());
        allEquipment.Add(1002, new Equipment_1002());
        allEquipment.Add(1003, new Equipment_1003());
        allEquipment.Add(1004, new Equipment_1004());
        allEquipment.Add(1005, new Equipment_1005());
        allEquipment.Add(1006, new Equipment_1006());
        allEquipment.Add(1007, new Equipment_1007());
        allEquipment.Add(1008, new Equipment_1008());
        allEquipment.Add(1009, new Equipment_1009());
        // allEquipment.Add(1010, new Equipment_1010());
        // allEquipment.Add(1011, new Equipment_1011());
        // allEquipment.Add(1012, new Equipment_1012());
        // allEquipment.Add(1013, new Equipment_1013());
        // allEquipment.Add(1014, new Equipment_1014());
        // allEquipment.Add(1015, new Equipment_1015());
        // allEquipment.Add(1016, new Equipment_1016());
        // allEquipment.Add(1017, new Equipment_1017());
        // allEquipment.Add(1018, new Equipment_1018());
        allEquipment.Add(1019, new Equipment_1019());
        // allEquipment.Add(1020, new Equipment_1020());
        // allEquipment.Add(1021, new Equipment_1021());
        allEquipment.Add(1022, new Equipment_1022());


//--------------测试--------------------------------------------
        
        //LoadDialogues(0);
        LoadEvents();
        LoadItems();
    }


    private void LoadAllAvgs()
    {
        for(int i = 1101; i <= 1122; i++)
        {
            LoadAVGDialogues(i);
        }

        for(int i = 1201; i <= 1207; i++)
        {
            LoadAVGDialogues(i);
        }

        for(int i = 1301; i <= 1303; i++)
        {
            LoadAVGDialogues(i);
        }

        for(int i = 2101; i <= 2112; i++)
        {
            LoadAVGDialogues(i);
        }

        for(int i = 2201; i <= 2208; i++)
        {
            LoadAVGDialogues(i);
        }

        for(int i = 3101; i <= 3103; i++)
        {
            LoadAVGDialogues(i);
        }

        for(int i = 3201; i <= 3203; i++)
        {
            LoadAVGDialogues(i);
        }

        for(int i = 3301; i <= 3303; i++)
        {
            LoadAVGDialogues(i);
        }
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
        
        // string path = Path.Combine(Application.dataPath, "Resources/ItemData/AllItems.csv");
        //读取新表：
        string path = Path.Combine(Application.dataPath, "Resources/ItemData/ItemSheet.csv");
        

        if (File.Exists(path))
        {
            string[] lines = File.ReadAllLines(path);
            for (int i = 0; i < lines.Length; i++)
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
                        Item.usableScene[j] = (int)(envs[j] - '0');     // 1-事件选择 2-战斗场景 3-迷宫内

                    if (values[6] == "1")                               //G列
                        Item.resetAfterDeath = true;
                    else
                        Item.resetAfterDeath = false;

                    if (values[7] == "1")                               //H列
                        Item.quickEquip = true;
                    else
                        Item.quickEquip = false;

                    if (values[8] == "1")                               //I列
                        Item.reObtain = true;
                    else
                        Item.reObtain = false;

                    Item.maxLimit = int.Parse(values[9]);               //J列

                    if (values[10] == "1")                              //K列
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

    private void LoadMaps()
    {
        LoadMapElements(0);
        InitMapElements(0);

        LoadMapElements(1);
        InitMapElements(1);

        LoadMapElements(2);
        InitMapElements(2);

        LoadMapElements(3);
        InitMapElements(3);
    }
    //MapId对应：0 -> 新手关； 2 -> 第二层
    private void LoadMapElements(int mapId)
    {
        for (int i = 0; i < MapManager.Instance[mapId].row; i++)              //根据地图长宽来进行打印
        {
            for (int j = 0; j < MapManager.Instance[mapId].colume; j++)         //初始化地图上所有的地块Id
            {
                // Debug.Log($"行：{MapManager.Instance[mapId].row} 列：{MapManager.Instance[mapId].colume}");
                switch(mapId){
                    case 0:
                        mapTutorialFloorIndex[i, j] = -1;
                    break;

                    case 1:
                        mapFirstFloorIndex[i, j] = -1;
                    break;

                    case 2:
                        mapSecondFloorIndex[i, j] = -1;
                    break;

                    case 3:
                        mapThirdFloorIndex[i, j] = -1;
                    break;


                }
            }
        }


        string path = null;
        if (mapId >= 0 && mapId <= 3)
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
                        //遇到空字符串就跳过：这样就是维持-1；
                        if(values[j] == "")
                            continue;

                        switch(mapId){
                        case 0:
                            mapTutorialFloorIndex[i, j] = int.Parse(values[j]);
                        break;

                        case 1:
                            mapFirstFloorIndex[i, j] = int.Parse(values[j]);
                        break;

                        case 2:
                            mapSecondFloorIndex[i, j] = int.Parse(values[j]);
                        break;

                        case 3:
                            mapThirdFloorIndex[i, j] = int.Parse(values[j]);
                        break;
    
                    }
   
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

    private void InitMapElements(int mapId)                     //!!先设定一个地图，之后再想办法拓展!!
    {
        switch(mapId){
            case 0:
                mapTutorialFloor = new MapElement[21, 21];
                for (int i = 0; i < mapTutorialFloor.GetLength(0); i++)        //mapSecondFloorIndex.GetLength(0) ==> 行数
                {
                    for (int j = 0; j < mapTutorialFloor.GetLength(1); j++)     //mapSecondFloorIndex.GetLength(1) ==> 列数
                    {
                        mapTutorialFloor[i, j] = MapManager.Instance.CreateMapElement(mapTutorialFloorIndex[i, j]);
                    }
                }
            break;

            case 1:
                mapFirstFloor = new MapElement[31, 31];
                for (int i = 0; i < mapFirstFloor.GetLength(0); i++)        //mapSecondFloorIndex.GetLength(0) ==> 行数
                {
                    for (int j = 0; j < mapFirstFloor.GetLength(1); j++)     //mapSecondFloorIndex.GetLength(1) ==> 列数
                    {
                        mapFirstFloor[i, j] = MapManager.Instance.CreateMapElement(mapFirstFloorIndex[i, j]);
                    }
                }
            break;

            case 2:
                mapSecondFloor = new MapElement[41, 41];
                for (int i = 0; i < mapSecondFloor.GetLength(0); i++)        //mapSecondFloorIndex.GetLength(0) ==> 行数
                {
                    for (int j = 0; j < mapSecondFloor.GetLength(1); j++)     //mapSecondFloorIndex.GetLength(1) ==> 列数
                    {
                        mapSecondFloor[i, j] = MapManager.Instance.CreateMapElement(mapSecondFloorIndex[i, j]);
                    }
                }
            break;

            case 3:
                mapThirdFloor = new MapElement[43, 55];
                for (int i = 0; i < mapThirdFloor.GetLength(0); i++)        //mapSecondFloorIndex.GetLength(0) ==> 行数
                {
                    for (int j = 0; j < mapThirdFloor.GetLength(1); j++)     //mapSecondFloorIndex.GetLength(1) ==> 列数
                    {
                        mapThirdFloor[i, j] = MapManager.Instance.CreateMapElement(mapThirdFloorIndex[i, j]);
                    }
                }
            break;

        }

    }

    private void LoadEvents()           //在关卡初始化时调用(根据传入的库Id来加载对应库中的文本)
    {
        startEvents = new Dictionary<int, Event>();
        optionEvents = new Dictionary<int, Event>();
        EventManager.Instance.weights = new Dictionary<int, float>();
        //加载已有事件数据(CSV格式)到events字典中，使用Assets(Application.dataPath)下的相对路径
        string path = Path.Combine(Application.dataPath, "Resources/EventData/EventCSV/Event.csv");
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
                        // Debug.LogWarning($"Start event added! id is{eventData.eventId}");

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

        string path = Path.Combine(Application.dataPath, "Resources/EventData/EventResult/EventResult.csv");
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
                    // @eventResult.spId = int.Parse(values[1]);

                    @eventResult.change_HP = float.Parse(values[1]);
                    @eventResult.change_HP_rate = float.Parse(values[2]);

                    @eventResult.change_STR = float.Parse(values[3]);
                    @eventResult.change_STR_rate = float.Parse(values[4]);

                    @eventResult.change_DEF = float.Parse(values[5]);
                    @eventResult.change_DEF_rate = float.Parse(values[6]);

                    @eventResult.change_LVL = float.Parse(values[7]);
                    @eventResult.change_LVL_rate = float.Parse(values[8]);

                    @eventResult.change_SAN = float.Parse(values[9]);
                    @eventResult.change_SAN_rate = float.Parse(values[10]);

                    @eventResult.change_SPD = float.Parse(values[11]);
                    @eventResult.change_SPD_rate = float.Parse(values[12]);

                    @eventResult.change_CRIT_Rate = float.Parse(values[13]);
                    @eventResult.change_CRIT_Rate_rate = float.Parse(values[14]);

                    @eventResult.change_CRIT_DMG = float.Parse(values[15]);
                    @eventResult.change_CRIT_DMG_rate = float.Parse(values[16]);

                    @eventResult.change_HIT = float.Parse(values[17]);
                    @eventResult.change_HIT_rate = float.Parse(values[18]);

                    @eventResult.change_AVO = float.Parse(values[19]);
                    @eventResult.change_AVO_rate = float.Parse(values[20]);

                    @eventResult.enemyId = int.Parse(values[21]);
                    @eventResult.itemId = int.Parse(values[22]);
                    @eventResult.itemCount = int.Parse(values[23]);


                    @eventResult.equipmentId = int.Parse(values[24]);

                    eventResults.Add(@eventResult.resultId, @eventResult);
                }

            }

        }

    }
    private void LoadKaidanTexts(int eventIndex, Event @event)     //用在Event加载的时候
    {
        @event.textLib = new Dictionary<int, KaidanText>();
        string path = Path.Combine(Application.dataPath, "Resources/DialogueData/EventDialogue.csv");

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
                    // kaidanText.bgId = int.Parse(values[6]);

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

    private void LoadAVGDialogues(int avgId)
    {
        //1107特殊处理：不存在该avg文件：
        if(avgId == 1107)
            return;

        // Debug.Log($"当前加载的avg id：{avgId}");

        string path = Path.Combine(Application.dataPath, $"Resources/DialogueData/AVG/{avgId}.csv");
        int showIndex = avgId;
        DialogueOrderBlock tempBlock = new DialogueOrderBlock();
        tempBlock.rootId = showIndex;

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


                    dialogue.showUpNPCName = values[3];
                    // if (values[3] == "奈亚拉")                               //D列
                    //     dialogue.showUpNPCName = E_NPCName.奈亚拉;
                    // else if (values[3] == "优格")
                    //     dialogue.showUpNPCName = E_NPCName.优格;
                    // else if (values[3] == "纱布")
                    //     dialogue.showUpNPCName = E_NPCName.纱布;
                    // else if(values[3] == "妇人")
                    //     dialogue.showUpNPCName = E_NPCName.妇人; 
                    // else if(values[3] == "施耐德太太")
                    //     dialogue.showUpNPCName = E_NPCName.施耐德太太; 
                    // else
                    //     dialogue.showUpNPCName = E_NPCName.None; 
                    


                    dialogue.positionId = int.Parse(values[4]);             //E列


                    dialogue.disappearNPCName = values[5];
                    // if (values[5] == "奈亚拉")                               //F列
                    //     dialogue.disappearNPCName = E_NPCName.奈亚拉;
                    // else if (values[5] == "优格")
                    //     dialogue.disappearNPCName = E_NPCName.优格;
                    // else if (values[5] == "纱布")
                    //     dialogue.disappearNPCName = E_NPCName.纱布;
                    // else if(values[5] == "妇人")
                    //     dialogue.disappearNPCName = E_NPCName.妇人; 
                    // else if(values[5] == "施耐德太太")
                    //     dialogue.disappearNPCName = E_NPCName.施耐德太太; 
                    // else
                    //     dialogue.disappearNPCName = E_NPCName.None; 
                        

                    dialogue.effectId = int.Parse(values[6]);               //G列


                    dialogue.diffNPCName = values[7]; 
                    // if (values[7] == "奈亚拉")                               //H列    
                    //     dialogue.diffNPCName = E_NPCName.奈亚拉;
                    // else if (values[7] == "优格")
                    //     dialogue.diffNPCName = E_NPCName.优格;
                    // else if (values[7] == "纱布")
                    //     dialogue.diffNPCName = E_NPCName.纱布;
                    // else
                    //     dialogue.diffNPCName = E_NPCName.None;                          


                     dialogue.conversationNPCName = values[8]; 
                    // if (values[8] == "奈亚拉")                               
                    //     dialogue.conversationNPCName = E_NPCName.奈亚拉;
                    // else if (values[8] == "优格")
                    //     dialogue.conversationNPCName = E_NPCName.优格;
                    // else if (values[8] == "纱布")
                    //     dialogue.conversationNPCName = E_NPCName.纱布;
                    // else if (values[8] == "妇人")
                    //     dialogue.conversationNPCName = E_NPCName.妇人;
                    // else if (values[8] == "施耐德太太")
                    //     dialogue.conversationNPCName = E_NPCName.施耐德太太;
                    // else
                    //     dialogue.conversationNPCName = E_NPCName.None;

                    dialogue.orderText = values[9];                         //I列

                    // Debug.LogWarning(values[10]);
                    dialogue.nextOrderId = int.Parse(values[10]);            //J列
                    

                    dialogue.audioClipStartName = values[11];               //K列
                    dialogue.audioClipEndName = values[12];                 //L列

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
                        orderBlockDic.Add(showIndex, tempBlock);
                        tempBlock = new DialogueOrderBlock();
                        showIndex++;
                    }
                }

                if(i + 1 == lines.Length)
                {
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
