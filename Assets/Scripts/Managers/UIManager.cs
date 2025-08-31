using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIManager : SingletonBaseManager<UIManager>
{

    public Transform fatherTransform;
    private Transform canvasTransform;
    public Transform CanvasTransform => canvasTransform;

    public Dictionary<string, BasePanel> shownPanelDic = new Dictionary<string, BasePanel>();

    public T ShowPanel<T>() where T:BasePanel
    {
        if(canvasTransform == null)
        {
            canvasTransform = GameObject.Find("Canvas").transform;
        }

        // 安全检查：确保父对象存在且未被销毁
        if (fatherTransform == null || fatherTransform.gameObject == null)
        {
            Debug.LogWarning("PanelFather is null or destroyed, attempting to find or recreate it");
            fatherTransform = GameObject.Find("PanelFather")?.transform;
            if (fatherTransform == null)
            {
                Debug.LogError("Cannot find PanelFather, unable to show panel");
                return null;
            }
        }

        // 检查父对象是否正在被销毁
        if (fatherTransform.gameObject.activeInHierarchy == false)
        {
            Debug.LogWarning($"PanelFather is being destroyed or inactive, cannot show panel {typeof(T).Name}");
            return null;
        }

        //此处得取的实际上是脚本的名字，但是我们需要通过实例化预设体的方式加载panel对象；
        //所以就必须要确保panel对象和挂载的脚本名称必须要一致；
        string panelName = typeof(T).Name;
        if (shownPanelDic.ContainsKey(panelName))
        {
            return shownPanelDic[panelName] as T;
        }
        else
        {
            string path = Path.Combine("Panels", panelName);
            GameObject panelObject = GameObject.Instantiate(Resources.Load<GameObject>(path));
            
            // 再次检查父对象在实例化后是否仍然有效
            if (fatherTransform == null || fatherTransform.gameObject == null)
            {
                Debug.LogError("PanelFather became null during panel instantiation, destroying created panel");
                GameObject.Destroy(panelObject);
                return null;
            }

            panelObject.transform.SetParent(fatherTransform,false);
            T panelScript = panelObject.GetComponent<T>();
            shownPanelDic.Add(panelName, panelScript);
            panelScript.ShowMe();
            // If this is a GameOverPanel, ensure its buttons are fixed/centered
            if (panelScript is GameOverPanel)
            {
                var goPanel = panelScript as GameOverPanel;
            }
            return panelScript;
        }

    }

    public T ShowPanel<T>(Transform father) where T:BasePanel
    {
        if(canvasTransform == null)
        {
            canvasTransform = GameObject.Find("Canvas").transform;
        }

        // 安全检查：确保指定的父对象存在且未被销毁
        if (father == null || father.gameObject == null)
        {
            Debug.LogWarning($"Specified father transform is null or destroyed, cannot show panel {typeof(T).Name}");
            return null;
        }

        // 检查父对象是否正在被销毁
        if (father.gameObject.activeInHierarchy == false)
        {
            Debug.LogWarning($"Specified father transform is being destroyed or inactive, cannot show panel {typeof(T).Name}");
            return null;
        }

        //此处得取的实际上是脚本的名字，但是我们需要通过实例化预设体的方式加载panel对象；
        //所以就必须要确保panel对象和挂载的脚本名称必须要一致；
        string panelName = typeof(T).Name;
        if (shownPanelDic.ContainsKey(panelName))
        {
            return shownPanelDic[panelName] as T;
        }
        else
        {
            string path = Path.Combine("Panels", panelName);
            Debug.Log(path);

            // 再次检查父对象在实例化前是否仍然有效
            if (father == null || father.gameObject == null)
            {
                Debug.LogError("Father transform became null before panel instantiation");
                return null;
            }

            GameObject panelObject = GameObject.Instantiate(Resources.Load<GameObject>(path), father, false);
            T panelScript = panelObject.GetComponent<T>();
            shownPanelDic.Add(panelName, panelScript);
            panelScript.ShowMe();
            return panelScript;
        }

    }

    //实现一个可以传入回调函数的ShowPanel方法：
    public T ShowPanel<T>(UnityAction action) where T : BasePanel
    {
        if(canvasTransform == null)
        {
            canvasTransform = GameObject.Find("Canvas").transform;
        }

        // 安全检查：确保父对象存在且未被销毁
        if (fatherTransform == null || fatherTransform.gameObject == null)
        {
            Debug.LogWarning("PanelFather is null or destroyed, attempting to find or recreate it");
            fatherTransform = GameObject.Find("PanelFather")?.transform;
            if (fatherTransform == null)
            {
                Debug.LogError("Cannot find PanelFather, unable to show panel with action");
                return null;
            }
        }

        // 检查父对象是否正在被销毁
        if (fatherTransform.gameObject.activeInHierarchy == false)
        {
            Debug.LogWarning($"PanelFather is being destroyed or inactive, cannot show panel {typeof(T).Name} with action");
            return null;
        }

        //此处得取的实际上是脚本的名字，但是我们需要通过实例化预设体的方式加载panel对象；
        //所以就必须要确保panel对象和挂载的脚本名称必须要一致；
        string panelName = typeof(T).Name;
        if (shownPanelDic.ContainsKey(panelName))
        {
            action?.Invoke();
            return shownPanelDic[panelName] as T;
        }
        else
        {
            string path = Path.Combine("Panels", panelName);
            GameObject panelObject = GameObject.Instantiate(Resources.Load<GameObject>(path));
            
            // 再次检查父对象在实例化后是否仍然有效
            if (fatherTransform == null || fatherTransform.gameObject == null)
            {
                Debug.LogError("PanelFather became null during panel instantiation, destroying created panel");
                GameObject.Destroy(panelObject);
                return null;
            }

            panelObject.transform.SetParent(fatherTransform, false);
            T panelScript = panelObject.GetComponent<T>();
            shownPanelDic.Add(panelName, panelScript);
            panelScript.ShowMe();
            // If this is a GameOverPanel, ensure its buttons are fixed/centered
            if (panelScript is GameOverPanel)
            {
                var goPanel = panelScript as GameOverPanel;
            }
            action?.Invoke();
            return panelScript;
        }

    }

    public T InstantShowPanel<T>() where T : BasePanel
    {
        if(canvasTransform == null)
        {
            canvasTransform = GameObject.Find("Canvas").transform;
        }

        // 安全检查：确保父对象存在且未被销毁
        if (fatherTransform == null || fatherTransform.gameObject == null)
        {
            Debug.LogWarning("PanelFather is null or destroyed, attempting to find or recreate it");
            fatherTransform = GameObject.Find("PanelFather")?.transform;
            if (fatherTransform == null)
            {
                Debug.LogError("Cannot find PanelFather, unable to instant show panel");
                return null;
            }
        }

        // 检查父对象是否正在被销毁
        if (fatherTransform.gameObject.activeInHierarchy == false)
        {
            Debug.LogWarning($"PanelFather is being destroyed or inactive, cannot instant show panel {typeof(T).Name}");
            return null;
        }

        //此处得取的实际上是脚本的名字，但是我们需要通过实例化预设体的方式加载panel对象；
        //所以就必须要确保panel对象和挂载的脚本名称必须要一致；
        string panelName = typeof(T).Name;
        if (shownPanelDic.ContainsKey(panelName))
        {
            return shownPanelDic[panelName] as T;
        }
        else
        {
            string path = Path.Combine("Panels", panelName);
            GameObject panelObject = GameObject.Instantiate(Resources.Load<GameObject>(path));
            
            // 再次检查父对象在实例化后是否仍然有效
            if (fatherTransform == null || fatherTransform.gameObject == null)
            {
                Debug.LogError("PanelFather became null during panel instantiation, destroying created panel");
                GameObject.Destroy(panelObject);
                return null;
            }

            panelObject.transform.SetParent(fatherTransform, false);
            T panelScript = panelObject.GetComponent<T>();
            shownPanelDic.Add(panelName, panelScript);
            return panelScript;
        }

    }


    public void HidePanel<T>(UnityAction callback = null) where T:BasePanel
    {
        string panelName = typeof(T).Name;
        if (shownPanelDic.ContainsKey(panelName))
        {
            T panelScript = shownPanelDic[panelName] as T;
            panelScript.HideMe(()=>
            {
                GameObject.Destroy(panelScript.gameObject);
                shownPanelDic.Remove(panelName);
                callback?.Invoke();
            });
            
        }

    }

    public void InstantHidePanel<T>(UnityAction callback = null) where T : BasePanel
    {
        string panelName = typeof(T).Name;
        if (shownPanelDic.ContainsKey(panelName))
        {
            T panelScript = shownPanelDic[panelName] as T;
            GameObject.Destroy(panelScript.gameObject);
            shownPanelDic.Remove(panelName);
            callback?.Invoke();

        }

    }


    public T GetPanel<T>() where T:BasePanel
    {
        string panelName = typeof(T).Name;
        if (!shownPanelDic.ContainsKey(panelName))
        {
            Debug.LogError("你要获取的面板尚未显示，错误出现在GetPanel方法上");
            return null;
        }
        return shownPanelDic[panelName] as T;

    }




    public void ErasePanelFromDic(string panelName)
    {
        if (shownPanelDic.ContainsKey(panelName))
            shownPanelDic.Remove(panelName);
    }


    private UIManager()
    {
        GameObject panelFatherGO = GameObject.Find("PanelFather");
        if (panelFatherGO == null)
        {
            Debug.LogWarning("PanelFather not found, creating new one");
            panelFatherGO = new GameObject("PanelFather");
        }
        fatherTransform = panelFatherGO.transform;
        GameObject.DontDestroyOnLoad(fatherTransform.gameObject);
    }

    /// <summary>
    /// 清理所有面板，在场景切换时调用
    /// </summary>
    public void CleanupAllPanels()
    {
        Debug.Log("Cleaning up all panels before scene transition");
        
        // 复制字典键，避免在遍历时修改字典
        var panelNames = new List<string>(shownPanelDic.Keys);
        
        foreach (string panelName in panelNames)
        {
            if (shownPanelDic.ContainsKey(panelName))
            {
                var panel = shownPanelDic[panelName];
                if (panel != null && panel.gameObject != null)
                {
                    GameObject.Destroy(panel.gameObject);
                }
                shownPanelDic.Remove(panelName);
            }
        }
        
        Debug.Log("All panels cleaned up");
    }

    /// <summary>
    /// 应用程序退出时的清理
    /// </summary>
    private void OnApplicationQuit()
    {
        CleanupAllPanels();
    }



}
