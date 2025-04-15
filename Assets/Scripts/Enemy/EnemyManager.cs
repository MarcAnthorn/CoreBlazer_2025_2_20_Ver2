using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : Singleton<EnemyManager>
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnemyValueChange(AttributeType type, float finalValue)
    {
        switch (type)
        {
            case AttributeType.HP:
                PlayerManager.Instance.player.HP.value += finalValue;
                break;
            case AttributeType.STR:
                PlayerManager.Instance.player.STR.value += finalValue;
                break;
            case AttributeType.DEF:
                PlayerManager.Instance.player.DEF.value += finalValue;
                break;
            case AttributeType.LVL:
                PlayerManager.Instance.player.LVL.value += finalValue;
                break;
            case AttributeType.SAN:
                PlayerManager.Instance.player.SAN.value += finalValue;
                break;
            case AttributeType.SPD:
                PlayerManager.Instance.player.SPD.value += finalValue;
                break;
            case AttributeType.CRIT_Rate:
                PlayerManager.Instance.player.CRIT_Rate.value += finalValue;
                break;
            case AttributeType.CRIT_DMG:
                PlayerManager.Instance.player.CRIT_DMG.value += finalValue;
                break;
            case AttributeType.HIT:
                PlayerManager.Instance.player.HIT.value += finalValue;
                break;
            case AttributeType.AVO:
                PlayerManager.Instance.player.AVO.value += finalValue;
                break;
            default:
                break;
        }
    }

}
