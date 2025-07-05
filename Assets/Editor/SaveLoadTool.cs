using UnityEditor;
using UnityEngine;

public static class SaveLoadTool
{
    [MenuItem("Tools/存档/保存游戏 (SaveGame)")]
    public static void SaveGameMenu()
    {
        SaveManager.Instance.SaveGame();
        Debug.Log("[SaveLoadTool] SaveGame() 已调用");
    }

    [MenuItem("Tools/存档/读取游戏 (LoadGame)")]
    public static void LoadGameMenu()
    {
        SaveManager.Instance.LoadGame();
        Debug.Log("[SaveLoadTool] LoadGame() 已调用");
    }
}
