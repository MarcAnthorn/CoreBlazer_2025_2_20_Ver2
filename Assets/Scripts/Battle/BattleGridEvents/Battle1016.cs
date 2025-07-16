using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battle1016 : BattleBase
{

    protected override void OnComplete(int enemyId)
    {
        base.OnComplete(enemyId);

        //达贡掉落：
        ItemManager.Instance.AddItem(516);
        Destroy(this.gameObject);
    }


    protected override void Awake()
    {
        base.Awake();

        enemyId = 1016;
    }
}
