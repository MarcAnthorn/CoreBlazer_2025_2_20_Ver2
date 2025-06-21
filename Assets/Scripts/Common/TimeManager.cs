using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TimeManager : Singleton<TimeManager>
{

    private class Timer
    {
        public Action OnStart;
        public Action OnComplete;
        public float duration;
        public int index;

        public float startTime;
        public float elapsedTime;

        //是否启动；一旦启动之后，该布尔变量就不会回归false
        public bool isStart;

        //是否悬挂；悬挂之后，计时会暂停；
        public bool isSuspend;
        public bool itemStartWork;

        public Timer(int _index, float _duration , Action OnStart = null, Action OnComplete = null)
        {
            index = _index;
            duration = _duration;
            isStart = false;
            isSuspend = false;
            itemStartWork = false;
            this.OnStart = OnStart;
            this.OnComplete = OnComplete;
        }

        public void CheckItemStart()
        {
            if (!itemStartWork && OnStart != null)
            {
                OnStart();
                itemStartWork = true;
            }
        }

        public void CheckItemEnd(int timerKey)
        {
            //如果运行累计的时间达到了我们设定的持续时间，则执行结束；
            if (elapsedTime >= duration && OnComplete != null)
            {
                OnComplete();
                TimeManager.Instance.SingleTimerOver(timerKey);
                TimeManager.Instance.RemoveTimer(timerKey);
            }
        }

    }

    private static int index = -1;
    private Dictionary<int, Timer> timersDic = new Dictionary<int, Timer>();

    private float gameStartTime;
    private float gameTime;
    private float interval = 0.1f;

    //协程句柄：
    private Coroutine coroutine;

    public int AddTimer(float duration, Action OnStart, Action OnComplete)
    {
        index++;
        timersDic.TryAdd(index, new Timer(index, duration, OnStart, OnComplete));

        return index;               //告诉外界当前索引的值是多少
    }

    //Marc添加：
    public void StartAllTimers()
    {
        foreach(Timer timer in timersDic.Values)
        {
            timer.isStart = true;
            timer.isSuspend = false;
        }
    }
    //直接从计时器中删除道具，不触发道具结束的回调的方法
    public void RemoveTimer(int key)
    {
        if(key == -1)   //无效key
            return;

        if(timersDic.ContainsKey(key))
        {
            var timer = timersDic[key];
            listToDelete.Add(timer);
        }
    
    }
    public void SuspendAllTimers()                         //进入事件 或者 游戏暂停时调用
    {
        foreach(Timer timer in timersDic.Values)
        {
            timer.isSuspend = true;
        }
    }
    public void ContinueAllTimers()                         //游戏恢复时调用
    {
        foreach (Timer timer in timersDic.Values)
        {
            timer.isSuspend = false;
        }
    }
    public void FinishAllTimers()                           //游戏结束时调用
    {
        foreach (Timer timer in timersDic.Values)
        {
            timer.isStart = false;
            timer.isSuspend = false;
        }
        timersDic.Clear();
        index = -1;
    }

    // public float GetGameTime()
    // {
    //     return gameTime;
    // }

    void Start()
    {
        // gameStartTime = Time.time;

        //开启协程：
        coroutine = StartCoroutine(CoroutineContent());
    }

    // Update is called once per frame
    // void Update()
    // {
    //     float gameTime = Time.time - gameStartTime;

    //     foreach (var pair in timersDic)
    //     {
    //         if (pair.Value.isStart && !pair.Value.isSuspend)
    //         {
    //             pair.Value.elapsedTime = Time.time - pair.Value.startTime;
    //             pair.Value.CheckItemStart();
    //             pair.Value.CheckItemEnd(pair.Key);
    //         }
    //     }
    // }

    private List<Timer> listToDelete = new List<Timer>();

    private IEnumerator CoroutineContent()
    {
        while(true)
        {
            yield return new WaitForSeconds(interval);
            //遍历所有仍在运作的TimeItem类
            foreach(var item in timersDic.Values)
            {
                if(!item.isStart || item.isSuspend)
                    continue;   //该事件类不参与运作了，那就跳过，直接访问下一个
        
                //进行是否需要启动的检查：
                item.CheckItemStart();

                //进行时间的累减：
                item.duration -= interval;     
                if(item.duration <= 0)
                {
                    item.OnComplete?.Invoke();
                    //由于仍在进行遍历，所以此时不可以直接将该item从Dictionary中移除；
                    //使用老办法：将所有符合删除条件的item记录入一个待删除列表；
                    listToDelete.Add(item);
                }

            }
            for(int i = 0;i < listToDelete.Count; i++)
            {
                timersDic.Remove(listToDelete[i].index);
                listToDelete[i] = null; //置空，防止出现内存泄漏；
            }
            listToDelete.Clear();
            
        }

    }  

    

    public void SingleTimerStart(int key)
    {
        Timer timer = timersDic[key];
        timer.startTime = Time.time;
        timer.isStart = true;
    }
    public void SingleTimerSuspend(int key)
    {
        Timer timer = timersDic[key];
        timer.isSuspend = true;
    }
    public void SingleTimerContinue(int key)
    {
        Timer timer = timersDic[key];
        timer.isSuspend = false;
    }
    public void SingleTimerOver(int key)
    {
        Timer timer = timersDic[key];
        timer.isStart = false;
        timer.isSuspend = false;
    }
    public float GetSingleTime(int key)
    {
        Timer timer = timersDic[key];
        if (timer.isStart)
        {
            return timer.elapsedTime;
        }
        else
        {
            Debug.LogWarning("还没有开始计时！");
            return -1;
        }
    }

}
