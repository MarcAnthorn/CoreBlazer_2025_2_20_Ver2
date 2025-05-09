using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

public class SoundEffectManager : SingletonBaseManager<SoundEffectManager>
{
    private SoundEffectManager()
    {

        //使用MonoManager中的Update进行容器遍历检测：是否有播放完成的音效？播放完成就删除；
        MonoManager.Instance.AddUpdateListener(sourceUpdate);
        //自该管理器实例化之后，就开始执行检测；

    }
    //需要一个背景音乐的组件
    private AudioSource musicSource = null;
    //声明List容器管理所有AudioSource
    List<AudioSource> soundList = new List<AudioSource>();

    //使用对象池解决音效的删除和增添问题；
    //由于对象池需要从预设体中进行实例化取用对象，我们需要制造预设体：soundObj;其中挂载AudioSource组件；

    //是否对当前场景的播放完毕的音效对象进行移除；
    //默认是移除的；
    private bool soundRemove = true;

    private float musicVolume = 0.5f;
    private float soundEffectVolume = 0.5f;
    public float  SoundEffectVolume => soundEffectVolume;

    //播放BGM
    public void PlayMusic(string musicPath)
    {
        string rootPath = Path.Combine("BGM", musicPath);

        if (musicSource == null)
        {
            GameObject musicCarrier = new GameObject(musicPath);
            musicSource = musicCarrier.AddComponent<AudioSource>();

            // musicCarrier.gameObject.SetActive(false);
            // musicCarrier.gameObject.SetActive(true);

            GameObject.DontDestroyOnLoad(musicCarrier);
        }
        ResourcesManager.Instance.LoadAsync<AudioClip>(rootPath, (clip_) =>
        {
            musicSource.clip = clip_;
            musicSource.loop = true;
            musicSource.volume = musicVolume;
            musicSource.Play();
        });

    }

    //停止BGM
    public void StopMusic()
    {
        if (musicSource == null)
        {
            Debug.Log("AudioSource未关联目标背景音乐，请处理");
            return;
        }
        musicSource.Stop();
    }
    //暂停BGM
    public void PauseMusic()
    {
        if (musicSource == null)
        {
            Debug.Log("AudioSource未关联目标背景音乐，请处理");
            return;
        }
        musicSource.Pause();
    }
    //设置BGM大小
    public void SetMusicVolume(float volume_)
    {
        musicSource.volume = volume_;
        musicVolume = volume_;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="soundPath">音乐的相对路径</param>
    /// <param name="isLoop"></param>
    /// <param name="callBack"></param>
    public void PlaySoundEffect(string soundPath, bool isLoop = false, UnityAction<AudioSource> callBack = null)
    {

        string rootPath = Path.Combine("BGM", soundPath);
        ResourcesManager.Instance.LoadAsync<AudioClip>(rootPath, (clip_) =>
        {
            //从对象池中取用音效组件对象soundObj:
            AudioSource soundSource = PoolManager.Instance.SpawnFromPool("Sound/soundObj",Vector3.zero,Quaternion.identity).GetComponent<AudioSource>();
            //如果是上限优化版本的对池，需要将改组件上的可能正在播放的clip暂停播放
            soundSource.loop = isLoop;
            soundSource.clip = clip_;
            soundSource.volume = soundEffectVolume;

            //注意：如果是上限优化版本的对象池，因此还没被轧入池子的对象又被再次取用了；确保List中没有相同的AudioSource组件再加入列表
             if (!soundList.Contains(soundSource))
                soundList.Add(soundSource);

            //调用回调函数传出去了AudioSource，那么直接在外部通过改组件获取音效对象，再调整父对象就可以了；
            soundSource.gameObject.SetActive(false);
            soundSource.gameObject.SetActive(true);
            callBack?.Invoke(soundSource);

        });
    }

    //用于执行检测音效播放完毕的方法；
    private void sourceUpdate()
    {
        if (!soundRemove)
            return;
        for (int i = soundList.Count - 1; i >= 0; i--)
        {
            if (soundList[i] != null && !soundList[i].isPlaying)
            {
                //以防万一，置空；
                soundList[i].clip = null;
                //改动：从删除组件 变为轧入组件依附的音效对象入池；
                PoolManager.Instance.ReturnToPool("Sound/soundObj", soundList[i].gameObject);
                soundList.RemoveAt(i);
            }
         
        }
    }

    //停止音效播放的方法：
    public void StopAllSoundEffect(AudioSource soundSource)
    {
        if (soundList.Contains(soundSource))
        {
            soundSource.Stop();

            soundSource.clip = null;
            //改动：从删除组件 变为轧入组件依附的音效对象入池；
            PoolManager.Instance.ReturnToPool("Sound/soundObj", soundSource.gameObject);
            soundList.Remove(soundSource);

        }
    }

    //改变音效音量大小的方法
    public void SetSoundVolume(float volume_)
    {
        soundEffectVolume = volume_;
        for (int i = 0; i < soundList.Count; i++)
        {
            soundList[i].volume = volume_;
        }
    }

    public void PlayAndPauseSound(bool isPause)
    {
        if (isPause)
        {
            //注意：坑！如果这里暂停，那就会触发Update中的移除方法；使得好好的AudioSource被移除；
            for (int i = 0; i < soundList.Count - 1; i++)
            {
                soundRemove = false;
                soundList[i].Stop();
            }
        }
        else
        {
            for (int i = 0; i < soundList.Count - 1; i++)
            {
                soundRemove = true;
                soundList[i].Play();
            }
        }

    }

    //清空音效的方法，提供外部当过场景的时候使用
    //一定在清空对象池之前再调用；因为此时仍会用到对象池！
    public void ClearSoundEffect()
    {
        for (int i = 0; i < soundList.Count - 1; i++)
        {
            soundList[i].Stop();
            soundList[i].clip = null;

            PoolManager.Instance.ReturnToPool("Sound/soundObj", soundList[i].gameObject);
        }
        //清空列表
        soundList.Clear();
    }

}
