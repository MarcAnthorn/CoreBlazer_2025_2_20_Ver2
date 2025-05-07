using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndPointThirdFloor : MonoBehaviour
{
    //播放结局：
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            SoundEffectManager.Instance.StopMusic();
            int avgId = 0;
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

            DialogueOrderBlock ob = LoadManager.Instance.orderBlockDic[avgId];
            var panel = UIManager.Instance.ShowPanel<AVGPanel>();
            panel.orderBlock = ob;
            panel.callback = OnComplete;
            EventHub.Instance.EventTrigger<bool>("Freeze", true);
        }

       
    }


    //回调函数：执行结局之后，退出当前游戏到主界面；
    public void OnComplete(int id)
    {
        Debug.LogWarning("退出游戏到主界面");
    }
}
