using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

/// <summary>
/// 自动EventSystem清理器
/// 在每个场景启动时自动检查并清理多余的EventSystem
/// </summary>
[DefaultExecutionOrder(-100)] // 确保在其他脚本之前执行
public class AutoEventSystemCleaner : MonoBehaviour
{
    [Header("自动清理设置")]
    [SerializeField] private bool enableAutoCleanup = true;
    [SerializeField] private bool logCleanupActions = true;
    
    void Awake()
    {
        if (enableAutoCleanup)
        {
            CleanupEventSystems();
        }
    }
    
    void Start()
    {
        // 在Start中再次检查，确保没有其他脚本创建了新的EventSystem
        if (enableAutoCleanup)
        {
            CleanupEventSystems();
        }
    }
    
    private void CleanupEventSystems()
    {
        EventSystem[] eventSystems = FindObjectsOfType<EventSystem>();
        
        if (eventSystems.Length <= 1)
        {
            if (logCleanupActions && eventSystems.Length == 1)
            {
                Debug.Log($"✅ EventSystem数量正常: 1个 ({eventSystems[0].gameObject.name})");
            }
            else if (eventSystems.Length == 0)
            {
                if (logCleanupActions)
                {
                    Debug.LogWarning("⚠️ 未找到EventSystem，这可能会导致UI交互问题");
                }
                CreateEventSystem();
            }
            return;
        }
        
        if (logCleanupActions)
        {
            Debug.LogWarning($"🧹 发现 {eventSystems.Length} 个EventSystem，正在清理多余的实例...");
        }
        
        // 找到最适合保留的EventSystem
        EventSystem keepEventSystem = ChooseBestEventSystem(eventSystems);
        
        // 销毁其他的EventSystem
        int destroyedCount = 0;
        for (int i = 0; i < eventSystems.Length; i++)
        {
            if (eventSystems[i] != keepEventSystem)
            {
                if (logCleanupActions)
                {
                    Debug.Log($"🗑️ 销毁多余的EventSystem: {eventSystems[i].gameObject.name}");
                }
                
                // 如果是DontDestroyOnLoad对象，需要特殊处理
                if (eventSystems[i].gameObject.scene.name == "DontDestroyOnLoad")
                {
                    Destroy(eventSystems[i].gameObject);
                }
                else
                {
                    Destroy(eventSystems[i].gameObject);
                }
                destroyedCount++;
            }
        }
        
        if (logCleanupActions)
        {
            Debug.Log($"✅ EventSystem清理完成！保留: {keepEventSystem.gameObject.name}，销毁了 {destroyedCount} 个多余实例");
        }
    }
    
    private EventSystem ChooseBestEventSystem(EventSystem[] eventSystems)
    {
        EventSystem best = eventSystems[0];
        
        foreach (EventSystem es in eventSystems)
        {
            // 优先选择有StandaloneInputModule的
            if (es.GetComponent<StandaloneInputModule>() != null)
            {
                best = es;
                break;
            }
            
            // 其次选择名字包含"EventSystem"的
            if (es.gameObject.name.Contains("EventSystem"))
            {
                best = es;
            }
        }
        
        return best;
    }
    
    private void CreateEventSystem()
    {
        if (logCleanupActions)
        {
            Debug.Log("🔧 创建新的EventSystem...");
        }
        
        GameObject eventSystemGO = new GameObject("EventSystem");
        EventSystem eventSystem = eventSystemGO.AddComponent<EventSystem>();
        StandaloneInputModule inputModule = eventSystemGO.AddComponent<StandaloneInputModule>();
        
        // 配置默认设置
        eventSystem.sendNavigationEvents = true;
        eventSystem.pixelDragThreshold = 10;
        
        if (logCleanupActions)
        {
            Debug.Log("✅ EventSystem创建完成");
        }
    }
    
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (enableAutoCleanup)
        {
            // 延迟一帧执行，确保所有对象都已加载
            StartCoroutine(DelayedCleanup());
        }
    }
    
    private System.Collections.IEnumerator DelayedCleanup()
    {
        yield return null; // 等待一帧
        CleanupEventSystems();
    }
    
    #if UNITY_EDITOR
    [ContextMenu("立即清理EventSystems")]
    private void ManualCleanup()
    {
        CleanupEventSystems();
    }
    
    [ContextMenu("显示当前EventSystem信息")]
    private void ShowEventSystemInfo()
    {
        EventSystem[] eventSystems = FindObjectsOfType<EventSystem>();
        Debug.Log($"=== EventSystem信息 ===");
        Debug.Log($"总数量: {eventSystems.Length}");
        
        for (int i = 0; i < eventSystems.Length; i++)
        {
            EventSystem es = eventSystems[i];
            string info = $"[{i + 1}] {es.gameObject.name}\n";
            info += $"    - 场景: {es.gameObject.scene.name}\n";
            info += $"    - 有InputModule: {es.GetComponent<StandaloneInputModule>() != null}\n";
            info += $"    - 是当前: {EventSystem.current == es}\n";
            info += $"    - GameObject ID: {es.gameObject.GetInstanceID()}";
            
            Debug.Log(info);
        }
        
        Debug.Log($"EventSystem.current: {(EventSystem.current != null ? EventSystem.current.gameObject.name : "null")}");
    }
    #endif
}
