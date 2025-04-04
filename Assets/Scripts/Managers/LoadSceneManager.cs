using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

//Unity中已经实现了静态类SceneManager及其加载场景的API；这里我们为作区分，将管理器特殊命名；
public class LoadSceneManager : SingletonBaseManager<LoadSceneManager>
{
    private LoadSceneManager() { }

    //不管是加载什么场景，在那之前都先重置ballIndex；
    //注意；以后的游戏需要将这些重置的语句删除：

    //实现同步加载场景的方法：
    public void LoadSceneSync(string sceneName, UnityAction callback = null)
    {
        ResetAllStaticManagers();
        SceneManager.LoadScene(sceneName);
        callback?.Invoke();
    }

    //实现异步加载场景的方法：
    public void LoadSceneAsync(string sceneName, UnityAction callback = null)
    {
        ResetAllStaticManagers();
        MonoManager.Instance.StartCoroutine(LoadAsync(sceneName, callback));
        //这样写，不能保证异步结束之后再进行回调；回调在协程中触发可以保证这点
        //callback?.Invoke();
    }

    //重载：实现异步加载场景的方法：
    public void LoadSceneAsync(int sceneIndex, UnityAction callback = null)
    {
        ResetAllStaticManagers();
        MonoManager.Instance.StartCoroutine(LoadAsync(sceneIndex, callback));
        //这样写，不能保证异步结束之后再进行回调；回调在协程中触发可以保证这点
        //callback?.Invoke();
    }

    private IEnumerator LoadAsync(string sceneName, UnityAction callback)
    {
        AsyncOperation ao = SceneManager.LoadSceneAsync(sceneName);
        while (!ao.isDone)
        {
            //利用事件中心，发布事件：场景切换的进度
            EventHub.Instance.EventTrigger<float>("LoadSceneProgress", ao.progress);
            yield return 0; //该语句和yield return new WaitForSeconds(0f);是一个意思
        }//只要加载未完成，就每帧都不断执行循环，每次循环都会更新进度

        EventHub.Instance.EventTrigger<float>("LoadSceneProgress", 1f);
        //这一步是避免加载到1的这个动作因为过快被跳过了没有传出去；这一步就是把1写死了，绝对会传出去；
        //千万注意：一定要确保事件发布的时候，传入的参数类型要一致！这里前往不要写作整型1

        callback?.Invoke();

    }

    
    //重载：通过场景index加载场景；
    private IEnumerator LoadAsync(int sceneIndex, UnityAction callback)
    {
        AsyncOperation ao = SceneManager.LoadSceneAsync(sceneIndex);
        while (!ao.isDone)
        {
            //利用事件中心，发布事件：场景切换的进度
            EventHub.Instance.EventTrigger<float>("LoadSceneProgress", ao.progress);
            yield return 0; //该语句和yield return new WaitForSeconds(0f);是一个意思
        }//只要加载未完成，就每帧都不断执行循环，每次循环都会更新进度

        EventHub.Instance.EventTrigger<float>("LoadSceneProgress", 1f);
        //这一步是避免加载到1的这个动作因为过快被跳过了没有传出去；这一步就是把1写死了，绝对会传出去；
        //千万注意：一定要确保事件发布的时候，传入的参数类型要一致！这里前往不要写作整型1

        callback?.Invoke();

    }

    //重置所有的静态管理器：
    private void ResetAllStaticManagers()
    {
        EventHub.Instance.ClearListener();
    }



}
