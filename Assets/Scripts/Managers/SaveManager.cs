using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;

// 玩家位置数据类，用于JSON序列化
[System.Serializable]
public class PlayerPositionData
{
    public float x;
    public float y;
    public float z;
    public string sceneName;
    
    public PlayerPositionData(float x, float y, float z, string sceneName)
    {
        this.x = x;
        this.y = y;
        this.z = z;
        this.sceneName = sceneName;
    }
}

public class SaveManager : SingletonBaseManager<SaveManager>
{
    private SaveManager(){}
    private static string savePath => Application.persistentDataPath + "/player_save.json";
    private static string playerPositionSavePath => Application.persistentDataPath + "/player_position_save.json";
    private static string gameLevelSavePath => Application.persistentDataPath + "/gamelevel_save.json";
    private static string equipmentSavePath => Application.persistentDataPath + "/equipment_save.json";
    private static string itemSavePath => Application.persistentDataPath + "/item_save.json";
    private static string avgDistributeSavePath => Application.persistentDataPath + "/avgdistribute_save.json";
    //存档接口：
    public void SaveGame()
    {

        SavePlayerAttribute();
        SaveGameLevel();
        SaveEquipment();
        SaveItem();
        SaveAVGDistribute();
        ManageRestLightData(true);
        SavePlayerPosition();
    }

    //读档接口：
    public void LoadGame()
    {
        
        
        LoadGameLevel();
        LoadPlayerPosition();
        LoadPlayerAttribute();
        LoadEquipment();
        LoadItem();
        LoadAVGDistribute();
        ManageRestLightData(false);
    }

    //清空当前游戏进度的方法；
    //在重新开始游戏的时候调用；
    public void ClearGame()
    {
        // 删除存档文件
        if (File.Exists(savePath)) File.Delete(savePath);
        if (File.Exists(gameLevelSavePath)) File.Delete(gameLevelSavePath);
        if (File.Exists(equipmentSavePath)) File.Delete(equipmentSavePath);
        if (File.Exists(itemSavePath)) File.Delete(itemSavePath);

        // 清空内存中的数据
        // 玩家属性重置
        PlayerManager.Instance.InitPlayer();
        
        // 关卡进度重置
        GameLevelManager.Instance.ResetAllProgress();

        // 装备重置
        var em = EquipmentManager.Instance;
        em.equipmentList.Clear();
        em.equipmentDurationDic.Clear();

        // 道具重置
        var im = ItemManager.Instance;
        im.itemList.Clear();
        im.itemCountDic.Clear();

        //avg贡献重置：
        AVGDistributeManager.Instance.ClearAllDistributionRecord();

        Debug.Log("所有存档已清空，玩家进度已重置！");
    }

//---------------------------玩家属性存档--------------------------------------------------
    // 存储玩家属性
    private void SavePlayerAttribute()
    {
        
        
        Scene currentScene = SceneManager.GetActiveScene();
        string sceneName = currentScene.name;
        int sceneIndex = currentScene.buildIndex;
        Debug.Log("当前场景名: " + sceneName);
        Debug.Log("当前场景索引: " + sceneIndex);
        Player player = PlayerManager.Instance.player;
        PlayerSaveData data = new PlayerSaveData
        {
            HP = new PlayerAttributeSaveData(player.HP),
            STR = new PlayerAttributeSaveData(player.STR),
            DEF = new PlayerAttributeSaveData(player.DEF),
            LVL = new PlayerAttributeSaveData(player.LVL),
            SAN = new PlayerAttributeSaveData(player.SAN),
            SPD = new PlayerAttributeSaveData(player.SPD),
            CRIT_Rate = new PlayerAttributeSaveData(player.CRIT_Rate),
            CRIT_DMG = new PlayerAttributeSaveData(player.CRIT_DMG),
            HIT = new PlayerAttributeSaveData(player.HIT),
            AVO = new PlayerAttributeSaveData(player.AVO)
            
        };
       

        string json = JsonConvert.SerializeObject(data, Formatting.Indented);
        File.WriteAllText(savePath, json);
        Debug.Log("玩家属性已保存到: " + savePath);
    }
    // 存储玩家位置
    public void SavePlayerPosition()
    {
        Vector3 position = PlayerManager.Instance.playerPosition;
        Scene currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        string sceneName = currentScene.name;
        
        // 使用PlayerPositionData类
        PlayerPositionData data = new PlayerPositionData(position.x, position.y, position.z, sceneName);
        
        string json = JsonConvert.SerializeObject(data, Formatting.Indented);
        File.WriteAllText(playerPositionSavePath, json);
        Debug.Log($"玩家位置已保存到: {playerPositionSavePath}, 坐标: {position}, 场景: {sceneName}");
    }
    // 读取玩家属性
    private void LoadPlayerAttribute()
    {
        Player player = PlayerManager.Instance.player;
        if (!File.Exists(savePath))
        {
            Debug.LogWarning("未找到存档文件，无法读档！");
            return;
        }

        string json = File.ReadAllText(savePath);
        PlayerSaveData data = JsonConvert.DeserializeObject<PlayerSaveData>(json);

        // 恢复属性（修正struct赋值问题，确保写回Player实例）
        ApplyAttribute(player, AttributeType.HP, data.HP);
        ApplyAttribute(player, AttributeType.STR, data.STR);
        ApplyAttribute(player, AttributeType.DEF, data.DEF);
        ApplyAttribute(player, AttributeType.LVL, data.LVL);
        ApplyAttribute(player, AttributeType.SAN, data.SAN);
        ApplyAttribute(player, AttributeType.SPD, data.SPD);
        ApplyAttribute(player, AttributeType.CRIT_Rate, data.CRIT_Rate);
        ApplyAttribute(player, AttributeType.CRIT_DMG, data.CRIT_DMG);
        ApplyAttribute(player, AttributeType.HIT, data.HIT);
        ApplyAttribute(player, AttributeType.AVO, data.AVO);
        

        Debug.Log("玩家属性已从存档恢复！");
    }
    // 读取玩家位置
    public void LoadPlayerPosition()
    {
        if (!File.Exists(playerPositionSavePath))
        {
            Debug.LogWarning("未找到玩家位置存档文件，无法读档！");
            return;
        }

        try
        {
            string json = File.ReadAllText(playerPositionSavePath);
            PlayerPositionData data = JsonConvert.DeserializeObject<PlayerPositionData>(json);
            
            if (data == null)
            {
                Debug.LogError("玩家位置存档数据反序列化失败！");
                return;
            }

            Vector3 position = new Vector3(data.x, data.y, data.z);

            // 设置玩家位置到PlayerManager
            PlayerManager.Instance.playerPosition = position;

            // 如果场景不同，先切换场景
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != data.sceneName)
            {
                // 注册场景加载完成事件，在场景加载后设置玩家位置
                UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoadedSetPosition;
                UnityEngine.SceneManagement.SceneManager.LoadScene(data.sceneName);
                Debug.Log($"开始切换到场景: {data.sceneName}，位置将在场景加载完成后设置");
            }
            else
            {
                // 同一场景直接设置位置
                PlayerController.SetPlayerPosition(position);
                Debug.Log($"玩家位置已设置为: {position} (同一场景)");
            }
            
            Debug.Log($"玩家位置已从存档恢复！坐标: {position}, 场景: {data.sceneName}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"读取玩家位置存档时发生错误: {e.Message}");
        }
    }
    
    // 场景加载完成后设置玩家位置的回调
    private void OnSceneLoadedSetPosition(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        // 取消注册事件
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoadedSetPosition;
        
        Debug.Log($"场景 '{scene.name}' 加载完成，准备设置玩家位置...");
        
        // 使用延迟调用设置位置，确保场景完全准备就绪
        DelayedSetPlayerPosition();
    }
    
    // 延迟设置玩家位置的方法
    private async void DelayedSetPlayerPosition()
    {
        try
        {
            // 等待一小段时间，确保场景完全加载和初始化
            await System.Threading.Tasks.Task.Delay(200);
            
            // 检查PlayerManager和PlayerTransform是否准备就绪
            int maxRetries = 25; // 最多重试25次（5秒）
            int retryCount = 0;
            
            while (PlayerManager.Instance?.PlayerTransform == null && retryCount < maxRetries)
            {
                await System.Threading.Tasks.Task.Delay(200);
                retryCount++;
                Debug.Log($"等待PlayerTransform准备就绪... ({retryCount}/{maxRetries})");
            }
            
            if (PlayerManager.Instance?.PlayerTransform == null)
            {
                Debug.LogError("场景加载完成后未能找到PlayerTransform，无法设置玩家位置！请检查场景中是否有Player对象。");
                return;
            }
            
            // 设置玩家位置
            Vector3 targetPosition = PlayerManager.Instance.playerPosition;
            PlayerController.SetPlayerPosition(targetPosition);
            
            Debug.Log($"✅ 场景加载完成！玩家位置已成功设置为: {targetPosition}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"延迟设置玩家位置时发生错误: {e.Message}");
        }
    }

    // 辅助方法：将存档属性赋值回Player.PlayerAttribute
    // 辅助方法：将存档属性赋值回Player.PlayerAttribute，确保写回Player实例
    private void ApplyAttribute(Player player, AttributeType type, PlayerAttributeSaveData data)
    {
        if (data == null) return;
        var attr = player.GetAttr(type);
        attr.value = data.value;
        attr.value_limit = data.value_limit;
        attr.type = data.type;
        player.SetAttr(type, attr);
        EventHub.Instance.EventTrigger("UpdateAllUIElements");
        Debug.Log($"属性 {type} 已恢复：值 = {attr.value}, 上限 = {attr.value_limit}");
    }

//---------------------------玩家属性存档--------------------------------------------------


//---------------------------关卡进度存档--------------------------------------------------
    // 存储GameLevelManager相关内容
    private void SaveGameLevel()
    {
        var glm = GameLevelManager.Instance;

        //序列化类容器
        GameLevelSaveData data = new GameLevelSaveData();

        //存储当前的关卡进度：
        data.gameLevelType = (int)glm.gameLevelType;

        //存储当前的AVG触发进度：
        data.avgIndexIsTriggeredList = new List<KeyValuePair<int, bool>>();
        foreach (var kv in glm.avgIndexIsTriggeredDic)
        {
            data.avgIndexIsTriggeredList.Add(new KeyValuePair<int, bool>(kv.Key, kv.Value));
        }

        //存储当前的安全屋强制剧情触发进度：
        data.avgShelterIsTriggeredList = new List<KeyValuePair<int, bool>>();
        foreach (var kv in glm.avgShelterIsTriggered)
        {
            data.avgShelterIsTriggeredList.Add(new KeyValuePair<int, bool>((int)kv.Key, kv.Value));
        }

        //序列化：
        string json = JsonConvert.SerializeObject(data, Formatting.Indented);
        File.WriteAllText(gameLevelSavePath, json);
        Debug.Log("关卡信息已保存到: " + gameLevelSavePath);
    }

    // 读取GameLevelManager相关内容
    private void LoadGameLevel()
    {
        if (!File.Exists(gameLevelSavePath))
        {
            Debug.LogWarning("未找到关卡存档文件，无法读档！");
            return;
        }
        var glm = GameLevelManager.Instance;
        string json = File.ReadAllText(gameLevelSavePath);
        GameLevelSaveData data = JsonConvert.DeserializeObject<GameLevelSaveData>(json);
        glm.gameLevelType = (E_GameLevelType)data.gameLevelType;
        glm.avgIndexIsTriggeredDic.Clear();
        foreach (var kv in data.avgIndexIsTriggeredList)
        {
            glm.avgIndexIsTriggeredDic[kv.Key] = kv.Value;
        }
        glm.avgShelterIsTriggered.Clear();
        foreach (var kv in data.avgShelterIsTriggeredList)
        {
            glm.avgShelterIsTriggered[(E_GameLevelType)kv.Key] = kv.Value;
        }
        Debug.Log("关卡信息已从存档恢复！");
    }

//---------------------------关卡进度存档--------------------------------------------------


//---------------------------道具装备存档--------------------------------------------------
// 存储EquipmentManager相关内容
    private void SaveEquipment()
    {
        var em = EquipmentManager.Instance;
        EquipmentSaveData data = new EquipmentSaveData();

        //存储当前的装备：注意：只存储持有的装备的id;
        data.equipmentList = new List<int>();
        foreach (var eq in em.equipmentList)
        {
            data.equipmentList.Add(eq.id); // 假设Equipment有id字段
        }

        //存储装备的耐久；
        data.equipmentDurationList = new List<KeyValuePair<int, int>>();
        foreach (var kv in em.equipmentDurationDic)
        {
            //为了避免同一类的装备在字典中互相冲突（相同id），使用一个算法将这些装备区分开来：
            //如：所有的1001取最后两位01，计算得出01 * 1000 + uniqueId；
            //最终通过整除1000 + 1000的方式恢复成1001；
            //对于1011，就是11 * 1000 + uniqueId；
            //最终通过整除1000 + 1000的方式恢复成1011；
            int uniqueId = (kv.Key.id % 100) * 1000 + EquipmentManager.Instance.uniqueId++;
            Debug.Log($"current unique id is{uniqueId}");
            data.equipmentDurationList.Add(new KeyValuePair<int, int>(uniqueId, kv.Value));
        }
        string json = JsonConvert.SerializeObject(data, Formatting.Indented);
        File.WriteAllText(equipmentSavePath, json);
        Debug.Log("装备信息已保存到: " + equipmentSavePath);
    }

    // 读取EquipmentManager相关内容
    private void LoadEquipment()
    {
        if (!File.Exists(equipmentSavePath))
        {
            Debug.LogWarning("未找到装备存档文件，无法读档！");
            return;
        }
        var em = EquipmentManager.Instance;
        string json = File.ReadAllText(equipmentSavePath);
        EquipmentSaveData data = JsonConvert.DeserializeObject<EquipmentSaveData>(json);
        em.equipmentList.Clear();

        //将所有的装备复原：
        em.equipmentDurationDic.Clear();

        foreach(var pair in data.equipmentDurationList)
        {
            int uniqueId = pair.Key;
            int rawId = (uniqueId / 1000) + 1000;
            Debug.Log($"current raw id is{rawId}");
            EquipmentManager.Instance.AddEquipment(rawId, pair.Value, true);
        }

        Debug.Log("装备信息已从存档恢复！");
    }

    // 存储ItemManager相关内容
    private void SaveItem()
    {
        var im = ItemManager.Instance;
        ItemSaveData data = new ItemSaveData();
        data.itemList = new List<int>(im.itemList);
        data.itemCountList = new List<KeyValuePair<int, int>>();
        foreach (var kv in im.itemCountDic)
        {
            data.itemCountList.Add(new KeyValuePair<int, int>(kv.Key, kv.Value));
        }
        string json = JsonConvert.SerializeObject(data, Formatting.Indented);
        File.WriteAllText(itemSavePath, json);
        Debug.Log("道具信息已保存到: " + itemSavePath);
    }

    // 读取ItemManager相关内容
    private void LoadItem()
    {
        if (!File.Exists(itemSavePath))
        {
            Debug.LogWarning("未找到道具存档文件，无法读档！");
            return;
        }
        var im = ItemManager.Instance;
        string json = File.ReadAllText(itemSavePath);
        ItemSaveData data = JsonConvert.DeserializeObject<ItemSaveData>(json);
        im.itemList.Clear();
        im.itemList.AddRange(data.itemList);
        im.itemCountDic.Clear();
        foreach (var kv in data.itemCountList)
        {
            im.itemCountDic[kv.Key] = kv.Value;
        }
        Debug.Log("道具信息已从存档恢复！");
    }

    //---------------------------道具装备存档--------------------------------------------------


    //---------------------------存档可序列化数据容器--------------------------------------------
    [System.Serializable]
    public class RestLightSaveData
    {
        public List<KeyValuePair<string, bool>> restPointList = new List<KeyValuePair<string, bool>>();
        public List<KeyValuePair<string, bool>> lightHouseList = new List<KeyValuePair<string, bool>>();
    }

    private static string restLightSavePath => Application.persistentDataPath + "/restlight_save.json";

    // 存取rest point与lighthouse的方法，isSave为true时保存，为false时读取
    public void ManageRestLightData(bool isSave)
    {
        if (isSave)
        {
            var data = new RestLightSaveData();
            // 序列化restPointDic
            foreach (var kv in GameLevelManager.Instance.restPointDic)
            {
                // key序列化为字符串，格式：关卡类型|x|y|z
                string keyStr = $"{kv.Key.Item1}|{kv.Key.Item2.x}|{kv.Key.Item2.y}|{kv.Key.Item2.z}";
                data.restPointList.Add(new KeyValuePair<string, bool>(keyStr, kv.Value));
            }
            // 序列化lightHouseIsDic
            foreach (var kv in GameLevelManager.Instance.lightHouseIsDic)
            {
                string keyStr = $"{kv.Key.Item1}|{kv.Key.Item2.x}|{kv.Key.Item2.y}|{kv.Key.Item2.z}";
                data.lightHouseList.Add(new KeyValuePair<string, bool>(keyStr, kv.Value));
            }
            string json = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(restLightSavePath, json);
            Debug.Log("RestPoint与LightHouse数据已保存到: " + restLightSavePath);
        }
        else
        {
            if (!File.Exists(restLightSavePath))
            {
                Debug.LogWarning("未找到RestLight存档文件，无法读档！");
                return;
            }
            string json = File.ReadAllText(restLightSavePath);
            RestLightSaveData data = JsonConvert.DeserializeObject<RestLightSaveData>(json);
            GameLevelManager.Instance.restPointDic.Clear();
            foreach (var kv in data.restPointList)
            {
                var arr = kv.Key.Split('|');
                var level = (E_GameLevelType)System.Enum.Parse(typeof(E_GameLevelType), arr[0]);
                var pos = new Vector3(float.Parse(arr[1]), float.Parse(arr[2]), float.Parse(arr[3]));
                GameLevelManager.Instance.restPointDic[(level, pos)] = kv.Value;
            }
            GameLevelManager.Instance.lightHouseIsDic.Clear();
            foreach (var kv in data.lightHouseList)
            {
                var arr = kv.Key.Split('|');
                var level = (E_GameLevelType)System.Enum.Parse(typeof(E_GameLevelType), arr[0]);
                var pos = new Vector3(float.Parse(arr[1]), float.Parse(arr[2]), float.Parse(arr[3]));
                GameLevelManager.Instance.lightHouseIsDic[(level, pos)] = kv.Value;
            }
            Debug.Log("RestPoint与LightHouse数据已从存档恢复！");
        }
    }
    // 存档数据结构
    [System.Serializable]
    public class PlayerAttributeSaveData
    {
        public float value;
        public float value_limit;
        public int type;

        public PlayerAttributeSaveData() { }
        public PlayerAttributeSaveData(Player.PlayerAttribute attr)
        {
            value = attr.value;
            value_limit = attr.value_limit;
            type = attr.type;
        }
    }

    [System.Serializable]
    public class PlayerSaveData
    {
        public PlayerAttributeSaveData HP;
        public PlayerAttributeSaveData STR;
        public PlayerAttributeSaveData DEF;
        public PlayerAttributeSaveData LVL;
        public PlayerAttributeSaveData SAN;
        public PlayerAttributeSaveData SPD;
        public PlayerAttributeSaveData CRIT_Rate;
        public PlayerAttributeSaveData CRIT_DMG;
        public PlayerAttributeSaveData HIT;
        public PlayerAttributeSaveData AVO;
        public PlayerAttributeSaveData SCENE;
        public PlayerAttributeSaveData POS; // 玩家位置数据
    }

    // GameLevel存档数据结构
    [System.Serializable]

    public class GameLevelSaveData
    {
        public int gameLevelType;
        public List<KeyValuePair<int, bool>> avgIndexIsTriggeredList;
        public List<KeyValuePair<int, bool>> avgShelterIsTriggeredList;
    }
    
    [System.Serializable]
    public class EquipmentSaveData
    {
        public List<int> equipmentList;
        public List<KeyValuePair<int, int>> equipmentDurationList;
    }

    [System.Serializable]
    public class ItemSaveData
    {
        public List<int> itemList;
        public List<KeyValuePair<int, int>> itemCountList;
    }

    public void SaveAVGDistribute()
    {
        var avgMgr = AVGDistributeManager.Instance;
        AVGDistributeSaveData data = new AVGDistributeSaveData();
        data.contributedAvgIdList = new List<int>(avgMgr.contributedAvgIdList);
        data.npcAvgQueueDic = new Dictionary<int, List<int>>();
        foreach (var kv in avgMgr.dicAVGDistributor)
        {
            data.npcAvgQueueDic[(int)kv.Key] = new List<int>(kv.Value);
        }
        string json = JsonConvert.SerializeObject(data, Formatting.Indented);
        File.WriteAllText(avgDistributeSavePath, json);
        Debug.Log("AVGDistributeManager数据已保存到: " + avgDistributeSavePath);
    }

    public void LoadAVGDistribute()
    {
        if (!File.Exists(avgDistributeSavePath))
        {
            Debug.LogWarning("未找到AVGDistribute存档文件，无法读档！");
            return;
        }
        var avgMgr = AVGDistributeManager.Instance;
        string json = File.ReadAllText(avgDistributeSavePath);
        AVGDistributeSaveData data = JsonConvert.DeserializeObject<AVGDistributeSaveData>(json);
        avgMgr.contributedAvgIdList.Clear();
        avgMgr.contributedAvgIdList.AddRange(data.contributedAvgIdList);
        // 还原事件队列
        // 输出字典所有内容
        Debug.Log("------npcAvgQueueDic完整内容------");
        foreach (var kv in data.npcAvgQueueDic)
        {
            Debug.Log($"key: {kv.Key}, value: [{string.Join(",", kv.Value)}]");
        }
        Debug.Log("------------");
        foreach (var kv in data.npcAvgQueueDic)
        {
            E_NPCName npcName;
            if (int.TryParse(kv.Key.ToString(), out int npcInt))
            {
                npcName = (E_NPCName)npcInt;
            }
            else
            {
                npcName = (E_NPCName)System.Enum.Parse(typeof(E_NPCName), kv.Key.ToString());
            }
            if (!avgMgr.dicAVGDistributor.ContainsKey(npcName))
                avgMgr.dicAVGDistributor[npcName] = new LinkedList<int>();
            Debug.Log($"当前NPC名称: {npcName}");
            avgMgr.dicAVGDistributor[npcName].Clear();
            foreach (var id in kv.Value)
            {
                avgMgr.dicAVGDistributor[npcName].AddLast(id);
            }
        }
        Debug.Log("AVGDistributeManager数据已从存档恢复！");
    }

    [System.Serializable]
    public class AVGDistributeSaveData
    {
        public List<int> contributedAvgIdList;
        public Dictionary<int, List<int>> npcAvgQueueDic;
    }

//---------------------------存档可序列化数据容器--------------------------------------------
}
