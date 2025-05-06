using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    // 可序列化的字典包装类
    [System.Serializable]
    public class SerializableEquipmentDict
    {
        public List<Equipment> keys = new List<Equipment>();
        public List<int> values = new List<int>();

        public void FromDictionary(Dictionary<Equipment, int> dict)
        {
            keys.Clear();
            values.Clear();
            foreach (var kvp in dict)
            {
                keys.Add(kvp.Key);
                values.Add(kvp.Value);
            }
        }

        public Dictionary<Equipment, int> ToDictionary()
        {
            var dict = new Dictionary<Equipment, int>();
            for (int i = 0; i < Mathf.Min(keys.Count, values.Count); i++)
            {
                dict[keys[i]] = values[i];
            }
            return dict;
        }
    }

    [System.Serializable]
    public class SerializableItemDict
    {
        public List<int> keys = new List<int>();
        public List<int> values = new List<int>();

        public void FromDictionary(Dictionary<int, int> dict)
        {
            keys.Clear();
            values.Clear();
            foreach (var kvp in dict)
            {
                keys.Add(kvp.Key);
                values.Add(kvp.Value);
            }
        }

        public Dictionary<int, int> ToDictionary()
        {
            var dict = new Dictionary<int, int>();
            for (int i = 0; i < Mathf.Min(keys.Count, values.Count); i++)
            {
                dict[keys[i]] = values[i];
            }
            return dict;
        }
    }

    [System.Serializable]
    public class GameSaveData
    {
        public Player player;
        public List<Equipment> equipmentList;
        public SerializableEquipmentDict equipmentDurationDict;
        public List<int> itemList;
        public SerializableItemDict itemCountDict;
    }

    public void SaveGameData()
    {
        GameSaveData saveData = new GameSaveData();

        // 玩家数据
        saveData.player = PlayerManager.Instance.player;

        // 装备数据
        saveData.equipmentList = EquipmentManager.Instance.equipmentList;
        saveData.equipmentDurationDict = new SerializableEquipmentDict();
        saveData.equipmentDurationDict.FromDictionary(EquipmentManager.Instance.equipmentDurationDic);

        // 道具数据
        saveData.itemList = ItemManager.Instance.itemList;
        saveData.itemCountDict = new SerializableItemDict();
        saveData.itemCountDict.FromDictionary(ItemManager.Instance.itemCountDic);

        string json = JsonUtility.ToJson(saveData, true);
        string path = Path.Combine(Application.dataPath, "Resources/SaveData/save.json");

        try
        {
            // 确保目录存在
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllText(path, json);
            Debug.Log("Game saved to: " + path);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Save failed: " + e.Message);
        }
    }

    public void LoadGameData()
    {
        string path = Path.Combine(Application.dataPath, "Resources/SaveData/save.json");

        if (!File.Exists(path))
        {
            Debug.LogWarning("No save file found at: " + path);
            CreateNewGame();
            return;
        }

        try
        {
            string json = File.ReadAllText(path);
            GameSaveData saveData = JsonUtility.FromJson<GameSaveData>(json);

            // 加载玩家数据
            PlayerManager.Instance.player = saveData.player ?? new Player();

            // 加载装备数据
            EquipmentManager.Instance.equipmentList = saveData.equipmentList ?? new List<Equipment>();
            EquipmentManager.Instance.equipmentDurationDic = saveData.equipmentDurationDict?.ToDictionary() ?? new Dictionary<Equipment, int>();

            // 加载道具数据
            ItemManager.Instance.itemList = saveData.itemList ?? new List<int>();
            ItemManager.Instance.itemCountDic = saveData.itemCountDict?.ToDictionary() ?? new Dictionary<int, int>();

            Debug.Log("Game loaded from: " + path);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Load failed: " + e.Message);
            CreateNewGame();
        }
    }

    private void CreateNewGame()
    {
        PlayerManager.Instance.player = new Player();
        EquipmentManager.Instance.equipmentList = new List<Equipment>();
        EquipmentManager.Instance.equipmentDurationDic = new Dictionary<Equipment, int>();
        ItemManager.Instance.itemList = new List<int>();
        ItemManager.Instance.itemCountDic = new Dictionary<int, int>();
    }
}