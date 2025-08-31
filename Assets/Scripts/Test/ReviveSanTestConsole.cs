using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 复活SAN系统测试控制台UI
/// 提供可视化的测试界面
/// </summary>
public class ReviveSanTestConsole : MonoBehaviour
{
    [Header("UI组件")]
    [SerializeField] private GameObject consolePanel;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private TextMeshProUGUI logText;
    [SerializeField] private Button calculateSanButton;
    [SerializeField] private Button clearDataButton;
    [SerializeField] private Button addInteractionButton;
    [SerializeField] private Button showDetailsButton;
    [SerializeField] private Button toggleConsoleButton;
    [SerializeField] private Button generateRandomButton;

    [Header("设置")]
    [SerializeField] private KeyCode toggleConsoleKey = KeyCode.F8;
    [SerializeField] private int maxLogLines = 20;

    private bool consoleVisible = false;
    private System.Text.StringBuilder logBuilder = new System.Text.StringBuilder();

    private void Start()
    {
        InitializeUI();
        SetupButtons();
        UpdateStatus();
        
        // 初始隐藏控制台
        SetConsoleVisibility(false);
    }

    private void Update()
    {
        // F8键切换控制台显示
        if (Input.GetKeyDown(toggleConsoleKey))
        {
            ToggleConsole();
        }

        // 定期更新状态
        if (Time.frameCount % 60 == 0) // 每60帧更新一次
        {
            UpdateStatus();
        }
    }

    private void InitializeUI()
    {
        if (consolePanel == null)
        {
            Debug.LogWarning("ReviveSanTestConsole: consolePanel 未设置");
            return;
        }

        // 确保文本组件存在
        if (statusText == null)
        {
            statusText = consolePanel.GetComponentInChildren<TextMeshProUGUI>();
        }

        if (logText == null)
        {
            // 查找名为"LogText"的组件
            var logTextObj = GameObject.Find("LogText");
            if (logTextObj != null)
            {
                logText = logTextObj.GetComponent<TextMeshProUGUI>();
            }
        }
    }

    private void SetupButtons()
    {
        if (calculateSanButton != null)
        {
            calculateSanButton.onClick.AddListener(() => {
                TestCalculateReviveSan();
                LogMessage("执行了计算复活SAN奖励");
            });
        }

        if (clearDataButton != null)
        {
            clearDataButton.onClick.AddListener(() => {
                TestClearReviveSanData();
                LogMessage("清空了复活SAN数据");
            });
        }

        if (addInteractionButton != null)
        {
            addInteractionButton.onClick.AddListener(() => {
                TestAddInteraction();
                LogMessage("添加了测试交互对象");
            });
        }

        if (showDetailsButton != null)
        {
            showDetailsButton.onClick.AddListener(() => {
                TestShowSanDetails();
                LogMessage("显示了复活SAN详情");
            });
        }

        if (toggleConsoleButton != null)
        {
            toggleConsoleButton.onClick.AddListener(ToggleConsole);
        }

        if (generateRandomButton != null)
        {
            generateRandomButton.onClick.AddListener(() => {
                GenerateRandomTestInteractions();
                LogMessage("生成了随机测试交互");
            });
        }
    }

    private void ToggleConsole()
    {
        consoleVisible = !consoleVisible;
        SetConsoleVisibility(consoleVisible);
        LogMessage($"控制台 {(consoleVisible ? "显示" : "隐藏")}");
    }

    private void SetConsoleVisibility(bool visible)
    {
        if (consolePanel != null)
        {
            consolePanel.SetActive(visible);
        }
        consoleVisible = visible;
    }

    private void UpdateStatus()
    {
        if (statusText == null) return;

        var status = new System.Text.StringBuilder();
        status.AppendLine("=== 复活SAN系统状态 ===");

        // 玩家SAN状态
        if (PlayerManager.Instance?.player != null)
        {
            var player = PlayerManager.Instance.player;
            status.AppendLine($"当前SAN: {player.SAN.value:F1}/{player.SAN.value_limit:F1}");
        }
        else
        {
            status.AppendLine("玩家数据: 未找到");
        }

        // 关卡信息
        if (GameLevelManager.Instance != null)
        {
            var glm = GameLevelManager.Instance;
            status.AppendLine($"当前关卡: {glm.gameLevelType}");
            
            // 统计激活的交互对象
            int activeLighthouses = 0;
            int activeRestPoints = 0;
            int activeKeyPoints = 0;
            int activeItemPoints = 0;

            if (glm.lightHouseIsDic != null)
            {
                foreach (var lighthouse in glm.lightHouseIsDic.Values)
                {
                    if (lighthouse) activeLighthouses++;
                }
            }
            if (glm.restPointDic != null)
            {
                foreach (var restPoint in glm.restPointDic.Values)
                {
                    if (restPoint) activeRestPoints++;
                }
            }
            if (glm.keyPointDic != null)
            {
                foreach (var keyPoint in glm.keyPointDic.Values)
                {
                    if (keyPoint) activeKeyPoints++;
                }
            }
            if (glm.itemPointDic != null)
            {
                foreach (var itemPoint in glm.itemPointDic.Values)
                {
                    if (itemPoint) activeItemPoints++;
                }
            }

            status.AppendLine($"激活的灯塔: {activeLighthouses}");
            status.AppendLine($"激活的休息点: {activeRestPoints}");
            status.AppendLine($"激活的关键点: {activeKeyPoints}");
            status.AppendLine($"激活的道具点: {activeItemPoints}");
        }
        else
        {
            status.AppendLine("关卡管理器: 未找到");
        }

        // SaveManager状态
        if (SaveManager.Instance != null)
        {
            status.AppendLine("SaveManager: 可用");
        }
        else
        {
            status.AppendLine("SaveManager: 未找到");
        }

        status.AppendLine($"\n按键说明:");
        status.AppendLine($"F8: 切换控制台");
        status.AppendLine($"F9-F12: 快捷测试功能");

        statusText.text = status.ToString();
    }

    private void LogMessage(string message)
    {
        var timestamp = System.DateTime.Now.ToString("HH:mm:ss");
        var logEntry = $"[{timestamp}] {message}";
        
        logBuilder.AppendLine(logEntry);
        
        // 限制日志行数
        var lines = logBuilder.ToString().Split('\n');
        if (lines.Length > maxLogLines)
        {
            logBuilder.Clear();
            for (int i = lines.Length - maxLogLines; i < lines.Length; i++)
            {
                if (!string.IsNullOrEmpty(lines[i]))
                {
                    logBuilder.AppendLine(lines[i]);
                }
            }
        }

        if (logText != null)
        {
            logText.text = logBuilder.ToString();
        }

        Debug.Log(logEntry);
    }

    // 测试方法（与ReviveSanTester中相同的逻辑）
    private void TestCalculateReviveSan()
    {
        if (SaveManager.Instance != null)
        {
            int sanReward = SaveManager.Instance.CalculateAndAwardReviveSan();
            LogMessage($"本次获得SAN奖励: {sanReward}");
        }
        else
        {
            LogMessage("错误: SaveManager.Instance 为空");
        }
    }

    private void TestClearReviveSanData()
    {
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.ClearReviveSanData();
            LogMessage("复活SAN数据已清空");
        }
        else
        {
            LogMessage("错误: SaveManager.Instance 为空");
        }
    }

    private void TestAddInteraction()
    {
        if (GameLevelManager.Instance != null)
        {
            var glm = GameLevelManager.Instance;
            var testPosition = new Vector3(
                UnityEngine.Random.Range(-100f, 100f),
                0f,
                UnityEngine.Random.Range(-100f, 100f)
            );
            
            var testKey = (glm.gameLevelType, testPosition);
            glm.lightHouseIsDic[testKey] = true;
            
            LogMessage($"添加测试灯塔 在位置 {testPosition}");
        }
        else
        {
            LogMessage("错误: GameLevelManager.Instance 为空");
        }
    }

    private void TestShowSanDetails()
    {
        if (SaveManager.Instance != null)
        {
            // 使用反射调用私有方法
            var method = typeof(SaveManager).GetMethod("ShowReviveSanDetails", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (method != null)
            {
                method.Invoke(SaveManager.Instance, null);
                LogMessage("显示复活SAN详情完成");
            }
            else
            {
                LogMessage("警告: 无法找到 ShowReviveSanDetails 方法");
            }
        }
        else
        {
            LogMessage("错误: SaveManager.Instance 为空");
        }
    }

    private void GenerateRandomTestInteractions()
    {
        if (GameLevelManager.Instance == null)
        {
            LogMessage("错误: GameLevelManager.Instance 为空");
            return;
        }

        var glm = GameLevelManager.Instance;
        int count = UnityEngine.Random.Range(3, 8);
        
        for (int i = 0; i < count; i++)
        {
            var randomPos = new Vector3(
                UnityEngine.Random.Range(-500f, 500f),
                0f,
                UnityEngine.Random.Range(-500f, 500f)
            );
            
            var key = (glm.gameLevelType, randomPos);
            
            // 随机选择交互类型
            int interactionType = UnityEngine.Random.Range(0, 4);
            switch (interactionType)
            {
                case 0:
                    glm.lightHouseIsDic[key] = true;
                    break;
                case 1:
                    glm.restPointDic[key] = true;
                    break;
                case 2:
                    glm.keyPointDic[key] = true;
                    break;
                case 3:
                    glm.itemPointDic[key] = true;
                    break;
            }
        }
        
        LogMessage($"生成了 {count} 个随机测试交互对象");
    }

    // 公共方法供外部调用
    public void ShowConsole()
    {
        SetConsoleVisibility(true);
    }

    public void HideConsole()
    {
        SetConsoleVisibility(false);
    }

    // 在GUI中显示简化信息（当UI组件不可用时）
    private void OnGUI()
    {
        if (consolePanel != null && consolePanel.activeInHierarchy) return; // UI可用时不显示GUI

        GUILayout.BeginArea(new Rect(10, 10, 300, 150));
        GUILayout.Label("复活SAN测试控制台 (简化模式)", GUI.skin.box);
        GUILayout.Label($"按 {toggleConsoleKey} 切换完整控制台");
        
        if (PlayerManager.Instance?.player != null)
        {
            var player = PlayerManager.Instance.player;
            GUILayout.Label($"SAN: {player.SAN.value:F1}/{player.SAN.value_limit:F1}");
        }
        
        if (GUILayout.Button("计算SAN奖励"))
        {
            TestCalculateReviveSan();
        }
        
        if (GUILayout.Button("清空SAN数据"))
        {
            TestClearReviveSanData();
        }
        
        GUILayout.EndArea();
    }
}
