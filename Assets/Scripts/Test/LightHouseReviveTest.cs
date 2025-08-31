using UnityEngine;

/// <summary>
/// 灯塔复活重置行为测试脚本
/// 验证灯塔在玩家死亡时保持状态用于SAN奖励计算
/// </summary>
public class LightHouseReviveTest : MonoBehaviour
{
    [Header("测试设置")]
    [SerializeField] private KeyCode testActivateLightHouseKey = KeyCode.L;
    [SerializeField] private KeyCode testPlayerDeadKey = KeyCode.D;
    [SerializeField] private KeyCode testResetLightHouseKey = KeyCode.R;
    [SerializeField] private KeyCode testShowStatusKey = KeyCode.S;

    private LightHouse[] lightHouses;

    private void Start()
    {
        // 查找场景中的所有灯塔
        lightHouses = FindObjectsOfType<LightHouse>();
        Debug.Log($"[LightHouseReviveTest] 找到 {lightHouses?.Length ?? 0} 个灯塔");
    }

    private void Update()
    {
        // L键：模拟激活附近的灯塔
        if (Input.GetKeyDown(testActivateLightHouseKey))
        {
            TestActivateNearbyLightHouse();
        }

        // D键：模拟玩家死亡
        if (Input.GetKeyDown(testPlayerDeadKey))
        {
            TestPlayerDead();
        }

        // R键：重置灯塔
        if (Input.GetKeyDown(testResetLightHouseKey))
        {
            TestResetLightHouses();
        }

        // S键：显示所有灯塔状态
        if (Input.GetKeyDown(testShowStatusKey))
        {
            TestShowLightHouseStatus();
        }
    }

    /// <summary>
    /// 测试激活附近的灯塔
    /// </summary>
    private void TestActivateNearbyLightHouse()
    {
        if (lightHouses == null || lightHouses.Length == 0)
        {
            Debug.LogWarning("[LightHouseReviveTest] 没有找到灯塔");
            return;
        }

        var playerPos = PlayerManager.Instance?.PlayerTransform?.position ?? Vector3.zero;
        LightHouse nearestLightHouse = null;
        float nearestDistance = float.MaxValue;

        // 找到最近的灯塔
        foreach (var lightHouse in lightHouses)
        {
            if (lightHouse != null)
            {
                float distance = Vector3.Distance(playerPos, lightHouse.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestLightHouse = lightHouse;
                }
            }
        }

        if (nearestLightHouse != null)
        {
            Debug.Log($"[LightHouseReviveTest] 模拟激活最近的灯塔，距离: {nearestDistance:F2}");
            
            // 模拟玩家接触灯塔
            var playerCollider = PlayerManager.Instance?.PlayerTransform?.GetComponent<Collider2D>();
            if (playerCollider != null)
            {
                // 触发OnTriggerEnter2D
                nearestLightHouse.SendMessage("OnTriggerEnter2D", playerCollider, SendMessageOptions.DontRequireReceiver);
            }
            else
            {
                Debug.LogWarning("[LightHouseReviveTest] 玩家Collider2D未找到，无法模拟触发");
            }
        }
        else
        {
            Debug.LogWarning("[LightHouseReviveTest] 没有找到可用的灯塔");
        }
    }

    /// <summary>
    /// 测试玩家死亡事件
    /// </summary>
    private void TestPlayerDead()
    {
        Debug.Log("[LightHouseReviveTest] 模拟玩家死亡事件");
        
        // 显示死亡前的状态
        Debug.Log("=== 玩家死亡前灯塔状态 ===");
        ShowLightHouseStatusDetails();
        
        // 触发玩家死亡事件
        EventHub.Instance?.EventTrigger("OnPlayerDead");
        
        // 等待一帧后显示死亡后的状态
        StartCoroutine(ShowStatusAfterDeath());
    }

    private System.Collections.IEnumerator ShowStatusAfterDeath()
    {
        yield return new WaitForEndOfFrame();
        
        Debug.Log("=== 玩家死亡后灯塔状态 ===");
        ShowLightHouseStatusDetails();
        
        // 验证状态是否保持
        VerifyLightHouseStatePreservation();
    }

    /// <summary>
    /// 测试重置灯塔
    /// </summary>
    private void TestResetLightHouses()
    {
        Debug.Log("[LightHouseReviveTest] 重置所有灯塔");
        
        if (lightHouses != null)
        {
            foreach (var lightHouse in lightHouses)
            {
                if (lightHouse != null)
                {
                    // 调用新的ResetLightHouse方法
                    lightHouse.SendMessage("ResetLightHouse", SendMessageOptions.DontRequireReceiver);
                }
            }
        }
        
        // 显示重置后的状态
        ShowLightHouseStatusDetails();
    }

    /// <summary>
    /// 显示灯塔状态
    /// </summary>
    private void TestShowLightHouseStatus()
    {
        ShowLightHouseStatusDetails();
    }

    /// <summary>
    /// 显示详细的灯塔状态信息
    /// </summary>
    private void ShowLightHouseStatusDetails()
    {
        if (GameLevelManager.Instance == null)
        {
            Debug.LogWarning("[LightHouseReviveTest] GameLevelManager.Instance 为空");
            return;
        }

        var glm = GameLevelManager.Instance;
        Debug.Log($"[LightHouseReviveTest] 当前关卡: {glm.gameLevelType}");
        
        if (glm.lightHouseIsDic != null)
        {
            Debug.Log($"[LightHouseReviveTest] 灯塔字典中共有 {glm.lightHouseIsDic.Count} 个条目:");
            
            int activatedCount = 0;
            foreach (var kvp in glm.lightHouseIsDic)
            {
                var (level, position) = kvp.Key;
                bool isActivated = kvp.Value;
                
                Debug.Log($"  灯塔 [{level}] {position} -> {(isActivated ? "已激活" : "未激活")}");
                
                if (isActivated) activatedCount++;
            }
            
            Debug.Log($"[LightHouseReviveTest] 总激活灯塔数: {activatedCount}/{glm.lightHouseIsDic.Count}");
        }
        else
        {
            Debug.LogWarning("[LightHouseReviveTest] lightHouseIsDic 字典为空");
        }
    }

    /// <summary>
    /// 验证灯塔状态保持（用于复活SAN奖励）
    /// </summary>
    private void VerifyLightHouseStatePreservation()
    {
        if (GameLevelManager.Instance?.lightHouseIsDic == null)
        {
            Debug.LogError("[LightHouseReviveTest] 无法验证状态保持：字典为空");
            return;
        }

        var glm = GameLevelManager.Instance;
        bool hasActivatedLightHouses = false;
        
        foreach (var kvp in glm.lightHouseIsDic)
        {
            if (kvp.Value) // 如果有激活的灯塔
            {
                hasActivatedLightHouses = true;
                break;
            }
        }

        if (hasActivatedLightHouses)
        {
            Debug.Log("✅ [LightHouseReviveTest] 验证通过：灯塔状态在玩家死亡后得到保持，可用于复活SAN奖励计算");
        }
        else
        {
            Debug.Log("ℹ️ [LightHouseReviveTest] 当前没有激活的灯塔，无法验证状态保持");
        }
    }

    /// <summary>
    /// 在GUI中显示测试信息
    /// </summary>
    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 350, 200));
        GUILayout.Label("灯塔复活重置测试面板", GUI.skin.box);
        GUILayout.Label($"L键: 激活附近灯塔");
        GUILayout.Label($"D键: 模拟玩家死亡");
        GUILayout.Label($"R键: 重置所有灯塔");
        GUILayout.Label($"S键: 显示灯塔状态");
        
        if (GameLevelManager.Instance?.lightHouseIsDic != null)
        {
            int totalLightHouses = GameLevelManager.Instance.lightHouseIsDic.Count;
            int activatedLightHouses = 0;
            
            foreach (var kvp in GameLevelManager.Instance.lightHouseIsDic)
            {
                if (kvp.Value) activatedLightHouses++;
            }
            
            GUILayout.Label($"灯塔状态: {activatedLightHouses}/{totalLightHouses} 已激活");
        }
        
        GUILayout.EndArea();
    }

    /// <summary>
    /// 测试完整的复活SAN奖励流程
    /// </summary>
    [ContextMenu("测试完整复活流程")]
    public void TestCompleteReviveFlow()
    {
        StartCoroutine(CompleteReviveFlowCoroutine());
    }

    private System.Collections.IEnumerator CompleteReviveFlowCoroutine()
    {
        Debug.Log("=== 开始完整复活流程测试 ===");

        // 1. 激活一些灯塔
        TestActivateNearbyLightHouse();
        yield return new WaitForSeconds(1f);

        // 2. 显示激活后状态
        Debug.Log("--- 激活后状态 ---");
        ShowLightHouseStatusDetails();
        yield return new WaitForSeconds(1f);

        // 3. 模拟玩家死亡
        TestPlayerDead();
        yield return new WaitForSeconds(2f);

        // 4. 计算SAN奖励
        if (SaveManager.Instance != null)
        {
            int sanReward = SaveManager.Instance.CalculateAndAwardReviveSan();
            Debug.Log($"复活SAN奖励: {sanReward}");
        }

        Debug.Log("=== 完整复活流程测试结束 ===");
    }
}
