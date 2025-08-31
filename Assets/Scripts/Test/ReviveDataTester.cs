using UnityEngine;

/// <summary>
/// 复活数据管理器测试脚本
/// 用于测试ReviveDataManager的功能
/// </summary>
public class ReviveDataTester : MonoBehaviour
{
    [Header("测试设置")]
    [SerializeField] private KeyCode testReviveKey = KeyCode.F9;
    [SerializeField] private KeyCode testShowDataKey = KeyCode.F10;
    [SerializeField] private KeyCode testClearHistoryKey = KeyCode.F11;

    private void Update()
    {
        // F9键：模拟复活
        if (Input.GetKeyDown(testReviveKey))
        {
            TestRevive();
        }

        // F10键：显示复活数据
        if (Input.GetKeyDown(testShowDataKey))
        {
            TestShowData();
        }

        // F11键：清空复活历史
        if (Input.GetKeyDown(testClearHistoryKey))
        {
            TestClearHistory();
        }
    }

    /// <summary>
    /// 测试复活功能
    /// </summary>
    private void TestRevive()
    {
        Debug.Log("=== 测试复活功能 (F9键) ===");
        
        if (ReviveDataManager.Instance != null)
        {
            ReviveDataManager.Instance.OnPlayerRevive();
            Debug.Log("测试复活完成！");
        }
        else
        {
            Debug.LogWarning("ReviveDataManager.Instance 为空，无法测试复活功能");
        }
    }

    /// <summary>
    /// 测试显示数据功能
    /// </summary>
    private void TestShowData()
    {
        Debug.Log("=== 测试显示复活数据 (F10键) ===");
        
        if (ReviveDataManager.Instance != null)
        {
            ReviveDataManager.Instance.TestShowReviveData();
            
            // 显示历史统计
            var stats = ReviveDataManager.Instance.GetReviveHistoryStats();
            Debug.Log($"复活历史统计: 总复活次数={stats.totalRevives}, 平均SAN={stats.averageSanOnRevive:F1}");
            
            // 显示格式化文本
            string formattedText = ReviveDataManager.Instance.GetFormattedReviveDataText();
            Debug.Log($"格式化复活数据:\n{formattedText}");
        }
        else
        {
            Debug.LogWarning("ReviveDataManager.Instance 为空，无法测试显示数据功能");
        }
    }

    /// <summary>
    /// 测试清空历史功能
    /// </summary>
    private void TestClearHistory()
    {
        Debug.Log("=== 测试清空复活历史 (F11键) ===");
        
        if (ReviveDataManager.Instance != null)
        {
            ReviveDataManager.Instance.ClearReviveHistory();
            Debug.Log("复活历史已清空！");
        }
        else
        {
            Debug.LogWarning("ReviveDataManager.Instance 为空，无法测试清空历史功能");
        }
    }

    /// <summary>
    /// 在GUI中显示测试信息
    /// </summary>
    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 150));
        GUILayout.Label("复活数据管理器测试面板", GUI.skin.box);
        GUILayout.Label($"F9: 模拟复活");
        GUILayout.Label($"F10: 显示复活数据");
        GUILayout.Label($"F11: 清空复活历史");
        GUILayout.Label($"R: EventIterator测试 (已完成事件)");
        
        if (ReviveDataManager.Instance != null)
        {
            var stats = ReviveDataManager.Instance.GetReviveHistoryStats();
            GUILayout.Label($"当前复活次数: {stats.totalRevives}");
        }
        else
        {
            GUILayout.Label("ReviveDataManager未初始化");
        }
        
        GUILayout.EndArea();
    }
}
