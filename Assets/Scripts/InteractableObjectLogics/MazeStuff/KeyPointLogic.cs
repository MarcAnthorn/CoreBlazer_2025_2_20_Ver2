using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyPointLogic : MonoBehaviour
{
    public int keyId = 509;
    
    private bool isTriggerLock = true; 
    private Vector3 offset = new Vector3(0, 0.5f);
    private GameObject txtObject;
    private bool isKeyCollected = false;

    // 钥匙点解锁状态字典的key
    private (E_GameLevelType, Vector3) keyPointKey;

    void Awake()
    {
        // 初始化钥匙点状态
        keyPointKey = (GameLevelManager.Instance.gameLevelType, this.transform.position);
        
        if (GameLevelManager.Instance.keyPointDic.TryGetValue(keyPointKey, out bool collected))
        {
            isKeyCollected = collected;
            if(isKeyCollected)
            {
                this.gameObject.SetActive(false);   //如果收集过了，那么直接失活；
            }
        }
        else
        {
            isKeyCollected = false;
            GameLevelManager.Instance.keyPointDic[keyPointKey] = false;
            Debug.Log($"[KeyPoint] 首次注册钥匙点到字典，状态: false, 关卡: {GameLevelManager.Instance.gameLevelType}, 坐标: {this.transform.position}, KeyId: {keyId}");
        }
    }

    private void Update() 
    {
        if(!isTriggerLock && Input.GetKeyDown(KeyCode.F))
        {
            CollectKey();
        }
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.gameObject.CompareTag("Player"))
        {
            // 如果钥匙还没被收集，显示交互提示
            if(!isKeyCollected)
            {
                isTriggerLock = false;  // 暂时解锁交互
                txtObject = PoolManager.Instance.SpawnFromPool("TipText");
                EventHub.Instance.EventTrigger<string, Vector3>("SetTipContent", "按下「F」收集钥匙", this.transform.position + offset);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other) 
    {
        if(txtObject != null && other.gameObject.CompareTag("Player"))
        {
            isTriggerLock = true;
            PoolManager.Instance.ReturnToPool("TipTexts", txtObject);
        }
    }

    private void CollectKey()
    {
        // 收集钥匙
        isKeyCollected = true;
        
        // 写入字典并输出日志
        GameLevelManager.Instance.keyPointDic[keyPointKey] = true;
        Debug.Log($"[KeyPoint] 钥匙被收集，状态已写入字典: true, 关卡: {GameLevelManager.Instance.gameLevelType}, 坐标: {this.transform.position}, KeyId: {keyId}");
        
        // 给玩家添加钥匙道具
        ItemManager.Instance.AddItem(keyId);
        
        // 显示收集成功提示
        EventHub.Instance.EventTrigger<string, Vector3>("SetTipContent", $"获得钥匙！", this.transform.position + offset);
        
        // 隐藏交互提示
        if(txtObject != null)
        {
            PoolManager.Instance.ReturnToPool("TipTexts", txtObject);
            txtObject = null;
        }
        
        // 可选：播放收集音效或特效
        // AudioManager.Instance.PlaySound("key_collect");
        
        // 可选：隐藏钥匙点（如果是一次性的）
        this.gameObject.SetActive(false);
        
        // 可选：触发相关事件
        EventHub.Instance.EventTrigger<int>("KeyCollected", keyId);
    }
}
