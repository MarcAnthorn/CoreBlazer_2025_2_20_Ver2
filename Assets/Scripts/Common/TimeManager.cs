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

        public float startTime;
        public float elapsedTime;
        public bool isStart;
        public bool isSuspend;
        public bool itemStartWork;

        public Timer(float duration = 0, Action OnStart = null, Action OnComplete = null)
        {
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
            if (duration > 0f && OnComplete != null)
            {
                OnComplete();
                TimeManager.Instance.SingleTimerOver(timerKey);
                TimeManager.Instance.RemoveTimer(timerKey);
            }
        }

    }

    private static int index = -1;
    private Dictionary<int, Timer> timers = new Dictionary<int, Timer>();

    private float gameStartTime;
    private float gameTime;

    public int AddTimer(float duration, Action OnStart, Action OnComplete)
    {
        index++;
        timers.TryAdd(index, new Timer(duration, OnStart, OnComplete));

        return index;               //告诉外界当前索引的值是多少
    }

    //Marc添加：
    public void StartAllTimers()
    {
        foreach(Timer timer in timers.Values)
        {
            timer.isStart = true;
        }
    }
    public void RemoveTimer(int key)
    {
        timers.Remove(key);
        index--;
    }
    public void SuspendAllTimers()                         //进入事件 或者 游戏暂停时调用
    {
        foreach(Timer timer in timers.Values)
        {
            timer.isSuspend = true;
        }
    }
    public void ContinueAllTimers()                         //游戏恢复时调用
    {
        foreach (Timer timer in timers.Values)
        {
            timer.isSuspend = false;
        }
    }
    public void FinishAllTimers()                           //游戏结束时调用
    {
        foreach (Timer timer in timers.Values)
        {
            timer.isStart = false;
            timer.isSuspend = false;
        }
        timers.Clear();
        index = -1;
    }

    public float GetGameTime()
    {
        return gameTime;
    }

    void Start()
    {
        gameStartTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        float gameTime = Time.time - gameStartTime;

        foreach (var pair in timers)
        {
            if (pair.Value.isStart && !pair.Value.isSuspend)
            {
                pair.Value.elapsedTime = Time.time - pair.Value.startTime;
                pair.Value.CheckItemStart();
                pair.Value.CheckItemEnd(pair.Key);
            }
        }

    }

    public void SingleTimerStart(int key)
    {
        Timer timer = timers[key];
        timer.startTime = Time.time;
        timer.isStart = true;
    }
    public void SingleTimerSuspend(int key)
    {
        Timer timer = timers[key];
        timer.isSuspend = true;
    }
    public void SingleTimerContinue(int key)
    {
        Timer timer = timers[key];
        timer.isSuspend = false;
    }
    public void SingleTimerOver(int key)
    {
        Timer timer = timers[key];
        timer.isStart = false;
        timer.isSuspend = false;
    }
    public float GetSingleTime(int key)
    {
        Timer timer = timers[key];
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
