using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 场景初始化器 - 确保场景有正确的EventSystem配置
/// 可以添加到任何场景中的空GameObject上
/// </summary>
public class SceneInitializer : MonoBehaviour
{
    [Header("场景初始化设置")]
    [SerializeField] private bool autoCreateEventSystemCleaner = true;
    [SerializeField] private bool debugMode = true;
    
    void Awake()
    {
        InitializeScene();
    }
    
    private void InitializeScene()
    {
        if (debugMode)
        {
            Debug.Log($"🚀 场景初始化开始: {gameObject.scene.name}");
        }
        
        // 检查并清理EventSystem
        CheckAndCleanupEventSystems();
        
        // 确保有AutoEventSystemCleaner
        if (autoCreateEventSystemCleaner)
        {
            EnsureEventSystemCleaner();
        }
        
        if (debugMode)
        {
            Debug.Log($"✅ 场景初始化完成: {gameObject.scene.name}");
        }
    }
    
    private void CheckAndCleanupEventSystems()
    {
        EventSystem[] eventSystems = FindObjectsOfType<EventSystem>();
        
        if (eventSystems.Length > 1)
        {
            if (debugMode)
            {
                Debug.LogWarning($"⚠️ 发现 {eventSystems.Length} 个EventSystem，立即清理...");
            }
            
            // 保留第一个，销毁其他的
            for (int i = 1; i < eventSystems.Length; i++)
            {
                if (debugMode)
                {
                    Debug.Log($"🗑️ 销毁多余EventSystem: {eventSystems[i].gameObject.name}");
                }
                Destroy(eventSystems[i].gameObject);
            }
        }
        else if (eventSystems.Length == 1)
        {
            if (debugMode)
            {
                Debug.Log($"✅ EventSystem正常: {eventSystems[0].gameObject.name}");
            }
        }
        else
        {
            // 创建EventSystem
            CreateEventSystem();
        }
    }
    
    private void CreateEventSystem()
    {
        if (debugMode)
        {
            Debug.Log("🔧 创建EventSystem...");
        }
        
        GameObject eventSystemGO = new GameObject("EventSystem");
        EventSystem eventSystem = eventSystemGO.AddComponent<EventSystem>();
        StandaloneInputModule inputModule = eventSystemGO.AddComponent<StandaloneInputModule>();
        
        // 配置
        eventSystem.sendNavigationEvents = true;
        eventSystem.pixelDragThreshold = 10;
        
        if (debugMode)
        {
            Debug.Log("✅ EventSystem创建完成");
        }
    }
    
    private void EnsureEventSystemCleaner()
    {
        AutoEventSystemCleaner existingCleaner = FindObjectOfType<AutoEventSystemCleaner>();
        
        if (existingCleaner == null)
        {
            GameObject cleanerGO = new GameObject("EventSystemCleaner");
            cleanerGO.AddComponent<AutoEventSystemCleaner>();
            
            // 设置为DontDestroyOnLoad
            DontDestroyOnLoad(cleanerGO);
            
            if (debugMode)
            {
                Debug.Log("🧹 创建AutoEventSystemCleaner");
            }
        }
        else
        {
            if (debugMode)
            {
                Debug.Log("✅ AutoEventSystemCleaner已存在");
            }
        }
    }
}
