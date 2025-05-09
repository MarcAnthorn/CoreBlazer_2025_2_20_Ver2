using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC20021 : NPCBase
{
    //后续节点：
    public GameObject reward;
   


    protected override void OnComplete(int avgId)
    {
        base.OnComplete(avgId);
        reward.SetActive(true);

    


        var panel = UIManager.Instance.ShowPanel<BattlePanel>();
        panel.InitEnemyInfo(1011);
        panel.callback = OnBattleComplete;
        EventHub.Instance.EventTrigger<bool>("Freeze", true);

        
        Destroy(this.gameObject);
    }

    protected override void Awake()
    {
        base.Awake();
        avgId = 2111;  

       //自己激活时，如果上一次死亡我触发过，那么直接调用OnComplete，然后将自己失活返回；
        if(GameLevelManager.Instance.avgIndexIsTriggeredDic.ContainsKey(avgId) && GameLevelManager.Instance.avgIndexIsTriggeredDic[avgId]) 
        {
            OnComplete(avgId);
            this.gameObject.SetActive(false);
            return;
        }
        
       GameLevelManager.Instance.avgIndexIsTriggeredDic.TryAdd(avgId, false);
    }

    private void OnBattleComplete(int id)
    {
        GameLevelManager.Instance.avgIndexIsTriggeredDic[avgId] = true;
        EquipmentManager.Instance.AddEquipment(1010, 1011, 1012, 1013, 1014, 1015);
    }

} 