using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class BuffManager : Singleton<BuffManager>
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    //仔细想了一下Buff的执行逻辑，似乎除开特殊Buff外的Buff只在数值将要变动时 起到增益/减益效果
    //故只用检测特殊情况下的Buff，并对其buffFunction进行调用即可
    void Update()
    {
        foreach (KeyValuePair<int, Buff> pair in buffs)
        {
            //一旦isTrigger被赋值为true，则代表该buff触发了
            if (pair.Value.isTrigger && pair.Value.buffFunction != null
                && pair.Value.useCase == UseCase.Maze)      //迷宫环境中才会触发
            {
                pair.Value.buffFunction.Invoke();
                buffs.Remove(pair.Key);
            }
        }
    }

    private int count = -1;
    public Dictionary<int, Buff> buffs = new Dictionary<int, Buff>();
    public Dictionary<int, Buff> EnemyBuffs = new Dictionary<int, Buff>();

    public int AddBuff(UseCase useCase, BuffType buffType, CalculationType calculationType, float extraChange)
    {
        Buff buff = new Buff(useCase, buffType, calculationType, extraChange);
        count++;
        buffs.Add(count, buff);

        return count;
    }

    public int AddBuff(UseCase useCase, BuffType buffType, SpecialBuffType specialBuffType, Func<float> buffFunction)
    {
        Buff buff = new Buff(useCase, buffType, specialBuffType, buffFunction);
        count++;
        buffs.Add(count, buff);

        return count;
    }

    public void RemoveBuff(int buffIndex)
    {
        buffs.Remove(buffIndex);
        count--;
    }

    private void ExecuteSpecialBuff(Buff buff, ref float finalValue)    //用来进行特殊处理
    {
        switch (buff.specialBuffType)
        {
            case SpecialBuffType.DamageWall:
                break;
            case SpecialBuffType.NoDamage:
                finalValue = buff.buffFunction.Invoke();
                break;
            default:
                break;
        }
    }

    //角色在受到Buff影响下的 战斗外的 成长变动
    public float BuffEffectInGrowUp(BuffType type, float value)            
    {
        for(int i = 0; i < buffs.Count; i++)
        {
            if (buffs[i].buffType == type && buffs[i].useCase == UseCase.GrowUp)     //成长类加成
            {
                float finalValue;
                finalValue = GetFinalValueFromBuff(buffs[i], value, buffs[i].extraValue);

                //如果有特殊处理:
                if (buffs[i].specialBuffType != SpecialBuffType.NONE && buffs[i].buffType == type)
                {
                    ExecuteSpecialBuff(buffs[i], ref finalValue);
                }

                return finalValue;
            }
        }

        return 0f;
    }

    // 角色在受到Buff影响下的 战斗内的 属性(基本就是HP或者SAN值)变动 (已弃用)
    //public float BuffEffectInBattle(BuffType type, float value)
    //{
    //    float finalValue = 0f;

    //    for (int i = 0; i < buffs.Count; i++)
    //    {
    //        if (buffs[i].buffType == type && buffs[i].useCase == UseCase.Battle)    //战斗时增益/减益
    //        {
    //            finalValue = GetFinalValueFromBuff(buffs[i], value, buffs[i].extraValue);

    //            //如果有特殊处理:
    //            if (buffs[i].specialBuffType != SpecialBuffType.NONE && buffs[i].buffType == type)
    //            {
    //                ExecuteSpecialBuff(buffs[i], ref finalValue);
    //            }
    //        }
    //    }

    //    return finalValue;

    //}

    //按照BuffType处理
    public float GetFinalValueFromBuff(Buff buff, float value, float extraValue)
    {
        float finalValue;
        finalValue = CalculationAfterBuff(buff.calculationType, value, extraValue);

        return finalValue;
    }

    //按照CalculationType处理
    public float CalculationAfterBuff(CalculationType type, float value, float extraValue)
    {
        switch (type)
        {
            case CalculationType.Add:
                value += extraValue;
                return value;
            case CalculationType.Multiply:
                value *= (1.0f + extraValue);
                return value;
            case CalculationType.NONE:      //如果CalculationType为NONE其实就说明了该buff不涉及到伤害计算
                value = 0;
                return value;
            default:
                return 0;
        }
    }

}
