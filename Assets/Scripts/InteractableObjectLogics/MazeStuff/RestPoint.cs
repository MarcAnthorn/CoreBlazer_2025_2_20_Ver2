using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestPoint : MonoBehaviour
{
    private bool isTriggerLock = true; 
    private Vector3 offset = new Vector3(0, 0.5f);
    private GameObject txtObject;
    private bool isLightResumed = false;

    //触发剧情2302的count：
    private static int count2302 = 0;

    private bool isRestPointUnlocked = false;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = this.GetComponent<SpriteRenderer>();
        
        // 加载并设置休息点图片
        Sprite lightHouseSprite = Resources.Load<Sprite>("ArtResources/POI/休息点");
        if (lightHouseSprite != null && spriteRenderer != null)
        {
            spriteRenderer.sprite = lightHouseSprite;
            // 设置尺寸缩放为0.1倍率
            transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        }
        else
        {
            Debug.LogWarning("[休息点] 无法加载休息点图片或找不到SpriteRenderer组件");
        }
        
        if (!GameLevelManager.Instance.avgIndexIsTriggeredDic.ContainsKey(2302))
        {
            //不包含，说明字典中都没有我的位置：
            //先初始化我在字典中的位置：
            GameLevelManager.Instance.avgIndexIsTriggeredDic.TryAdd(2302, false);
        }

        // restPointDic初始化与日志
        var key = (GameLevelManager.Instance.gameLevelType, this.transform.position);
        if (GameLevelManager.Instance.restPointDic.TryGetValue(key, out bool unlocked))
        {
            isRestPointUnlocked = unlocked;
            Debug.Log($"[RestPoint] 初始化休息点状态: {unlocked}, 关卡: {GameLevelManager.Instance.gameLevelType}, 坐标: {this.transform.position}");
        }
        else
        {
            isRestPointUnlocked = false;
            GameLevelManager.Instance.restPointDic[key] = false;
            Debug.Log($"[RestPoint] 首次注册休息点到字典，状态: false, 关卡: {GameLevelManager.Instance.gameLevelType}, 坐标: {this.transform.position}");
        }
    }


    private void Update() {
        if(!isTriggerLock && Input.GetKeyDown(KeyCode.F))
        {
            //解锁当前的休息点：
            isRestPointUnlocked = true; 
            // 写入字典并输出日志
            var key = (GameLevelManager.Instance.gameLevelType, this.transform.position);
            GameLevelManager.Instance.restPointDic[key] = true;
            Debug.Log($"[RestPoint] 休息点被解锁，状态已写入字典: true, 关卡: {GameLevelManager.Instance.gameLevelType}, 坐标: {this.transform.position}");
            TriggerRest();
        }
    }


    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.CompareTag("Player"))
        {
            //如果count2302是0，并且字典中不是true，说明没有触发过：
            //包含是在Awake就一定包含了的，这个判断是否包含只是以防万一
            if(count2302 == 0 && GameLevelManager.Instance.avgIndexIsTriggeredDic.ContainsKey(2302) && !GameLevelManager.Instance.avgIndexIsTriggeredDic[2302])
            {
                count2302++;
                GameLevelManager.Instance.avgIndexIsTriggeredDic[2302] = true;
                

                //触发AVG：并且添加回调函数
                UIManager.Instance.ShowPanel<AVGPanel>().InitAVG(2302, CallbackFor2302);

            }

            //没有解锁过，但是有钥匙，那么就给出提示：
            else if(ItemManager.Instance.CheckIfItemExist(509))  //存在钥匙
            {
                isTriggerLock = false;  //暂时解锁
                txtObject = PoolManager.Instance.SpawnFromPool("TipText");
                EventHub.Instance.EventTrigger<string, Vector3>("SetTipContent", "按下「F」和休息点交互", this.transform.position + offset);
            }

        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        //如果休息点解锁过，那么就执行对应的内容：
        if(isRestPointUnlocked && !isLightResumed)
        {
            TriggerRest();
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if(txtObject != null && other.gameObject.CompareTag("Player"))
        {
            isTriggerLock = true;
            isLightResumed = false;
            PoolManager.Instance.ReturnToPool("TipTexts", txtObject);

            //开启灯光衰减：
            EventHub.Instance.EventTrigger("TriggerLightShrinking", true);
        }


    }

    private void TriggerRest()
    {
        isLightResumed = true;
        EventHub.Instance.EventTrigger("ResumeLight", -1f);
        PlayerManager.Instance.player.HP.SetValueToLimit(); 

        //关闭灯光衰减：
        EventHub.Instance.EventTrigger("TriggerLightShrinking", false);
    }

    //2302AVG的回调函数：
    private void CallbackFor2302(int id)
    {
        //将当前的休息点变化为达贡：
        Debug.LogWarning("当前休息点变为达贡");
        GameObject dagoon =  Resources.Load<GameObject>("NPC/达贡");
        Instantiate<GameObject>(dagoon, this.transform.position, Quaternion.identity);

        //向达贡字典中添加默认通用对话：
        AVGDistributeManager.Instance.ContributeAVGId(E_NPCName.达贡, 2303, 0);
        this.gameObject.SetActive(false);

        //新增三神的新交流：
        AVGDistributeManager.Instance.ContributeAVGId(E_NPCName.奈亚拉, 2402);
        AVGDistributeManager.Instance.ContributeAVGId(E_NPCName.优格, 2403);
        AVGDistributeManager.Instance.ContributeAVGId(E_NPCName.莎布, 2404);
        AVGDistributeManager.Instance.ContributeAVGId(E_NPCName.格赫罗斯, 2401);
    }
}
