using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffManager : Singleton<BuffManager>
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        foreach(Buff buff in buffs.Values)
        {
            if (buff.isTrigger && buff.buffAction != null)
            {
                buff.buffAction.Invoke();
            }
        }
    }

    private int count = -1;
    public Dictionary<int, Buff> buffs = new Dictionary<int, Buff>();

    public int AddBuff(BuffType buffType, float extraChange, Action buffAction = null)
    {
        Buff buff = new Buff(buffType, extraChange);
        count++;
        buffs.Add(count, buff);

        return count;
    }

    public int AddBuff(BuffType buffType, Action buffAction)
    {
        Buff buff = new Buff(buffType, buffAction);
        count++;
        buffs.Add(count, buff);

        return count;
    }

    public void RemoveBuff(int buffIndex)
    {
        buffs.Remove(buffIndex);
        count--;
    }

    public void ModifyPlayerAttribute(BuffType type)
    {
        for(int i = 0; i < buffs.Count; i++)
        {
            if (buffs[i].buffType == type)
            {
                //按照该buff进行处理
                ValueChangeFromBuff(type, buffs[i].extraValue);
            }
        }
    }

    private void ValueChangeFromBuff(BuffType type, float extraValue)
    {
        switch (type)
        {
            case BuffType.HP_Change:
                PlayerManager.Instance.player.HP.value += extraValue;
                break;
            case BuffType.STR_Change:
                PlayerManager.Instance.player.STR.value += extraValue;
                break;
            case BuffType.DEF_Change:
                PlayerManager.Instance.player.DEF.value += extraValue;
                break;
            case BuffType.LVL_Change:
                PlayerManager.Instance.player.LVL.value += extraValue;
                break;
            case BuffType.SAN_Change:
                PlayerManager.Instance.player.SAN.value += extraValue;
                break;
            case BuffType.SPD_Change:
                PlayerManager.Instance.player.SPD.value += extraValue;
                break;
            case BuffType.CRIT_Rate_Change:
                PlayerManager.Instance.player.CRIT_Rate.value += extraValue;
                break;
            case BuffType.CRIT_DMG_Change:
                PlayerManager.Instance.player.CRIT_DMG.value += extraValue;
                break;
            case BuffType.HIT_Change:
                PlayerManager.Instance.player.HIT.value += extraValue;
                break;
            case BuffType.AVO_Change:
                PlayerManager.Instance.player.AVO.value += extraValue;
                break;
            default:
                break;
        }
    }

}
