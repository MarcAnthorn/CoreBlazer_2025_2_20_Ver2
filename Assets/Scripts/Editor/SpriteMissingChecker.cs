using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections.Generic;

public class SpriteMissingChecker : EditorWindow
{
    private List<SpriteRenderer> missingSpriteRenderers = new List<SpriteRenderer>();
    private Vector2 scrollPos;

    [MenuItem("Tools/检测Prefab中丢失Sprite的SpriteRenderer")]
    public static void ShowWindow()
    {
        GetWindow<SpriteMissingChecker>("Prefab Sprite 丢失检测");
    }

    private void OnGUI()
    {
        if (GUILayout.Button("检测当前打开的Prefab"))
        {
            DetectMissingSprites();
        }

        if (missingSpriteRenderers.Count > 0)
        {
            EditorGUILayout.LabelField("以下SpriteRenderer的Sprite丢失：", EditorStyles.boldLabel);
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            foreach (var sr in missingSpriteRenderers)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.ObjectField(sr.gameObject, typeof(GameObject), true);
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();

            EditorGUILayout.Space();
            if (GUILayout.Button("全部关联同一个Sprite"))
            {
                string path = EditorUtility.OpenFilePanel("选择Sprite", Application.dataPath, "png");
                if (!string.IsNullOrEmpty(path))
                {
                    string assetPath = "Assets" + path.Substring(Application.dataPath.Length);
                    Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
                    if (sprite != null)
                    {
                        foreach (var sr in missingSpriteRenderers)
                        {
                            Undo.RecordObject(sr, "批量关联Sprite");
                            sr.sprite = sprite;
                            EditorUtility.SetDirty(sr);
                        }
                        AssetDatabase.SaveAssets();
                        EditorUtility.DisplayDialog("完成", "所有SpriteRenderer已关联该Sprite。", "确定");
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("错误", "选中的文件不是Sprite资源！", "确定");
                    }
                }
            }
        }
        else
        {
            EditorGUILayout.LabelField("未检测到Sprite丢失的SpriteRenderer。");
        }
    }

    private void DetectMissingSprites()
    {
        missingSpriteRenderers.Clear();

        // 获取当前Prefab Stage
        var prefabStage = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
        if (prefabStage == null)
        {
            EditorUtility.DisplayDialog("提示", "请在Prefab模式下打开Prefab后再检测！", "确定");
            return;
        }

        var root = prefabStage.prefabContentsRoot;
        var allSpriteRenderers = root.GetComponentsInChildren<SpriteRenderer>(true);

        foreach (var sr in allSpriteRenderers)
        {
            if (sr.sprite == null)
            {
                missingSpriteRenderers.Add(sr);
            }
            else
            {
                // 检查missing（丢失引用的情况）
                string path = AssetDatabase.GetAssetPath(sr.sprite);
                if (string.IsNullOrEmpty(path))
                {
                    missingSpriteRenderers.Add(sr);
                }
            }
        }

        if (missingSpriteRenderers.Count == 0)
        {
            EditorUtility.DisplayDialog("检测完成", "没有Sprite丢失的SpriteRenderer。", "确定");
        }
    }
} 