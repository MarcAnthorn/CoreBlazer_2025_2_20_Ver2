using UnityEngine;

/// <summary>
/// 复活SAN奖励系统测试脚本
/// 用于测试基于交互对象的SAN奖励计算功能
/// </summary>
public class ReviveSanTester : MonoBehaviour
{
    [Header("测试按键设置")]
    [SerializeField] private KeyCode testCalculateSanKey = KeyCode.F12;
    [SerializeField] private KeyCode testClearSanDataKey = KeyCode.F11;
    [SerializeField] private KeyCode testAddTestInteractionKey = KeyCode.F10;
    [SerializeField] private KeyCode testShowSanDetailsKey = KeyCode.F9;
    [SerializeField] private KeyCode testFullReviveProcessKey = KeyCode.F7; // 新增：完整复活流程测试

    [Header("测试设置")]
    [SerializeField] private bool enableDebugMode = true;

    private void Update()
    {
        // F12键：计算复活SAN奖励
        if (Input.GetKeyDown(testCalculateSanKey))
        {
            TestCalculateReviveSan();
        }

        // F11键：清空复活SAN数据
        if (Input.GetKeyDown(testClearSanDataKey))
        {
            TestClearReviveSanData();
        }

        // F10键：添加测试交互对象
        if (Input.GetKeyDown(testAddTestInteractionKey))
        {
            TestAddInteraction();
        }

        // F9键：显示复活SAN详情
        if (Input.GetKeyDown(testShowSanDetailsKey))
        {
            TestShowSanDetails();
        }

        // F7键：测试完整复活流程
        if (Input.GetKeyDown(testFullReviveProcessKey))
        {
            TestFullReviveProcess();
        }
    }

    /// <summary>
    /// 测试计算复活SAN奖励
    /// </summary>
    private void TestCalculateReviveSan()
    {
        Debug.Log("=== 测试计算复活SAN奖励 (F12键) ===");
        
        if (SaveManager.Instance != null)
        {
            int sanReward = SaveManager.Instance.CalculateAndAwardReviveSan();
            Debug.Log($"测试完成！本次获得SAN奖励: {sanReward}");
            
            // 显示当前玩家SAN状态
            if (PlayerManager.Instance?.player != null)
            {
                var player = PlayerManager.Instance.player;
                Debug.Log($"当前玩家SAN: {player.SAN.value:F1}/{player.SAN.value_limit:F1}");
            }
        }
        else
        {
            Debug.LogError("SaveManager.Instance 为空，无法测试SAN奖励计算");
        }
    }

    /// <summary>
    /// 测试清空复活SAN数据
    /// </summary>
    private void TestClearReviveSanData()
    {
        Debug.Log("=== 测试清空复活SAN数据 (F11键) ===");
        
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.ClearReviveSanData();
            Debug.Log("复活SAN数据已清空！");
        }
        else
        {
            Debug.LogError("SaveManager.Instance 为空，无法清空SAN数据");
        }
    }

    /// <summary>
    /// 测试添加交互对象（模拟玩家与对象交互）
    /// </summary>
    private void TestAddInteraction()
    {
        Debug.Log("=== 测试添加交互对象 (F10键) ===");
        
        if (GameLevelManager.Instance != null)
        {
            var glm = GameLevelManager.Instance;
            
            // 模拟激活一个灯塔
            var testPosition = new Vector3(100f, 0f, 100f);
            var testKey = (glm.gameLevelType, testPosition);
            
            if (!glm.lightHouseIsDic.ContainsKey(testKey))
            {
                glm.lightHouseIsDic[testKey] = true;
                Debug.Log($"模拟激活灯塔 在位置 {testPosition}");
            }
            else if (!glm.lightHouseIsDic[testKey])
            {
                glm.lightHouseIsDic[testKey] = true;
                Debug.Log($"激活已存在的灯塔 在位置 {testPosition}");
            }
            else
            {
                Debug.Log($"灯塔已经激活 在位置 {testPosition}");
            }
            
            // 模拟激活一个休息点
            var restPosition = new Vector3(200f, 0f, 200f);
            var restKey = (glm.gameLevelType, restPosition);
            
            if (!glm.restPointDic.ContainsKey(restKey))
            {
                glm.restPointDic[restKey] = true;
                Debug.Log($"模拟激活休息点 在位置 {restPosition}");
            }
            else if (!glm.restPointDic[restKey])
            {
                glm.restPointDic[restKey] = true;
                Debug.Log($"激活已存在的休息点 在位置 {restPosition}");
            }
            else
            {
                Debug.Log($"休息点已经激活 在位置 {restPosition}");
            }
        }
        else
        {
            Debug.LogError("GameLevelManager.Instance 为空，无法添加测试交互");
        }
    }

    /// <summary>
    /// 测试显示复活SAN详情
    /// </summary>
    private void TestShowSanDetails()
    {
        Debug.Log("=== 测试显示复活SAN详情 (F9键) ===");
        
        if (SaveManager.Instance != null)
        {
            // 使用反射调用私有方法 ShowReviveSanDetails
            var method = typeof(SaveManager).GetMethod("ShowReviveSanDetails", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (method != null)
            {
                method.Invoke(SaveManager.Instance, null);
            }
            else
            {
                Debug.LogWarning("无法找到 ShowReviveSanDetails 方法");
            }
            
            // 也可以调用LoadReviveSanData方法显示数据
            var loadMethod = typeof(SaveManager).GetMethod("LoadReviveSanData", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (loadMethod != null)
            {
                var sanData = loadMethod.Invoke(SaveManager.Instance, null);
                if (sanData != null)
                {
                    Debug.Log($"当前复活SAN数据类型: {sanData.GetType().Name}");
                }
            }
        }
        else
        {
            Debug.LogError("SaveManager.Instance 为空，无法显示SAN详情");
        }
    }

    /// <summary>
    /// 测试完整的复活流程
    /// </summary>
    [ContextMenu("测试完整复活流程")]
    public void TestFullReviveProcess()
    {
        Debug.Log("=== 测试完整复活流程 ===");
        
        // 1. 添加一些测试交互
        TestAddInteraction();
        
        // 2. 等待一帧
        StartCoroutine(DelayedReviveTest());
    }

    private System.Collections.IEnumerator DelayedReviveTest()
    {
        yield return new WaitForEndOfFrame();
        
        // 3. 模拟复活流程：先计算SAN奖励
        int sanReward = 0;
        if (SaveManager.Instance != null)
        {
            sanReward = SaveManager.Instance.CalculateAndAwardReviveSan();
            Debug.Log($"模拟复活：获得SAN奖励 {sanReward}");
        }
        
        yield return new WaitForSeconds(0.5f);
        
        // 4. 显示复活数据（不重复计算SAN）
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.SaveGameOnReviveAndShowData(calculateSanReward: false);
        }
        
        yield return new WaitForSeconds(0.5f);
        
        // 5. 模拟显示复活SAN奖励面板
        if (sanReward > 0)
        {
            ShowMockReviveSanPanel(sanReward);
        }
        
        Debug.Log("=== 完整复活流程测试完成 ===");
    }

    /// <summary>
    /// 模拟显示复活SAN奖励面板
    /// </summary>
    private void ShowMockReviveSanPanel(int sanReward)
    {
        try
        {
            float currentSAN = 0f;
            if (PlayerManager.Instance?.player != null)
            {
                currentSAN = PlayerManager.Instance.player.SAN.value;
            }

            string rewardInfo = $"复活奖励测试\n获得 SAN +{sanReward}";
            
            Debug.Log($"=== 模拟复活SAN奖励面板 ===");
            Debug.Log($"奖励信息: {rewardInfo}");
            Debug.Log($"当前SAN: {currentSAN}");
            Debug.Log($"========================");

            // 如果GameOverPanel可用，尝试显示实际面板
            if (typeof(GameOverPanel) != null)
            {
                var method = typeof(GameOverPanel).GetMethod("ShowSANOnlyPanel", 
                    System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                
                if (method != null)
                {
                    method.Invoke(null, new object[] {
                        currentSAN,
                        (System.Action)(() => Debug.Log("测试复活SAN面板已关闭")),
                        null, // mazeProgress
                        null, // stageProgress  
                        rewardInfo // exploreTip
                    });
                    Debug.Log("实际复活SAN奖励面板已显示");
                }
                else
                {
                    Debug.Log("ShowSANOnlyPanel方法未找到，仅显示日志信息");
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"显示模拟复活SAN面板时发生错误: {e.Message}");
        }
    }

    /// <summary>
    /// 在GUI中显示测试信息
    /// </summary>
    private void OnGUI()
    {
        if (!enableDebugMode) return;
        
        GUILayout.BeginArea(new Rect(10, 350, 450, 250));
        GUILayout.Label("复活SAN奖励系统测试面板", GUI.skin.box);
        GUILayout.Label($"F12: 计算复活SAN奖励");
        GUILayout.Label($"F11: 清空复活SAN数据");
        GUILayout.Label($"F10: 添加测试交互对象");
        GUILayout.Label($"F9: 显示复活SAN详情");
        GUILayout.Label($"F7: 测试完整复活流程 (推荐)", GUI.skin.box);
        
        if (PlayerManager.Instance?.player != null)
        {
            var player = PlayerManager.Instance.player;
            GUILayout.Label($"当前SAN: {player.SAN.value:F1}/{player.SAN.value_limit:F1}");
        }
        
        if (GameLevelManager.Instance != null)
        {
            var glm = GameLevelManager.Instance;
            GUILayout.Label($"当前关卡: {glm.gameLevelType}");
            
            // 计算激活的灯塔数量
            int activeLighthouses = 0;
            if (glm.lightHouseIsDic != null)
            {
                foreach (var lighthouse in glm.lightHouseIsDic.Values)
                {
                    if (lighthouse) activeLighthouses++;
                }
            }
            GUILayout.Label($"激活的灯塔: {activeLighthouses}");
            
            // 计算激活的休息点数量
            int activeRestPoints = 0;
            if (glm.restPointDic != null)
            {
                foreach (var restPoint in glm.restPointDic.Values)
                {
                    if (restPoint) activeRestPoints++;
                }
            }
            GUILayout.Label($"激活的休息点: {activeRestPoints}");
        }
        
        GUILayout.EndArea();
    }

    /// <summary>
    /// 生成随机测试交互对象
    /// </summary>
    [ContextMenu("生成随机测试交互")]
    public void GenerateRandomTestInteractions()
    {
        if (GameLevelManager.Instance == null)
        {
            Debug.LogError("GameLevelManager.Instance 为空");
            return;
        }

        var glm = GameLevelManager.Instance;
        var random = new System.Random();
        
        // 随机生成一些交互对象
        for (int i = 0; i < 5; i++)
        {
            var randomPos = new Vector3(
                random.Next(-500, 500),
                0f,
                random.Next(-500, 500)
            );
            
            var key = (glm.gameLevelType, randomPos);
            
            // 随机选择交互类型
            int interactionType = random.Next(0, 4);
            switch (interactionType)
            {
                case 0:
                    glm.lightHouseIsDic[key] = true;
                    Debug.Log($"生成测试灯塔 在 {randomPos}");
                    break;
                case 1:
                    glm.restPointDic[key] = true;
                    Debug.Log($"生成测试休息点 在 {randomPos}");
                    break;
                case 2:
                    glm.keyPointDic[key] = true;
                    Debug.Log($"生成测试关键点 在 {randomPos}");
                    break;
                case 3:
                    glm.itemPointDic[key] = true;
                    Debug.Log($"生成测试道具点 在 {randomPos}");
                    break;
            }
        }
        
        Debug.Log("随机测试交互对象生成完成！");
    }
}
