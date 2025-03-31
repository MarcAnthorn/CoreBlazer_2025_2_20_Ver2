using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Timers : Singleton<Timers>
{

    private class Timer
    {
        public Action OnComplete;
        public float Duration;

        public float startTime;
        public float elapsedTime;
        public bool isStart;
        public bool isSuspend;

        public Timer(float duration = 0, Action OnComplete = null)
        {
            isStart = false;
            isSuspend = false;
            this.OnComplete = OnComplete;
        }

    }

    private int index = -1;
    private Dictionary<int, Timer> timers = new Dictionary<int, Timer>();

    private float gameStartTime;
    private float gameTime;

    public int AddTimer()
    {
        index++;
        timers.Add(index, new Timer());

        return index;
    }
    public void RemoveTimer(int key)
    {
        timers.Remove(key);
        index--;
    }
    public void StopAllTimers()                         //进入事件 或者 游戏暂停时调用
    {
        foreach(Timer timer in timers.Values)
        {
            timer.isSuspend = true;
        }
    }
    public void ContinueAllTimers()                         //进入事件 或者 游戏暂停时调用
    {
        foreach (Timer timer in timers.Values)
        {
            timer.isSuspend = false;
        }
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

        foreach(Timer timer in timers.Values)
        {
            if (timer.isStart && !timer.isSuspend)
            {
                timer.elapsedTime = Time.time - timer.startTime;
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
