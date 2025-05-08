using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//注意：此类和下面的对象池管理器声明在同一个脚本中，这是一个数据类；
public class PoolData
{
    //用来存储球筒中的对象的栈对象
    private Stack<GameObject> dataStack = new Stack<GameObject>();

    //球筒根对象，用于归纳所有的分类的池中对象；
    private GameObject objRoot;
    //注意以上两个对象都是私有的，如果想要跨类访问数据，正常情况下是不可以的；
    //与其将以上两个成员声明为public,不如使用公共属性来进行访问；
    public int Count => dataStack.Count;

    private bool isOptimized;

    //提供方法：Pop；用于出栈：
    public GameObject Pop()
    {
        return dataStack.Pop(); //调用API，并且返回对象；
    }

    //提供方法：Push：用于轧入对象入池；
    public void Push(GameObject obj)
    {
        if (obj == null)
            Debug.LogError("当前希望push入对象栈的对象为空，请检查！");

        if (objRoot == null)
            Debug.LogError("当前希望挂载对象的父对象Root为空，请检查！");

        //开启优化，再建立父子关系；否则不建立
        if (isOptimized)
            obj.transform.SetParent(objRoot.transform);
        dataStack.Push(obj);
    }


    //声明一个含参的构造函数；参数：传入柜子的根对象，方便我们设置根对象之间的父子关系；
    public PoolData(GameObject root, string key_,bool isOptimized_)
    {
        //使用字典中Stack对应的key作为根对象名；
        objRoot = new GameObject(key_ + "Root");
        isOptimized = isOptimized_;
        //开启优化，再设置父子关系；
        if (isOptimized)
            objRoot.transform.SetParent(root.transform);
    }

}


public class PoolManager : SingletonBaseManager<PoolManager>
{
    private PoolManager() { }

    //经过优化，下方的字典的value从Stack变为类封装了Stack的数据结构类；
    private Dictionary<string, PoolData> poolDictionary = new Dictionary<string, PoolData>();

    //创建池子的根对象
    private GameObject poolRoot;

    //布尔类型，用于判断是否开启窗口优化功能；默认开启；
    public static bool isOptimized = true;

    public GameObject SpawnFromPool(string key, Vector3 position_, Quaternion rotation_)
    {
        GameObject objFromPool;

        //注意：经过优化，if中的Count不再是Stack的直接API调用；而是PoolData数据类的只读属性；两者只有数值是相等的；
        if (poolDictionary.ContainsKey(key) && poolDictionary[key].Count > 0)
        {
            //经过优化，下方的Pop不再是Stack API的直接调用，而是类中的一个公共方法；
            objFromPool = poolDictionary[key].Pop();
            objFromPool.SetActive(true);
            //取用对象时，断开父子关系；也就是说被独立；
            if (isOptimized)
                objFromPool.transform.SetParent(null);
        }
        else
        {
            objFromPool = GameObject.Instantiate(Resources.Load<GameObject>(key));

        }
        objFromPool.transform.position = position_;
        objFromPool.transform.rotation *= rotation_;
        return objFromPool;
    }

    public GameObject SpawnFromPool(string key)
    {
        GameObject objFromPool;

        //注意：经过优化，if中的Count不再是Stack的直接API调用；而是PoolData数据类的只读属性；两者只有数值是相等的；
        if (poolDictionary.ContainsKey(key) && poolDictionary[key].Count > 0)
        {
            //经过优化，下方的Pop不再是Stack API的直接调用，而是类中的一个公共方法；
            objFromPool = poolDictionary[key].Pop();
            objFromPool.SetActive(true);
            //取用对象时，断开父子关系；也就是说被独立；
            if (isOptimized)
                objFromPool.transform.SetParent(null);
        }
        else
        {
            objFromPool = GameObject.Instantiate(Resources.Load<GameObject>(key));

        }
        return objFromPool;
    }

     public GameObject SpawnFromPool(string key, Transform father)
    {
        GameObject objFromPool;

        //注意：经过优化，if中的Count不再是Stack的直接API调用；而是PoolData数据类的只读属性；两者只有数值是相等的；
        if (poolDictionary.ContainsKey(key) && poolDictionary[key].Count > 0)
        {
            //经过优化，下方的Pop不再是Stack API的直接调用，而是类中的一个公共方法；
            objFromPool = poolDictionary[key].Pop();
            objFromPool.SetActive(true);
            //取用对象时，断开父子关系；也就是说被独立；
            if (isOptimized)
                objFromPool.transform.SetParent(null);

            objFromPool.transform.SetParent(father, false);
            
        }
        else
        {
            objFromPool = GameObject.Instantiate(Resources.Load<GameObject>(key), father, false);

        }
        return objFromPool;
    }



    public void ReturnToPool(string key, GameObject objToPool)
    {
        //当对象要被归纳时，若根对象为空，并且开启了优化功能，则创建根对象；
        if (poolRoot == null && isOptimized)
            poolRoot = new GameObject("Pool");   //此语句表示的是创建一个以“Pool”命名的空物体；

        if (objToPool == null)
            Debug.LogError("当前希望加入对象池的对象为空，请进行检查！");
        objToPool.SetActive(false);

        if (poolDictionary.ContainsKey(key))
        {
            //Push同样也是PoolData的方法，而非API；
            poolDictionary[key].Push(objToPool);
        }
        else
        {
            //注意；优化后，实例化的将不再是Stack，而是数据结构类PoolData，并且传入其根对象作为父对象；
            poolDictionary.Add(key, new PoolData(poolRoot, key,isOptimized));
            poolDictionary[key].Push(objToPool);
        }
    }

    public void ClearPool()
    {
        poolDictionary.Clear();
        //过场景手动清除poolRoot根对象；保证在新场景中的对象是重新声明的对象；
        poolRoot = null;
    }
}
