using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 运行时守护：确保场景中只有一个 EventSystem
/// 在 Awake 中清理多余的 EventSystem，保留最合适的实例
/// </summary>
[DefaultExecutionOrder(-200)]
public class RuntimeEventSystemGuard : MonoBehaviour
{
    void Awake()
    {
        CleanupEventSystems();
    }

    private void CleanupEventSystems()
    {
        EventSystem[] systems = FindObjectsOfType<EventSystem>();
        if (systems == null || systems.Length <= 1) return;

        Debug.LogWarning($"RuntimeEventSystemGuard: 发现 {systems.Length} 个 EventSystem，正在清理多余实例...");

        EventSystem keep = systems[0];
        // 优先保留有 StandaloneInputModule 的
        foreach (var s in systems)
        {
            if (s.GetComponent<StandaloneInputModule>() != null)
            {
                keep = s;
                break;
            }
        }

        for (int i = 0; i < systems.Length; i++)
        {
            if (systems[i] != keep)
            {
                Debug.LogWarning($"RuntimeEventSystemGuard: 销毁 EventSystem: {systems[i].gameObject.name}");
                Destroy(systems[i].gameObject);
            }
        }

        Debug.LogWarning($"RuntimeEventSystemGuard: 保留 EventSystem: {keep.gameObject.name}");
    }
}
