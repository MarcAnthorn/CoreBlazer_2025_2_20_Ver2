using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;



//和事件中心一样，为了确保资源管理器类型的唯一性，使用封装+里氏替换的方式实现优化；
public abstract class ResInfoBase { }

public class ResInfo<T> : ResInfoBase
{
    //泛型记录资源类型
    public T asset;

    //用于异步加载结束后，传递已加载资源到外部的委托
    public UnityAction<T> callback;

    //当出现同步加载和异步加载同时发生且加载同一个资源的时候，需要停止异步加载
    //我们因此需要一个协程返回值类型的字段来存储异步加载开启时开启的协程；
    public Coroutine coroutine;

}

public class ResourcesManager : SingletonBaseManager<ResourcesManager>
{
    private ResourcesManager() { }

    //声明一个字典实现加载过/加载中的资源的记录
    private Dictionary<string, ResInfoBase> resDic = new Dictionary<string, ResInfoBase>();
    //注意字典的string是路径+资源类型拼接的，防止出现同名资源；

    public void LoadAsync<T>(string path, UnityAction<T> callback) where T:Object
    {
        string key = path + "_" + typeof(T).Name;
        if (!resDic.ContainsKey(key))
        {
            //如果没有相应的资源信息，开启协程加载资源
            //在开启协程之前，先声明一个资源记录数据结构类：
            ResInfo<T> resInfo = new ResInfo<T>();
            //加入字典中
            resDic.Add(key, resInfo);
            //记录传入的委托函数：
            resInfo.callback += callback;

            //开启协程：并且记录协程，用于之后可能会停止该协程；
            //由于callback已经被记录入字典中，所以此处不需要callback作为参数了：
            resInfo.coroutine = MonoManager.Instance.StartCoroutine(RealLoadAsync<T>(path));
        }
        else    //如果字典中已经存在有相关的资源记录了，该怎么办？
        {
            ResInfo<T> info = resDic[key] as ResInfo<T>;
            //如果该资源还没加载完
            if (info.asset == null)  //asset为空，就说明还没加载完毕；也就是说asset还没有被赋值；
            {
                //这步的意思就是：既然你的asset还没有被赋值，说明你其中的回调函数还没有被启用；
                //既然这样的话，那我就直接把传入的委托加入你还没有被调用委托中去，两个一口气执行，避免再次调用协程；
                info.callback += callback;
            }
            else    //已经加载完毕过了的逻辑
            {
                //已经加载过了？那就好办了
                //说明你其中的asset已经是存储了资源的了，那我就直接调用外部传入的回调函数，将该资源传递出去就行，省得再进行一次协同程序的加载；
                callback?.Invoke(info.asset);
                
            }
        }

    }


    private IEnumerator RealLoadAsync<T>(string path) where T: Object
    {
        ResourceRequest resourceRequest = Resources.LoadAsync<T>(path);
        yield return resourceRequest;

        string key = path + "_" + typeof(T).Name;
        //资源加载结束后，如果存在资源记录：
        if (resDic.ContainsKey(key))
        {
            ResInfo<T> resInfo = resDic[key] as ResInfo<T>;
            //取出对应的资源信息；并且记录加载完成的资源
            resInfo.asset = resourceRequest.asset as T;
            //调用回调函数；
            resInfo.callback?.Invoke(resInfo.asset);

            //任务完成了，将这个resInfo的相关内容全部置空，防止可能的内存泄漏；
            //不清空asset！因为这个是已加载资源，在上面的直接激活回调函数中用得上！
            resInfo.callback = null;
            resInfo.coroutine = null;
        }
    }

    //顺带封装一个同步加载的方法
    public T Load<T>(string path) where T :Object
    {
        return Resources.Load<T>(path);
    }




    public void UnloadUnusedAssets()
    {
        MonoManager.Instance.StartCoroutine(RealUnloadUnusedAssets());
    }

    private IEnumerator RealUnloadUnusedAssets()
    {
        AsyncOperation asyncOperation = Resources.UnloadUnusedAssets();
        yield return asyncOperation;
    }

}
