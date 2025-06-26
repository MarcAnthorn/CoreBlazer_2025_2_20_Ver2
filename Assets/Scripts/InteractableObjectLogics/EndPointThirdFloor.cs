using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EndPointThirdFloor : MonoBehaviour
{
    //播放结局：
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            SoundEffectManager.Instance.StopMusic();
            int avgId = 1206;

            UIManager.Instance.ShowPanel<AVGPanel>().InitAVG(avgId, OnComplete);
 
        }     
    }


    //回调函数：1206之后，执行战斗：
    public void OnComplete(int id)
    {
        //广播战斗：
        var panel = UIManager.Instance.ShowPanel<BattlePanel>();
        panel.InitEnemyInfo(1014);
        panel.callback = OnCompleteAfterBattle;
        EventHub.Instance.EventTrigger<bool>("Freeze", true);
    }


    public void OnCompleteAfterBattle(int id)
    {
        UIManager.Instance.ShowPanel<AVGPanel>().InitAVG(1207, OnCompleteAfter1207);
        EventHub.Instance.EventTrigger<bool>("Freeze", true);
    }

    public void OnCompleteAfter1207(int id)
    {
        int avgId;
        //根据玩家当前的san值播放结局剧情：
        var san = PlayerManager.Instance.player.SAN.value;
        if(san > 60)
        {
            Debug.Log("愚人结局达成");
            avgId = 1303;
        }
        else if(san <= 60 && san >= 20)
        {
            Debug.Log("月亮结局达成");
            avgId = 1302;
        }
        else{
            Debug.Log("高塔结局达成");
            avgId = 1301;
        }
        LeanTween.delayedCall(0.5f, ()=>{
            UIManager.Instance.ShowPanel<AVGPanel>().InitAVG(avgId, OnCompletAfterEnd);
            EventHub.Instance.EventTrigger<bool>("Freeze", true);
        });
    }

    public void OnCompletAfterEnd(int id)
    {
        Debug.LogWarning("回到游戏主界面");
        EventHub.Instance.EventTrigger<UnityAction>("ShowMask", ()=>{
            LoadSceneManager.Instance.LoadSceneAsync("StartScene");  
            EventHub.Instance.EventTrigger<bool>("Freeze", false);
        
            //清空玩家当前的存档：
            SaveManager.Instance.ClearGame(); 
        });
        
    }
}
