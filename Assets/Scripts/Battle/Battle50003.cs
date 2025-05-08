using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battle50003 : BattleBase
{

    protected override void OnComplete(int enemyId)
    {
        base.OnComplete(enemyId);

        Destroy(this.gameObject);
    }


    protected override void Awake()
    {
        base.Awake();

        enemyId = Random.Range(1006, 1011);
    }

} 