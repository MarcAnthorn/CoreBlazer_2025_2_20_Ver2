using UnityEngine;

/// <summary>
/// 复活SAN系统测试场景管理器
/// 自动配置测试环境和组件
/// </summary>
public class ReviveSanTestScene : MonoBehaviour
{
    [Header("测试设置")]
    [SerializeField] private bool autoSetupOnStart = true;
    [SerializeField] private bool enableTestConsole = true;
    [SerializeField] private bool enableKeyboardTester = true;
    
    [Header("测试数据")]
    [SerializeField] private int initialTestSAN = 50;
    [SerializeField] private int testInteractionCount = 5;

    private ReviveSanTester keyboardTester;
    // private ReviveSanTestConsole consoleUI; // Commented out - class not found

    private void Start()
    {
        if (autoSetupOnStart)
        {
            SetupTestEnvironment();
        }
    }

    /// <summary>
    /// 配置测试环境
    /// </summary>
    public void SetupTestEnvironment()
    {
        Debug.Log("=== 配置复活SAN测试环境 ===");

        // 1. 设置测试组件
        SetupTestComponents();

        // 2. 初始化玩家SAN（如果需要）
        InitializePlayerSAN();

        // 3. 生成一些测试交互对象
        GenerateTestInteractions();

        // 4. 显示环境信息
        ShowEnvironmentInfo();

        Debug.Log("复活SAN测试环境配置完成！");
    }

    /// <summary>
    /// 设置测试组件
    /// </summary>
    private void SetupTestComponents()
    {
        // 添加键盘测试器
        if (enableKeyboardTester && keyboardTester == null)
        {
            keyboardTester = gameObject.GetComponent<ReviveSanTester>();
            if (keyboardTester == null)
            {
                keyboardTester = gameObject.AddComponent<ReviveSanTester>();
            }
            Debug.Log("✓ 键盘测试器已配置");
        }

        // 添加控制台UI（如果有UI Canvas）
        // Commented out - ReviveSanTestConsole class not found
        /*
        if (enableTestConsole && consoleUI == null)
        {
            var canvas = FindObjectOfType<Canvas>();
            if (canvas != null)
            {
                consoleUI = canvas.gameObject.GetComponent<ReviveSanTestConsole>();
                if (consoleUI == null)
                {
                    consoleUI = canvas.gameObject.AddComponent<ReviveSanTestConsole>();
                }
                Debug.Log("✓ 测试控制台UI已配置");
            }
            else
            {
                Debug.Log("! 未找到Canvas，跳过UI控制台配置");
            }
        }
        */
    }

    /// <summary>
    /// 初始化玩家SAN值（用于测试）
    /// </summary>
    private void InitializePlayerSAN()
    {
        if (PlayerManager.Instance?.player != null)
        {
            var player = PlayerManager.Instance.player;
            if (player.SAN.value <= 0)
            {
                player.SAN.SetValue(initialTestSAN);
                Debug.Log($"✓ 初始化玩家SAN为: {initialTestSAN}");
            }
            else
            {
                Debug.Log($"✓ 玩家当前SAN: {player.SAN.value:F1}");
            }
        }
        else
        {
            Debug.Log("! 未找到玩家对象，跳过SAN初始化");
        }
    }

    /// <summary>
    /// 生成测试交互对象
    /// </summary>
    private void GenerateTestInteractions()
    {
        if (GameLevelManager.Instance == null)
        {
            Debug.Log("! GameLevelManager.Instance 为空，跳过测试交互生成");
            return;
        }

        var glm = GameLevelManager.Instance;
        var random = new System.Random(System.DateTime.Now.Millisecond);

        Debug.Log($"生成 {testInteractionCount} 个测试交互对象...");

        for (int i = 0; i < testInteractionCount; i++)
        {
            var position = new Vector3(
                random.Next(-100, 100),
                0f,
                random.Next(-100, 100)
            );

            var key = (glm.gameLevelType, position);

            // 根据索引分配不同类型的交互对象
            switch (i % 4)
            {
                case 0:
                    glm.lightHouseIsDic[key] = false; // 未激活状态
                    Debug.Log($"  生成灯塔 在 {position} (未激活)");
                    break;
                case 1:
                    glm.restPointDic[key] = false;
                    Debug.Log($"  生成休息点 在 {position} (未激活)");
                    break;
                case 2:
                    glm.keyPointDic[key] = false;
                    Debug.Log($"  生成关键点 在 {position} (未激活)");
                    break;
                case 3:
                    glm.itemPointDic[key] = false;
                    Debug.Log($"  生成道具点 在 {position} (未激活)");
                    break;
            }
        }

        Debug.Log("✓ 测试交互对象生成完成");
    }

    /// <summary>
    /// 显示环境信息
    /// </summary>
    private void ShowEnvironmentInfo()
    {
        Debug.Log("=== 测试环境信息 ===");

        // SaveManager状态
        if (SaveManager.Instance != null)
        {
            Debug.Log("✓ SaveManager 可用");
        }
        else
        {
            Debug.Log("✗ SaveManager 不可用");
        }

        // GameLevelManager状态
        if (GameLevelManager.Instance != null)
        {
            var glm = GameLevelManager.Instance;
            Debug.Log($"✓ GameLevelManager 可用，当前关卡: {glm.gameLevelType}");
            
            // 统计交互对象
            var lighthouses = glm.lightHouseIsDic?.Count ?? 0;
            var restPoints = glm.restPointDic?.Count ?? 0;
            var keyPoints = glm.keyPointDic?.Count ?? 0;
            var itemPoints = glm.itemPointDic?.Count ?? 0;
            
            Debug.Log($"  交互对象统计: 灯塔{lighthouses}, 休息点{restPoints}, 关键点{keyPoints}, 道具点{itemPoints}");
        }
        else
        {
            Debug.Log("✗ GameLevelManager 不可用");
        }

        // PlayerManager状态
        if (PlayerManager.Instance?.player != null)
        {
            var player = PlayerManager.Instance.player;
            Debug.Log($"✓ PlayerManager 可用，玩家SAN: {player.SAN.value:F1}/{player.SAN.value_limit:F1}");
        }
        else
        {
            Debug.Log("✗ PlayerManager 或玩家对象不可用");
        }

        // EventIterator状态
        if (EventIterator.Instance != null)
        {
            Debug.Log("✓ EventIterator 可用");
        }
        else
        {
            Debug.Log("✗ EventIterator 不可用");
        }

        Debug.Log("=== 测试按键说明 ===");
        Debug.Log("F12: 计算复活SAN奖励");
        Debug.Log("F11: 清空复活SAN数据");
        Debug.Log("F10: 添加测试交互对象");
        Debug.Log("F9: 显示复活SAN详情");
        Debug.Log("F8: 切换测试控制台 (如果有UI)");
    }

    /// <summary>
    /// 激活一些测试交互对象（模拟玩家游戏过程）
    /// </summary>
    [ContextMenu("激活测试交互对象")]
    public void ActivateTestInteractions()
    {
        if (GameLevelManager.Instance == null)
        {
            Debug.Log("GameLevelManager.Instance 为空");
            return;
        }

        var glm = GameLevelManager.Instance;
        int activated = 0;

        // 激活一半的灯塔
        var lighthouseKeys = new System.Collections.Generic.List<(E_GameLevelType, Vector3)>(glm.lightHouseIsDic.Keys);
        for (int i = 0; i < lighthouseKeys.Count && i < 3; i++)
        {
            var key = lighthouseKeys[i];
            if (!glm.lightHouseIsDic[key])
            {
                glm.lightHouseIsDic[key] = true;
                activated++;
                Debug.Log($"激活灯塔 在 {key.Item2}");
            }
        }

        // 激活一些休息点
        var restKeys = new System.Collections.Generic.List<(E_GameLevelType, Vector3)>(glm.restPointDic.Keys);
        for (int i = 0; i < restKeys.Count && i < 2; i++)
        {
            var key = restKeys[i];
            if (!glm.restPointDic[key])
            {
                glm.restPointDic[key] = true;
                activated++;
                Debug.Log($"激活休息点 在 {key.Item2}");
            }
        }

        Debug.Log($"总共激活了 {activated} 个交互对象");
    }

    /// <summary>
    /// 模拟完整的复活流程测试
    /// </summary>
    [ContextMenu("执行完整复活测试")]
    public void RunFullReviveTest()
    {
        StartCoroutine(FullReviveTestCoroutine());
    }

    private System.Collections.IEnumerator FullReviveTestCoroutine()
    {
        Debug.Log("=== 开始完整复活测试流程 ===");

        // 1. 激活一些交互对象
        ActivateTestInteractions();
        yield return new WaitForSeconds(1f);

        // 2. 显示激活前状态
        if (SaveManager.Instance != null)
        {
            Debug.Log("计算SAN奖励前的状态:");
            ShowEnvironmentInfo();
        }
        yield return new WaitForSeconds(1f);

        // 3. 计算SAN奖励
        if (SaveManager.Instance != null)
        {
            int sanReward = SaveManager.Instance.CalculateAndAwardReviveSan();
            Debug.Log($"本次复活获得SAN奖励: {sanReward}");
        }
        yield return new WaitForSeconds(1f);

        // 4. 显示计算后状态
        if (PlayerManager.Instance?.player != null)
        {
            var player = PlayerManager.Instance.player;
            Debug.Log($"计算SAN奖励后玩家SAN: {player.SAN.value:F1}/{player.SAN.value_limit:F1}");
        }

        Debug.Log("=== 完整复活测试流程结束 ===");
    }

    /// <summary>
    /// 重置测试环境
    /// </summary>
    [ContextMenu("重置测试环境")]
    public void ResetTestEnvironment()
    {
        Debug.Log("=== 重置测试环境 ===");

        // 清空复活SAN数据
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.ClearReviveSanData();
            Debug.Log("✓ 复活SAN数据已清空");
        }

        // 重置交互对象状态
        if (GameLevelManager.Instance != null)
        {
            var glm = GameLevelManager.Instance;

            // 将所有交互对象设为未激活
            var lighthouseKeys = new System.Collections.Generic.List<(E_GameLevelType, Vector3)>(glm.lightHouseIsDic.Keys);
            foreach (var key in lighthouseKeys)
            {
                glm.lightHouseIsDic[key] = false;
            }

            var restKeys = new System.Collections.Generic.List<(E_GameLevelType, Vector3)>(glm.restPointDic.Keys);
            foreach (var key in restKeys)
            {
                glm.restPointDic[key] = false;
            }

            var keyKeys = new System.Collections.Generic.List<(E_GameLevelType, Vector3)>(glm.keyPointDic.Keys);
            foreach (var key in keyKeys)
            {
                glm.keyPointDic[key] = false;
            }

            var itemKeys = new System.Collections.Generic.List<(E_GameLevelType, Vector3)>(glm.itemPointDic.Keys);
            foreach (var key in itemKeys)
            {
                glm.itemPointDic[key] = false;
            }

            Debug.Log("✓ 交互对象状态已重置");
        }

        // 重置玩家SAN
        if (PlayerManager.Instance?.player != null)
        {
            var player = PlayerManager.Instance.player;
            player.SAN.SetValue(initialTestSAN);
            Debug.Log($"✓ 玩家SAN已重置为: {initialTestSAN}");
        }

        Debug.Log("测试环境重置完成！");
    }
}
