using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 场景切换时的UI清理管理器
/// 确保在场景切换时正确清理UI面板，防止PanelFather被销毁时的错误
/// </summary>
public class SceneTransitionUIManager : MonoBehaviour
{
    private void Awake()
    {
        // 确保这个对象在场景切换时不被销毁
        DontDestroyOnLoad(gameObject);
        
        // 监听场景切换事件
        SceneManager.sceneUnloaded += OnSceneUnloaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        // 移除事件监听
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    /// <summary>
    /// 场景卸载时的处理
    /// </summary>
    /// <param name="scene">被卸载的场景</param>
    private void OnSceneUnloaded(Scene scene)
    {
        Debug.Log($"Scene '{scene.name}' unloaded, cleaning up UI panels");
        
        // 清理所有UI面板，避免在PanelFather被销毁时出现错误
        if (UIManager.Instance != null)
        {
            UIManager.Instance.CleanupAllPanels();
        }
        else
        {
            Debug.LogWarning("UIManager.Instance is null during scene unload");
        }
    }

    /// <summary>
    /// 场景加载完成时的处理
    /// </summary>
    /// <param name="scene">加载的场景</param>
    /// <param name="mode">加载模式</param>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"Scene '{scene.name}' loaded with mode {mode}");
        
        // 确保UIManager在新场景中正确初始化
        if (UIManager.Instance != null)
        {
            Debug.Log("UIManager instance available in new scene");
        }
        else
        {
            Debug.LogWarning("UIManager.Instance is null in new scene");
        }
    }

    /// <summary>
    /// 应用程序退出时的清理
    /// </summary>
    private void OnApplicationQuit()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.CleanupAllPanels();
        }
    }

    /// <summary>
    /// 应用程序暂停时的处理（移动设备）
    /// </summary>
    /// <param name="pauseStatus">暂停状态</param>
    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            Debug.Log("Application paused, ensuring UI state is safe");
            // 在应用暂停时可以进行一些UI状态保存
        }
        else
        {
            Debug.Log("Application resumed");
        }
    }

    /// <summary>
    /// 应用程序失去焦点时的处理
    /// </summary>
    /// <param name="hasFocus">是否有焦点</param>
    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            Debug.Log("Application lost focus");
        }
        else
        {
            Debug.Log("Application gained focus");
        }
    }
}
