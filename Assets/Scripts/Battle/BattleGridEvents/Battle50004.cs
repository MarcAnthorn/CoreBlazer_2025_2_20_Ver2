using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battle50004 : BattleBase
{

    protected override void OnComplete(int enemyId)
    {
        base.OnComplete(enemyId);

        //生成boss宝箱：
        PoolManager.Instance.SpawnFromPool("MapPOIs/RewardBoss", this.transform.position, Quaternion.identity);
        
        Destroy(this.gameObject);
    
    }


    protected override void Awake()
    {
        base.Awake();

        enemyId = 1012;
    }

} 