using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Timers : Singleton<Timers>
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
                Timers.Instance.SingleTimerOver(timerKey);
                Timers.Instance.RemoveTimer(timerKey);
            }
        }

    }

    private static int index = -1;
    private Dictionary<int, Timer> timers = new Dictionary<int, Timer>();

    private float gameStartTime;
    private float gameTime;

    public int AddTimer()
    {
        index++;
        timers.TryAdd(index, new Timer());

        return index;               //������統ǰ������ֵ�Ƕ���
    }
    public void RemoveTimer(int key)
    {
        timers.Remove(key);
        index--;
    }
    public void SuspendAllTimers()                         //�����¼� ���� ��Ϸ��ͣʱ����
    {
        foreach(Timer timer in timers.Values)
        {
            timer.isSuspend = true;
        }
    }
    public void ContinueAllTimers()                         //��Ϸ�ָ�ʱ����
    {
        foreach (Timer timer in timers.Values)
        {
            timer.isSuspend = false;
        }
    }
    public void FinishAllTimers()                           //��Ϸ����ʱ����
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
            Debug.LogWarning("��û�п�ʼ��ʱ��");
            return -1;
        }
    }

}
