using System;
using System.Diagnostics;
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour  //单例模式，作父类被继承
{
    private static T _instance;
    private static object _lock = new object();

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = FindAnyObjectByType<T>();

                        if (_instance == null)
                        {
                            GameObject obj = new GameObject(typeof(T).Name);
                            _instance = obj.AddComponent<T>();
                            DontDestroyOnLoad(obj);
                        }
                    }
                }
            }
            return _instance;
        }
    }

    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
    }

    protected virtual void OnDestroy()
    {
        if (_instance != this)
        {
            _instance = null;
        }
    }

}