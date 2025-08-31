using UnityEngine;

/// <summary>
/// 灯塔SAN奖励逻辑测试脚本
/// 用于验证多个相同ID灯塔的SAN计算是否正确
/// </summary>
public class LightHouseSanTest : MonoBehaviour
{
    [Header("测试控制")]
    [SerializeField] private KeyCode testKey = KeyCode.L;
    [SerializeField] private KeyCode detailTestKey = KeyCode.Semicolon; // 分号键
    
    void Update()
    {
        // L键：快速测试灯塔逻辑
        if (Input.GetKeyDown(testKey))
        {
            TestLightHouseSanLogic();
        }
        
        // 分号键：详细测试
        if (Input.GetKeyDown(detailTestKey))
        {
            DetailedLightHouseTest();
        }
    }
    
    /// <summary>
    /// 快速测试灯塔SAN逻辑
    /// </summary>
    private void TestLightHouseSanLogic()
    {
        Debug.Log("🔧 === L键测试：灯塔SAN逻辑 ===");
        
        if (SaveManager.Instance != null)
        {
            #pragma warning disable CS0618 // 忽略过时警告
            SaveManager.Instance.DebugTestLightHouseSanLogic();
            #pragma warning restore CS0618
        }
        else
        {
            Debug.LogError("❌ SaveManager.Instance 为空");
        }
    }
    
    /// <summary>
    /// 详细测试灯塔数据
    /// </summary>
    private void DetailedLightHouseTest()
    {
        Debug.Log("🔧 === 分号键测试：详细灯塔数据分析 ===");
        
        if (SaveManager.Instance != null)
        {
            #pragma warning disable CS0618 // 忽略过时警告
            SaveManager.Instance.DebugShowAllGameData();
            #pragma warning restore CS0618
            
            // 模拟计算SAN奖励
            Debug.Log("🔧 === 模拟SAN奖励计算 ===");
            int sanReward = SaveManager.Instance.CalculateAndAwardReviveSan();
            Debug.Log($"🔧 模拟计算结果：获得 {sanReward} SAN");
        }
        else
        {
            Debug.LogError("❌ SaveManager.Instance 为空");
        }
    }
    
    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 200, 400, 100));
        GUILayout.Label("灯塔SAN测试工具", GUI.skin.box);
        GUILayout.Label($"按 {testKey} 键：快速测试灯塔逻辑");
        GUILayout.Label($"按 {detailTestKey} 键：详细测试和计算");
        GUILayout.EndArea();
    }
}
