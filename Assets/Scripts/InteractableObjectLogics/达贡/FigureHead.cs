using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FigureHead : MonoBehaviour
{
    public int itemId = 512; // 道具ID，由用户填写
    
    private bool isTriggerLock = true; 
    private Vector3 offset = new Vector3(0, 0.5f);
    private GameObject txtObject;
    private bool isItemCollected = false;

    // 道具点解锁状态字典的key
    private (E_GameLevelType, Vector3) itemPointKey;

    void Awake()
    {
        // 初始化道具点状态
        itemPointKey = (GameLevelManager.Instance.gameLevelType, this.transform.position);
        
        if (GameLevelManager.Instance.itemPointDic.TryGetValue(itemPointKey, out bool collected))
        {
            isItemCollected = collected;
            if(isItemCollected)
            {
                this.gameObject.SetActive(false);   //如果收集过了，那么直接失活；
            }
        }
        else
        {
            isItemCollected = false;
            GameLevelManager.Instance.itemPointDic[itemPointKey] = false;
            Debug.Log($"[FigureHead] 首次注册道具点到字典，状态: false, 关卡: {GameLevelManager.Instance.gameLevelType}, 坐标: {this.transform.position}, ItemId: {itemId}");
        }
    }

    private void Update() 
    {
        if(!isTriggerLock && Input.GetKeyDown(KeyCode.F))
        {
            CollectItem();
        }
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.gameObject.CompareTag("Player"))
        {
            // 如果道具还没被收集，显示交互提示
            if(!isItemCollected)
            {
                isTriggerLock = false;  // 暂时解锁交互
                txtObject = PoolManager.Instance.SpawnFromPool("TipText");
                EventHub.Instance.EventTrigger<string, Vector3>("SetTipContent", "按下「F」收集道具", this.transform.position + offset);
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

    private void CollectItem()
    {
        // 收集道具
        isItemCollected = true;
        
        // 写入字典并输出日志
        GameLevelManager.Instance.itemPointDic[itemPointKey] = true;
        Debug.Log($"[FigureHead] 道具被收集，状态已写入字典: true, 关卡: {GameLevelManager.Instance.gameLevelType}, 坐标: {this.transform.position}, ItemId: {itemId}");
        
        // 给玩家添加道具
        ItemManager.Instance.AddItem(itemId);

        
        // 隐藏交互提示
        if(txtObject != null)
        {
            PoolManager.Instance.ReturnToPool("TipTexts", txtObject);
            txtObject = null;
        }
        
        // 可选：播放收集音效或特效
        // AudioManager.Instance.PlaySound("item_collect");
        
        UIManager.Instance.ShowPanel<AVGPanel>().InitAVG(2308);

        // 隐藏道具点（如果是一次性的）
        this.gameObject.SetActive(false);
    }
}
