using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SaveGameData()
    {
        // 以角色信息为例
        Player player = PlayerManager.Instance.player;
        string json1 = JsonUtility.ToJson(player);
        string path1 = Application.dataPath + "Resources/GameDatas/playerInfo.json";
        File.WriteAllText(path1, json1);
        Debug.Log("playerInfo saved to: " + path1);

        // 处理装备保存逻辑
        List<Equipment> equipmentList = EquipmentManager.Instance.equipmentList;
        Dictionary<Equipment, int> equipmentDurationDic = EquipmentManager.Instance.equipmentDurationDic;
        string json2 = JsonUtility.ToJson(equipmentList);
        string json3 = JsonUtility.ToJson(equipmentDurationDic);
        string path2 = Application.dataPath + "Resources/GameDatas/EquipmentListInfo.json";
        string path3 = Application.dataPath + "Resources/GameDatas/EquipmentDicInfo.json";
        File.WriteAllText(path2, json2);
        File.WriteAllText(path3, json3);
        Debug.Log("EquipmentListInfo saved to: " + path2);
        Debug.Log("EquipmentDicInfo saved to: " + path2);

        // 处理道具保存逻辑
        List<int> itemList = ItemManager.Instance.itemList;
        Dictionary<int, int> itemCountDic = ItemManager.Instance.itemCountDic;
        string json4 = JsonUtility.ToJson(itemList);
        string json5 = JsonUtility.ToJson(itemCountDic);
        string path4 = Application.dataPath + "Resources/GameDatas/ItemListInfo.json";
        string path5 = Application.dataPath + "Resources/GameDatas/ItemDicInfo.json";
        File.WriteAllText(path4, json4);
        File.WriteAllText(path5, json5);
        Debug.Log("ItemListInfo saved to: " + path4);
        Debug.Log("ItemDicInfo saved to: " + path5);
    }

    public void LoadGameData()
    {
        string path1 = Application.persistentDataPath + "/playerInfo.json";
        if (File.Exists(path1))
        {
            string json1 = File.ReadAllText(path1);
            // 这里我选择直接将JSON信息传入PlayerManager.Instance.player，外部调用此方法即可直接加载
            PlayerManager.Instance.player = JsonUtility.FromJson<Player>(json1);
            Debug.Log("playerInfo loaded from: " + path1);
        }
        else
        {
            Debug.LogWarning("playerInfo file not found at: " + path1);
            // 若没有找到玩家的JSON存储信息，则创建一个新的角色
            PlayerManager.Instance.player = new Player();
        }

        // 这里继续处理装备等加载逻辑
        string path2 = Application.persistentDataPath + "/EquipmentListInfo.json";
        if (File.Exists(path2))
        {
            string json2 = File.ReadAllText(path2);
            EquipmentManager.Instance.equipmentList = JsonUtility.FromJson<List<Equipment>>(json2);
            Debug.Log("EquipmentListInfo loaded from: " + path2);
        }
        else
        {
            Debug.LogWarning("EquipmentListInfo file not found at: " + path2);
            EquipmentManager.Instance.equipmentList = new List<Equipment>();
        }
        string path3 = Application.persistentDataPath + "/EquipmentDicInfo.json";
        if (File.Exists(path3))
        {
            string json3 = File.ReadAllText(path3);
            EquipmentManager.Instance.equipmentDurationDic = JsonUtility.FromJson<Dictionary<Equipment, int>>(json3);
            Debug.Log("EquipmentDicInfo loaded from: " + path3);
        }
        else
        {
            Debug.LogWarning("EquipmentDicInfo file not found at: " + path3);
            EquipmentManager.Instance.equipmentDurationDic = new Dictionary<Equipment, int>();
        }

        // 处理道具保存逻辑
        string path4 = Application.persistentDataPath + "/ItemListInfo.json";
        if (File.Exists(path4))
        {
            string json4 = File.ReadAllText(path4);
            ItemManager.Instance.itemList = JsonUtility.FromJson<List<int>>(json4);
            Debug.Log("ItemListInfo loaded from: " + path4);
        }
        else
        {
            Debug.LogWarning("ItemListInfo file not found at: " + path4);
            ItemManager.Instance.itemList = new List<int>();
        }
        string path5 = Application.persistentDataPath + "/ItemDicInfo.json";
        if (File.Exists(path5))
        {
            string json5 = File.ReadAllText(path5);
            ItemManager.Instance.itemCountDic = JsonUtility.FromJson<Dictionary<int, int>>(json5);
            Debug.Log("ItemDicInfo loaded from: " + path5);
        }
        else
        {
            Debug.LogWarning("ItemDicInfo file not found at: " + path5);
            ItemManager.Instance.itemCountDic = new Dictionary<int, int>();
        }

        return;
    }

}
