using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIManager : SingletonBaseManager<UIManager>
{

    public Transform canvasTransform;

    private Dictionary<string, BasePanel> shownPanelDic = new Dictionary<string, BasePanel>();

    public T ShowPanel<T>() where T:BasePanel
    {
        //此处得取的实际上是脚本的名字，但是我们需要通过实例化预设体的方式加载panel对象；
        //所以就必须要确保panel对象和挂载的脚本名称必须要一致；
        string panelName = typeof(T).Name;
        if (shownPanelDic.ContainsKey(panelName))
        {
            return shownPanelDic[panelName] as T;
        }
        else
        {
            string path = "Panels/" + panelName;
            GameObject panelObject = GameObject.Instantiate(Resources.Load<GameObject>(path));
            panelObject.transform.SetParent(canvasTransform,false);
            T panelScript = panelObject.GetComponent<T>();
            shownPanelDic.Add(panelName, panelScript);
            panelScript.ShowMe();
            return panelScript;
        }

    }

    public T ShowPanel<T>(Transform father) where T:BasePanel
    {
        //此处得取的实际上是脚本的名字，但是我们需要通过实例化预设体的方式加载panel对象；
        //所以就必须要确保panel对象和挂载的脚本名称必须要一致；
        string panelName = typeof(T).Name;
        if (shownPanelDic.ContainsKey(panelName))
        {
            return shownPanelDic[panelName] as T;
        }
        else
        {
            string path = "Panels/" + panelName;
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
        //此处得取的实际上是脚本的名字，但是我们需要通过实例化预设体的方式加载panel对象；
        //所以就必须要确保panel对象和挂载的脚本名称必须要一致；
        string panelName = typeof(T).Name;
        if (shownPanelDic.ContainsKey(panelName))
        {
            return shownPanelDic[panelName] as T;
        }
        else
        {
            string path = "Panels/" + panelName;
            GameObject panelObject = GameObject.Instantiate(Resources.Load<GameObject>(path));
            panelObject.transform.SetParent(canvasTransform, false);
            T panelScript = panelObject.GetComponent<T>();
            shownPanelDic.Add(panelName, panelScript);
            panelScript.ShowMe();
            action?.Invoke();
            return panelScript;
        }

    }

    public T InstantShowPanel<T>() where T : BasePanel
    {
        //此处得取的实际上是脚本的名字，但是我们需要通过实例化预设体的方式加载panel对象；
        //所以就必须要确保panel对象和挂载的脚本名称必须要一致；
        string panelName = typeof(T).Name;
        if (shownPanelDic.ContainsKey(panelName))
        {
            return shownPanelDic[panelName] as T;
        }
        else
        {
            string path = "Panels/" + panelName;
            GameObject panelObject = GameObject.Instantiate(Resources.Load<GameObject>(path));
            panelObject.transform.SetParent(canvasTransform, false);
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
        canvasTransform = GameObject.Find("Canvas").transform;
        GameObject.DontDestroyOnLoad(canvasTransform.gameObject);
    }



}
