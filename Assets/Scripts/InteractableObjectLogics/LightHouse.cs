using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightHouse : MonoBehaviour
{
    // 自动分配唯一ID
    private static int nextLightHouseId = 0;
    public int lightHouseId { get; private set; }

    private Light2D light2D;
    // private BoxCollider2D collider;
    private bool lightLock = false;
    private void Awake()
    {
        // 分配唯一ID
        lightHouseId = nextLightHouseId++;
        light2D = this.GetComponent<Light2D>();
        EventHub.Instance.AddEventListener("OnPlayerDead", OnPlayerDead);
        EventHub.Instance.AddEventListener<bool>("TriggerLight", TriggerLight);
        Debug.Log($"LightHouse自动分配的id为: {lightHouseId}", this);
        Vector3 pos = this.transform.position;
        Debug.Log($"LightHouse id={lightHouseId} 坐标为: {pos}", this);

        // 读取字典初始化lightLock
        var key = (GameLevelManager.Instance.gameLevelType, pos);
        if (GameLevelManager.Instance.lightHouseIsDic.TryGetValue(key, out bool isLocked))
        {
            lightLock = isLocked;
            Debug.Log($"[LightHouse] 初始化灯塔状态: {isLocked}, 关卡: {GameLevelManager.Instance.gameLevelType}, 坐标: {pos}");
        }
        else
        {
            lightLock = false;
            GameLevelManager.Instance.lightHouseIsDic[key] = false;
            Debug.Log($"[LightHouse] 首次注册灯塔到字典，状态: false, 关卡: {GameLevelManager.Instance.gameLevelType}, 坐标: {pos}");
        }
    }
    void Start()
    {
        lightLock = false;
        light2D.intensity = 1;
    }

    void Update()
    {
        // ...existing code...
    }

    void OnDestroy()
    {
        EventHub.Instance.RemoveEventListener("OnPlayerDead", OnPlayerDead);
        EventHub.Instance.RemoveEventListener<bool>("TriggerLight", TriggerLight);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player") && !lightLock)
        {
            LeanTween.value(gameObject, 1f, 0f, 1f)
                .setOnUpdate((float val) => {
                    light2D.intensity = val;
            });
            
            EventHub.Instance.EventTrigger("ResumeLight");
            lightLock = true;

            // 记录状态到字典
            var key = (GameLevelManager.Instance.gameLevelType, this.transform.position);
            GameLevelManager.Instance.lightHouseIsDic[key] = true;
            Debug.Log($"[LightHouse] 灯塔被触发，状态已写入字典: true, 关卡: {GameLevelManager.Instance.gameLevelType}, 坐标: {this.transform.position}");

            //灯光回复，尝试触发音效：
            SoundEffectManager.Instance.PlaySoundEffect("接触灯塔");
        }
    }

    private void OnPlayerDead()
    {
        lightLock = false;
        light2D.intensity = 1;
        // 重置字典状态
        var key = (GameLevelManager.Instance.gameLevelType, this.transform.position);
        GameLevelManager.Instance.lightHouseIsDic[key] = false;
        Debug.LogWarning($"[LightHouse] 灯塔重置，状态已写入字典: false, 关卡: {GameLevelManager.Instance.gameLevelType}, 坐标: {this.transform.position}");
    }

    // 在进入安全屋的时候，触发的取消灯光的方法：
    private void TriggerLight(bool isOn)
    {
        light2D.enabled = isOn;
    }


}

