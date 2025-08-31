using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightHouse : MonoBehaviour
{
    private Light2D light2D;
    private SpriteRenderer spriteRenderer;
    // private BoxCollider2D collider;
    private bool lightLock = false;
    
    private void Awake()
    {
        light2D = this.GetComponent<Light2D>();
        EventHub.Instance.AddEventListener("OnPlayerDead", OnPlayerDead);
        EventHub.Instance.AddEventListener<bool>("TriggerLight", TriggerLight);
        Vector3 pos = this.transform.position;
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
        // 使用Awake中读取的状态，不强制重置
        // lightLock = false; // 已注释，使用Awake中读取的状态
        light2D.intensity = 1;
        
        Debug.Log($"[LightHouse] Start完成，当前状态: lightLock={lightLock}, 关卡: {GameLevelManager.Instance.gameLevelType}, 坐标: {this.transform.position}");
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
            // 设置灯光值到上限
            PlayerController.SetPlayerAttribute(AttributeType.LVL, PlayerManager.Instance.player.LVL.value_limit);
            
            // 触发灯光恢复事件（这会重新启动衰减并重置计数器）
            EventHub.Instance.EventTrigger("ResumeLight", 40f);
            
            PlayerManager.Instance.player.DebugInfo();

            // 设置灯塔锁定状态
            lightLock = true;

            // 记录状态到字典
            var key = (GameLevelManager.Instance.gameLevelType, this.transform.position);
            GameLevelManager.Instance.lightHouseIsDic[key] = true;
            
            Debug.Log($"[LightHouse] 灯塔被触发，状态已写入字典: true, 关卡: {GameLevelManager.Instance.gameLevelType}, 坐标: {this.transform.position}");

            //灯光回复，尝试触发音效：
            SoundEffectManager.Instance.PlaySoundEffect("接触灯塔");
        }
        else if(collision.gameObject.CompareTag("Player") && lightLock)
        {
            Debug.Log($"[LightHouse] 灯塔已被激活，无法重复触发, 关卡: {GameLevelManager.Instance.gameLevelType}, 坐标: {this.transform.position}");
        }
    }

    private void OnPlayerDead()
    {
        // 复活重置：不重置灯塔的激活状态，保持用于SAN奖励计算
        // 只重置视觉效果，让灯塔在视觉上重置但保持逻辑状态
        light2D.intensity = 1;
        
        // 不重置字典状态和lightLock，保持激活状态用于复活SAN奖励
        // var key = (GameLevelManager.Instance.gameLevelType, this.transform.position);
        // GameLevelManager.Instance.lightHouseIsDic[key] = false; // 已注释，不重置状态
        
        Debug.Log($"[LightHouse] 玩家死亡，保持灯塔激活状态用于复活SAN奖励计算, 关卡: {GameLevelManager.Instance.gameLevelType}, 坐标: {this.transform.position}");
    }

    /// <summary>
    /// 真正重置灯塔状态（用于清空游戏数据等情况）
    /// </summary>
    public void ResetLightHouse()
    {
        lightLock = false;
        light2D.intensity = 1;
        
        // 重置字典状态
        var key = (GameLevelManager.Instance.gameLevelType, this.transform.position);
        GameLevelManager.Instance.lightHouseIsDic[key] = false;
        
        Debug.Log($"[LightHouse] 灯塔完全重置，状态已写入字典: false, 关卡: {GameLevelManager.Instance.gameLevelType}, 坐标: {this.transform.position}");
    }

    // 在进入安全屋的时候，触发的取消灯光的方法：
    private void TriggerLight(bool isOn)
    {
        light2D.enabled = isOn;
    }


}

