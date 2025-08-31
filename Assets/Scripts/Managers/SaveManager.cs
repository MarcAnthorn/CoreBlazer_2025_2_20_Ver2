using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;
using System.Linq;

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
    private static string reviveSanDataSavePath => Application.persistentDataPath + "/revive_san_data_save.json";

    /// <summary>
    /// 交互对象SAN值映射表
    /// 根据策划提供的表格数据
    /// </summary>
    private static readonly Dictionary<int, InteractionReward> interactionRewards = new Dictionary<int, InteractionReward>
    {
        // 基础交互对象
        { 10005, new InteractionReward("灯塔", "普通POI", 1) },
        { 10003, new InteractionReward("假墙", "高级POI", 3) },
        { 10014, new InteractionReward("特殊墙壁", "高级POI", 3) },
        
        // 墙中鼠支线
        { 20016, new InteractionReward("墙中鼠支线点", "墙中鼠支线", 2) },
        { 20017, new InteractionReward("墙中鼠支线起点", "墙中鼠支线", 2) },
        { 20018, new InteractionReward("墙中鼠支线重要节点", "墙中鼠支线", 2) },
        { 20019, new InteractionReward("墙中鼠节点9", "墙中鼠支线", 2) },
        { 20025, new InteractionReward("墙中鼠节点10", "墙中鼠支线", 2) },
        { 20021, new InteractionReward("墙中鼠节点11", "墙中鼠支线", 2) },
        
        // 宝箱类
        { 20022, new InteractionReward("普通宝箱", "普通POI", 1) },
        { 20023, new InteractionReward("高级宝箱", "普通POI", 1) },
        { 20024, new InteractionReward("特殊宝箱1", "普通POI", 1) },
        { 20026, new InteractionReward("特殊宝箱2", "普通POI", 1) },
        
        // 第二层支线
        { 30016, new InteractionReward("第二层支线", "第二章支线", 4) },
        { 30017, new InteractionReward("第二层支线起点", "第二章支线", 4) },
        { 30018, new InteractionReward("第二层支线节点8", "第二章支线", 4) },
        
        // 第三层支线
        { 40016, new InteractionReward("第三层支线", "第三章支线", 6) },
        { 40017, new InteractionReward("第三层支线结局", "第三章支线", 6) },
        
        // 战斗和特殊点
        { 50001, new InteractionReward("塔罗牌投放点", "普通POI", 1) },
        { 50002, new InteractionReward("1级战斗事件点", "中级POI", 2) },
        { 50003, new InteractionReward("2级战斗事件点", "高级POI", 3) },
        { 50004, new InteractionReward("第一层关底boss", "第一层BOSS", 4) },
        { 50005, new InteractionReward("第二层关底boss", "第二层BOSS", 10) },
        { 50006, new InteractionReward("第三层关底boss", "第三层BOSS", 100) },
        
        // 特殊交互点
        { 60001, new InteractionReward("追逐怪刷新点", "高级POI", 3) },
        { 60002, new InteractionReward("休息点", "中级POI", 2) },
        { 70001, new InteractionReward("达贡剧情触发点+休息点", "达贡支线", 4) },
        
        // 怪谈事件库
        { 20010, new InteractionReward("低级怪谈事件库", "普通POI", 1) },
        { 20020, new InteractionReward("中级怪谈事件库", "中级POI", 2) },
        { 20030, new InteractionReward("高级怪谈事件库", "高级POI", 3) }
    };

    /// <summary>
    /// 交互奖励数据结构
    /// </summary>
    [System.Serializable]
    public class InteractionReward
    {
        public string name;
        public string type;
        public int sanValue;
        
        public InteractionReward(string name, string type, int sanValue)
        {
            this.name = name;
            this.type = type;
            this.sanValue = sanValue;
        }
    }

    /// <summary>
    /// 复活SAN数据记录
    /// </summary>
    [System.Serializable]
    public class ReviveSanData
    {
        public List<int> processedInteractionIds = new List<int>();
        public List<int> processedEventIds = new List<int>();
        public int totalSanEarned = 0;
        public int reviveCount = 0;
        public List<ReviveSanSnapshot> reviveHistory = new List<ReviveSanSnapshot>();
    }

    /// <summary>
    /// 单次复活的SAN数据快照
    /// </summary>
    [System.Serializable]
    public class ReviveSanSnapshot
    {
        public string reviveTime;
        public int sanEarnedThisRevive;
        public int totalSanEarned;
        public List<InteractionSanDetail> interactionDetails = new List<InteractionSanDetail>();
        public int reviveNumber;
    }

    /// <summary>
    /// 交互SAN详情
    /// </summary>
    [System.Serializable]
    public class InteractionSanDetail
    {
        public int interactionId;
        public string interactionName;
        public string interactionType;
        public int sanValue;
        public bool isNewThisRevive;
    }
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

    /// <summary>
    /// 复活时保存游戏并显示所有储存的事件与互动过的物体
    /// 集成EventIterator功能，类似其显示模式
    /// </summary>
    /// <param name="calculateSanReward">是否计算SAN奖励，默认为false（避免重复计算）</param>
    public void SaveGameOnReviveAndShowData(bool calculateSanReward = false)
    {
        Debug.Log("======================================");
        Debug.Log("=== 复活时保存游戏并显示数据 ===");
        Debug.Log("======================================");
        
        // 1. 执行标准保存
        SaveGame();
        Debug.Log("游戏保存完成");
        
        // 2. 可选：计算并给予复活SAN奖励
        int sanReward = 0;
        if (calculateSanReward)
        {
            sanReward = CalculateAndAwardReviveSan();
            Debug.Log($"✓ 复活SAN奖励计算完成，本次获得SAN: {sanReward}");
        }
        else
        {
            Debug.Log("! 跳过SAN奖励计算（已在PlayerController中计算）");
        }
        
        // 3. 显示复活时的基本信息
        ShowReviveBasicInfo();
        
        // 4. 显示已完成的事件信息
        ShowCompletedEventsInfo();
        
        // 5. 显示互动过的物体信息
        ShowInteractedObjectsInfo();
        
        // 6. 显示道具装备信息
        ShowItemsAndEquipmentsInfo();
        
        // 7. 显示AVG剧情信息
        ShowAVGProgressInfo();
        
        // 8. 显示SAN奖励详情
        ShowReviveSanDetails();
        
        // 9. 显示总体统计信息
        ShowOverallStatistics();
        
        Debug.Log("======================================");
        Debug.Log("=== 复活数据显示完成 ===");
        Debug.Log("======================================");
    }

    /// <summary>
    /// 计算并给予复活时的SAN奖励
    /// 确保不会重复计算之前复活时已经给予的奖励
    /// </summary>
    /// <returns>本次复活获得的SAN值</returns>
    public int CalculateAndAwardReviveSan()
    {
        Debug.Log("=== 开始计算复活SAN奖励 ===");
        
        // 加载复活SAN数据
        ReviveSanData sanData = LoadReviveSanData();
        
        // 获取当前所有交互过的对象
        List<int> currentInteractionIds = GetAllInteractionIds();
        List<int> currentEventIds = GetAllCompletedEventIds();
        
        // ============ 详细调试信息 ============
        Debug.Log("📊 === 交互对象数据详情 ===");
        Debug.Log($"📋 当前游玩激活的交互对象总数: {currentInteractionIds.Count}");
        Debug.Log($"📋 当前交互对象ID列表: [{string.Join(", ", currentInteractionIds)}]");
        
        Debug.Log($"📋 历史已处理的交互对象总数: {sanData.processedInteractionIds.Count}");
        Debug.Log($"📋 历史已处理ID列表: [{string.Join(", ", sanData.processedInteractionIds)}]");
        
        Debug.Log($"📋 当前游玩完成的事件总数: {currentEventIds.Count}");
        Debug.Log($"📋 当前事件ID列表: [{string.Join(", ", currentEventIds)}]");
        
        Debug.Log($"📋 历史已处理的事件总数: {sanData.processedEventIds.Count}");
        Debug.Log($"📋 历史已处理事件ID列表: [{string.Join(", ", sanData.processedEventIds)}]");
        
        // 特殊处理灯塔(ID=10005)的计算逻辑
        int currentLightHouseCount = GetCurrentActiveLightHouseCount();
        int processedLightHouseCount = GetProcessedLightHouseCount(sanData);
        int newLightHouseCount = currentLightHouseCount - processedLightHouseCount;
        
        Debug.Log("🗼 === 灯塔特殊处理逻辑 ===");
        Debug.Log($"🗼 当前激活灯塔数量: {currentLightHouseCount}");
        Debug.Log($"🗼 历史已处理灯塔数量: {processedLightHouseCount}");
        Debug.Log($"🗼 本次新增灯塔数量: {newLightHouseCount}");
        
        // 计算新增的交互和事件（排除灯塔，单独处理）
        List<int> newInteractionIds = currentInteractionIds.Except(sanData.processedInteractionIds).Where(id => id != 10005).ToList();
        List<int> newEventIds = currentEventIds.Except(sanData.processedEventIds).ToList();
        
        Debug.Log("🆕 === 本次新增数据 ===");
        Debug.Log($"🆕 本次新增交互对象数量(不含灯塔): {newInteractionIds.Count}");
        Debug.Log($"🆕 本次新增交互对象ID: [{string.Join(", ", newInteractionIds)}]");
        Debug.Log($"🆕 本次新增事件数量: {newEventIds.Count}");
        Debug.Log($"🆕 本次新增事件ID: [{string.Join(", ", newEventIds)}]");
        
        // 计算SAN奖励
        int sanReward = 0;
        List<InteractionSanDetail> interactionDetails = new List<InteractionSanDetail>();
        
        Debug.Log("💰 === 开始计算SAN奖励 ===");
        
        // 计算交互对象奖励（不含灯塔）
        foreach (int interactionId in newInteractionIds)
        {
            Debug.Log($"💰 处理交互对象ID: {interactionId}");
            if (interactionRewards.ContainsKey(interactionId))
            {
                var reward = interactionRewards[interactionId];
                sanReward += reward.sanValue;
                
                interactionDetails.Add(new InteractionSanDetail
                {
                    interactionId = interactionId,
                    interactionName = reward.name,
                    interactionType = reward.type,
                    sanValue = reward.sanValue,
                    isNewThisRevive = true
                });
                
                Debug.Log($"💰 ✅ 新交互对象: {reward.name} ({reward.type}) +{reward.sanValue} SAN，累计: {sanReward} SAN");
            }
            else
            {
                Debug.LogWarning($"💰 ⚠️ 未找到交互对象 {interactionId} 的SAN奖励配置");
            }
        }
        
        // 特殊处理灯塔奖励
        Debug.Log("🗼 === 开始计算灯塔奖励 ===");
        if (newLightHouseCount > 0)
        {
            if (interactionRewards.ContainsKey(10005))
            {
                var lightHouseReward = interactionRewards[10005];
                int lightHouseSanReward = newLightHouseCount * lightHouseReward.sanValue;
                sanReward += lightHouseSanReward;
                
                // 为每个新灯塔添加详情记录
                for (int i = 0; i < newLightHouseCount; i++)
                {
                    interactionDetails.Add(new InteractionSanDetail
                    {
                        interactionId = 10005,
                        interactionName = lightHouseReward.name,
                        interactionType = lightHouseReward.type,
                        sanValue = lightHouseReward.sanValue,
                        isNewThisRevive = true
                    });
                }
                
                Debug.Log($"🗼 ✅ 新激活 {newLightHouseCount} 个灯塔，每个 +{lightHouseReward.sanValue} SAN，总计 +{lightHouseSanReward} SAN");
                Debug.Log($"🗼 灯塔奖励累计，总SAN: {sanReward}");
            }
            else
            {
                Debug.LogWarning($"🗼 ⚠️ 未找到灯塔(ID:10005)的SAN奖励配置");
            }
        }
        else
        {
            Debug.Log("🗼 本次复活无新增灯塔");
        }
        
        Debug.Log($"💰 交互对象奖励计算完成，共获得: {sanReward} SAN");
        
        // 计算事件奖励（事件可以给予额外的SAN奖励，如果需要的话）
        Debug.Log("📜 === 开始计算事件奖励 ===");
        foreach (int eventId in newEventIds)
        {
            // 这里可以根据事件类型给予不同的SAN奖励
            // 暂时不实现，专注于交互对象的奖励
            Debug.Log($"📜 新完成事件: {eventId} (暂无奖励配置)");
        }
        Debug.Log("📜 事件奖励计算完成 (当前版本暂无事件奖励)");
        
        // 更新复活SAN数据
        Debug.Log("💾 === 开始更新复活数据 ===");
        Debug.Log($"💾 添加 {newInteractionIds.Count} 个新交互ID到已处理列表");
        Debug.Log($"💾 添加 {newLightHouseCount} 个新灯塔ID(10005)到已处理列表");
        Debug.Log($"💾 添加 {newEventIds.Count} 个新事件ID到已处理列表");
        
        // 添加非灯塔的交互对象
        sanData.processedInteractionIds.AddRange(newInteractionIds);
        
        // 特殊处理灯塔：为每个新灯塔添加一个10005的记录
        for (int i = 0; i < newLightHouseCount; i++)
        {
            sanData.processedInteractionIds.Add(10005);
        }
        
        sanData.processedEventIds.AddRange(newEventIds);
        sanData.totalSanEarned += sanReward;
        sanData.reviveCount++;
        
        Debug.Log($"💾 更新后已处理交互对象数量: {sanData.processedInteractionIds.Count}");
        Debug.Log($"💾 更新后已处理交互对象列表: [{string.Join(", ", sanData.processedInteractionIds)}]");
        Debug.Log($"💾 更新后总SAN收益: {sanData.totalSanEarned}");
        Debug.Log($"💾 复活次数: {sanData.reviveCount}");
        
        // 创建本次复活的快照
        ReviveSanSnapshot snapshot = new ReviveSanSnapshot
        {
            reviveTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            sanEarnedThisRevive = sanReward,
            totalSanEarned = sanData.totalSanEarned,
            interactionDetails = interactionDetails,
            reviveNumber = sanData.reviveCount
        };
        
        sanData.reviveHistory.Add(snapshot);
        Debug.Log($"💾 创建复活快照: 时间[{snapshot.reviveTime}] 本次SAN[{sanReward}] 总SAN[{sanData.totalSanEarned}]");
        
        // 限制历史记录数量
        if (sanData.reviveHistory.Count > 20)
        {
            sanData.reviveHistory.RemoveAt(0);
            Debug.Log("💾 移除最老的复活记录，保持历史记录在20条以内");
        }
        
        // 保存复活SAN数据
        SaveReviveSanData(sanData);
        Debug.Log("💾 复活SAN数据已保存到文件");
        
        // 给予玩家SAN值
        Debug.Log("🎯 === 开始应用SAN奖励 ===");
        if (sanReward > 0 && PlayerManager.Instance?.player != null)
        {
            float currentSan = PlayerManager.Instance.player.SAN.value;
            float maxSan = PlayerManager.Instance.player.SAN.value_limit;
            float newSan = Mathf.Min(currentSan + sanReward, maxSan);
            
            PlayerManager.Instance.player.SAN.SetValue(newSan);
            
            Debug.Log($"🎯 ✅ 玩家SAN值更新: {currentSan:F1} -> {newSan:F1} (+{sanReward})");
            
            if (newSan >= maxSan)
            {
                Debug.Log($"🎯 ⚠️ SAN值已达到上限 ({maxSan})");
            }
            
            // 触发UI更新
            EventHub.Instance.EventTrigger("UpdateAllUIElements");
            Debug.Log("🎯 已触发UI更新事件");
        }
        else if (sanReward == 0)
        {
            Debug.Log("🎯 本次复活无SAN奖励");
        }
        else
        {
            Debug.LogWarning("🎯 ⚠️ PlayerManager.Instance?.player 为空，无法应用SAN奖励");
        }
        
        Debug.Log($"🎉 === 复活SAN奖励计算完成，本次获得: {sanReward} SAN ===");
        Debug.Log("===============================================");
        return sanReward;
    }

    /// <summary>
    /// 获取所有交互过的对象ID
    /// 从GameLevelManager的各种交互字典中收集
    /// </summary>
    /// <returns>交互过的对象ID列表</returns>
    private List<int> GetAllInteractionIds()
    {
        List<int> interactionIds = new List<int>();
        
        if (GameLevelManager.Instance == null) 
        {
            Debug.LogWarning("⚠️ GameLevelManager.Instance 为空，无法获取交互对象数据");
            return interactionIds;
        }
        
        var glm = GameLevelManager.Instance;
        
        Debug.Log("🔍 === 开始收集交互对象数据 ===");
        
        // 从各种交互字典中收集已激活的交互点
        // 注意：这里假设字典的key包含了交互对象的ID信息
        // 实际实现可能需要根据具体的数据结构调整
        
        // 休息点
        Debug.Log($"🛏️ 检查休息点数据，字典总数: {glm.restPointDic.Count}");
        int restPointCount = 0;
        foreach (var kv in glm.restPointDic)
        {
            Debug.Log($"   休息点 [{kv.Key.Item1}, {kv.Key.Item2}] 状态: {(kv.Value ? "已激活" : "未激活")}");
            if (kv.Value) // 已激活
            {
                // 从位置信息推断交互ID，这里需要根据实际情况调整
                // 暂时使用简化的映射逻辑
                int interactionId = GetInteractionIdFromPosition(kv.Key.Item1, kv.Key.Item2, "RestPoint");
                if (interactionId > 0 && !interactionIds.Contains(interactionId))
                {
                    interactionIds.Add(interactionId);
                    restPointCount++;
                    Debug.Log($"   ✅ 添加休息点ID: {interactionId}");
                }
            }
        }
        Debug.Log($"🛏️ 休息点收集完成，已激活: {restPointCount} 个");
        
        // 灯塔
        Debug.Log($"🗼 检查灯塔数据，字典总数: {glm.lightHouseIsDic.Count}");
        int lightHouseCount = 0;
        Debug.Log(glm.lightHouseIsDic);

        foreach (var kv in glm.lightHouseIsDic)
        {
            Debug.Log($"   灯塔 [{kv.Key.Item1}, {kv.Key.Item2}] 状态: {(kv.Value ? "已激活" : "未激活")}");
            if (kv.Value) // 已激活
            {
                
                int interactionId = GetInteractionIdFromPosition(kv.Key.Item1, kv.Key.Item2, "LightHouse");
                Debug.Log($"   灯塔 [{kv.Key.Item1}, {kv.Key.Item2}] 状态: 已激活 {interactionId}" );

                    interactionIds.Add(interactionId);
                    lightHouseCount++;
                    Debug.Log($"   ✅ 添加灯塔ID: {interactionId}");
                
            }
        }
        Debug.Log($"🗼 灯塔收集完成，已激活: {lightHouseCount} 个");
        
        // 关键点
        Debug.Log($"🎯 检查关键点数据，字典总数: {glm.keyPointDic.Count}");
        int keyPointCount = 0;
        foreach (var kv in glm.keyPointDic)
        {
            Debug.Log($"   关键点 [{kv.Key.Item1}, {kv.Key.Item2}] 状态: {(kv.Value ? "已激活" : "未激活")}");
            if (kv.Value) // 已激活
            {
                int interactionId = GetInteractionIdFromPosition(kv.Key.Item1, kv.Key.Item2, "KeyPoint");
                if (interactionId > 0 && !interactionIds.Contains(interactionId))
                {
                    interactionIds.Add(interactionId);
                    keyPointCount++;
                    Debug.Log($"   ✅ 添加关键点ID: {interactionId}");
                }
            }
        }
        Debug.Log($"🎯 关键点收集完成，已激活: {keyPointCount} 个");
        
        // 道具点
        Debug.Log($"📦 检查道具点数据，字典总数: {glm.itemPointDic.Count}");
        int itemPointCount = 0;
        foreach (var kv in glm.itemPointDic)
        {
            Debug.Log($"   道具点 [{kv.Key.Item1}, {kv.Key.Item2}] 状态: {(kv.Value ? "已激活" : "未激活")}");
            if (kv.Value) // 已激活
            {
                int interactionId = GetInteractionIdFromPosition(kv.Key.Item1, kv.Key.Item2, "ItemPoint");
                if (interactionId > 0 && !interactionIds.Contains(interactionId))
                {
                    interactionIds.Add(interactionId);
                    itemPointCount++;
                    Debug.Log($"   ✅ 添加道具点ID: {interactionId}");
                }
            }
        }
        Debug.Log($"📦 道具点收集完成，已激活: {itemPointCount} 个");
        
        Debug.Log("🔍 === 交互对象收集汇总 ===");
        Debug.Log($"🛏️ 休息点: {restPointCount} 个");
        Debug.Log($"🗼 灯塔: {lightHouseCount} 个"); 
        Debug.Log($"🎯 关键点: {keyPointCount} 个");
        Debug.Log($"📦 道具点: {itemPointCount} 个");
        Debug.Log($"📊 总计收集到 {interactionIds.Count} 个交互对象ID: [{string.Join(", ", interactionIds)}]");
        return interactionIds;
    }

    /// <summary>
    /// 从位置和类型信息推断交互对象ID
    /// 这是一个简化的映射方法，实际实现需要根据游戏的具体数据结构调整
    /// </summary>
    /// <param name="level">关卡类型</param>
    /// <param name="position">位置</param>
    /// <param name="type">交互类型</param>
    /// <returns>交互对象ID，如果无法推断则返回0</returns>
    private int GetInteractionIdFromPosition(E_GameLevelType level, Vector3 position, string type)
    {
        // 这里是简化的映射逻辑
        // 实际游戏中应该有更精确的ID映射机制
        
        Debug.Log($"🔍 ID映射查询: 关卡[{level}] 位置[{position}] 类型[{type}]");
        
        // 根据关卡和类型生成基础ID
        int baseId = 0;
        switch (level)
        {
            case E_GameLevelType.Tutorial:
                baseId = type == "LightHouse" ? 10005 : 
                        type == "RestPoint" ? 60002 : 
                        type == "KeyPoint" ? 20016 : 
                        type == "ItemPoint" ? 20022 : 0;
                break;
            case E_GameLevelType.First:
                baseId = type == "LightHouse" ? 10005 : 
                        type == "RestPoint" ? 60002 : 
                        type == "KeyPoint" ? 20016 : 
                        type == "ItemPoint" ? 20022 : 0;
                break;
            case E_GameLevelType.Second:
                baseId = type == "LightHouse" ? 10005 : 
                        type == "RestPoint" ? 60002 : 
                        type == "KeyPoint" ? 30016 : 
                        type == "ItemPoint" ? 20023 : 0;
                break;
            case E_GameLevelType.Third:
                baseId = type == "LightHouse" ? 10005 : 
                        type == "RestPoint" ? 60002 : 
                        type == "KeyPoint" ? 40016 : 
                        type == "ItemPoint" ? 20024 : 0;
                break;
            default:
                Debug.LogWarning($"⚠️ 未知关卡类型: {level}，无法映射交互ID");
                break;
        }
        
        Debug.Log($"🔍 ID映射结果: 关卡[{level}] 类型[{type}] -> ID[{baseId}]");
        return baseId;
    }

    /// <summary>
    /// 获取当前激活的灯塔数量
    /// 专门用于处理灯塔ID相同的问题
    /// </summary>
    /// <returns>当前激活的灯塔数量</returns>
    private int GetCurrentActiveLightHouseCount()
    {
        int count = 0;
        if (GameLevelManager.Instance == null) 
        {
            Debug.LogWarning("⚠️ GameLevelManager.Instance 为空，无法获取灯塔数据");
            return count;
        }
        
        var glm = GameLevelManager.Instance;
        foreach (var kv in glm.lightHouseIsDic)
        {
            if (kv.Value) // 已激活
            {
                count++;
            }
        }
        
        Debug.Log($"🗼 当前激活灯塔数量统计: {count}");
        return count;
    }

    /// <summary>
    /// 获取历史已处理的灯塔数量
    /// 通过计算历史数据中灯塔ID(10005)的出现次数
    /// </summary>
    /// <param name="sanData">复活SAN数据</param>
    /// <returns>历史已处理的灯塔数量</returns>
    private int GetProcessedLightHouseCount(ReviveSanData sanData)
    {
        // 在processedInteractionIds中计算ID为10005的数量
        int count = sanData.processedInteractionIds.Count(id => id == 10005);
        Debug.Log($"🗼 历史已处理灯塔数量: {count}");
        return count;
    }

    /// <summary>
    /// 获取所有已完成的事件ID
    /// </summary>
    /// <returns>已完成事件ID列表</returns>
    private List<int> GetAllCompletedEventIds()
    {
        Debug.Log("📜 === 开始收集事件数据 ===");
        
        if (EventIterator.Instance != null)
        {
            var eventIds = EventIterator.Instance.GetCompletedEventIds();
            Debug.Log($"📜 通过EventIterator获取到 {eventIds.Count} 个已完成事件");
            Debug.Log($"📜 已完成事件ID列表: [{string.Join(", ", eventIds)}]");
            return eventIds;
        }
        else
        {
            Debug.LogWarning("⚠️ EventIterator.Instance 为空，无法获取事件数据");
            return new List<int>();
        }
    }

    /// <summary>
    /// 加载复活SAN数据
    /// </summary>
    /// <returns>复活SAN数据</returns>
    private ReviveSanData LoadReviveSanData()
    {
        Debug.Log("📂 === 开始加载复活SAN数据 ===");
        Debug.Log($"📂 数据文件路径: {reviveSanDataSavePath}");
        
        if (!File.Exists(reviveSanDataSavePath))
        {
            Debug.Log("📂 数据文件不存在，创建新的复活SAN数据");
            return new ReviveSanData();
        }

        try
        {
            string json = File.ReadAllText(reviveSanDataSavePath);
            Debug.Log($"📂 读取文件内容，大小: {json.Length} 字符");
            
            ReviveSanData data = JsonConvert.DeserializeObject<ReviveSanData>(json);
            Debug.Log($"📂 ✅ 复活SAN数据加载成功");
            Debug.Log($"📂 已处理交互对象: {data.processedInteractionIds.Count} 个");
            Debug.Log($"📂 已处理事件: {data.processedEventIds.Count} 个");
            Debug.Log($"📂 总SAN收益: {data.totalSanEarned}");
            Debug.Log($"📂 复活次数: {data.reviveCount}");
            Debug.Log($"📂 历史记录: {data.reviveHistory.Count} 条");
            
            return data ?? new ReviveSanData();
            
        }
        catch (System.Exception e)
        {
            Debug.LogError($"📂 ❌ 加载复活SAN数据失败: {e.Message}");
            return new ReviveSanData();
        }
    }

    /// <summary>
    /// 保存复活SAN数据
    /// </summary>
    /// <param name="data">要保存的数据</param>
    private void SaveReviveSanData(ReviveSanData data)
    {
        Debug.Log("💾 === 开始保存复活SAN数据 ===");
        try
        {
            string json = JsonConvert.SerializeObject(data, Formatting.Indented);
            Debug.Log($"💾 序列化数据，大小: {json.Length} 字符");
            Debug.Log($"💾 保存路径: {reviveSanDataSavePath}");
            
            // 确保目录存在
            string directory = Path.GetDirectoryName(reviveSanDataSavePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
                Debug.Log($"💾 创建目录: {directory}");
            }
            
            File.WriteAllText(reviveSanDataSavePath, json);
            Debug.Log($"💾 ✅ 复活SAN数据保存成功");
            Debug.Log($"💾 数据内容摘要: 交互[{data.processedInteractionIds.Count}个] 事件[{data.processedEventIds.Count}个] 总SAN[{data.totalSanEarned}] 复活[{data.reviveCount}次]");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"💾 ❌ 保存复活SAN数据失败: {e.Message}");
        }
    }

    /// <summary>
    /// 显示复活SAN奖励详情
    /// </summary>
    private void ShowReviveSanDetails()
    {
        Debug.Log("=== 复活SAN奖励详情 ===");
        
        ReviveSanData sanData = LoadReviveSanData();
        
        if (sanData.reviveHistory.Count > 0)
        {
            var lastRevive = sanData.reviveHistory.LastOrDefault();
            if (lastRevive != null)
            {
                Debug.Log($"本次复活获得SAN: {lastRevive.sanEarnedThisRevive}");
                Debug.Log($"累计复活SAN: {lastRevive.totalSanEarned}");
                Debug.Log($"复活次数: {lastRevive.reviveNumber}");
                
                if (lastRevive.interactionDetails.Count > 0)
                {
                    Debug.Log("本次新增交互奖励:");
                    foreach (var detail in lastRevive.interactionDetails)
                    {
                        if (detail.isNewThisRevive)
                        {
                            Debug.Log($"  ✓ {detail.interactionName} ({detail.interactionType}) +{detail.sanValue} SAN");
                        }
                    }
                }
                else
                {
                    Debug.Log("本次复活没有新的交互奖励");
                }
            }
        }
        else
        {
            Debug.Log("这是第一次复活，暂无历史数据");
        }
        
        // 显示当前玩家SAN状态
        if (PlayerManager.Instance?.player != null)
        {
            var player = PlayerManager.Instance.player;
            Debug.Log($"当前玩家SAN: {player.SAN.value:F1}/{player.SAN.value_limit:F1}");
        }
    }

    /// <summary>
    /// 清空复活SAN数据（用于测试或重置）
    /// </summary>
    public void ClearReviveSanData()
    {
        if (File.Exists(reviveSanDataSavePath))
        {
            File.Delete(reviveSanDataSavePath);
            Debug.Log("复活SAN数据已清空");
        }
    }

    /// <summary>
    /// 显示复活时的基本信息
    /// </summary>
    private void ShowReviveBasicInfo()
    {
        Debug.Log("=== 复活基本信息 ===");
        Debug.Log($"复活时间: {System.DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        
        if (PlayerManager.Instance?.player != null)
        {
            var player = PlayerManager.Instance.player;
            Debug.Log($"复活时SAN值: {player.SAN.value:F1}/{player.SAN.value_limit:F1}");
            Debug.Log($"复活时HP值: {player.HP.value:F1}/{player.HP.value_limit:F1}");
            Debug.Log($"玩家等级: {player.LVL.value:F1}");
        }
        
        if (GameLevelManager.Instance != null)
        {
            Debug.Log($"当前关卡: {GameLevelManager.Instance.gameLevelType}");
        }
        
        if (PlayerManager.Instance != null)
        {
            Debug.Log($"玩家位置: {PlayerManager.Instance.playerPosition}");
        }
    }

    /// <summary>
    /// 显示已完成的事件信息
    /// </summary>
    private void ShowCompletedEventsInfo()
    {
        Debug.Log("=== 已完成事件信息 ===");
        
        if (EventIterator.Instance != null)
        {
            // 使用EventIterator的功能显示事件信息
            var completedEventIds = EventIterator.Instance.GetCompletedEventIds();
            var eventStats = EventIterator.Instance.GetEventCompletionStats();
            
            Debug.Log($"总事件数: {eventStats.TotalEvents}");
            Debug.Log($"已完成事件数: {eventStats.CompletedEvents}");
            Debug.Log($"事件完成率: {eventStats.CompletionPercentage:F1}%");
            Debug.Log($"起始事件: {eventStats.CompletedStartEvents}/{eventStats.TotalStartEvents}");
            Debug.Log($"选项事件: {eventStats.CompletedOptionEvents}/{eventStats.TotalOptionEvents}");
            
            if (completedEventIds.Count > 0)
            {
                Debug.Log($"已完成事件ID列表: [{string.Join(", ", completedEventIds)}]");
                
                // 显示最近完成的几个事件的详细信息
                var completedEvents = EventIterator.Instance.GetCompletedEvents();
                var recentEvents = completedEvents.Count > 5 ? 
                    completedEvents.GetRange(completedEvents.Count - 5, 5) : completedEvents;
                    
                Debug.Log("最近完成的事件详情:");
                foreach (var evt in recentEvents)
                {
                    Debug.Log($"  ✓ 事件ID: {evt.eventId} | 库ID: {evt.libId} | 描述: {evt.evDescription ?? "无描述"}");
                }
            }
            else
            {
                Debug.Log("目前没有已完成的事件");
            }
        }
        else
        {
            Debug.LogWarning("EventIterator.Instance 为空，无法显示事件信息");
        }
    }

    /// <summary>
    /// 显示互动过的物体信息
    /// </summary>
    private void ShowInteractedObjectsInfo()
    {
        Debug.Log("=== 互动物体信息 ===");
        
        if (GameLevelManager.Instance != null)
        {
            var glm = GameLevelManager.Instance;
            
            // 休息点信息
            int totalRestPoints = glm.restPointDic.Count;
            int activatedRestPoints = 0;
            foreach (var rp in glm.restPointDic)
            {
                if (rp.Value) activatedRestPoints++;
            }
            Debug.Log($"休息点: {activatedRestPoints}/{totalRestPoints} 已激活");
            
            // 灯塔信息
            int totalLightHouses = glm.lightHouseIsDic.Count;
            int activatedLightHouses = 0;
            foreach (var lh in glm.lightHouseIsDic)
            {
                if (lh.Value) activatedLightHouses++;
            }
            Debug.Log($"灯塔: {activatedLightHouses}/{totalLightHouses} 已激活");
            
            // 关键点信息
            int totalKeyPoints = glm.keyPointDic.Count;
            int activatedKeyPoints = 0;
            foreach (var kp in glm.keyPointDic)
            {
                if (kp.Value) activatedKeyPoints++;
            }
            Debug.Log($"关键点: {activatedKeyPoints}/{totalKeyPoints} 已触发");
            
            // 道具点信息
            int totalItemPoints = glm.itemPointDic.Count;
            int activatedItemPoints = 0;
            foreach (var ip in glm.itemPointDic)
            {
                if (ip.Value) activatedItemPoints++;
            }
            Debug.Log($"道具点: {activatedItemPoints}/{totalItemPoints} 已收集");
            
            // 门锁信息
            int totalDoors = glm.doorIsUnlockedDic.Count;
            int unlockedDoors = 0;
            foreach (var door in glm.doorIsUnlockedDic)
            {
                if (door.Value) unlockedDoors++;
            }
            Debug.Log($"门锁: {unlockedDoors}/{totalDoors} 已解锁");
            
            // 计算总体交互完成率
            int totalInteractables = totalRestPoints + totalLightHouses + totalKeyPoints + totalItemPoints + totalDoors;
            int activatedInteractables = activatedRestPoints + activatedLightHouses + activatedKeyPoints + activatedItemPoints + unlockedDoors;
            float interactionPercentage = totalInteractables > 0 ? ((float)activatedInteractables / totalInteractables * 100f) : 0f;
            
            Debug.Log($"总体交互完成率: {interactionPercentage:F1}% ({activatedInteractables}/{totalInteractables})");
        }
        else
        {
            Debug.LogWarning("GameLevelManager.Instance 为空，无法显示交互物体信息");
        }
    }

    /// <summary>
    /// 显示道具装备信息
    /// </summary>
    private void ShowItemsAndEquipmentsInfo()
    {
        Debug.Log("=== 道具装备信息 ===");
        
        // 道具信息
        if (ItemManager.Instance != null)
        {
            var itemList = ItemManager.Instance.itemList;
            var itemCountDic = ItemManager.Instance.itemCountDic;
            
            Debug.Log($"道具种类数: {itemList.Count}");
            Debug.Log($"道具总数量: {itemCountDic.Values.Sum()}");
            
            if (itemList.Count > 0)
            {
                Debug.Log("持有道具列表:");
                foreach (var itemId in itemList)
                {
                    int count = itemCountDic.ContainsKey(itemId) ? itemCountDic[itemId] : 1;
                    if (LoadManager.Instance?.allItems?.ContainsKey(itemId) == true)
                    {
                        var item = LoadManager.Instance.allItems[itemId];
                        Debug.Log($"  • 道具ID: {itemId} | 数量: {count} | 名称: {item.name ?? "未知道具"}");
                    }
                    else
                    {
                        Debug.Log($"  • 道具ID: {itemId} | 数量: {count} | 名称: 未知道具");
                    }
                }
            }
        }
        else
        {
            Debug.LogWarning("ItemManager.Instance 为空，无法显示道具信息");
        }
        
        // 装备信息
        if (EquipmentManager.Instance != null)
        {
            var equipmentList = EquipmentManager.Instance.equipmentList;
            var equipmentDurationDic = EquipmentManager.Instance.equipmentDurationDic;
            
            Debug.Log($"装备数量: {equipmentList.Count}");
            Debug.Log($"装备总耐久: {equipmentDurationDic.Values.Sum()}");
            
            if (equipmentList.Count > 0)
            {
                Debug.Log("持有装备列表:");
                foreach (var equipment in equipmentList)
                {
                    int durability = equipmentDurationDic.ContainsKey(equipment) ? equipmentDurationDic[equipment] : 0;
                    Debug.Log($"  • 装备ID: {equipment.id} | 耐久: {durability} | 名称: {equipment.name ?? "未知装备"}");
                }
            }
        }
        else
        {
            Debug.LogWarning("EquipmentManager.Instance 为空，无法显示装备信息");
        }
    }

    /// <summary>
    /// 显示AVG剧情进度信息
    /// </summary>
    private void ShowAVGProgressInfo()
    {
        Debug.Log("=== AVG剧情进度信息 ===");
        
        if (GameLevelManager.Instance != null)
        {
            var glm = GameLevelManager.Instance;
            
            // NPC事件AVG统计
            int totalAVGEvents = glm.avgIndexIsTriggeredDic.Count;
            int triggeredAVGEvents = 0;
            foreach (var avg in glm.avgIndexIsTriggeredDic)
            {
                if (avg.Value) triggeredAVGEvents++;
            }
            Debug.Log($"NPC事件AVG: {triggeredAVGEvents}/{totalAVGEvents} 已触发");
            
            // 安全屋AVG统计
            int totalShelterAVG = glm.avgShelterIsTriggered.Count;
            int triggeredShelterAVG = 0;
            foreach (var shelter in glm.avgShelterIsTriggered)
            {
                if (shelter.Value) triggeredShelterAVG++;
            }
            Debug.Log($"安全屋AVG: {triggeredShelterAVG}/{totalShelterAVG} 已触发");
            
            // 显示已触发的AVG列表
            if (triggeredAVGEvents > 0)
            {
                Debug.Log("已触发的NPC事件AVG ID:");
                foreach (var avg in glm.avgIndexIsTriggeredDic)
                {
                    if (avg.Value)
                    {
                        Debug.Log($"  ✓ AVG ID: {avg.Key}");
                    }
                }
            }
            
            if (triggeredShelterAVG > 0)
            {
                Debug.Log("已触发的安全屋AVG:");
                foreach (var shelter in glm.avgShelterIsTriggered)
                {
                    if (shelter.Value)
                    {
                        Debug.Log($"  ✓ 关卡: {shelter.Key}");
                    }
                }
            }
        }
        else
        {
            Debug.LogWarning("GameLevelManager.Instance 为空，无法显示AVG信息");
        }
    }

    /// <summary>
    /// 显示总体统计信息
    /// </summary>
    private void ShowOverallStatistics()
    {
        Debug.Log("=== 总体统计信息 ===");
        
        // 计算游戏总体完成度
        float eventCompletionRate = 0f;
        float interactionCompletionRate = 0f;
        float avgCompletionRate = 0f;
        
        // 事件完成率
        if (EventIterator.Instance != null)
        {
            var eventStats = EventIterator.Instance.GetEventCompletionStats();
            eventCompletionRate = eventStats.CompletionPercentage;
        }
        
        // 交互完成率
        if (GameLevelManager.Instance != null)
        {
            var glm = GameLevelManager.Instance;
            int totalInteractables = glm.restPointDic.Count + glm.lightHouseIsDic.Count + 
                                   glm.keyPointDic.Count + glm.itemPointDic.Count + glm.doorIsUnlockedDic.Count;
            int activatedInteractables = 0;
            foreach (var kv in glm.restPointDic) if (kv.Value) activatedInteractables++;
            foreach (var kv in glm.lightHouseIsDic) if (kv.Value) activatedInteractables++;
            foreach (var kv in glm.keyPointDic) if (kv.Value) activatedInteractables++;
            foreach (var kv in glm.itemPointDic) if (kv.Value) activatedInteractables++;
            foreach (var kv in glm.doorIsUnlockedDic) if (kv.Value) activatedInteractables++;
            
            interactionCompletionRate = totalInteractables > 0 ? 
                ((float)activatedInteractables / totalInteractables * 100f) : 0f;
                
            // AVG完成率
            int totalAVG = glm.avgIndexIsTriggeredDic.Count + glm.avgShelterIsTriggered.Count;
            int triggeredAVG = 0;
            foreach (var kv in glm.avgIndexIsTriggeredDic) if (kv.Value) triggeredAVG++;
            foreach (var kv in glm.avgShelterIsTriggered) if (kv.Value) triggeredAVG++;
            
            avgCompletionRate = totalAVG > 0 ? ((float)triggeredAVG / totalAVG * 100f) : 0f;
        }
        
        float overallCompletionRate = (eventCompletionRate + interactionCompletionRate + avgCompletionRate) / 3f;
        
        Debug.Log($"事件完成率: {eventCompletionRate:F1}%");
        Debug.Log($"交互完成率: {interactionCompletionRate:F1}%");
        Debug.Log($"AVG完成率: {avgCompletionRate:F1}%");
        Debug.Log($"总体游戏完成度: {overallCompletionRate:F1}%");
        
        // 显示存档文件信息
        Debug.Log("=== 存档文件信息 ===");
        Debug.Log($"玩家属性存档: {savePath}");
        Debug.Log($"玩家位置存档: {playerPositionSavePath}");
        Debug.Log($"关卡进度存档: {gameLevelSavePath}");
        Debug.Log($"装备存档: {equipmentSavePath}");
        Debug.Log($"道具存档: {itemSavePath}");
        Debug.Log($"AVG分发存档: {avgDistributeSavePath}");
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
        if (File.Exists(reviveSanDataSavePath)) File.Delete(reviveSanDataSavePath);

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

        Debug.Log("所有存档已清空，玩家进度已重置！包括复活SAN数据");
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
            
            Debug.Log($"场景加载完成！玩家位置已成功设置为: {targetPosition}");
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

    /// <summary>
    /// 调试方法：显示所有交互数据和游玩数据的详细信息
    /// 可以在Update中按特定键触发，或通过外部调用进行调试
    /// </summary>
    [System.Obsolete("此方法仅用于调试")]
    public void DebugShowAllGameData()
    {
        Debug.Log("🔧 ================ 调试：显示所有游戏数据 ================");
        
        // 1. 显示当前交互状态
        Debug.Log("🔧 === 当前交互状态 ===");
        List<int> currentInteractions = GetAllInteractionIds();
        List<int> currentEvents = GetAllCompletedEventIds();
        
        // 2. 显示历史数据
        Debug.Log("🔧 === 历史复活数据 ===");
        ReviveSanData sanData = LoadReviveSanData();
        
        // 3. 显示差异对比
        Debug.Log("🔧 === 数据差异对比 ===");
        List<int> newInteractions = currentInteractions.Except(sanData.processedInteractionIds).ToList();
        List<int> newEvents = currentEvents.Except(sanData.processedEventIds).ToList();
        
        Debug.Log($"🔧 新增交互: [{string.Join(", ", newInteractions)}]");
        Debug.Log($"🔧 新增事件: [{string.Join(", ", newEvents)}]");
        
        // 4. 显示奖励配置状态
        Debug.Log("🔧 === 奖励配置状态 ===");
        foreach (int interactionId in newInteractions)
        {
            if (interactionRewards.ContainsKey(interactionId))
            {
                var reward = interactionRewards[interactionId];
                Debug.Log($"🔧 ✅ ID[{interactionId}]: {reward.name} ({reward.type}) = {reward.sanValue} SAN");
            }
            else
            {
                Debug.Log($"🔧 ❌ ID[{interactionId}]: 未配置奖励");
            }
        }
        
        Debug.Log("🔧 ============================================");
    }

    /// <summary>
    /// 调试方法：测试灯塔SAN计算逻辑
    /// 验证多个相同ID灯塔的奖励计算是否正确
    /// </summary>
    [System.Obsolete("此方法仅用于调试")]
    public void DebugTestLightHouseSanLogic()
    {
        Debug.Log("🗼 ================ 灯塔SAN逻辑测试 ================");
        
        // 获取当前数据
        int currentLightHouses = GetCurrentActiveLightHouseCount();
        ReviveSanData sanData = LoadReviveSanData();
        int processedLightHouses = GetProcessedLightHouseCount(sanData);
        int newLightHouses = currentLightHouses - processedLightHouses;
        
        Debug.Log($"🗼 测试结果:");
        Debug.Log($"🗼 当前激活灯塔: {currentLightHouses} 个");
        Debug.Log($"🗼 历史已处理: {processedLightHouses} 个");
        Debug.Log($"🗼 应获得奖励: {newLightHouses} 个");
        
        if (newLightHouses > 0 && interactionRewards.ContainsKey(10005))
        {
            int perLightHouseSan = interactionRewards[10005].sanValue;
            int totalSan = newLightHouses * perLightHouseSan;
            Debug.Log($"🗼 每个灯塔SAN: {perLightHouseSan}");
            Debug.Log($"🗼 总SAN奖励: {totalSan}");
        }
        
        Debug.Log("🗼 ============================================");
    }

    [System.Serializable]
    public class AVGDistributeSaveData
    {
        public List<int> contributedAvgIdList;
        public Dictionary<int, List<int>> npcAvgQueueDic;
    }

//---------------------------存档可序列化数据容器--------------------------------------------
}
