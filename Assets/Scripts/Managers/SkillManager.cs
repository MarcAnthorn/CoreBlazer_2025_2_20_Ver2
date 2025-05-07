using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class SkillManager : Singleton<SkillManager>
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 释放技能
    public void ReleaseSkill(ref int actionPoint, int skillId, Player player, Enemy enemy)
    {
        switch (skillId)
        {
            case 1001:
                Skill_1001(enemy);
                if (actionPoint >= 1)
                    actionPoint -= 1;
                else
                    Debug.Log("角色行动点不足以释放技能 格斗");
                break;
            case 1002:
                Skill_1002(enemy);
                if (actionPoint >= 0)
                    actionPoint -= 0;
                else
                    Debug.Log("角色行动点不足以释放技能 毒针");
                break;
            case 1003:
                Skill_1003(enemy);
                if (actionPoint >= 2)
                    actionPoint -= 2;
                else
                    Debug.Log("角色行动点不足以释放技能 新月之辉");
                break;
            case 1004:
                Skill_1004(player);
                if (actionPoint >= 1)
                    actionPoint -= 1;
                else
                    Debug.Log("角色行动点不足以释放技能 心火");
                break;
            case 1005:
                Skill_1005(enemy);
                if (actionPoint >= 2)
                    actionPoint -= 2;
                else
                    Debug.Log("角色行动点不足以释放技能 破势击");
                break;
            case 1006:
                Skill_1006(enemy);
                if (actionPoint >= 2)
                    actionPoint -= 2;
                else
                    Debug.Log("角色行动点不足以释放技能 缚心铎声");
                break;
            case 1007:
                Skill_1007(enemy);
                if (actionPoint >= 2)
                    actionPoint -= 2;
                else
                    Debug.Log("角色行动点不足以释放技能 落日");
                break;
            case 1008:
                Skill_1008(enemy);
                if (actionPoint >= 2)
                    actionPoint -= 2;
                else
                    Debug.Log("角色行动点不足以释放技能 湖中女的复仇");
                break;
            case 1009:
                Skill_1009(enemy);
                if (actionPoint >= 2)
                    actionPoint -= 2;
                else
                    Debug.Log("角色行动点不足以释放技能 魔音灌耳");
                break;
            case 1010:
                Skill_1010(enemy);
                if (actionPoint >= 2)
                    actionPoint -= 2;
                else
                    Debug.Log("角色行动点不足以释放技能 伤口污染");
                break;
            case 1020:
                Skill_1020(player);
                if (actionPoint >= 1)
                    actionPoint -= 1;
                else
                    Debug.Log("角色行动点不足以释放技能 鼠群意志");
                break;
            case 1023:
                Skill_1023();
                if (actionPoint >= 1)
                    actionPoint -= 1;
                else
                    Debug.Log("角色行动点不足以释放技能 最后一次守护");
                break;
            default:
                break;
        }

    }

    // 结算角色造成的伤害
    public void PlayerDamageCalculation(Enemy enemy, float rowDamage, DamageType damageType, Action action = null)
    {
        // 计算防御收益
        rowDamage = Mathf.Max(0, rowDamage - enemy.DEF);
        Debug.Log($"raw damage is{rowDamage}");
        // 计算角色身上的Buff加成
        float damage = PlayerManager.Instance.CalculateDamageAfterBuff(AttributeType.HP, rowDamage);

        Debug.Log($"Damage before buff is{damage}");
        
        List<Damage> damages = PlayerManager.Instance.CauseDamage(enemy, damage, damageType);
        if (damages.Count == 0)
        {
            Debug.Log("角色发出的伤害被闪避了!");
        }
        else
        {
            foreach (var dmg in damages)
            {
                // 造成伤害之前进行一些加成计算
                // 先计算易伤(DeBuff)加成
                dmg.damage = TurnCounter.Instance.CalculateWithEnemyBuff(TriggerTiming.CalculateDebuffDamage, dmg.damageType, enemy.positionId, dmg.damage);

                Debug.LogWarning($"step 1 结算出的对敌人的伤害：{dmg.damage}");

                // 再判断伤害类型(方法内部自行判断)，并计算角色增伤(GoodBuff)加成
                dmg.damage = TurnCounter.Instance.CalculateWithPlayerBuff(TriggerTiming.CalculateGoodBuffDamage, dmg.damageType, dmg.damage);

                Debug.LogWarning($"step 2 结算出的对敌人的伤害：{dmg.damage}");

                //结算出的对敌人的伤害
                Debug.LogWarning($"结算出的对敌人的伤害：{dmg.damage}");
                
                //调用敌人受击方法
                EnemyManager.Instance.EnemyHurted(enemy.positionId, dmg);

                // 之所以将附加Buff的方法放到这里来调用，是为了将 伤害命中 与 Buff命中 协同
                if(action != null)
                {
                    action.Invoke();
                }
            }
        }
    }
        
    // 格斗
    public void Skill_1001(Enemy enemy)
    {
        Debug.Log("角色发动 格斗！");
        //将STR属性值转化为 攻击值 
        float rowDamage = PlayerManager.Instance.player.STR.value * 1f;
        SkillManager.Instance.PlayerDamageCalculation(enemy, rowDamage, DamageType.STR);
    }

    // 毒针
    public void Skill_1002(Enemy enemy)
    {
        Debug.Log("角色发动 毒针！");
        float rowDamage = PlayerManager.Instance.player.SPD.value * 0.2f + 8;
        Action action = () => AddBuffToSkill_1002(enemy);
        SkillManager.Instance.PlayerDamageCalculation(enemy, rowDamage, DamageType.Skill, action);
    }
    private void AddBuffToSkill_1002(Enemy enemy)
    {
        BattleBuff buff = new BattleBuff_1001();
        // enemy.buffs.Add(buff);

        TurnCounter.Instance.AddEnemyBuff(buff);
    }

    // 新月之辉
    public void Skill_1003(Enemy enemy)
    {
        Debug.Log("角色发动 新月之辉！");
        float rowDamage = 50;
        Action action = () => AddBuffToSkill_1003(enemy);
        SkillManager.Instance.PlayerDamageCalculation(enemy, rowDamage, DamageType.Skill, action);
    }
    private void AddBuffToSkill_1003(Enemy enemy)
    {
        float randomValue= UnityEngine.Random.Range(0f, 1f);
        if(randomValue <= 0.6f)
        {
            Debug.Log("至圣斩 施加了易伤效果");
            for (int i = 0; i < 10; i++)
            {
                BattleBuff buff = new BattleBuff_1002();
                TurnCounter.Instance.AddEnemyBuff(buff);
            }
            return;
        }
        Debug.Log("至圣斩 未能施加易伤效果"); 
    }

    // 心火
    public void Skill_1004(Player player)
    {
        Debug.Log("角色发动 心火！");
        player.HP.value -= 10;
        AddBuffToSkill_1004();
    }
    private void AddBuffToSkill_1004()
    {
        BattleBuff buff = new BattleBuff_1003();
        TurnCounter.Instance.AddPlayerBuff(buff);
    }

    // 破势击
    public void Skill_1005(Enemy enemy)
    {
        Debug.Log("角色发动 破势击！");
        float rowDamage = 50;
        Action action = () => AddBuffToSkill_1005(enemy);
        SkillManager.Instance.PlayerDamageCalculation(enemy, rowDamage, DamageType.Skill, action);
    }
    private void AddBuffToSkill_1005(Enemy enemy)
    {
        BattleBuff buff = new BattleBuff_1004();
        TurnCounter.Instance.AddEnemyBuff(buff);
    }

    // 缚心铎声
    public void Skill_1006(Enemy enemy)
    {
        Debug.Log("角色发动 缚心铎声！");
        float rowDamage = 50;
        Action action = () => AddBuffToSkill_1006(enemy);
        SkillManager.Instance.PlayerDamageCalculation(enemy, rowDamage, DamageType.Skill, action);
    }
    private void AddBuffToSkill_1006(Enemy enemy)
    {
        BattleBuff buff = new BattleBuff_1005();
        TurnCounter.Instance.AddEnemyBuff(buff);
    }

    // 落日
    public void Skill_1007(Enemy enemy)
    {
        Debug.Log("角色发动 落日！");
        float rowDamage = 50;
        Action action = () => AddBuffToSkill_1007(enemy);
        SkillManager.Instance.PlayerDamageCalculation(enemy, rowDamage, DamageType.Skill, action);
    }
    private void AddBuffToSkill_1007(Enemy enemy)
    {
        BattleBuff buff = new BattleBuff_1006();
        TurnCounter.Instance.AddEnemyBuff(buff);
    }

    // 湖中女的复仇
    public void Skill_1008(Enemy enemy)
    {
        Debug.Log("角色发动 湖中女的复仇！");
        float rowDamage = 50;
        Action action = () => AddBuffToSkill_1008(enemy);
        SkillManager.Instance.PlayerDamageCalculation(enemy, rowDamage, DamageType.Skill, action);
    }
    private void AddBuffToSkill_1008(Enemy enemy)
    {
        BattleBuff buff = new BattleBuff_1007();
        TurnCounter.Instance.AddEnemyBuff(buff);
    }

    // 魔音灌耳
    public void Skill_1009(Enemy enemy)
    {
        Debug.Log("角色发动 魔音灌耳！");
        float rowDamage = 50;
        Action action = () => AddBuffToSkill_1009(enemy);
        SkillManager.Instance.PlayerDamageCalculation(enemy, rowDamage, DamageType.Skill, action);
    }
    private void AddBuffToSkill_1009(Enemy enemy)
    {
        BattleBuff buff = new BattleBuff_1008();
        TurnCounter.Instance.AddEnemyBuff(buff);
    }

    // 伤口污染
    public void Skill_1010(Enemy enemy)
    {
        Debug.Log("角色发动 伤口污染！");
        float rowDamage = 50;
        Action action = () => AddBuffToSkill_1010(enemy);
        SkillManager.Instance.PlayerDamageCalculation(enemy, rowDamage, DamageType.Skill, action);
    }
    private void AddBuffToSkill_1010(Enemy enemy)
    {
        BattleBuff buff = new BattleBuff_1009();
        TurnCounter.Instance.AddEnemyBuff(buff);
    }

    // 鼠群意志
    public void Skill_1020(Player player)
    {
        Debug.Log("角色发动 鼠群意志！");
        player.HP.value -= 20;
        AddBuffToSkill_1020();
    }
    private void AddBuffToSkill_1020()
    {
        BattleBuff buff = new BattleBuff_1019();
        TurnCounter.Instance.AddPlayerBuff(buff);
    }

    // 最后一次守护
    public void Skill_1023()
    {
        Debug.Log("角色发动 最后一次守护！");
        AddBuffToSkill_1023();
    }
    private void AddBuffToSkill_1023()
    {
        BattleBuff buff = new BattleBuff_1022();
        TurnCounter.Instance.AddPlayerBuff(buff);
    }

}
