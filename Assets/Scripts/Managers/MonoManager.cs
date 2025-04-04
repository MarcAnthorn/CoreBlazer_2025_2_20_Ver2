using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


//公共Mono模块实现
public class MonoManager : Singleton<MonoManager>
{
    //实现生命周期函数
    private void Update()
    {
        updateEvent?.Invoke();
    }

    private void FixedUpdate()
    {
        fixedUpdateEvent?.Invoke();
    }

    private void LateUpdate()
    {
        lateUpdateEvent?.Invoke();
    }

    //外部如何实现变相调用内部的Update等函数呢？
    //解决方法：使用事件或者委托，存储外部需要执行的方法；
    //命名空间：UnityEngine.Events; UnityAction是无参数无返回值的事件类型；
    private event UnityAction updateEvent;      //知识回顾！！事件的声明是  访问修饰符 event 委托类型 事件名 该事件不需要引用实例化也可以使用！
    private event UnityAction fixedUpdateEvent;
    private event UnityAction lateUpdateEvent;

    //提供方法供外部使用，实现外部向事件内部添加方法；
    //为事件添加方法，一般被称作是事件的订阅；此时的外部类，一般被称作事件的订阅者( Subscriber 有时也被称作观察者、监听器)；
    //而公共模块作为事件的发出者，也会被称作是发布者( Publisher 有时也被称作被观察者、主题)；发布者和订阅者之间的联系，就是事件；
    //补充说明：该公共Mono模块通过事件的方式执行外部类的方法，本质上就是一种很好的解耦的方式；每个类只需要关心自己的东西，而不需要关心别的类是否需要修改；
    //以上本质上就是 设计模式之一：观察者模式( Observer Pattern )的一种体现；

    public void AddUpdateListener(UnityAction updateFunction)
    {
        updateEvent += updateFunction;      //本质就是事件，也就是特殊的多播委托的方法添加；
    }
    public void AddFixedUpdateListener(UnityAction fixedUpdateFunction)
    {
        updateEvent += fixedUpdateFunction;
    }
    public void AddLateUpdateListener(UnityAction lateUpdateFunction)
    {
        updateEvent += lateUpdateFunction;
    }

    //实现为外部移除方法的方法：
    public void RemoveUpdateListener(UnityAction updateFunction)
    {
        updateEvent -= updateFunction;
    }
    public void RemoveFixedUpdateListener(UnityAction fixedUpdateFunction)
    {
        updateEvent -= fixedUpdateFunction;
    }
    public void RemoveLateUpdateListener(UnityAction lateUpdateFunction)
    {
        updateEvent -= lateUpdateFunction;
    }

    //对于协同程序而言，其实没必须再在Manager内部声明任何的协同程序开启/关闭函数了
    //我们可以在外部通过单例模式直接使用MonoBehaviour中自带的协同程序开启/关闭函数；
}

