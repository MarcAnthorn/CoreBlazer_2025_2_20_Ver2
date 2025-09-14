using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Unity.VisualScripting;


public class ResultPanel : BasePanel
{
   
    //进度
    public TextMeshProUGUI txtProgress;
    //获得的san值：
    public TextMeshProUGUI txtSanGained;

    //事件完成
    public Transform contentEvents;

    //下一步button：
    public Button btnNextStep;

    public Image imgFirstPassed;
    public Image imgFirstNotPassed;
    public Image imgSecondPassed;
    public Image imgSecondNotPassed;
    public Image imgThirdPassed;
    public Image imgThirdNotPassed;
    public Image imgFourthPassed;
    public Image imgFourthNotPassed;

    public GameObject resultListItemPrefab = null;

    // 创建移除列表，用于存储需要删除的事件
    List<ResultEvent> eventsToRemove = new List<ResultEvent>();
    List<ResultEvent> eventList;

    protected override void Init()
    {
        eventList = GameLevelManager.Instance.eventList;
        //获取进度：
        switch(GameLevelManager.Instance.gameLevelType)
        {
            case E_GameLevelType.Tutorial:
                imgFirstPassed.gameObject.SetActive(true);
                imgSecondPassed.gameObject.SetActive(false);
                imgThirdPassed.gameObject.SetActive(false);
                imgFourthPassed.gameObject.SetActive(false);

                txtProgress.text = "25%";
            break;

            case E_GameLevelType.First:
                imgFirstPassed.gameObject.SetActive(true);
                imgSecondPassed.gameObject.SetActive(true);
                imgThirdPassed.gameObject.SetActive(false);
                imgFourthPassed.gameObject.SetActive(false);

                txtProgress.text = "50%";
            break;

            case E_GameLevelType.Second:
                imgFirstPassed.gameObject.SetActive(true);
                imgSecondPassed.gameObject.SetActive(true);
                imgThirdPassed.gameObject.SetActive(true);
                imgFourthPassed.gameObject.SetActive(false);

                txtProgress.text = "75%";
            break;

            case E_GameLevelType.Third:
                imgFirstPassed.gameObject.SetActive(true);
                imgSecondPassed.gameObject.SetActive(true);
                imgThirdPassed.gameObject.SetActive(true);
                imgFourthPassed.gameObject.SetActive(false);

                txtProgress.text = "100%";
            break;

        }

        if(resultListItemPrefab == null)
            resultListItemPrefab = Resources.Load<GameObject>("ResultListItem");

        //获取结算清单：
        int sanity = 0;
    
        
        foreach(var _event in eventList)
        {
            // 检查是否满足条件（这里需要根据您的具体业务逻辑来判断）
            // 示例：假设要删除gameLevel不匹配当前关卡的事件
            if (_event.gameLevel >= (int)GameLevelManager.Instance.gameLevelType)
            {
                // 不满足条件，加入移除列表
                eventsToRemove.Add(_event);
            }
            
            // 满足条件的事件，正常处理
            GameObject item = Instantiate(resultListItemPrefab, contentEvents);
            ResultListItem script = item.GetComponent<ResultListItem>();

            script.Init(_event.eventName, _event.contributeSan, _event.mutiplier);
            sanity += _event.contributeSan * _event.mutiplier;
        }
        
        // 遍历完成后，统一删除不满足条件的事件
        foreach(var eventToRemove in eventsToRemove)
        {
            eventList.Remove(eventToRemove);
        }

        eventsToRemove.Clear();


        PlayerManager.Instance.player.SAN.AddValue(sanity);
        txtSanGained.text = "本轮获得SAN值：" + sanity.ToString();
    


        //btn注册：
        btnNextStep.onClick.AddListener(()=>{
            UIManager.Instance.HidePanel<ResultPanel>();
        });

    }
}

//结算清单数据类：
public class ResultEvent{
    public string eventName;
    public int contributeSan;
    public int mutiplier;

    //事件所处层：
    public int gameLevel;


    public ResultEvent(string _eventName, int _san, int _mutiplier)
    {
        eventName = _eventName;
        contributeSan = _san;
        mutiplier = _mutiplier;
        gameLevel = (int)GameLevelManager.Instance.gameLevelType;
    }
}
