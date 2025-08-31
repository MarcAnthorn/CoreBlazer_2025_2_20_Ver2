using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// EventSystem单例管理器
/// 确保场景中始终只有一个EventSystem实例
/// </summary>
public class EventSystemManager : MonoBehaviour
{
    private static EventSystemManager instance;
    private EventSystem eventSystem;
    
    [Header("EventSystem设置")]
    [SerializeField] private bool sendNavigationEvents = true;
    [SerializeField] private int pixelDragThreshold = 10;
    
    void Awake()
    {
        // 确保只有一个EventSystemManager实例
        if (instance != null && instance != this)
        {
            Debug.LogWarning($"发现多个EventSystemManager，销毁重复的实例: {gameObject.name}");
            Destroy(gameObject);
            return;
        }
        
        instance = this;
        
        // 设置为不销毁
        DontDestroyOnLoad(gameObject);
        
        // 初始化EventSystem
        InitializeEventSystem();
    }
    
    private void InitializeEventSystem()
    {
        // 查找场景中所有的EventSystem
        EventSystem[] existingEventSystems = FindObjectsOfType<EventSystem>();
        
        if (existingEventSystems.Length > 1)
        {
            Debug.LogWarning($"场景中发现 {existingEventSystems.Length} 个EventSystem，正在清理...");
            
            // 保留第一个，销毁其他的
            for (int i = 1; i < existingEventSystems.Length; i++)
            {
                Debug.Log($"销毁多余的EventSystem: {existingEventSystems[i].gameObject.name}");
                Destroy(existingEventSystems[i].gameObject);
            }
            
            eventSystem = existingEventSystems[0];
        }
        else if (existingEventSystems.Length == 1)
        {
            eventSystem = existingEventSystems[0];
            Debug.Log("找到现有EventSystem，使用该实例");
        }
        else
        {
            // 没有EventSystem，创建一个
            CreateEventSystem();
        }
        
        // 配置EventSystem
        ConfigureEventSystem();
    }
    
    private void CreateEventSystem()
    {
        Debug.Log("未找到EventSystem，正在创建新的实例...");
        
        GameObject eventSystemGO = new GameObject("EventSystem");
        eventSystem = eventSystemGO.AddComponent<EventSystem>();
        
        // 添加输入模块
        var inputModule = eventSystemGO.AddComponent<StandaloneInputModule>();
        
        // 设置为子对象
        eventSystemGO.transform.SetParent(transform);
        
        Debug.Log("EventSystem创建完成");
    }
    
    private void ConfigureEventSystem()
    {
        if (eventSystem != null)
        {
            eventSystem.sendNavigationEvents = sendNavigationEvents;
            eventSystem.pixelDragThreshold = pixelDragThreshold;
            
            Debug.Log($"EventSystem配置完成 - Navigation: {sendNavigationEvents}, DragThreshold: {pixelDragThreshold}");
        }
    }
    
    void Start()
    {
        // 再次检查，确保没有新的EventSystem被创建
        ValidateEventSystemCount();
    }
    
    private void ValidateEventSystemCount()
    {
        EventSystem[] eventSystems = FindObjectsOfType<EventSystem>();
        
        if (eventSystems.Length > 1)
        {
            Debug.LogError($"启动时发现 {eventSystems.Length} 个EventSystem！正在进行清理...");
            
            // 保留我们管理的那个，销毁其他的
            for (int i = 0; i < eventSystems.Length; i++)
            {
                if (eventSystems[i] != eventSystem)
                {
                    Debug.LogWarning($"销毁额外的EventSystem: {eventSystems[i].gameObject.name}");
                    Destroy(eventSystems[i].gameObject);
                }
            }
        }
        else if (eventSystems.Length == 1)
        {
            Debug.Log("EventSystem数量正常 - 1个");
        }
        else
        {
            Debug.LogError("未找到EventSystem！");
        }
    }
    
    void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }
    
    /// <summary>
    /// 获取当前的EventSystem实例
    /// </summary>
    public static EventSystem GetEventSystem()
    {
        if (instance != null && instance.eventSystem != null)
        {
            return instance.eventSystem;
        }
        
        return EventSystem.current;
    }
    
    /// <summary>
    /// 手动检查并清理多余的EventSystem
    /// </summary>
    public static void CleanupEventSystems()
    {
        if (instance != null)
        {
            instance.ValidateEventSystemCount();
        }
    }
    
    #if UNITY_EDITOR
    [ContextMenu("检查EventSystem数量")]
    private void DebugEventSystemCount()
    {
        EventSystem[] eventSystems = FindObjectsOfType<EventSystem>();
        Debug.Log($"当前场景中EventSystem数量: {eventSystems.Length}");
        
        for (int i = 0; i < eventSystems.Length; i++)
        {
            Debug.Log($"EventSystem {i + 1}: {eventSystems[i].gameObject.name} (GameObject ID: {eventSystems[i].gameObject.GetInstanceID()})");
        }
    }
    
    [ContextMenu("强制清理EventSystems")]
    private void ForceCleanupEventSystems()
    {
        CleanupEventSystems();
    }
    #endif
}
