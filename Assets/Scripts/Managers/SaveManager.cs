using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

public class SaveManager : SingletonBaseManager<SaveManager>
{
    private SaveManager(){}
    private static string savePath => Application.persistentDataPath + "/player_save.json";
    private static string gameLevelSavePath => Application.persistentDataPath + "/gamelevel_save.json";
    private static string equipmentSavePath => Application.persistentDataPath + "/equipment_save.json";
    private static string itemSavePath => Application.persistentDataPath + "/item_save.json";

    //存档接口：
    public void SaveGame()
    {
        SavePlayerAttribute(); 
        SaveGameLevel();
        SaveEquipment();
        SaveItem();
    }

    //读档接口：
    public void LoadGame()
    {
        LoadPlayerAttribute();
        LoadGameLevel();
        LoadEquipment();
        LoadItem();
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

        Debug.Log("所有存档已清空，玩家进度已重置！");
    }

//---------------------------玩家属性存档--------------------------------------------------
    // 存储玩家属性
    private void SavePlayerAttribute()
    {
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

        // 恢复属性
        ApplyAttribute(player.HP, data.HP);
        ApplyAttribute(player.STR, data.STR);
        ApplyAttribute(player.DEF, data.DEF);
        ApplyAttribute(player.LVL, data.LVL);
        ApplyAttribute(player.SAN, data.SAN);
        ApplyAttribute(player.SPD, data.SPD);
        ApplyAttribute(player.CRIT_Rate, data.CRIT_Rate);
        ApplyAttribute(player.CRIT_DMG, data.CRIT_DMG);
        ApplyAttribute(player.HIT, data.HIT);
        ApplyAttribute(player.AVO, data.AVO);

        Debug.Log("玩家属性已从存档恢复！");
    }

    // 辅助方法：将存档属性赋值回Player.PlayerAttribute
    private void ApplyAttribute(Player.PlayerAttribute attr, PlayerAttributeSaveData data)
    {
        if (data == null) return;
        attr.value = data.value;
        attr.value_limit = data.value_limit;
        attr.type = data.type;
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

//---------------------------存档可序列化数据容器--------------------------------------------
}
