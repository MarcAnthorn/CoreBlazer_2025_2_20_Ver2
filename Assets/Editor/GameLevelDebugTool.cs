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

    [MenuItem("Tools/调试/设置 玩家属性")]
    public static void SetPlayerAttributes()
    {
        var player = PlayerManager.Instance?.player;
        if (player == null)
        {
            EditorUtility.DisplayDialog("玩家属性信息", "未找到PlayerManager或player实例！", "OK");
            return;
        }
        SimpleInputDialog.Show("属性类型", "请输入属性类型（如HP、STR、DEF、LVL、SAN、SPD、CRIT_Rate、CRIT_DMG、HIT、AVO）", "HP", (attrTypeStr) =>
        {
            if (!string.IsNullOrEmpty(attrTypeStr))
            {
                SimpleInputDialog.Show($"属性值 - {attrTypeStr}", $"请输入{attrTypeStr}的新值", "100", (valueStr) =>
                {
                    if (float.TryParse(valueStr, out float newValue))
                    {
                        if (System.Enum.TryParse(attrTypeStr, out AttributeType attrType))
                        {
                            var saveMgr = SaveManager.Instance;
                            var oldAttr = player.GetAttr(attrType);
                            var data = new SaveManager.PlayerAttributeSaveData { value = newValue, value_limit = oldAttr.value_limit, type = (int)attrType };
                            var method = typeof(SaveManager).GetMethod("ApplyAttribute", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                            method.Invoke(saveMgr, new object[] { player, attrType, data });
                            EditorUtility.DisplayDialog("成功", $"已通过ApplyAttribute设置{attrType}={newValue}", "OK");
                        }
                        else
                        {
                            EditorUtility.DisplayDialog("错误", $"无法识别的属性类型: {attrTypeStr}", "OK");
                        }
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("错误", $"输入的属性值无效: {valueStr}", "OK");
                    }
                });
            }
        });
    }

    // 简单输入弹窗工具类
    public class SimpleInputDialog : EditorWindow
    {
        private string prompt;
        private string input;
        private System.Action<string> onClose;
        private static SimpleInputDialog windowInstance;

        public static void Show(string title, string prompt, string defaultValue, System.Action<string> onClose)
        {
            if (windowInstance != null) windowInstance.Close();
            windowInstance = CreateInstance<SimpleInputDialog>();
            windowInstance.titleContent = new GUIContent(title);
            windowInstance.prompt = prompt;
            windowInstance.input = defaultValue;
            windowInstance.onClose = onClose;
            windowInstance.position = new Rect(Screen.width / 2, Screen.height / 2, 350, 100);
            windowInstance.ShowUtility();
        }

        void OnGUI()
        {
            EditorGUILayout.LabelField(prompt, EditorStyles.wordWrappedLabel);
            GUILayout.Space(10);
            GUI.SetNextControlName("InputField");
            input = EditorGUILayout.TextField(input);
            GUI.FocusControl("InputField");
            GUILayout.Space(10);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("确定"))
            {
                onClose?.Invoke(input);
                Close();
            }
            if (GUILayout.Button("取消"))
            {
                onClose?.Invoke(null);
                Close();
            }
            EditorGUILayout.EndHorizontal();
        }
    }

    [MenuItem("Tools/调试/显示 玩家属性信息")]
    public static void ShowPlayerAttributes()
    {
        var player = PlayerManager.Instance?.player;
        if (player == null)
        {
            EditorUtility.DisplayDialog("玩家属性信息", "未找到PlayerManager或player实例！", "OK");
            return;
        }
        
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.AppendLine("[玩家属性信息]");
        sb.AppendLine($"HP: {player.HP.value} / {player.HP.value_limit}");
        sb.AppendLine($"STR: {player.STR.value} / {player.STR.value_limit}");
        sb.AppendLine($"DEF: {player.DEF.value} / {player.DEF.value_limit}");
        sb.AppendLine($"LVL: {player.LVL.value} / {player.LVL.value_limit}");
        sb.AppendLine($"SAN: {player.SAN.value} / {player.SAN.value_limit}");
        sb.AppendLine($"SPD: {player.SPD.value} / {player.SPD.value_limit}");
        sb.AppendLine($"CRIT_Rate: {player.CRIT_Rate.value} / {player.CRIT_Rate.value_limit}");
        sb.AppendLine($"CRIT_DMG: {player.CRIT_DMG.value} / {player.CRIT_DMG.value_limit}");
        sb.AppendLine($"HIT: {player.HIT.value} / {player.HIT.value_limit}");
        sb.AppendLine($"AVO: {player.AVO.value} / {player.AVO.value_limit}");
        EditorUtility.DisplayDialog("玩家属性信息", sb.ToString(), "OK");
        Debug.Log(sb.ToString());
    }
    
}
