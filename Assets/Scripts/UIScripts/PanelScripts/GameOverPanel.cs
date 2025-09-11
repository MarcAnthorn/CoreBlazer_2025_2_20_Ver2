using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

/// <summary>
/// 精简版结算面板：仅显示 SAN 值与关闭按钮
/// 目的：极简 UI，避免其他复杂逻辑影响可见性与交互
/// </summary>
public class GameOverPanel : BasePanel
{
    [Header("SAN 显示")]
    public TextMeshProUGUI txtSANTotal;

    [Header("关闭按钮")]
    public Button btnClose;

    private float totalSANGained = 0f;
    private UnityAction onCloseCallback;


    //--------------------------必备UI组件--------------------------

    //总进度
    public TextMeshProUGUI txtProgress;

    //事件完成
    public Transform contentEvents;
    //获得的san值：
    public TextMeshProUGUI txtSanGained;
    //下一步button：
    public Button btnNextStep;

    public Image imgFirstPassed;
    public Image imgFirstNotPassed;
    public Image imgSecondPassed;
    public Image imgSecondNotPassed;
    public Image imgThirdPassed;
    public Image imgThirdNotPassed;
    public Image imgFourthPassed;
    public Image imgFourthNotPassed;



    //-------------------------------------------------------------

    protected override void Init()
    {
        // 只保留必要初始化：字体与关闭按钮绑定
        TextManager.SetContentFont(this.gameObject);

        if (btnClose != null)
        {
            btnClose.onClick.RemoveAllListeners();
            btnClose.onClick.AddListener(() => {
                // 解冻玩家（如果之前冻结过）
                if (EventHub.Instance != null)
                {
                    EventHub.Instance.EventTrigger<bool>("Freeze", false);
                }
                onCloseCallback?.Invoke();
                // 判断是否为运行时创建的面板（无 UIManager 管理）
                if (transform.parent == null || transform.parent.GetComponent<Canvas>() != null || gameObject.name == "GameOverPanel_SimpleSAN")
                {
                    Destroy(this.gameObject);
                }
                else
                {
                    UIManager.Instance?.HidePanel<GameOverPanel>();
                }
            });
        }

        UpdateSANDisplay();


    }

    /// <summary>
    /// 设置并刷新 SAN 值显示
    /// </summary>
    public void SetSAN(float sanValue, UnityAction onClose = null)
    {
        totalSANGained = sanValue;
        onCloseCallback = onClose;
        UpdateSANDisplay();
    }

    private void UpdateSANDisplay()
    {
        if (txtSANTotal != null)
        {
            // txtSANTotal.text = $"SAN：{totalSANGained:F0}";
            //Marc更改：
            txtSanGained.text = $"本轮获得San值：{totalSANGained:F0}";
        }
    }

    private void OnDestroy()
    {
        if (EventHub.Instance != null)
            EventHub.Instance.EventTrigger<bool>("Freeze", false);
    }

    /// <summary>
    /// 运行时创建一个最小化的 SAN 面板（方便在没有 prefab 的情况下快速展示）
    /// </summary>
    public void ShowSANOnlyPanel(
        float sanValue,
        UnityAction onClose = null,
        string mazeProgress = null,
        string stageProgress = null,
        string exploreTip = "探索完成")
    {
        Canvas canvas = UnityEngine.Object.FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("ShowSANOnlyPanel: 找不到 Canvas，无法创建面板。");
            onClose?.Invoke();
            return;
        }

        // 自动获取当前迷宫进度和通关进度
        if (mazeProgress == null)
        {
            mazeProgress = GetCurrentMazeProgress();
        }
        if (stageProgress == null)
        {
            stageProgress = GetOverallProgress();
        }

        // GameObject root = new GameObject("GameOverPanel_SimpleSAN");
        // root.layer = canvas.gameObject.layer;
        // var rootRt = root.AddComponent<RectTransform>();
        // root.transform.SetParent(canvas.transform, false);
        // rootRt.anchorMin = Vector2.zero;
        // rootRt.anchorMax = Vector2.one;
        // rootRt.offsetMin = Vector2.zero;
        // rootRt.offsetMax = Vector2.zero;

        // var bg = root.AddComponent<Image>();
        // bg.color = new Color(0f, 0f, 0f, 0.5f);

        // var panel = root.AddComponent<GameOverPanel>();

        // // 左上角：探索完成
        // GameObject tipGo = new GameObject("ExploreTip");
        // tipGo.transform.SetParent(root.transform, false);
        // var tipRt = tipGo.AddComponent<RectTransform>();
        // tipRt.anchorMin = new Vector2(0f, 1f);
        // tipRt.anchorMax = new Vector2(0f, 1f);
        // tipRt.pivot = new Vector2(0f, 1f);
        // tipRt.anchoredPosition = new Vector2(40, -40);
        // tipRt.sizeDelta = new Vector2(320, 60);
        // var tipText = tipGo.AddComponent<TextMeshProUGUI>();
        // tipText.text = exploreTip;
        // tipText.fontSize = 28;
        // tipText.color = new Color(1f, 0.95f, 0.5f, 1f);
        // tipText.alignment = TextAlignmentOptions.TopLeft;

        // // 左侧中部：迷宫进度
        // GameObject mazeGo = new GameObject("MazeProgress");
        // mazeGo.transform.SetParent(root.transform, false);
        // var mazeRt = mazeGo.AddComponent<RectTransform>();
        // mazeRt.anchorMin = new Vector2(0f, 0.5f);
        // mazeRt.anchorMax = new Vector2(0f, 0.5f);
        // mazeRt.pivot = new Vector2(0f, 0.5f);
        // mazeRt.anchoredPosition = new Vector2(40, 0);
        // mazeRt.sizeDelta = new Vector2(400, 120);
        // var mazeText = mazeGo.AddComponent<TextMeshProUGUI>();
        // mazeText.text = mazeProgress ?? "迷宫进度：-";
        // mazeText.fontSize = 24;
        // mazeText.color = Color.white;
        // mazeText.alignment = TextAlignmentOptions.Left;

        // // 左下角：通关进度
        // GameObject stageGo = new GameObject("StageProgress");
        // stageGo.transform.SetParent(root.transform, false);
        // var stageRt = stageGo.AddComponent<RectTransform>();
        // stageRt.anchorMin = new Vector2(0f, 0f);
        // stageRt.anchorMax = new Vector2(0f, 0f);
        // stageRt.pivot = new Vector2(0f, 0f);
        // stageRt.anchoredPosition = new Vector2(40, 40);
        // stageRt.sizeDelta = new Vector2(400, 60);
        // var stageText = stageGo.AddComponent<TextMeshProUGUI>();
        // stageText.text = stageProgress ?? "通关进度：-";
        // stageText.fontSize = 20;
        // stageText.color = new Color(0.7f, 1f, 0.7f, 1f);
        // stageText.alignment = TextAlignmentOptions.Left;

        // // 右上角：结算清单
        // GameObject settlementGo = new GameObject("SettlementList");
        // settlementGo.transform.SetParent(root.transform, false);
        // var settlementRt = settlementGo.AddComponent<RectTransform>();
        // settlementRt.anchorMin = new Vector2(1f, 1f);
        // settlementRt.anchorMax = new Vector2(1f, 1f);
        // settlementRt.pivot = new Vector2(1f, 1f);
        // settlementRt.anchoredPosition = new Vector2(-40, -40);
        // settlementRt.sizeDelta = new Vector2(400, 60);
        // var settlementText = settlementGo.AddComponent<TextMeshProUGUI>();
        // settlementText.text = "结算清单";
        // settlementText.fontSize = 28;
        // settlementText.color = new Color(1f, 0.8f, 0.3f, 1f);
        // settlementText.alignment = TextAlignmentOptions.TopRight;

        // // 右侧中部：怪谈事件列表
        // GameObject eventsGo = new GameObject("EventsList");
        // eventsGo.transform.SetParent(root.transform, false);
        // var eventsRt = eventsGo.AddComponent<RectTransform>();
        // eventsRt.anchorMin = new Vector2(1f, 0.5f);
        // eventsRt.anchorMax = new Vector2(1f, 0.5f);
        // eventsRt.pivot = new Vector2(1f, 0.5f);
        // eventsRt.anchoredPosition = new Vector2(-40, 0);
        // eventsRt.sizeDelta = new Vector2(450, 200);
        // var eventsText = eventsGo.AddComponent<TextMeshProUGUI>();
        // eventsText.text = GetCompletedEventsText();
        // eventsText.fontSize = 18;
        // eventsText.color = new Color(0.9f, 0.9f, 0.9f, 1f);
        // eventsText.alignment = TextAlignmentOptions.TopRight;

        // // 右下角：SAN值显示
        // GameObject rightSanGo = new GameObject("RightSANDisplay");
        // rightSanGo.transform.SetParent(root.transform, false);
        // var rightSanRt = rightSanGo.AddComponent<RectTransform>();
        // rightSanRt.anchorMin = new Vector2(1f, 0f);
        // rightSanRt.anchorMax = new Vector2(1f, 0f);
        // rightSanRt.pivot = new Vector2(1f, 0f);
        // rightSanRt.anchoredPosition = new Vector2(-40, 40);
        // rightSanRt.sizeDelta = new Vector2(300, 80);
        // var rightSanText = rightSanGo.AddComponent<TextMeshProUGUI>();
        // rightSanText.text = $"当前SAN：{sanValue:F0}";
        // rightSanText.fontSize = 24;
        // rightSanText.color = new Color(0.5f, 1f, 0.8f, 1f);
        // rightSanText.alignment = TextAlignmentOptions.BottomRight;

        // // 内容容器（中间 关闭按钮，移除SAN显示，因为右下角已经有了）
        // GameObject content = new GameObject("Content");
        // content.transform.SetParent(root.transform, false);
        // var contentRt = content.AddComponent<RectTransform>();
        // contentRt.sizeDelta = new Vector2(200, 100);
        // contentRt.anchorMin = contentRt.anchorMax = new Vector2(0.5f, 0.5f);
        // contentRt.anchoredPosition = Vector2.zero;
        // var contentBg = content.AddComponent<Image>();
        // contentBg.color = new Color(0.06f, 0.06f, 0.06f, 0.95f);

        // // 关闭按钮
        // GameObject btnGo = new GameObject("CloseButton");
        // btnGo.transform.SetParent(content.transform, false);
        // var btnRt = btnGo.AddComponent<RectTransform>();
        // btnRt.sizeDelta = new Vector2(160, 44);
        // btnRt.anchorMin = btnRt.anchorMax = new Vector2(0.5f, 0.5f);
        // btnRt.anchoredPosition = Vector2.zero;
        // var button = btnGo.AddComponent<Button>();
        // var btnImg = btnGo.AddComponent<Image>();
        // btnImg.color = new Color(0.18f, 0.55f, 0.18f, 1f);

        // GameObject btnText = new GameObject("Text");
        // btnText.transform.SetParent(btnGo.transform, false);
        // var btnTextRt = btnText.AddComponent<RectTransform>();
        // btnTextRt.anchorMin = btnTextRt.anchorMax = new Vector2(0.5f, 0.5f);
        // btnTextRt.anchoredPosition = Vector2.zero;
        // var btnTmp = btnText.AddComponent<TextMeshProUGUI>();
        // btnTmp.text = "关闭";
        // btnTmp.alignment = TextAlignmentOptions.Center;
        // btnTmp.fontSize = 20;
        // btnTmp.color = Color.white;

        // // 赋值并显示
        // panel.txtSANTotal = rightSanText; // 使用右下角的SAN文本
        // panel.btnClose = button;
        // panel.SetSAN(sanValue, onClose);
        // panel.ShowMe();
    }

    /// <summary>
    /// 获取已完成的怪谈事件文本
    /// 使用SaveManager的详细数据显示交互对象和SAN奖励信息
    /// </summary>
    private string GetCompletedEventsText()
    {
        try
        {
            var resultText = new System.Text.StringBuilder();
            
            // 1. 显示已完成的怪谈事件 - 紧凑布局
            resultText.AppendLine("📜 怪谈事件:");
            
            var completedEvents = new List<string>();
            
            // 通过 LoadManager 获取已完成事件
            if (LoadManager.Instance != null)
            {
                // 从 startEvents 获取已触发的事件
                if (LoadManager.Instance.startEvents != null)
                {
                    foreach (var eventPair in LoadManager.Instance.startEvents)
                    {
                        var evt = eventPair.Value;
                        if (evt != null && evt.isTrigger)
                        {
                            string eventDesc = !string.IsNullOrEmpty(evt.evDescription) ? 
                                evt.evDescription : $"事件 {evt.eventId}";
                            
                            if (eventDesc.Length > 15)
                                eventDesc = eventDesc.Substring(0, 15) + "...";
                            
                            completedEvents.Add(eventDesc);
                        }
                    }
                }
                
                // 从 optionEvents 获取已触发的事件
                if (LoadManager.Instance.optionEvents != null)
                {
                    foreach (var eventPair in LoadManager.Instance.optionEvents)
                    {
                        var evt = eventPair.Value;
                        if (evt != null && evt.isTrigger)
                        {
                            string eventDesc = !string.IsNullOrEmpty(evt.evDescription) ? 
                                evt.evDescription : $"事件 {evt.eventId}";
                            
                            if (eventDesc.Length > 15)
                                eventDesc = eventDesc.Substring(0, 15) + "...";
                            
                            completedEvents.Add(eventDesc);
                        }
                    }
                }
            }
            
            // 显示事件列表（最多2个，紧凑显示）
            if (completedEvents.Count > 0)
            {
                for (int i = 0; i < Math.Min(completedEvents.Count, 2); i++)
                {
                    resultText.AppendLine($"• {completedEvents[i]}");
                }
                if (completedEvents.Count > 2)
                    resultText.AppendLine($"• 等{completedEvents.Count}个事件");
            }
            else
            {
                resultText.AppendLine("• 暂无事件");
            }
            
            // 2. 显示交互对象统计 - 单行紧凑显示
            if (SaveManager.Instance != null)
            {
                try
                {
                    // 获取统计数据并紧凑显示
                    if (GameLevelManager.Instance != null)
                    {
                        var glm = GameLevelManager.Instance;
                        int restPoints = 0, lightHouses = 0, keyPoints = 0, itemPoints = 0;
                        
                        // 统计激活的交互点
                        foreach (var kv in glm.restPointDic)
                            if (kv.Value) restPoints++;
                        foreach (var kv in glm.lightHouseIsDic)
                            if (kv.Value) lightHouses++;
                        foreach (var kv in glm.keyPointDic)
                            if (kv.Value) keyPoints++;
                        foreach (var kv in glm.itemPointDic)
                            if (kv.Value) itemPoints++;
                        
                        // 紧凑显示交互统计
                        resultText.AppendLine("🔍 交互统计:");
                        resultText.AppendLine($"休息{restPoints} 灯塔{lightHouses} 关键{keyPoints} 道具{itemPoints}");
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogWarning($"获取交互统计失败: {ex.Message}");
                    resultText.AppendLine("交互数据获取失败");
                }
            }
            
            // 3. 显示SAN奖励信息 - 紧凑显示
            if (SaveManager.Instance != null)
            {
                try
                {
                    // 尝试获取当前玩家SAN值
                    if (PlayerManager.Instance?.player != null)
                    {
                        float currentSan = PlayerManager.Instance.player.SAN.value;
                        float maxSan = PlayerManager.Instance.player.SAN.value_limit;
                        resultText.AppendLine($"💰 SAN: {currentSan:F0}/{maxSan:F0}");
                        
                        // 计算预估SAN奖励
                        int estimatedSanReward = 0;
                        if (GameLevelManager.Instance != null)
                        {
                            var glm = GameLevelManager.Instance;
                            
                            // 根据激活的交互点估算SAN奖励
                            foreach (var kv in glm.restPointDic)
                                if (kv.Value) estimatedSanReward += 2; // 休息点2 SAN
                            foreach (var kv in glm.lightHouseIsDic)
                                if (kv.Value) estimatedSanReward += 1; // 灯塔1 SAN
                            foreach (var kv in glm.itemPointDic)
                                if (kv.Value) estimatedSanReward += 1; // 道具点1 SAN
                            
                            if (estimatedSanReward > 0)
                            {
                                resultText.AppendLine($"预估奖励: +{estimatedSanReward}");
                            }
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogWarning($"获取SAN奖励信息失败: {ex.Message}");
                    resultText.AppendLine("SAN信息获取失败");
                }
            }
            
            return resultText.ToString().TrimEnd('\n');
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"GetCompletedEventsText 执行失败: {ex.Message}");
            return "数据获取失败";
        }
    }
    
    /// <summary>
    /// 获取交互对象统计信息
    /// </summary>
    private string GetInteractionStatistics()
    {
        if (GameLevelManager.Instance == null) return null;
        
        var glm = GameLevelManager.Instance;
        var stats = new System.Text.StringBuilder();
        
        // 统计各类交互点
        int restPoints = 0, lightHouses = 0, keyPoints = 0, itemPoints = 0;
        
        foreach (var kv in glm.restPointDic)
            if (kv.Value) restPoints++;
        foreach (var kv in glm.lightHouseIsDic)
            if (kv.Value) lightHouses++;
        foreach (var kv in glm.keyPointDic)
            if (kv.Value) keyPoints++;
        foreach (var kv in glm.itemPointDic)
            if (kv.Value) itemPoints++;
        
        stats.AppendLine($"• 休息点: {restPoints} 个 (+{restPoints * 2} SAN)");
        stats.AppendLine($"• 灯塔: {lightHouses} 个 (+{lightHouses * 1} SAN)");
        stats.AppendLine($"• 关键点: {keyPoints} 个");
        stats.AppendLine($"• 道具点: {itemPoints} 个 (+{itemPoints * 1} SAN)");
        
        return stats.ToString().TrimEnd('\n');
    }
    
    /// <summary>
    /// 获取SAN奖励信息
    /// </summary>
    private string GetSanRewardInfo()
    {
        // 这里需要访问SaveManager的复活SAN数据
        // 由于SaveManager的LoadReviveSanData是private方法，我们需要其他方式获取信息
        
        var sanInfo = new System.Text.StringBuilder();
        
        // 尝试获取当前玩家SAN值
        if (PlayerManager.Instance?.player != null)
        {
            float currentSan = PlayerManager.Instance.player.SAN.value;
            float maxSan = PlayerManager.Instance.player.SAN.value_limit;
            sanInfo.AppendLine($"• 当前SAN: {currentSan:F0}/{maxSan:F0}");
        }
        
        // 计算本次可能的SAN奖励
        int estimatedSanReward = 0;
        if (GameLevelManager.Instance != null)
        {
            var glm = GameLevelManager.Instance;
            
            // 根据激活的交互点估算SAN奖励
            foreach (var kv in glm.restPointDic)
                if (kv.Value) estimatedSanReward += 2; // 休息点2 SAN
            foreach (var kv in glm.lightHouseIsDic)
                if (kv.Value) estimatedSanReward += 1; // 灯塔1 SAN
            foreach (var kv in glm.itemPointDic)
                if (kv.Value) estimatedSanReward += 1; // 道具点1 SAN
            
            if (estimatedSanReward > 0)
            {
                sanInfo.AppendLine($"• 预估本次奖励: +{estimatedSanReward} SAN");
            }
        }
        
        return sanInfo.ToString().TrimEnd('\n');
    }
        
    /// <summary>
    /// 显示复活SAN奖励面板 - 集成SaveManager的详细数据
    /// 这个方法专门用于复活时显示详细的SAN奖励和游戏进度信息
    /// </summary>
    /// <param name="sanReward">本次获得的SAN奖励</param>
    /// <param name="onClose">关闭回调</param>
    public void ShowReviveSanRewardPanel(int sanReward, UnityAction onClose = null)
    {
        Canvas canvas = UnityEngine.Object.FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("ShowReviveSanRewardPanel: 找不到 Canvas，无法创建面板。");
            onClose?.Invoke();
            return;
        }

        Debug.Log($"🎉 创建复活SAN奖励面板，本次奖励: {sanReward} SAN");

        GameObject root = new GameObject("GameOverPanel_ReviveSanReward");
        root.layer = canvas.gameObject.layer;
        var rootRt = root.AddComponent<RectTransform>();
        root.transform.SetParent(canvas.transform, false);
        rootRt.anchorMin = Vector2.zero;
        rootRt.anchorMax = Vector2.one;
        rootRt.offsetMin = Vector2.zero;
        rootRt.offsetMax = Vector2.zero;

        var bg = root.AddComponent<Image>();
        bg.color = new Color(0f, 0f, 0f, 0.7f);

        var panel = root.AddComponent<GameOverPanel>();

        // === 标题区域 ===
        GameObject titleGo = new GameObject("Title");
        titleGo.transform.SetParent(root.transform, false);
        var titleRt = titleGo.AddComponent<RectTransform>();
        titleRt.anchorMin = new Vector2(0.25f, 0.85f); // 在中心区域的顶部
        titleRt.anchorMax = new Vector2(0.75f, 0.95f); // 宽度50%，高度10%
        titleRt.offsetMin = Vector2.zero;
        titleRt.offsetMax = Vector2.zero;
        var titleText = titleGo.AddComponent<TextMeshProUGUI>();
        titleText.text = "复活奖励结算";
        titleText.fontSize = 36;
        titleText.color = new Color(1f, 0.8f, 0.2f, 1f);
        titleText.alignment = TextAlignmentOptions.Center;

        // === 左侧：游戏进度信息 ===
        GameObject leftPanel = new GameObject("LeftPanel");
        leftPanel.transform.SetParent(root.transform, false);
        var leftRt = leftPanel.AddComponent<RectTransform>();
        leftRt.anchorMin = new Vector2(0.25f, 0.2f);  // 左边距25%
        leftRt.anchorMax = new Vector2(0.495f, 0.8f); // 左半部分，上下距20%
        leftRt.offsetMin = Vector2.zero;
        leftRt.offsetMax = Vector2.zero;
        var leftBg = leftPanel.AddComponent<Image>();
        leftBg.color = new Color(0.1f, 0.1f, 0.2f, 0.8f);

        GameObject leftContent = new GameObject("LeftContent");
        leftContent.transform.SetParent(leftPanel.transform, false);
        var leftContentRt = leftContent.AddComponent<RectTransform>();
        leftContentRt.anchorMin = Vector2.zero;
        leftContentRt.anchorMax = Vector2.one;
        leftContentRt.offsetMin = new Vector2(15, 15);
        leftContentRt.offsetMax = new Vector2(-15, -15);
        var leftContentText = leftContent.AddComponent<TextMeshProUGUI>();
        leftContentText.text = GetDetailedGameProgress();
        leftContentText.fontSize = 18;
        leftContentText.color = Color.white;
        leftContentText.alignment = TextAlignmentOptions.TopLeft;

        // === 右侧：SAN奖励详情 ===
        GameObject rightPanel = new GameObject("RightPanel");
        rightPanel.transform.SetParent(root.transform, false);
        var rightRt = rightPanel.AddComponent<RectTransform>();
        rightRt.anchorMin = new Vector2(0.505f, 0.2f); // 右半部分
        rightRt.anchorMax = new Vector2(0.75f, 0.8f);  // 右边距25%，上下距20%
        rightRt.offsetMin = Vector2.zero;
        rightRt.offsetMax = Vector2.zero;
        var rightBg = rightPanel.AddComponent<Image>();
        rightBg.color = new Color(0.1f, 0.2f, 0.1f, 0.8f);

        GameObject rightContent = new GameObject("RightContent");
        rightContent.transform.SetParent(rightPanel.transform, false);
        var rightContentRt = rightContent.AddComponent<RectTransform>();
        rightContentRt.anchorMin = Vector2.zero;
        rightContentRt.anchorMax = Vector2.one;
        rightContentRt.offsetMin = new Vector2(15, 15);
        rightContentRt.offsetMax = new Vector2(-15, -15);
        var rightContentText = rightContent.AddComponent<TextMeshProUGUI>();
        rightContentText.text = GetDetailedSanRewardInfo(sanReward);
        rightContentText.fontSize = 18;
        rightContentText.color = new Color(0.8f, 1f, 0.8f, 1f);
        rightContentText.alignment = TextAlignmentOptions.TopLeft;

        // === 底部按钮区域 ===
        GameObject bottomPanel = new GameObject("BottomPanel");
        bottomPanel.transform.SetParent(root.transform, false);
        var bottomRt = bottomPanel.AddComponent<RectTransform>();
        bottomRt.anchorMin = new Vector2(0.4f, 0.1f);  // 居中按钮，底部距10%
        bottomRt.anchorMax = new Vector2(0.6f, 0.18f); // 按钮宽度20%
        bottomRt.offsetMin = Vector2.zero;
        bottomRt.offsetMax = Vector2.zero;

        GameObject btnGo = new GameObject("CloseButton");
        btnGo.transform.SetParent(bottomPanel.transform, false);
        var btnRt = btnGo.AddComponent<RectTransform>();
        btnRt.anchorMin = Vector2.zero;
        btnRt.anchorMax = Vector2.one;
        btnRt.offsetMin = Vector2.zero;
        btnRt.offsetMax = Vector2.zero;
        var button = btnGo.AddComponent<Button>();
        var btnImg = btnGo.AddComponent<Image>();
        btnImg.color = new Color(0.2f, 0.6f, 0.2f, 1f);

        GameObject btnText = new GameObject("Text");
        btnText.transform.SetParent(btnGo.transform, false);
        var btnTextRt = btnText.AddComponent<RectTransform>();
        btnTextRt.anchorMin = Vector2.zero;
        btnTextRt.anchorMax = Vector2.one;
        btnTextRt.offsetMin = Vector2.zero;
        btnTextRt.offsetMax = Vector2.zero;
        var btnTmp = btnText.AddComponent<TextMeshProUGUI>();
        btnTmp.text = "确认 - 继续游戏";
        btnTmp.alignment = TextAlignmentOptions.Center;
        btnTmp.fontSize = 24;
        btnTmp.color = Color.white;

        // 赋值并显示
        panel.txtSANTotal = rightContentText; // 复用文本组件
        panel.btnClose = button;
        panel.SetSAN(sanReward, onClose);
        panel.ShowMe();

        Debug.Log("复活SAN奖励面板创建完成");
    }

    /// <summary>
    /// 获取详细的游戏进度信息 - 紧凑布局
    /// </summary>
    private string GetDetailedGameProgress()
    {
        var progress = new System.Text.StringBuilder();
        
        progress.AppendLine("游戏进度");
        progress.AppendLine("==============");
        
        // 当前关卡信息 - 单行显示
        if (GameLevelManager.Instance != null)
        {
            var currentLevel = GameLevelManager.Instance.gameLevelType;
            string levelName = GetLevelName(currentLevel);
            
            // 通关进度
            int currentLevelIndex = (int)currentLevel;
            int totalLevels = 5;
            float progressPercent = (float)currentLevelIndex / (totalLevels - 1) * 100f;
            
            progress.AppendLine($"{levelName} ({progressPercent:F1}%)");
        }
        
        // 交互对象统计 - 紧凑显示
        if (GameLevelManager.Instance != null)
        {
            var glm = GameLevelManager.Instance;
            int restPoints = 0, lightHouses = 0, keyPoints = 0, itemPoints = 0;
            
            foreach (var kv in glm.restPointDic)
                if (kv.Value) restPoints++;
            foreach (var kv in glm.lightHouseIsDic)
                if (kv.Value) lightHouses++;
            foreach (var kv in glm.keyPointDic)
                if (kv.Value) keyPoints++;
            foreach (var kv in glm.itemPointDic)
                if (kv.Value) itemPoints++;
            
            progress.AppendLine("� 交互统计:");
            progress.AppendLine($"{restPoints} {lightHouses} {keyPoints} {itemPoints}");
        }
        
        // 已完成事件 - 单行显示
        var completedEventCount = 0;
        if (LoadManager.Instance != null)
        {
            if (LoadManager.Instance.startEvents != null)
            {
                foreach (var evt in LoadManager.Instance.startEvents.Values)
                {
                    if (evt != null && evt.isTrigger) completedEventCount++;
                }
            }
            if (LoadManager.Instance.optionEvents != null)
            {
                foreach (var evt in LoadManager.Instance.optionEvents.Values)
                {
                    if (evt != null && evt.isTrigger) completedEventCount++;
                }
            }
        }
        progress.AppendLine($"� 完成事件: {completedEventCount} 个");
        
        return progress.ToString();
    }

    /// <summary>
    /// 获取详细的SAN奖励信息 - 紧凑布局
    /// </summary>
    private string GetDetailedSanRewardInfo(int sanReward)
    {
        var sanInfo = new System.Text.StringBuilder();
        
        sanInfo.AppendLine("SAN奖励");
        sanInfo.AppendLine("==============");
        
        // 当前SAN状态 - 紧凑显示
        if (PlayerManager.Instance?.player != null)
        {
            float currentSan = PlayerManager.Instance.player.SAN.value;
            float maxSan = PlayerManager.Instance.player.SAN.value_limit;
            float newSan = Mathf.Min(currentSan + sanReward, maxSan);
            
            sanInfo.AppendLine($"{currentSan:F0} → {newSan:F0}/{maxSan:F0}");
            sanInfo.AppendLine($"   本次: +{sanReward}");
            
            if (newSan >= maxSan)
            {
                sanInfo.AppendLine("已达上限");
            }
        }
        
        // 奖励来源分析 - 紧凑显示
        if (GameLevelManager.Instance != null && SaveManager.Instance != null)
        {
            var glm = GameLevelManager.Instance;
            
            // 分别统计各类交互点的SAN贡献
            int restPointSan = 0, lightHouseSan = 0, itemPointSan = 0;
            
            foreach (var kv in glm.restPointDic)
                if (kv.Value) restPointSan += 2; // 休息点2 SAN
            foreach (var kv in glm.lightHouseIsDic)
                if (kv.Value) lightHouseSan += 1; // 灯塔1 SAN
            foreach (var kv in glm.itemPointDic)
                if (kv.Value) itemPointSan += 1; // 道具点1 SAN
            
            sanInfo.AppendLine("奖励来源:");
            if (restPointSan > 0)
                sanInfo.AppendLine($"🛏️ +{restPointSan}");
            if (lightHouseSan > 0)
                sanInfo.AppendLine($"🗼 +{lightHouseSan}");
            if (itemPointSan > 0)
                sanInfo.AppendLine($"📦 +{itemPointSan}");
        }
        
        // 简化的历史信息
        sanInfo.AppendLine("本次复活获得:");
        sanInfo.AppendLine($"   {sanReward} SAN");
        
        return sanInfo.ToString();
    }

    /// <summary>
    /// 获取当前迷宫进度信息
    /// </summary>
    private string GetCurrentMazeProgress()
    {
        if (GameLevelManager.Instance == null)
            return "迷宫进度：无法获取";

        var currentLevel = GameLevelManager.Instance.gameLevelType;
        string levelName = GetLevelName(currentLevel);
        
        return $"当前迷宫：{levelName}";
    }

    /// <summary>
    /// 获取总体通关进度
    /// </summary>
    private string GetOverallProgress()
    {
        if (GameLevelManager.Instance == null)
            return "通关进度：无法获取";

        var currentLevel = GameLevelManager.Instance.gameLevelType;
        int currentLevelIndex = (int)currentLevel;
        int totalLevels = 5; // Tutorial, First, Second, Third, Central
        
        // 计算通关进度百分比：当前关卡索引 / 总关卡数 * 100%
        float progressPercent = (float)currentLevelIndex / (totalLevels - 1) * 100f;

        //设置显示UI：Marc添加：
        txtProgress.text = $"{(float)currentLevelIndex / (totalLevels - 1) * 100f}%";
        
        return $"通关进度：{progressPercent:F1}% ({currentLevelIndex}/{totalLevels - 1})";
    }

    /// <summary>
    /// 根据关卡类型获取关卡名称
    /// </summary>
    private string GetLevelName(E_GameLevelType levelType)
    {
        switch (levelType)
        {
            case E_GameLevelType.Tutorial:
                return "新手教程";
            case E_GameLevelType.First:
                return "第一层";
            case E_GameLevelType.Second:
                return "第二层";
            case E_GameLevelType.Third:
                return "第三层";
            case E_GameLevelType.Central:
                return "中央区域";
            default:
                return "未知区域";
        }
    }
}
