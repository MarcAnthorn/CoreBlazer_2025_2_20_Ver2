using UnityEngine;
using UnityEditor;
using TMPro;
using System.Collections.Generic;
using UnityEditor.SceneManagement;

public class TMPFontReplacer : EditorWindow
{
    private TMP_FontAsset sourceFont;
    private TMP_FontAsset targetFont;
    private bool includeInactive = true;
    private bool includePrefabs = true;
    private bool includeScenes = true;
    private Vector2 scrollPosition;
    private List<GameObject> foundObjects = new List<GameObject>();
    private List<TextMeshProUGUI> tmpUGUIComponents = new List<TextMeshProUGUI>();
    private List<TextMeshPro> tmpComponents = new List<TextMeshPro>();

    [MenuItem("Tools/TMP Font Replacer")]
    public static void ShowWindow()
    {
        GetWindow<TMPFontReplacer>("TMP Font Replacer");
    }

    private void OnEnable()
    {
        // 自动加载字体资源
        sourceFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/TextMeshPro/Resources/LiberationSans SDF.asset");
        targetFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/TextMeshPro/Resources/Noto_Sans_SC/static/NotoSansSC-Black SDF 1.asset");
    }

    private void OnGUI()
    {
        GUILayout.Label("TMP Font Replacer", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        // 字体资源选择
        EditorGUILayout.LabelField("Font Assets", EditorStyles.boldLabel);
        sourceFont = (TMP_FontAsset)EditorGUILayout.ObjectField("Source Font (LiberationSans SDF)", sourceFont, typeof(TMP_FontAsset), false);
        targetFont = (TMP_FontAsset)EditorGUILayout.ObjectField("Target Font (NotoSansSC-Black SDF)", targetFont, typeof(TMP_FontAsset), false);

        EditorGUILayout.Space();

        // 搜索选项
        EditorGUILayout.LabelField("Search Options", EditorStyles.boldLabel);
        includeInactive = EditorGUILayout.Toggle("Include Inactive Objects", includeInactive);
        includePrefabs = EditorGUILayout.Toggle("Include Prefabs", includePrefabs);
        includeScenes = EditorGUILayout.Toggle("Include Scenes", includeScenes);

        EditorGUILayout.Space();

        // 按钮
        if (GUILayout.Button("Scan for TMP Components"))
        {
            ScanForTMPComponents();
        }

        if (GUILayout.Button("Replace All Fonts"))
        {
            if (EditorUtility.DisplayDialog("Confirm Replacement", 
                $"Are you sure you want to replace {tmpUGUIComponents.Count + tmpComponents.Count} TMP components from '{sourceFont?.name}' to '{targetFont?.name}'?", 
                "Yes", "Cancel"))
            {
                ReplaceAllFonts();
            }
        }

        EditorGUILayout.Space();

        // 显示结果
        if (foundObjects.Count > 0)
        {
            EditorGUILayout.LabelField($"Found {foundObjects.Count} objects with TMP components", EditorStyles.boldLabel);
            EditorGUILayout.LabelField($"TextMeshProUGUI: {tmpUGUIComponents.Count}");
            EditorGUILayout.LabelField($"TextMeshPro: {tmpComponents.Count}");

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(200));
            
            foreach (var obj in foundObjects)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.ObjectField(obj, typeof(GameObject), true);
                if (GUILayout.Button("Select", GUILayout.Width(60)))
                {
                    Selection.activeGameObject = obj;
                }
                EditorGUILayout.EndHorizontal();
            }
            
            EditorGUILayout.EndScrollView();
        }
    }

    private void ScanForTMPComponents()
    {
        foundObjects.Clear();
        tmpUGUIComponents.Clear();
        tmpComponents.Clear();

        // 搜索场景中的对象
        if (includeScenes)
        {
            var allObjects = includeInactive ? 
                Resources.FindObjectsOfTypeAll<GameObject>() : 
                FindObjectsOfType<GameObject>();

            foreach (var obj in allObjects)
            {
                // 跳过不在场景中的对象（如预制体）
                if (!obj.scene.IsValid())
                    continue;

                CheckObjectForTMPComponents(obj);
            }
        }

        // 搜索预制体
        if (includePrefabs)
        {
            string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab");
            foreach (string guid in prefabGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (prefab != null)
                {
                    // 检查根对象和所有子物体
                    CheckObjectForTMPComponents(prefab);
                    foreach (var child in GetAllChildren(prefab.transform))
                    {
                        CheckObjectForTMPComponents(child.gameObject);
                    }
                }
            }
        }

        Debug.Log($"Scan completed. Found {foundObjects.Count} objects with TMP components.");
    }

    private void CheckObjectForTMPComponents(GameObject obj)
    {
        bool hasTMPComponent = false;

        // 检查TextMeshProUGUI组件
        var tmpUGUI = obj.GetComponent<TextMeshProUGUI>();
        if (tmpUGUI != null && tmpUGUI.font == sourceFont)
        {
            tmpUGUIComponents.Add(tmpUGUI);
            hasTMPComponent = true;
        }

        // 检查TextMeshPro组件
        var tmp = obj.GetComponent<TextMeshPro>();
        if (tmp != null && tmp.font == sourceFont)
        {
            tmpComponents.Add(tmp);
            hasTMPComponent = true;
        }

        // 检查子对象
        foreach (Transform child in obj.transform)
        {
            CheckObjectForTMPComponents(child.gameObject);
        }

        if (hasTMPComponent && !foundObjects.Contains(obj))
        {
            foundObjects.Add(obj);
        }
    }

    private void ReplaceAllFonts()
    {
        if (sourceFont == null || targetFont == null)
        {
            EditorUtility.DisplayDialog("Error", "Please select both source and target fonts.", "OK");
            return;
        }

        int replacedCount = 0;

        // 替换TextMeshProUGUI组件
        foreach (var tmpUGUI in tmpUGUIComponents)
        {
            if (tmpUGUI != null && tmpUGUI.font == sourceFont)
            {
                Undo.RecordObject(tmpUGUI, "Replace TMP Font");
                tmpUGUI.font = targetFont;
                EditorUtility.SetDirty(tmpUGUI);
                replacedCount++;
            }
        }

        // 替换TextMeshPro组件
        foreach (var tmp in tmpComponents)
        {
            if (tmp != null && tmp.font == sourceFont)
            {
                Undo.RecordObject(tmp, "Replace TMP Font");
                tmp.font = targetFont;
                EditorUtility.SetDirty(tmp);
                replacedCount++;
            }
        }

        // 保存场景
        if (includeScenes)
        {
            EditorSceneManager.SaveOpenScenes();
        }

        // 保存资源
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog("Replacement Complete", 
            $"Successfully replaced {replacedCount} TMP components from '{sourceFont.name}' to '{targetFont.name}'.", "OK");

        Debug.Log($"Font replacement completed. Replaced {replacedCount} components.");
    }

    private IEnumerable<Transform> GetAllChildren(Transform parent)
    {
        foreach (Transform child in parent)
        {
            yield return child;
            foreach (var grandChild in GetAllChildren(child))
            {
                yield return grandChild;
            }
        }
    }
} 