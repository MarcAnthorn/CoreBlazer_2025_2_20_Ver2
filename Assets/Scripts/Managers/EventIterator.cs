using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 事件结果遍历器 - 用于获取已完成事件的相关信息
/// </summary>
public class EventIterator : Singleton<EventIterator>
{
    /// <summary>
    /// 初始化时的提示信息
    /// </summary>
    private void Start()
    {
        Debug.Log("EventIterator 已初始化 - 按 P 键可查看所有事件信息");
    }

    /// <summary>
    /// 获取已完成的事件ID列表
    /// </summary>
    /// <returns>已完成事件的ID列表</returns>
    public List<int> GetCompletedEventIds()
    {
        List<int> completedEventIds = new List<int>();

        if (LoadManager.Instance == null) return completedEventIds;

        // 遍历起始事件
        if (LoadManager.Instance.startEvents != null)
        {
            foreach (var eventPair in LoadManager.Instance.startEvents)
            {
                if (eventPair.Value != null && eventPair.Value.isTrigger)
                {
                    completedEventIds.Add(eventPair.Key);
                }
            }
        }

        // 遍历选项事件
        if (LoadManager.Instance.optionEvents != null)
        {
            foreach (var eventPair in LoadManager.Instance.optionEvents)
            {
                if (eventPair.Value != null && eventPair.Value.isTrigger)
                {
                    completedEventIds.Add(eventPair.Key);
                }
            }
        }

        // 按ID排序返回
        completedEventIds.Sort();
        return completedEventIds;
    }

    /// <summary>
    /// 获取已完成的事件详细信息
    /// </summary>
    /// <returns>已完成事件的详细信息列表</returns>
    public List<Event> GetCompletedEvents()
    {
        List<Event> completedEvents = new List<Event>();

        if (LoadManager.Instance == null) return completedEvents;

        // 遍历起始事件
        if (LoadManager.Instance.startEvents != null)
        {
            foreach (var eventPair in LoadManager.Instance.startEvents)
            {
                if (eventPair.Value != null && eventPair.Value.isTrigger)
                {
                    completedEvents.Add(eventPair.Value);
                }
            }
        }

        // 遍历选项事件
        if (LoadManager.Instance.optionEvents != null)
        {
            foreach (var eventPair in LoadManager.Instance.optionEvents)
            {
                if (eventPair.Value != null && eventPair.Value.isTrigger)
                {
                    completedEvents.Add(eventPair.Value);
                }
            }
        }

        // 按事件ID排序
        completedEvents.Sort((a, b) => a.eventId.CompareTo(b.eventId));
        return completedEvents;
    }

    /// <summary>
    /// 获取指定关卡已完成的事件ID
    /// </summary>
    /// <param name="libId">关卡库ID (如 2001, 2002, 2003 等)</param>
    /// <returns>指定关卡已完成事件的ID列表</returns>
    public List<int> GetCompletedEventIdsByLevel(int libId)
    {
        List<int> completedEventIds = new List<int>();

        if (LoadManager.Instance == null) return completedEventIds;

        // 遍历起始事件
        if (LoadManager.Instance.startEvents != null)
        {
            foreach (var eventPair in LoadManager.Instance.startEvents)
            {
                if (eventPair.Value != null && 
                    eventPair.Value.isTrigger && 
                    eventPair.Value.libId == libId)
                {
                    completedEventIds.Add(eventPair.Key);
                }
            }
        }

        // 遍历选项事件
        if (LoadManager.Instance.optionEvents != null)
        {
            foreach (var eventPair in LoadManager.Instance.optionEvents)
            {
                if (eventPair.Value != null && 
                    eventPair.Value.isTrigger && 
                    eventPair.Value.libId == libId)
                {
                    completedEventIds.Add(eventPair.Key);
                }
            }
        }

        completedEventIds.Sort();
        return completedEventIds;
    }

    /// <summary>
    /// 获取当前关卡的已完成事件ID
    /// </summary>
    /// <returns>当前关卡已完成事件的ID列表</returns>
    public List<int> GetCurrentLevelCompletedEventIds()
    {
        if (GameLevelManager.Instance == null) return new List<int>();

        // 根据当前关卡类型确定库ID
        int currentLibId = GetLibIdFromGameLevel(GameLevelManager.Instance.gameLevelType);
        return GetCompletedEventIdsByLevel(currentLibId);
    }

    /// <summary>
    /// 检查指定事件是否已完成
    /// </summary>
    /// <param name="eventId">事件ID</param>
    /// <returns>事件是否已完成</returns>
    public bool IsEventCompleted(int eventId)
    {
        if (LoadManager.Instance == null) return false;

        // 检查起始事件
        if (LoadManager.Instance.startEvents != null && LoadManager.Instance.startEvents.ContainsKey(eventId))
        {
            return LoadManager.Instance.startEvents[eventId] != null && LoadManager.Instance.startEvents[eventId].isTrigger;
        }

        // 检查选项事件
        if (LoadManager.Instance.optionEvents != null && LoadManager.Instance.optionEvents.ContainsKey(eventId))
        {
            return LoadManager.Instance.optionEvents[eventId] != null && LoadManager.Instance.optionEvents[eventId].isTrigger;
        }

        return false;
    }

    /// <summary>
    /// 获取已完成事件的统计信息
    /// </summary>
    /// <returns>已完成事件的统计信息</returns>
    public EventCompletionStats GetEventCompletionStats()
    {
        EventCompletionStats stats = new EventCompletionStats();

        if (LoadManager.Instance == null) return stats;

        // 统计起始事件
        if (LoadManager.Instance.startEvents != null)
        {
            stats.TotalStartEvents = LoadManager.Instance.startEvents.Count;
            foreach (var eventPair in LoadManager.Instance.startEvents)
            {
                if (eventPair.Value != null && eventPair.Value.isTrigger)
                {
                    stats.CompletedStartEvents++;
                }
            }
        }

        // 统计选项事件
        if (LoadManager.Instance.optionEvents != null)
        {
            stats.TotalOptionEvents = LoadManager.Instance.optionEvents.Count;
            foreach (var eventPair in LoadManager.Instance.optionEvents)
            {
                if (eventPair.Value != null && eventPair.Value.isTrigger)
                {
                    stats.CompletedOptionEvents++;
                }
            }
        }

        stats.TotalEvents = stats.TotalStartEvents + stats.TotalOptionEvents;
        stats.CompletedEvents = stats.CompletedStartEvents + stats.CompletedOptionEvents;

        return stats;
    }

    /// <summary>
    /// 获取指定关卡的事件完成统计
    /// </summary>
    /// <param name="libId">关卡库ID</param>
    /// <returns>指定关卡的事件完成统计</returns>
    public EventCompletionStats GetEventCompletionStatsByLevel(int libId)
    {
        EventCompletionStats stats = new EventCompletionStats();

        if (LoadManager.Instance == null) return stats;

        // 统计指定关卡的起始事件
        if (LoadManager.Instance.startEvents != null)
        {
            foreach (var eventPair in LoadManager.Instance.startEvents)
            {
                if (eventPair.Value != null && eventPair.Value.libId == libId)
                {
                    stats.TotalStartEvents++;
                    if (eventPair.Value.isTrigger)
                    {
                        stats.CompletedStartEvents++;
                    }
                }
            }
        }

        // 统计指定关卡的选项事件
        if (LoadManager.Instance.optionEvents != null)
        {
            foreach (var eventPair in LoadManager.Instance.optionEvents)
            {
                if (eventPair.Value != null && eventPair.Value.libId == libId)
                {
                    stats.TotalOptionEvents++;
                    if (eventPair.Value.isTrigger)
                    {
                        stats.CompletedOptionEvents++;
                    }
                }
            }
        }

        stats.TotalEvents = stats.TotalStartEvents + stats.TotalOptionEvents;
        stats.CompletedEvents = stats.CompletedStartEvents + stats.CompletedOptionEvents;

        return stats;
    }

    /// <summary>
    /// 获取已完成事件的描述信息
    /// </summary>
    /// <param name="maxCount">最大返回数量，默认为-1表示返回所有</param>
    /// <returns>已完成事件的描述信息列表</returns>
    public List<string> GetCompletedEventDescriptions(int maxCount = -1)
    {
        List<string> descriptions = new List<string>();
        List<Event> completedEvents = GetCompletedEvents();

        int count = maxCount > 0 ? Mathf.Min(maxCount, completedEvents.Count) : completedEvents.Count;

        for (int i = 0; i < count; i++)
        {
            Event evt = completedEvents[i];
            string description = !string.IsNullOrEmpty(evt.evDescription) 
                ? evt.evDescription 
                : $"事件 {evt.eventId}";
            
            // 限制描述长度
            if (description.Length > 30)
            {
                description = description.Substring(0, 30) + "...";
            }
            
            descriptions.Add(description);
        }

        return descriptions;
    }

    /// <summary>
    /// 获取已完成事件的格式化文本（用于UI显示）
    /// </summary>
    /// <param name="maxCount">最大显示数量</param>
    /// <param name="prefix">每行前缀</param>
    /// <returns>格式化的事件文本</returns>
    public string GetCompletedEventsFormattedText(int maxCount = 6, string prefix = "• ")
    {
        List<string> descriptions = GetCompletedEventDescriptions(maxCount);
        
        if (descriptions.Count == 0)
        {
            return "暂无已完成事件";
        }

        string result = "";
        for (int i = 0; i < descriptions.Count; i++)
        {
            result += prefix + descriptions[i];
            if (i < descriptions.Count - 1)
            {
                result += "\n";
            }
        }

        // 如果还有更多事件，添加提示
        List<int> allCompletedIds = GetCompletedEventIds();
        if (allCompletedIds.Count > maxCount)
        {
            result += $"\n...等共{allCompletedIds.Count}个事件";
        }

        return result;
    }

    /// <summary>
    /// 根据游戏关卡类型获取对应的库ID
    /// </summary>
    /// <param name="levelType">游戏关卡类型</param>
    /// <returns>对应的库ID</returns>
    private int GetLibIdFromGameLevel(E_GameLevelType levelType)
    {
        switch (levelType)
        {
            case E_GameLevelType.Tutorial:
                return 2001;
            case E_GameLevelType.First:
                return 2001;
            case E_GameLevelType.Second:
                return 2002;
            case E_GameLevelType.Third:
                return 2003;
            case E_GameLevelType.Central:
                return 2004;
            default:
                return 2001;
        }
    }

    /// <summary>
    /// 获取事件的详细信息（包括结果数据）
    /// </summary>
    /// <param name="eventId">事件ID</param>
    /// <returns>事件详细信息，如果不存在或未完成则返回null</returns>
    public Event GetCompletedEventDetails(int eventId)
    {
        if (!IsEventCompleted(eventId)) return null;

        if (LoadManager.Instance?.startEvents?.ContainsKey(eventId) == true)
        {
            return LoadManager.Instance.startEvents[eventId];
        }

        if (LoadManager.Instance?.optionEvents?.ContainsKey(eventId) == true)
        {
            return LoadManager.Instance.optionEvents[eventId];
        }

        return null;
    }

    /// <summary>
    /// 调试方法：打印所有已完成事件的信息
    /// </summary>
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public void DebugPrintCompletedEvents()
    {
        List<Event> completedEvents = GetCompletedEvents();
        Debug.Log($"=== 已完成事件列表 (总计: {completedEvents.Count}) ===");
        
        foreach (Event evt in completedEvents)
        {
            Debug.Log($"事件ID: {evt.eventId}, 库ID: {evt.libId}, 描述: {evt.evDescription}");
        }
        
        EventCompletionStats stats = GetEventCompletionStats();
        Debug.Log($"=== 完成统计 ===\n{stats.ToString()}");
    }

    /// <summary>
    /// Unity Update 方法 - 检测键盘输入
    /// </summary>
    private void Update()
    {
        // 按下 P 键输出事件信息
        if (Input.GetKeyDown(KeyCode.P))
        {
            TestPrintAllEventsInfo();
        }
    }

    /// <summary>
    /// 测试方法：按下P键在控制台输出所有事件和已完成事件信息
    /// </summary>
    public void TestPrintAllEventsInfo()
    {
        Debug.Log("======================================");
        Debug.Log("=== 事件信息详细输出 (按P键触发) ===");
        Debug.Log("======================================");

        if (LoadManager.Instance == null)
        {
            Debug.LogError("LoadManager.Instance 为空，无法获取事件数据！");
            return;
        }

        // 1. 输出所有起始事件信息
        Debug.Log("=== 所有起始事件 ===");
        if (LoadManager.Instance.startEvents != null && LoadManager.Instance.startEvents.Count > 0)
        {
            Debug.Log($"起始事件总数: {LoadManager.Instance.startEvents.Count}");
            foreach (var eventPair in LoadManager.Instance.startEvents)
            {
                var evt = eventPair.Value;
                string status = evt != null && evt.isTrigger ? "[已完成]" : "[未完成]";
                string desc = evt?.evDescription ?? "无描述";
                Debug.Log($"  事件ID: {eventPair.Key} | 库ID: {evt?.libId ?? 0} | 状态: {status} | 描述: {desc}");
            }
        }
        else
        {
            Debug.Log("  没有找到起始事件数据");
        }

        // 2. 输出所有选项事件信息
        Debug.Log("=== 所有选项事件 ===");
        if (LoadManager.Instance.optionEvents != null && LoadManager.Instance.optionEvents.Count > 0)
        {
            Debug.Log($"选项事件总数: {LoadManager.Instance.optionEvents.Count}");
            foreach (var eventPair in LoadManager.Instance.optionEvents)
            {
                var evt = eventPair.Value;
                string status = evt != null && evt.isTrigger ? "[已完成]" : "[未完成]";
                string desc = evt?.evDescription ?? "无描述";
                Debug.Log($"  事件ID: {eventPair.Key} | 库ID: {evt?.libId ?? 0} | 状态: {status} | 描述: {desc}");
            }
        }
        else
        {
            Debug.Log("  没有找到选项事件数据");
        }

        // 3. 输出已完成事件汇总
        Debug.Log("=== 已完成事件汇总 ===");
        List<int> completedEventIds = GetCompletedEventIds();
        List<Event> completedEvents = GetCompletedEvents();
        
        if (completedEvents.Count > 0)
        {
            Debug.Log($"已完成事件总数: {completedEvents.Count}");
            Debug.Log($"已完成事件ID列表: [{string.Join(", ", completedEventIds)}]");
            
            Debug.Log("已完成事件详细信息:");
            foreach (Event evt in completedEvents)
            {
                Debug.Log($"  ✓ 事件ID: {evt.eventId} | 库ID: {evt.libId} | 描述: {evt.evDescription ?? "无描述"}");
            }
        }
        else
        {
            Debug.Log("目前没有已完成的事件");
        }

        // 4. 输出统计信息
        Debug.Log("=== 完成统计 ===");
        EventCompletionStats stats = GetEventCompletionStats();
        Debug.Log($"总事件数: {stats.TotalEvents}");
        Debug.Log($"已完成事件数: {stats.CompletedEvents}");
        Debug.Log($"总完成率: {stats.CompletionPercentage:F1}%");
        Debug.Log($"起始事件: {stats.CompletedStartEvents}/{stats.TotalStartEvents} ({stats.StartEventCompletionPercentage:F1}%)");
        Debug.Log($"选项事件: {stats.CompletedOptionEvents}/{stats.TotalOptionEvents} ({stats.OptionEventCompletionPercentage:F1}%)");

        // 5. 输出当前关卡信息
        Debug.Log("=== 当前关卡信息 ===");
        if (GameLevelManager.Instance != null)
        {
            Debug.Log($"当前关卡: {GameLevelManager.Instance.gameLevelType}");
            List<int> currentLevelCompletedIds = GetCurrentLevelCompletedEventIds();
            Debug.Log($"当前关卡已完成事件数: {currentLevelCompletedIds.Count}");
            if (currentLevelCompletedIds.Count > 0)
            {
                Debug.Log($"当前关卡已完成事件ID: [{string.Join(", ", currentLevelCompletedIds)}]");
            }
        }
        else
        {
            Debug.Log("GameLevelManager.Instance 为空");
        }

        // 6. 输出格式化的UI文本（供参考）
        Debug.Log("=== UI显示文本预览 ===");
        string formattedText = GetCompletedEventsFormattedText();
        Debug.Log($"UI格式化文本:\n{formattedText}");

        Debug.Log("======================================");
        Debug.Log("=== 事件信息输出完成 ===");
        Debug.Log("======================================");
    }
}

/// <summary>
/// 事件完成统计信息类
/// </summary>
[System.Serializable]
public class EventCompletionStats
{
    public int TotalEvents;              // 总事件数
    public int CompletedEvents;          // 已完成事件数
    public int TotalStartEvents;         // 起始事件总数
    public int CompletedStartEvents;     // 已完成起始事件数
    public int TotalOptionEvents;        // 选项事件总数
    public int CompletedOptionEvents;    // 已完成选项事件数

    /// <summary>
    /// 获取完成率百分比
    /// </summary>
    public float CompletionPercentage
    {
        get
        {
            if (TotalEvents == 0) return 0f;
            return (float)CompletedEvents / TotalEvents * 100f;
        }
    }

    /// <summary>
    /// 获取起始事件完成率
    /// </summary>
    public float StartEventCompletionPercentage
    {
        get
        {
            if (TotalStartEvents == 0) return 0f;
            return (float)CompletedStartEvents / TotalStartEvents * 100f;
        }
    }

    /// <summary>
    /// 获取选项事件完成率
    /// </summary>
    public float OptionEventCompletionPercentage
    {
        get
        {
            if (TotalOptionEvents == 0) return 0f;
            return (float)CompletedOptionEvents / TotalOptionEvents * 100f;
        }
    }

    public override string ToString()
    {
        return $"事件完成统计: {CompletedEvents}/{TotalEvents} ({CompletionPercentage:F1}%) " +
               $"起始事件: {CompletedStartEvents}/{TotalStartEvents} " +
               $"选项事件: {CompletedOptionEvents}/{TotalOptionEvents}";
    }
}
