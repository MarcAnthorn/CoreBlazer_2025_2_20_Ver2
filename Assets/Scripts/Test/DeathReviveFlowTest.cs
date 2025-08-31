using UnityEngine;

/// <summary>
/// 死亡复活流程测试脚本
/// 验证死亡时不显示面板，复活时才显示面板的新流程
/// </summary>
public class DeathReviveFlowTest : MonoBehaviour
{
    [Header("测试设置")]
    [SerializeField] private KeyCode testDeathKey = KeyCode.K; // K键模拟死亡
    [SerializeField] private KeyCode testBattleDeathKey = KeyCode.B; // B键模拟战斗死亡
    [SerializeField] private KeyCode testSanDeathKey = KeyCode.M; // M键模拟SAN归零死亡
    [SerializeField] private KeyCode testShowStatusKey = KeyCode.I; // I键显示状态

    [Header("测试数据")]
    [SerializeField] private bool enableTestLogs = true;

    private void Update()
    {
        // K键：模拟普通死亡
        if (Input.GetKeyDown(testDeathKey))
        {
            TestNormalDeath();
        }

        // B键：模拟战斗死亡
        if (Input.GetKeyDown(testBattleDeathKey))
        {
            TestBattleDeath();
        }

        // M键：模拟SAN归零死亡
        if (Input.GetKeyDown(testSanDeathKey))
        {
            TestSanZeroDeath();
        }

        // I键：显示当前状态
        if (Input.GetKeyDown(testShowStatusKey))
        {
            ShowCurrentStatus();
        }
    }

    /// <summary>
    /// 测试普通死亡流程
    /// </summary>
    private void TestNormalDeath()
    {
        if (enableTestLogs)
            Debug.Log("=== 开始测试普通死亡流程 ===");

        // 设置玩家HP为低值但不为0，SAN也保持正值
        if (PlayerManager.Instance?.player != null)
        {
            var player = PlayerManager.Instance.player;
            player.HP.SetValue(10f); // 设置较低HP
            player.SAN.SetValue(30f); // 保持正SAN值
            
            if (enableTestLogs)
                Debug.Log($"设置测试状态 - HP: {player.HP.value}, SAN: {player.SAN.value}");
        }

        // 触发玩家死亡事件
        if (EventHub.Instance != null)
        {
            EventHub.Instance.EventTrigger("OnPlayerDead");
            if (enableTestLogs)
                Debug.Log("已触发普通死亡事件，应该直接进入复活流程，不显示死亡面板");
        }
        else
        {
            Debug.LogError("EventHub.Instance 为空，无法测试死亡流程");
        }
    }

    /// <summary>
    /// 测试战斗死亡流程
    /// </summary>
    private void TestBattleDeath()
    {
        if (enableTestLogs)
            Debug.Log("=== 开始测试战斗死亡流程 ===");

        // 模拟战斗失败触发的死亡
        if (PlayerManager.Instance?.player != null)
        {
            var player = PlayerManager.Instance.player;
            player.HP.SetValue(5f);
            player.SAN.SetValue(25f);
            
            if (enableTestLogs)
                Debug.Log($"模拟战斗失败状态 - HP: {player.HP.value}, SAN: {player.SAN.value}");
        }

        // 模拟BattleManager触发的死亡事件
        if (EventHub.Instance != null)
        {
            EventHub.Instance.EventTrigger("OnPlayerDead");
            if (enableTestLogs)
                Debug.Log("已触发战斗死亡事件，应该直接进入复活流程");
        }
    }

    /// <summary>
    /// 测试SAN归零死亡流程
    /// </summary>
    private void TestSanZeroDeath()
    {
        if (enableTestLogs)
            Debug.Log("=== 开始测试SAN归零死亡流程 ===");

        // 设置SAN为0或负值
        if (PlayerManager.Instance?.player != null)
        {
            var player = PlayerManager.Instance.player;
            player.HP.SetValue(50f); // HP正常
            player.SAN.SetValue(0f); // SAN归零
            
            if (enableTestLogs)
                Debug.Log($"设置SAN归零状态 - HP: {player.HP.value}, SAN: {player.SAN.value}");
        }

        // 触发SAN归零死亡
        if (EventHub.Instance != null)
        {
            EventHub.Instance.EventTrigger("OnPlayerDead");
            if (enableTestLogs)
                Debug.Log("已触发SAN归零死亡，应该播放特殊剧情而不是复活流程");
        }
    }

    /// <summary>
    /// 显示当前玩家状态
    /// </summary>
    private void ShowCurrentStatus()
    {
        if (PlayerManager.Instance?.player != null)
        {
            var player = PlayerManager.Instance.player;
            Debug.Log($"=== 当前玩家状态 ===");
            Debug.Log($"HP: {player.HP.value:F1}/{player.HP.value_limit:F1}");
            Debug.Log($"SAN: {player.SAN.value:F1}/{player.SAN.value_limit:F1}");
            Debug.Log($"LVL: {player.LVL.value:F1}");
        }
        else
        {
            Debug.LogWarning("玩家对象未找到");
        }

        if (GameLevelManager.Instance != null)
        {
            Debug.Log($"当前关卡: {GameLevelManager.Instance.gameLevelType}");
        }
    }

    /// <summary>
    /// 模拟完整的死亡-复活循环
    /// </summary>
    [ContextMenu("测试完整死亡复活循环")]
    public void TestCompleteDeathReviveCycle()
    {
        StartCoroutine(CompleteDeathReviveCycleCoroutine());
    }

    private System.Collections.IEnumerator CompleteDeathReviveCycleCoroutine()
    {
        Debug.Log("=== 开始完整死亡复活循环测试 ===");

        // 1. 显示初始状态
        ShowCurrentStatus();
        yield return new WaitForSeconds(1f);

        // 2. 激活一些交互对象（为复活SAN奖励做准备）
        if (SaveManager.Instance != null)
        {
            Debug.Log("--- 模拟激活交互对象 ---");
            
            // 模拟激活一些灯塔和休息点
            if (GameLevelManager.Instance != null)
            {
                var glm = GameLevelManager.Instance;
                var testPos1 = new Vector3(100f, 0f, 100f);
                var testPos2 = new Vector3(200f, 0f, 200f);
                
                glm.lightHouseIsDic[(glm.gameLevelType, testPos1)] = true;
                glm.restPointDic[(glm.gameLevelType, testPos2)] = true;
                
                Debug.Log("已激活测试交互对象：1个灯塔 + 1个休息点");
            }
        }
        yield return new WaitForSeconds(1f);

        // 3. 触发死亡
        Debug.Log("--- 触发死亡事件 ---");
        TestNormalDeath();
        yield return new WaitForSeconds(2f);

        // 4. 等待复活流程完成
        Debug.Log("--- 等待复活流程 ---");
        yield return new WaitForSeconds(3f);

        // 5. 显示最终状态
        Debug.Log("--- 复活后状态 ---");
        ShowCurrentStatus();

        Debug.Log("=== 完整死亡复活循环测试结束 ===");
    }

    /// <summary>
    /// 验证面板显示行为
    /// </summary>
    [ContextMenu("验证面板显示时机")]
    public void VerifyPanelDisplayTiming()
    {
        Debug.Log("=== 验证面板显示时机 ===");
        Debug.Log("预期行为：");
        Debug.Log("1. 死亡时：不显示任何面板，直接进入复活流程");
        Debug.Log("2. 复活时：显示复活面板，包含SAN奖励信息");
        Debug.Log("3. SAN归零死亡：显示特殊剧情AVG面板");
        Debug.Log("============================");
        
        Debug.Log("测试方法：");
        Debug.Log("K键 - 测试普通死亡（应该不显示面板）");
        Debug.Log("B键 - 测试战斗死亡（应该不显示面板）");
        Debug.Log("M键 - 测试SAN归零死亡（应该显示AVG面板）");
        Debug.Log("I键 - 显示当前玩家状态");
    }

    /// <summary>
    /// 在GUI中显示测试信息
    /// </summary>
    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 200, 400, 250));
        GUILayout.Label("死亡复活流程测试面板", GUI.skin.box);
        
        GUILayout.Label("测试按键：");
        GUILayout.Label("K键: 普通死亡（不显示面板）");
        GUILayout.Label("B键: 战斗死亡（不显示面板）");
        GUILayout.Label("M键: SAN归零死亡（显示AVG）");
        GUILayout.Label("I键: 显示当前状态");
        
        GUILayout.Space(10);
        GUILayout.Label("预期行为：", GUI.skin.box);
        GUILayout.Label("• 死亡时不显示GameOverPanel");
        GUILayout.Label("• 复活时显示完整的复活面板");
        GUILayout.Label("• 面板包含SAN奖励和游戏进度");
        
        if (PlayerManager.Instance?.player != null)
        {
            var player = PlayerManager.Instance.player;
            GUILayout.Space(5);
            GUILayout.Label($"当前HP: {player.HP.value:F1}/{player.HP.value_limit:F1}");
            GUILayout.Label($"当前SAN: {player.SAN.value:F1}/{player.SAN.value_limit:F1}");
        }
        
        GUILayout.EndArea();
    }

    /// <summary>
    /// 重置玩家状态到健康状态
    /// </summary>
    [ContextMenu("重置玩家状态")]
    public void ResetPlayerToHealthyState()
    {
        if (PlayerManager.Instance?.player != null)
        {
            var player = PlayerManager.Instance.player;
            player.HP.SetValue(player.HP.value_limit);
            player.SAN.SetValue(50f); // 设置为中等SAN值
            
            Debug.Log($"玩家状态已重置 - HP: {player.HP.value}, SAN: {player.SAN.value}");
        }
    }
}
