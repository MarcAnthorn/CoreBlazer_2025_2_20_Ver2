using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


//这是一个基于观察者模式+单例模式实现的事件中心模块；
//可以通过发布+订阅的方式，进行脚本之间的方法调用


//声明一个抽象类
public abstract class EventInfoBaseClass { }


//事件中心的数据容器单元（含参数）
//注意：这两步要看懂，为什么不直接声明一个泛型类作为下方字典的value类型，而要声明一个抽象类作为字典的value类型，然后去用里氏替换原则装载这个泛型类？
//本质上是为了保证事件中心模块的唯一性，即全局只有一类事件中心模块；
public class EventInfo<T> : EventInfoBaseClass
{
    public UnityAction<T> action_;

    //构造函数，便于外部实例化时顺带传入监听方法；
    public EventInfo(UnityAction<T> actions)
    {
        action_ += actions;
    }
}

// 新增：支持两个参数的事件容器类
public class EventInfo<T1, T2> : EventInfoBaseClass
{
    public UnityAction<T1, T2> action_;

    public EventInfo(UnityAction<T1, T2> actions)
    {
        action_ += actions;
    }
}




//事件中心的数据容器单元（不含参数）
public class EventInfoNoPara : EventInfoBaseClass
{
    public UnityAction action_;

    public EventInfoNoPara(UnityAction action)
    {
        action_ += action;
    }
}


//事件中心模块：继承自单例模式基类

public class EventHub : SingletonBaseManager<EventHub>
{
    private EventHub() { }

    internal void EventTrigger<T>()
    {
        throw new NotImplementedException();
    }

    //如果事件中心模块这样定义，同时字典这样写：
    //public class EventHub<T> : SingletonBaseManager<EventHub<T>>
    //private Dictionary<string, EventInfo<T>> eventDictionary = new Dictionary<string, EventInfo<T>>();
    //实际上就是违背了全局只有一种事件中心模块的规则；
    //因为对于每个传入的T类型的数据，实际上都是一个全新类型的事件中心模块；
    //因此，我们选择使用一个非泛型的抽象类作为字典的value，同时使用里氏替换原则，利用抽象类对象来装载其泛型子类的实例；
    //这样就不会出现因为泛型 而被迫违背事件中心的唯一性了；

    private Dictionary<string, EventInfoBaseClass> eventDictionary = new Dictionary<string, EventInfoBaseClass>();

    public void EventTrigger<T>(string eventName, T info)
    {
        if (eventDictionary.ContainsKey(eventName))
        {
            (eventDictionary[eventName] as EventInfo<T>).action_?.Invoke(info);      //info被作为参数传入；此时所有订阅者的监听方法都会收到该参数；
        }
    }


    //双参数的事件：
    public void EventTrigger<T1, T2>(string eventName, T1 arg1, T2 arg2)
    {
        if (eventDictionary.ContainsKey(eventName))
        {
            (eventDictionary[eventName] as EventInfo<T1, T2>)?.action_?.Invoke(arg1, arg2);
        }
    }



    //支持多播委托的事件订阅；
    public void AddEventListener<T>(string eventName, UnityAction<T> function)
    {
        if (eventDictionary.ContainsKey(eventName))
        {
            if(eventDictionary == null)
                Debug.LogWarning("add eventDictionary is null");

            if(function == null)
                Debug.LogWarning("add function is null");

            (eventDictionary[eventName] as EventInfo<T>).action_ += function;  //as父转子实现子类成员委托调用
        }
        else
        {
            eventDictionary.Add(eventName, new EventInfo<T>(function));     //里氏替换父装载子类实例
        }
    }

    //重载：不支持多播委托的事件订阅；
    public void AddEventListenerNotMultuple<T>(string eventName, UnityAction<T> function)
    {
        if (eventDictionary.ContainsKey(eventName) && (eventDictionary[eventName] as EventInfo<T>).action_ == null)
            (eventDictionary[eventName] as EventInfo<T>).action_ += function;  //as父转子实现子类成员委托调用
        else
        {
            eventDictionary.Add(eventName, new EventInfo<T>(function));     //里氏替换父装载子类实例
        }
    }


    //多参数事件监听：
    public void AddEventListener<T1, T2>(string eventName, UnityAction<T1, T2> function)
    {
        if (eventDictionary.ContainsKey(eventName))
        {
            (eventDictionary[eventName] as EventInfo<T1, T2>).action_ += function;
        }
        else
        {
            eventDictionary.Add(eventName, new EventInfo<T1, T2>(function));
        }
    }




    //从特定key下指定移除订阅的方法
    public void RemoveEventListener<T>(string eventName, UnityAction<T> function)
    {
        if (eventDictionary.ContainsKey(eventName))
            (eventDictionary[eventName] as EventInfo<T>).action_ -= function;   //根据里氏替换原则，就算父类对象中装载了子类实例，要想访问子类中的成员，也必须要使用as进行转换
    }

    //双参数的移除事件监听：
    public void RemoveEventListener<T1, T2>(string eventName, UnityAction<T1, T2> function)
    {
        if (eventDictionary.ContainsKey(eventName))
        {
            (eventDictionary[eventName] as EventInfo<T1, T2>).action_ -= function;
        }
    }



    //清楚所有订阅的方法
    public void ClearListener()
    {
        eventDictionary.Clear();
    }


    //移除特定key的所有订阅事件
    public void ClearListener(string eventName)
    {
        if (eventDictionary.ContainsKey(eventName))
            eventDictionary.Remove(eventName);
    }




    //所有订阅事件和发布事件的无参的重载
    public void EventTrigger(string eventName)  //默认为null，这样外部就算没有信息可交流， 不传参，也不会报错；
    {
        if (eventDictionary.ContainsKey(eventName))
        {
            (eventDictionary[eventName] as EventInfoNoPara).action_?.Invoke();      //info被作为参数传入；此时所有订阅者的监听方法都会收到该参数；
        }
    }

    public void AddEventListener(string eventName, UnityAction function)
    {
        if (eventDictionary.ContainsKey(eventName))
            (eventDictionary[eventName] as EventInfoNoPara).action_ += function;  //as父转子实现子类成员委托调用
        else
        {
            eventDictionary.Add(eventName, new EventInfoNoPara(function));     //里氏替换父装载子类实例
        }
    }

    public void RemoveEventListener(string eventName, UnityAction function)
    {
        if (eventDictionary.ContainsKey(eventName))
            (eventDictionary[eventName] as EventInfoNoPara).action_ -= function;   //根据里氏替换原则，就算父类对象中装载了子类实例，要想访问子类中的成员，也必须要使用as进行转换
    }


    public void PrintKeys()
    {
        foreach(var key in eventDictionary.Keys){
            Debug.LogWarning(key);
        }
    }


}






