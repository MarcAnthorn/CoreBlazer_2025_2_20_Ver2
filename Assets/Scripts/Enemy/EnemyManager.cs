using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;



//该脚本中的enemies已弃用；
public class EnemyManager : Singleton<EnemyManager>
{
    //所有的敌方buff存在于Enemy中的buffs中；
    // public Dictionary<int, Enemy> enemies = new Dictionary<int, Enemy>();   // <positionId, enemy>

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private BuffType GetEnemyBuffType(AttributeType type)
    {
        switch (type)
        {
            case AttributeType.HP:
                return BuffType.HP_Change;
            case AttributeType.STR:
                return BuffType.STR_Change;
            case AttributeType.DEF:
                return BuffType.DEF_Change;
            case AttributeType.LVL:
                return BuffType.LVL_Change;
            case AttributeType.SAN:
                return BuffType.SAN_Change;
            case AttributeType.SPD:
                return BuffType.SPD_Change;
            case AttributeType.CRIT_Rate:
                return BuffType.CRIT_Rate_Change;
            case AttributeType.CRIT_DMG:
                return BuffType.CRIT_DMG_Change;
            case AttributeType.HIT:
                return BuffType.HIT_Change;
            case AttributeType.AVO:
                return BuffType.AVO_Change;
            default:
                return BuffType.NONE;
        }
    }

    //战斗内的 敌人对玩家造成影响的方法(一般来说就是HP和SAN值上的影响)
    public float CalculateDamageAfterBuff(Enemy enemy, AttributeType type, float value)
    {
        //这是为了明确buff要不要执行(根据buffType来判断)
        BuffType buffType = GetEnemyBuffType(type);

        //如果是易伤就跳过；

        //finalValue表示 战斗过程 造成的实际数值变化
        float finalValue = TurnCounter.Instance.EnemyBuffsBuffEffectInBattle(enemy, buffType, value);


        return value;
    }
    
    // 计算敌人造成的伤害
    public void DamageCalculation(Player player, Enemy enemy, float rowDamage, DamageType damageType, Action action = null)
    {
        // 计算防御收益
        rowDamage = Mathf.Max(0, rowDamage - player.DEF.value);
        Debug.LogWarning($"current defense value:{player.DEF.value}");
        Debug.LogWarning($"current raw attack value:{rowDamage}");

        // 计算敌人身上的Buff
        float damageValue = rowDamage;
        EnemyManager.Instance.CalculateDamageAfterBuff(enemy, AttributeType.HP, rowDamage);

        Debug.LogWarning($"current damage value:{damageValue}");

        List<Damage> damages = EnemyManager.Instance.CauseDamage(enemy, damageValue, damageType);
        
        if (damages.Count == 0)
        {
            Debug.Log("敌人发出的伤害被闪避了!");
            EventHub.Instance.EventTrigger("UpdateDamangeText", (float)-1, false);
        }
        else 
        {
            int count = 1;
            foreach (var dmg in damages)
            {
                Debug.Log($"进行第 {count} 次连击!");
                // 造成伤害之前进行一些加成计算
                // 先计算易伤(DeBuff)加成
                dmg.damage = TurnCounter.Instance.CalculateWithPlayerBuff(TriggerTiming.CalculateDebuffDamage, dmg.damageType, dmg.damage);
                // 再判断伤害类型(方法内部自行判断)，并计算敌人增伤(GoodBuff)加成
                dmg.damage = TurnCounter.Instance.CalculateWithEnemyBuff(TriggerTiming.CalculateGoodBuffDamage, dmg.damageType, enemy.positionId, dmg.damage);

                // 角色受击后添加新的Buff
                if (count == 1)
                {
                    TurnCounter.Instance.DealWithPlayerBuff(TriggerTiming.BeHit, dmg.damageType);
                }

                //结算出的对玩家的伤害
                Debug.LogWarning($"结算出的对玩家的伤害：{dmg.damage}");
                EventHub.Instance.EventTrigger("UpdateDamangeText", dmg.damage, false);


                //调用玩家受击方法+特殊效果(中毒)
                //改方法内部存在对玩家的死亡判断；
                // TurnCounter.Instance.AddPlayerBuff(new BattleBuff_1001());
                PlayerManager.Instance.player.BeHurted(dmg);

                if (action != null && count == 1)
                {
                    action.Invoke();
                }
                // 调用UI更新：
                EventHub.Instance.EventTrigger("UpdateAllUIElements");
                count++;
            }

        }
    }

    // 造成伤害
    public List<Damage> CauseDamage(Enemy enemy, float singleDamage, DamageType damageType)
    {
        List<Damage> damages = new List<Damage>();
        if (JugdeAvoid(enemy))
        {
            return damages;
        }
        else
        {
            JudgeHit(enemy, singleDamage, damageType, out damages);
        }

        return damages;
    }

    //命中判定
    private bool JugdeAvoid(Enemy enemy)
    {
        float avo = enemy.AVO;
        float random = UnityEngine.Random.Range(0f, 1f);
        if (random < avo)
        {
            return true;
        }

        return false;
    }

    //连击判定
    private void JudgeHit(Enemy enemy, float singleDamage, DamageType damageType, out List<Damage> damages)
    {
        /*连击判定(由Player类来处理每一次连击的效果)
         *  对每一次连击进行暴击判定
         */
        List<Damage> damages_return = new List<Damage>();

        //获取连击率
        float hit = enemy.HIT;

        // baseHit 
        int baseHit = (int)Math.Ceiling(hit);       //向上取整
        if ((float)baseHit == hit)
            baseHit++;

        // hitRate
        float hitRate = hit + 1 - baseHit;
        float crit_rate = enemy.CRIT_Rate;
        float crit_dmg = enemy.CRIT_DMG;

        float constHit = baseHit;
        for (int i = 0; i < constHit + 1; i++)
        {
            Damage tempDamage = new Damage(damageType);
            float random1 = UnityEngine.Random.Range(0f, 1f);
            if (random1 < crit_rate)
            {
                tempDamage.damage = singleDamage * (1 + crit_dmg);
                tempDamage.isCritical = true;
            }
            else
            {
                tempDamage.damage = singleDamage;
                tempDamage.isCritical = false;
            }

            // 对下一次是否继续循环进行判断（连击判断）
            if (baseHit == 0)
            {
                float random2 = UnityEngine.Random.Range(0f, 1f);
                if (random2 >= hitRate)
                {
                    break;
                }
            }

            damages_return.Add(tempDamage);
            baseHit--;
        }

        damages = damages_return;

    }

    public void EnemyHurted(int positionId, Damage damage)
    {
        // 在这里可以添加一些对伤害的检测(比如检测是否是暴击伤害) + 局内效果实现


        BattleManager.Instance.enemies[positionId].BeHurted(damage);
    }

    // 敌人技能定义处
    // 格斗
    public void EnemySkill_1001(Enemy enemy)
    {
        Debug.Log("敌人发动 格斗！");
        //将STR属性值转化为 攻击值 
        float rowDamage = enemy.STR * 1f;

        Debug.LogWarning($"raw damage is {rowDamage}");

        EnemyManager.Instance.DamageCalculation(PlayerManager.Instance.player, enemy, rowDamage, DamageType.STR);

        
    }

    // 毒针
    public void EnemySkill_1002(Enemy enemy)
    {
        Debug.Log("敌人发动 毒针！");
        //将STR属性值转化为 攻击值 
        float rowDamage = enemy.SPD * 0.2f + 8;
        Action action = () => AddBuffToEnemySkill_1002();
        EnemyManager.Instance.DamageCalculation(PlayerManager.Instance.player, enemy, rowDamage, DamageType.Skill, action);
    }
    private void AddBuffToEnemySkill_1002()
    {
        BattleBuff buff = new BattleBuff_1001();
        TurnCounter.Instance.AddPlayerBuff(buff);
    }

    // 新月之辉
    public void EnemySkill_1003(Enemy enemy)
    {
        Debug.Log("敌人发动 新月之辉！");
        float rowDamage = 50;
        Action action = () => AddBuffToEnemySkill_1003();
        EnemyManager.Instance.DamageCalculation(PlayerManager.Instance.player, enemy, rowDamage, DamageType.Skill, action);
    }
    private void AddBuffToEnemySkill_1003()
    {
        float randomValue = UnityEngine.Random.Range(0f, 1f);
        if (randomValue <= 0.6f)
        {
            Debug.Log("至圣斩 施加了易伤效果");
            for (int i = 0; i < 10; i++)
            {
                BattleBuff buff = new BattleBuff_1002();
                TurnCounter.Instance.AddPlayerBuff(buff);
            }
            return;
        }
        Debug.Log("至圣斩 未能施加易伤效果");
    }

    // 心火
    public void EnemySkill_1004(Enemy enemy)
    {
        Debug.Log("敌人发动 心火！");
        enemy.HP -= 10;
        AddBuffToEnemySkill_1004();
    }
    private void AddBuffToEnemySkill_1004()
    {
        BattleBuff buff = new BattleBuff_1003();
        TurnCounter.Instance.AddEnemyBuff(buff);
    }

    // 破势击
    public void EnemySkill_1005(Enemy enemy)
    {
        Debug.Log("敌人发动 破势击！");
        float rowDamage = 30 + enemy.STR * 1.0f;
        Action action = () => AddBuffToEnemySkill_1005();
        EnemyManager.Instance.DamageCalculation(PlayerManager.Instance.player, enemy, rowDamage, DamageType.Skill, action);
    }
    private void AddBuffToEnemySkill_1005()
    {
        BattleBuff buff = new BattleBuff_1004();
        TurnCounter.Instance.AddPlayerBuff(buff);
    }

    // 缚心铎声
    public void EnemySkill_1006(Enemy enemy)
    {
        Debug.Log("敌人发动 缚心铎声！");
        float rowDamage = 50 + enemy.STR * 1.0f;   // SAN值用STR代替
        Action action = () => AddBuffToEnemySkill_1006();
        EnemyManager.Instance.DamageCalculation(PlayerManager.Instance.player, enemy, rowDamage, DamageType.Skill, action);
    }
    private void AddBuffToEnemySkill_1006()
    {
        BattleBuff buff = new BattleBuff_1005();
        TurnCounter.Instance.AddPlayerBuff(buff);
    }

    // 落日
    public void EnemySkill_1007(Enemy enemy)
    {
        Debug.Log("敌人发动 落日！");
        float rowDamage = 50;
        Action action = () => AddBuffToEnemySkill_1007();
        EnemyManager.Instance.DamageCalculation(PlayerManager.Instance.player, enemy, rowDamage, DamageType.Skill, action);
    }
    private void AddBuffToEnemySkill_1007()
    {
        BattleBuff buff = new BattleBuff_1006();
        TurnCounter.Instance.AddPlayerBuff(buff);
    }

    // 湖中女的复仇
    public void EnemySkill_1008(Enemy enemy)
    {
        Debug.Log("敌人发动 湖中女的复仇！");
        float rowDamage = 50 + (enemy.HP_limit - enemy.HP) * 1.0f;
        Action action = () => AddBuffToEnemySkill_1008();
        EnemyManager.Instance.DamageCalculation(PlayerManager.Instance.player, enemy, rowDamage, DamageType.Skill, action);
    }
    private void AddBuffToEnemySkill_1008()
    {
        BattleBuff buff = new BattleBuff_1007();
        TurnCounter.Instance.AddPlayerBuff(buff);
    }

    // 魔音灌耳
    public void EnemySkill_1009(Enemy enemy)
    {
        Debug.Log("敌人发动 魔音灌耳！");
        float rowDamage = 50;
        Action action = () => AddBuffToEnemySkill_1009();
        EnemyManager.Instance.DamageCalculation(PlayerManager.Instance.player, enemy, rowDamage, DamageType.Skill, action);
    }
    private void AddBuffToEnemySkill_1009()
    {
        BattleBuff buff = new BattleBuff_1008();
        TurnCounter.Instance.AddPlayerBuff(buff);
    }

    // 伤口污染
    public void EnemySkill_1010(Enemy enemy)
    {
        Debug.Log("敌人发动 伤口污染！");
        float rowDamage = 50 + (enemy.HP_limit - enemy.HP) * 1.0f;
        Action action = () => AddBuffToEnemySkill_1010();
        EnemyManager.Instance.DamageCalculation(PlayerManager.Instance.player, enemy, rowDamage, DamageType.Skill, action);
    }
    private void AddBuffToEnemySkill_1010()
    {
        BattleBuff buff = new BattleBuff_1009();
        TurnCounter.Instance.AddPlayerBuff(buff);
    }

    // 恨意凝视
    public void EnemySkill_1012(Enemy enemy)
    {
        Debug.Log("敌人发动 恨意凝视！");
        float rowDamage = 20 + enemy.DEF * 1.0f;
        Action action = () => AddBuffToEnemySkill_1012();
        EnemyManager.Instance.DamageCalculation(PlayerManager.Instance.player, enemy, rowDamage, DamageType.Skill, action);
    }
    private void AddBuffToEnemySkill_1012()
    {
        BattleBuff buff = new BattleBuff_1011();
        TurnCounter.Instance.AddPlayerBuff(buff);
    }

    // 瘟疫吐息
    public void EnemySkill_1013(Enemy enemy)
    {
        Debug.Log("敌人发动 瘟疫吐息！");
        float rowDamage = 20;
        Action action = () => AddBuffToEnemySkill_1013();
        EnemyManager.Instance.DamageCalculation(PlayerManager.Instance.player, enemy, rowDamage, DamageType.Skill, action);
    }
    private void AddBuffToEnemySkill_1013()
    {
        BattleBuff buff = new BattleBuff_1012();
        TurnCounter.Instance.AddPlayerBuff(buff);
    }

    // 怨念
    public void EnemySkill_1016(Enemy enemy)
    {
        Debug.Log("敌人发动 怨念！");
        float rowDamage = 30 + BattleManager.Instance.player.STR.value * 0.5f;
        Action action = () => AddBuffToEnemySkill_1016();
        EnemyManager.Instance.DamageCalculation(PlayerManager.Instance.player, enemy, rowDamage, DamageType.Skill, action);
    }
    private void AddBuffToEnemySkill_1016()
    {
        BattleBuff buff = new BattleBuff_1015();
        TurnCounter.Instance.AddPlayerBuff(buff);
    }

    // 鼠群意志
    public void EnemySkill_1020(Enemy enemy)
    {
        Debug.Log("敌人发动 鼠群意志！");
        enemy.HP -= 20 + enemy.STR * 2.0f + enemy.SPD * 1.0f;   // SAN值用STR代替
        AddBuffToEnemySkill_1020();
    }
    private void AddBuffToEnemySkill_1020()
    {
        BattleBuff buff = new BattleBuff_1019();
        TurnCounter.Instance.AddEnemyBuff(buff);
    }

    // 禁咒
    public void EnemySkill_1022(Enemy enemy)
    {
        Debug.Log("敌人发动 禁咒！");
        float rowDamage = 20 + (BattleManager.Instance.player.HP.value_limit - BattleManager.Instance.player.HP.value) * 0.8f;
        Action action = () => AddBuffToEnemySkill_1022();
        EnemyManager.Instance.DamageCalculation(PlayerManager.Instance.player, enemy, rowDamage, DamageType.Skill, action);
    }
    private void AddBuffToEnemySkill_1022()
    {
        BattleBuff buff = new BattleBuff_1021();
        TurnCounter.Instance.AddPlayerBuff(buff);
    }

    // 最后一次守护
    public void EnemySkill_1023(Enemy enemy)
    {
        Debug.Log("敌人发动 最后一次守护！");
        AddBuffToEnemySkill_1023();
    }
    private void AddBuffToEnemySkill_1023()
    {
        BattleBuff buff = new BattleBuff_1022();
        TurnCounter.Instance.AddEnemyBuff(buff);
    }

}
