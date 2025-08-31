using UnityEngine;

/// <summary>
/// SaveManager复活功能测试脚本
/// 用于测试复活时保存游戏并显示数据的功能
/// </summary>
public class SaveManagerReviveTester : MonoBehaviour
{
    [Header("测试按键设置")]
    [SerializeField] private KeyCode testReviveSaveKey = KeyCode.F8;
    [SerializeField] private KeyCode testNormalSaveKey = KeyCode.F7;

    private void Update()
    {
        // F7键：普通保存
        if (Input.GetKeyDown(testNormalSaveKey))
        {
            TestNormalSave();
        }

        // F8键：复活保存并显示数据
        if (Input.GetKeyDown(testReviveSaveKey))
        {
            TestReviveSaveAndShowData();
        }
    }

    /// <summary>
    /// 测试普通保存功能
    /// </summary>
    private void TestNormalSave()
    {
        Debug.Log("=== 测试普通保存功能 (F7键) ===");
        
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.SaveGame();
            Debug.Log("普通保存完成！");
        }
        else
        {
            Debug.LogError("SaveManager.Instance 为空，无法进行普通保存");
        }
    }

    /// <summary>
    /// 测试复活保存并显示数据功能
    /// </summary>
    private void TestReviveSaveAndShowData()
    {
        Debug.Log("=== 测试复活保存并显示数据功能 (F8键) ===");
        
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.SaveGameOnReviveAndShowData();
            Debug.Log("复活保存并显示数据完成！");
        }
        else
        {
            Debug.LogError("SaveManager.Instance 为空，无法进行复活保存");
        }
    }

    /// <summary>
    /// 在GUI中显示测试信息
    /// </summary>
    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 200, 300, 100));
        GUILayout.Label("SaveManager复活功能测试", GUI.skin.box);
        GUILayout.Label($"F7: 普通保存游戏");
        GUILayout.Label($"F8: 复活保存并显示数据");
        GUILayout.Label($"R: EventIterator测试");
        GUILayout.EndArea();
    }
}
