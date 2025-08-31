using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

/// <summary>
/// 复活数据管理器 - 在复活时自动储存游戏并显示所有储存的事件与互动过的物体
/// 基于SaveManager架构，类似EventIterator的使用模式
/// </summary>
public class ReviveDataManager : Singleton<ReviveDataManager>
{
    [Header("调试设置")]
    [SerializeField] private bool enableDebugOutput = true;
    [SerializeField] private KeyCode testKey = KeyCode.R; // 按R键测试复活数据显示
    
    [Header("复活数据统计")]
    [SerializeField] private int totalReviveCount = 0;
    [SerializeField] private float lastReviveTime = 0f;

    /// <summary>
    /// 复活时的数据快照
    /// </summary>
    [System.Serializable]
    public class ReviveSnapshot
    {
        public string reviveTime;           // 复活时间
        public float sanValue;              // 复活时的SAN值
        public E_GameLevelType currentLevel; // 复活时的关卡
        public Vector3 playerPosition;      // 复活时的玩家位置
        public int completedEventsCount;    // 已完成事件数量
        public int totalInteractionsCount;  // 总交互物体数量
        public List<int> completedEventIds; // 已完成事件ID列表
        public ReviveDataSummary dataSummary; // 数据汇总
    }

    /// <summary>
    /// 复活数据汇总信息
    /// </summary>
    [System.Serializable]
    public class ReviveDataSummary
    {
        // 事件相关
        public int totalEvents;
        public int completedEvents;
        public int totalStartEvents;
        public int completedStartEvents;
        public int totalOptionEvents;
        public int completedOptionEvents;
        
        // 交互物体相关
        public int totalRestPoints;
        public int activatedRestPoints;
        public int totalLightHouses;
        public int activatedLightHouses;
        public int totalKeyPoints;
        public int activatedKeyPoints;
        public int totalItemPoints;
        public int activatedItemPoints;
        public int totalDoors;
        public int unlockedDoors;
        
        // 道具装备相关
        public int totalItems;
        public int totalEquipments;
        public int totalEquipmentDurability;
        
        // AVG相关
        public int totalAVGTriggered;
        public int shelterAVGTriggered;
        
        // 游戏进度相关
        public float completionPercentage;
        public float interactionPercentage;
    }

    private List<ReviveSnapshot> reviveHistory = new List<ReviveSnapshot>();
    private static string reviveDataSavePath => Application.persistentDataPath + "/revive_data_save.json";

    protected override void Awake()
    {
        base.Awake();
        LoadReviveHistory();
        
        if (enableDebugOutput)
        {
            Debug.Log("ReviveDataManager 已初始化 - 按 R 键可查看复活数据信息");
        }
    }

    private void Update()
    {
        // 按下测试键显示复活数据信息
        if (Input.GetKeyDown(testKey))
        {
            TestShowReviveData();
        }
    }

    /// <summary>
    /// 复活时调用的主方法 - 自动保存游戏并显示数据
    /// 应该在PlayerController的复活逻辑中调用此方法
    /// </summary>
    public void OnPlayerRevive()
    {
        Debug.Log("=== 玩家复活 - 开始数据处理 ===");
        
        // 1. 自动保存游戏
        AutoSaveGameOnRevive();
        
        // 2. 创建复活数据快照
        ReviveSnapshot snapshot = CreateReviveSnapshot();
        
        // 3. 保存复活记录
        SaveReviveSnapshot(snapshot);
        
        // 4. 显示复活数据信息
        DisplayReviveData(snapshot);
        
        // 5. 更新统计信息
        UpdateReviveStatistics();
        
        Debug.Log("=== 玩家复活 - 数据处理完成 ===");
    }

    /// <summary>
    /// 复活时自动保存游戏
    /// </summary>
    private void AutoSaveGameOnRevive()
    {
        if (SaveManager.Instance == null)
        {
            Debug.LogError("SaveManager.Instance 为空，无法自动保存游戏！");
            return;
        }

        try
        {
            Debug.Log("复活时自动保存游戏...");
            SaveManager.Instance.SaveGame();
            Debug.Log("复活时游戏保存完成！");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"复活时自动保存游戏失败: {e.Message}");
        }
    }

    /// <summary>
    /// 创建复活数据快照
    /// </summary>
    private ReviveSnapshot CreateReviveSnapshot()
    {
        ReviveSnapshot snapshot = new ReviveSnapshot();
        
        // 基础信息
        snapshot.reviveTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        snapshot.sanValue = PlayerManager.Instance?.player?.SAN != null ? PlayerManager.Instance.player.SAN.value : 0f;
        snapshot.currentLevel = GameLevelManager.Instance?.gameLevelType ?? E_GameLevelType.Tutorial;
        snapshot.playerPosition = PlayerManager.Instance?.playerPosition ?? Vector3.zero;
        
        // 事件信息
        if (EventIterator.Instance != null)
        {
            snapshot.completedEventIds = EventIterator.Instance.GetCompletedEventIds();
            snapshot.completedEventsCount = snapshot.completedEventIds.Count;
        }
        else
        {
            snapshot.completedEventIds = new List<int>();
            snapshot.completedEventsCount = 0;
        }
        
        // 创建数据汇总
        snapshot.dataSummary = CreateDataSummary();
        snapshot.totalInteractionsCount = CalculateTotalInteractions(snapshot.dataSummary);
        
        return snapshot;
    }

    /// <summary>
    /// 创建数据汇总信息
    /// </summary>
    private ReviveDataSummary CreateDataSummary()
    {
        ReviveDataSummary summary = new ReviveDataSummary();
        
        // 事件统计
        if (EventIterator.Instance != null)
        {
            var stats = EventIterator.Instance.GetEventCompletionStats();
            summary.totalEvents = stats.TotalEvents;
            summary.completedEvents = stats.CompletedEvents;
            summary.totalStartEvents = stats.TotalStartEvents;
            summary.completedStartEvents = stats.CompletedStartEvents;
            summary.totalOptionEvents = stats.TotalOptionEvents;
            summary.completedOptionEvents = stats.CompletedOptionEvents;
        }

        // 交互物体统计
        if (GameLevelManager.Instance != null)
        {
            var glm = GameLevelManager.Instance;
            
            // RestPoints统计
            summary.totalRestPoints = glm.restPointDic.Count;
            summary.activatedRestPoints = glm.restPointDic.Values.Count(v => v);
            
            // LightHouses统计
            summary.totalLightHouses = glm.lightHouseIsDic.Count;
            summary.activatedLightHouses = glm.lightHouseIsDic.Values.Count(v => v);
            
            // KeyPoints统计
            summary.totalKeyPoints = glm.keyPointDic.Count;
            summary.activatedKeyPoints = glm.keyPointDic.Values.Count(v => v);
            
            // ItemPoints统计
            summary.totalItemPoints = glm.itemPointDic.Count;
            summary.activatedItemPoints = glm.itemPointDic.Values.Count(v => v);
            
            // Doors统计
            summary.totalDoors = glm.doorIsUnlockedDic.Count;
            summary.unlockedDoors = glm.doorIsUnlockedDic.Values.Count(v => v);
            
            // AVG统计
            summary.totalAVGTriggered = glm.avgIndexIsTriggeredDic.Values.Count(v => v);
            summary.shelterAVGTriggered = glm.avgShelterIsTriggered.Values.Count(v => v);
        }

        // 道具装备统计
        if (ItemManager.Instance != null)
        {
            summary.totalItems = ItemManager.Instance.itemList?.Count ?? 0;
        }

        if (EquipmentManager.Instance != null)
        {
            summary.totalEquipments = EquipmentManager.Instance.equipmentList?.Count ?? 0;
            summary.totalEquipmentDurability = EquipmentManager.Instance.equipmentDurationDic?.Values.Sum() ?? 0;
        }

        // 计算完成率
        int totalInteractables = summary.totalRestPoints + summary.totalLightHouses + 
                                summary.totalKeyPoints + summary.totalItemPoints + summary.totalDoors;
        int activatedInteractables = summary.activatedRestPoints + summary.activatedLightHouses + 
                                   summary.activatedKeyPoints + summary.activatedItemPoints + summary.unlockedDoors;

        summary.completionPercentage = summary.totalEvents > 0 ? 
            ((float)summary.completedEvents / summary.totalEvents * 100f) : 0f;
        summary.interactionPercentage = totalInteractables > 0 ? 
            ((float)activatedInteractables / totalInteractables * 100f) : 0f;

        return summary;
    }

    /// <summary>
    /// 计算总交互数量
    /// </summary>
    private int CalculateTotalInteractions(ReviveDataSummary summary)
    {
        return summary.activatedRestPoints + summary.activatedLightHouses + 
               summary.activatedKeyPoints + summary.activatedItemPoints + 
               summary.unlockedDoors + summary.totalAVGTriggered + summary.shelterAVGTriggered;
    }

    /// <summary>
    /// 保存复活快照
    /// </summary>
    private void SaveReviveSnapshot(ReviveSnapshot snapshot)
    {
        reviveHistory.Add(snapshot);
        
        // 限制历史记录数量，避免文件过大
        if (reviveHistory.Count > 50)
        {
            reviveHistory.RemoveAt(0);
        }
        
        SaveReviveHistory();
    }

    /// <summary>
    /// 显示复活数据信息
    /// </summary>
    private void DisplayReviveData(ReviveSnapshot snapshot)
    {
        if (!enableDebugOutput) return;

        Debug.Log("======================================");
        Debug.Log("=== 复活数据详细信息 ===");
        Debug.Log("======================================");
        
        Debug.Log($"复活时间: {snapshot.reviveTime}");
        Debug.Log($"复活时SAN值: {snapshot.sanValue:F1}");
        Debug.Log($"当前关卡: {snapshot.currentLevel}");
        Debug.Log($"玩家位置: {snapshot.playerPosition}");
        Debug.Log($"总复活次数: {totalReviveCount + 1}");
        
        Debug.Log("=== 事件完成情况 ===");
        var summary = snapshot.dataSummary;
        Debug.Log($"总事件数: {summary.totalEvents}");
        Debug.Log($"已完成事件数: {summary.completedEvents}");
        Debug.Log($"事件完成率: {summary.completionPercentage:F1}%");
        Debug.Log($"起始事件: {summary.completedStartEvents}/{summary.totalStartEvents}");
        Debug.Log($"选项事件: {summary.completedOptionEvents}/{summary.totalOptionEvents}");
        
        if (snapshot.completedEventIds.Count > 0)
        {
            Debug.Log($"已完成事件ID: [{string.Join(", ", snapshot.completedEventIds)}]");
        }
        else
        {
            Debug.Log("目前没有已完成的事件");
        }

        Debug.Log("=== 交互物体情况 ===");
        Debug.Log($"休息点: {summary.activatedRestPoints}/{summary.totalRestPoints}");
        Debug.Log($"灯塔: {summary.activatedLightHouses}/{summary.totalLightHouses}");
        Debug.Log($"关键点: {summary.activatedKeyPoints}/{summary.totalKeyPoints}");
        Debug.Log($"道具点: {summary.activatedItemPoints}/{summary.totalItemPoints}");
        Debug.Log($"门: {summary.unlockedDoors}/{summary.totalDoors}");
        Debug.Log($"交互完成率: {summary.interactionPercentage:F1}%");
        Debug.Log($"总交互次数: {snapshot.totalInteractionsCount}");

        Debug.Log("=== 道具装备情况 ===");
        Debug.Log($"道具数量: {summary.totalItems}");
        Debug.Log($"装备数量: {summary.totalEquipments}");
        Debug.Log($"装备总耐久: {summary.totalEquipmentDurability}");

        Debug.Log("=== AVG剧情情况 ===");
        Debug.Log($"已触发AVG事件: {summary.totalAVGTriggered}");
        Debug.Log($"已触发安全屋AVG: {summary.shelterAVGTriggered}");

        Debug.Log("======================================");
        Debug.Log("=== 复活数据信息输出完成 ===");
        Debug.Log("======================================");
    }

    /// <summary>
    /// 更新复活统计信息
    /// </summary>
    private void UpdateReviveStatistics()
    {
        totalReviveCount++;
        lastReviveTime = Time.time;
    }

    /// <summary>
    /// 测试方法：手动显示当前复活数据
    /// </summary>
    public void TestShowReviveData()
    {
        Debug.Log("=== 手动测试复活数据显示 ===");
        ReviveSnapshot snapshot = CreateReviveSnapshot();
        snapshot.reviveTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " (测试)";
        DisplayReviveData(snapshot);
    }

    /// <summary>
    /// 获取最近的复活记录
    /// </summary>
    /// <param name="count">获取最近几次的记录，默认为5次</param>
    /// <returns>最近的复活记录列表</returns>
    public List<ReviveSnapshot> GetRecentReviveHistory(int count = 5)
    {
        if (reviveHistory.Count == 0) return new List<ReviveSnapshot>();
        
        int startIndex = Mathf.Max(0, reviveHistory.Count - count);
        return reviveHistory.GetRange(startIndex, reviveHistory.Count - startIndex);
    }

    /// <summary>
    /// 获取复活历史统计信息
    /// </summary>
    /// <returns>复活历史统计</returns>
    public ReviveHistoryStats GetReviveHistoryStats()
    {
        ReviveHistoryStats stats = new ReviveHistoryStats();
        
        if (reviveHistory.Count == 0) return stats;
        
        stats.totalRevives = reviveHistory.Count;
        stats.averageSanOnRevive = reviveHistory.Average(r => r.sanValue);
        stats.mostFrequentReviveLevel = reviveHistory
            .GroupBy(r => r.currentLevel)
            .OrderByDescending(g => g.Count())
            .FirstOrDefault()?.Key ?? E_GameLevelType.Tutorial;
        
        stats.progressOverTime = reviveHistory.Select(r => r.dataSummary.completionPercentage).ToList();
        stats.firstReviveTime = reviveHistory.FirstOrDefault()?.reviveTime ?? "";
        stats.lastReviveTime = reviveHistory.LastOrDefault()?.reviveTime ?? "";
        
        return stats;
    }

    /// <summary>
    /// 清空复活历史记录
    /// </summary>
    public void ClearReviveHistory()
    {
        reviveHistory.Clear();
        totalReviveCount = 0;
        lastReviveTime = 0f;
        SaveReviveHistory();
        Debug.Log("复活历史记录已清空");
    }

    /// <summary>
    /// 保存复活历史到文件
    /// </summary>
    private void SaveReviveHistory()
    {
        try
        {
            ReviveHistoryData data = new ReviveHistoryData
            {
                reviveSnapshots = reviveHistory,
                totalReviveCount = totalReviveCount,
                lastReviveTime = lastReviveTime
            };
            
            string json = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(reviveDataSavePath, json);
            
            if (enableDebugOutput)
            {
                Debug.Log($"复活数据已保存到: {reviveDataSavePath}");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"保存复活数据失败: {e.Message}");
        }
    }

    /// <summary>
    /// 从文件加载复活历史
    /// </summary>
    private void LoadReviveHistory()
    {
        if (!File.Exists(reviveDataSavePath))
        {
            Debug.Log("未找到复活数据文件，将创建新的记录");
            return;
        }

        try
        {
            string json = File.ReadAllText(reviveDataSavePath);
            ReviveHistoryData data = JsonConvert.DeserializeObject<ReviveHistoryData>(json);
            
            if (data != null)
            {
                reviveHistory = data.reviveSnapshots ?? new List<ReviveSnapshot>();
                totalReviveCount = data.totalReviveCount;
                lastReviveTime = data.lastReviveTime;
                
                if (enableDebugOutput)
                {
                    Debug.Log($"复活数据已加载，历史记录数: {reviveHistory.Count}，总复活次数: {totalReviveCount}");
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"加载复活数据失败: {e.Message}");
            reviveHistory = new List<ReviveSnapshot>();
        }
    }

    /// <summary>
    /// 复活历史统计信息
    /// </summary>
    [System.Serializable]
    public class ReviveHistoryStats
    {
        public int totalRevives;
        public float averageSanOnRevive;
        public E_GameLevelType mostFrequentReviveLevel;
        public List<float> progressOverTime;
        public string firstReviveTime;
        public string lastReviveTime;
    }

    /// <summary>
    /// 复活历史数据存储结构
    /// </summary>
    [System.Serializable]
    public class ReviveHistoryData
    {
        public List<ReviveSnapshot> reviveSnapshots;
        public int totalReviveCount;
        public float lastReviveTime;
    }

    /// <summary>
    /// 获取格式化的复活数据文本（用于UI显示）
    /// </summary>
    /// <returns>格式化的文本</returns>
    public string GetFormattedReviveDataText()
    {
        if (reviveHistory.Count == 0)
        {
            return "暂无复活记录";
        }

        var lastRevive = reviveHistory.LastOrDefault();
        if (lastRevive == null) return "暂无复活记录";

        var summary = lastRevive.dataSummary;
        
        string text = $"最近复活信息:\n";
        text += $"• 复活时间: {lastRevive.reviveTime}\n";
        text += $"• SAN值: {lastRevive.sanValue:F1}\n";
        text += $"• 当前关卡: {lastRevive.currentLevel}\n";
        text += $"• 事件完成: {summary.completedEvents}/{summary.totalEvents} ({summary.completionPercentage:F1}%)\n";
        text += $"• 交互完成: {summary.interactionPercentage:F1}%\n";
        text += $"• 总复活次数: {totalReviveCount}";

        return text;
    }

    /// <summary>
    /// 在OnDestroy时保存数据
    /// </summary>
    private void OnDestroy()
    {
        SaveReviveHistory();
    }

    /// <summary>
    /// 在应用暂停时保存数据
    /// </summary>
    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            SaveReviveHistory();
        }
    }
}
