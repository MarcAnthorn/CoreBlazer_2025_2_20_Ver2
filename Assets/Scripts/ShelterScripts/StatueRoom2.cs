using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StatueRoom2 : MonoBehaviour
{
    
    private bool isTriggerLock = true;
    private TipPanel tipPanel;

    private void Update() {
        if(!isTriggerLock)
        {
            if(Input.GetKeyDown(KeyCode.J))
            {
                tipPanel = UIManager.Instance.ShowPanel<TipPanel>();
                tipPanel.setTipAction("是否离开安全屋进入关卡");
                tipPanel.setOnConfirmAction += TipConfirmAction;
                tipPanel.setOnCancelAction += TipCancelAction;
                EventHub.Instance.EventTrigger<bool>("Freeze", true);

                //防止反复触发输入读取：
                isTriggerLock = true;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.CompareTag("Player"))
        {
            isTriggerLock = false;

        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if(other.gameObject.CompareTag("Player"))
        {
            isTriggerLock = true;
        }
    }

    private void TipConfirmAction()
    {
        Debug.Log("进入对应的关卡");
        //更新PlayerManager中的玩家所处的场景：
        PlayerManager.Instance.playerSceneIndex = E_PlayerSceneIndex.Maze;
        UIManager.Instance.HidePanel<TipPanel>(()=>{
            //失活所有需要失活的过场景不移除的对象：
            //该方法定义在TestCanvas中，该脚本挂载在Canvas上；
            EventHub.Instance.EventTrigger<bool>("TestClearFunction", false);

            GameLevelManager.Instance.gameLevelType = E_GameLevelType.Second;
            
            EventHub.Instance.EventTrigger<UnityAction>("ShowMask", ()=>{
                LoadSceneManager.Instance.LoadSceneAsync("MazeScene");
            });
        });

    }

    private void TipCancelAction()
    {
        Debug.Log("TipPanel取消");
        EventHub.Instance.EventTrigger("Freeze", false);
        UIManager.Instance.HidePanel<TipPanel>();
        isTriggerLock = false;
    }


}
