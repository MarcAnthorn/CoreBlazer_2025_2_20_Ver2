using UnityEditor;
using UnityEngine;
using System.Text;

public class GameLevelDebugTool
{
    [MenuItem("Tools/调试/显示 avgIndexIsTriggeredDic")]
    public static void ShowAvgIndexIsTriggeredDic()
    {
        var glm = GameLevelManager.Instance;
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("[avgIndexIsTriggeredDic]");
        foreach (var kv in glm.avgIndexIsTriggeredDic)
            sb.AppendLine($"  id: {kv.Key}, state: {kv.Value}");
        Debug.Log(sb.ToString());
        EditorUtility.DisplayDialog("avgIndexIsTriggeredDic", sb.ToString(), "OK");
    }

    [MenuItem("Tools/调试/显示 avgShelterIsTriggered")]
    public static void ShowAvgShelterIsTriggered()
    {
        var glm = GameLevelManager.Instance;
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("[avgShelterIsTriggered]");
        foreach (var kv in glm.avgShelterIsTriggered)
            sb.AppendLine($"  level: {kv.Key}, triggered: {kv.Value}");
        Debug.Log(sb.ToString());
        EditorUtility.DisplayDialog("avgShelterIsTriggered", sb.ToString(), "OK");
    }

    [MenuItem("Tools/调试/显示 doorIsUnlockedDic")]
    public static void ShowDoorIsUnlockedDic()
    {
        var glm = GameLevelManager.Instance;
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("[doorIsUnlockedDic]");
        foreach (var kv in glm.doorIsUnlockedDic)
            sb.AppendLine($"  doorId: {kv.Key}, unlocked: {kv.Value}");
        Debug.Log(sb.ToString());
        EditorUtility.DisplayDialog("doorIsUnlockedDic", sb.ToString(), "OK");
    }

    [MenuItem("Tools/调试/显示 restPointDic")]
    public static void ShowRestPointDic()
    {
        var glm = GameLevelManager.Instance;
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("[restPointDic]");
        foreach (var kv in glm.restPointDic)
            sb.AppendLine($"  key: ({kv.Key.Item1}, {kv.Key.Item2}), value: {kv.Value}");
        Debug.Log(sb.ToString());
        EditorUtility.DisplayDialog("restPointDic", sb.ToString(), "OK");
    }

    [MenuItem("Tools/调试/显示 lightHouseIsDic")]
    public static void ShowLightHouseIsDic()
    {
        var glm = GameLevelManager.Instance;
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("[lightHouseIsDic]");
        foreach (var kv in glm.lightHouseIsDic)
            sb.AppendLine($"  key: ({kv.Key.Item1}, {kv.Key.Item2}), value: {kv.Value}");
        Debug.Log(sb.ToString());
        EditorUtility.DisplayDialog("lightHouseIsDic", sb.ToString(), "OK");
    }
}
