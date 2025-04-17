using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : Singleton<EnemyManager>
{
    public Dictionary<int, Enemy> enemies = new Dictionary<int, Enemy>();   // <positionId, enemy>

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnemyValueChange(int id, Damage damage)
    {
        enemies[id].BeHurted(damage);
    }

}
