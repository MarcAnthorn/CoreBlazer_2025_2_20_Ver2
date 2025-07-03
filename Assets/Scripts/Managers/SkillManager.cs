using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.EventSystems.EventTrigger;

public class SkillManager : Singleton<SkillManager>
{
    Equipment equipment;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override void Awake()
    {
        base.Awake();
        // EventHub.Instance.AddEventListener<Equipment>("BroadcastNowEquipment", BroadcastNowEquipment);
    }

    protected override void OnDestroy() {
        base.OnDestroy();
        // EventHub.Instance.RemoveEventListener<Equipment>("BroadcastNowEquipment", BroadcastNowEquipment);
    }
    public void BroadcastNowEquipment(Equipment _equipment)
    {
        equipment = _equipment;
    }

    // 释放技能
    public void ReleaseSkill(ref int actionPoint, int skillId, Player player, Enemy enemy)
    {
        bool isActionPointEnough = true;
        var skillDic = LoadManager.Instance.allSkills;

        //技能名改成全自动了，只需要写 isActionPointEnough = false; 就行；
        switch (skillId)
        {
            case 1001:
                if (actionPoint >= 1)
                {
                    actionPoint -= 1;
                    Skill_1001(enemy);
                }
                else
                    isActionPointEnough = false;
                break;

            case 1002:
                if (actionPoint >= 0)
                {
                    actionPoint -= 0;
                    Skill_1002(enemy);
                }
                else
                    isActionPointEnough = false;
                break;

            case 1003:
                Skill_1003(enemy);
                if (actionPoint >= 3) 
                { 
                    actionPoint -= 3;
                    Skill_1003(enemy);
                }
                else
                    isActionPointEnough = false;
                break;

            case 1004:
                if (actionPoint >= 1)
                {
                    actionPoint -= 1;
                    Skill_1004(player);
                }
                else
                    isActionPointEnough = false;
                break;

            case 1005:
                if (actionPoint >= 2)
                {
                    actionPoint -= 2;
                    Skill_1005(player, enemy);
                }
                else
                    isActionPointEnough = false;
                break;

            case 1006:
                if (actionPoint >= 2)
                {
                    actionPoint -= 2;
                    Skill_1006(player, enemy);
                }
                else
                    isActionPointEnough = false;
                break;
                
            case 1007:
                if (actionPoint >= 2)
                {
                    actionPoint -= 2;
                    Skill_1007(enemy);
                }
                else
                    isActionPointEnough = false;
                break;

            case 1008:
                if (actionPoint >= 2)
                {
                    actionPoint -= 2;
                    Skill_1008(player, enemy);
                }
                else
                    isActionPointEnough = false;
                break;

            case 1009:
                if (actionPoint >= 2)
                {
                    actionPoint -= 2;
                    Skill_1009(enemy);
                }
                else
                    isActionPointEnough = false;
                break;

            case 1010:
                if (actionPoint >= 2)
                {
                    actionPoint -= 2;
                    Skill_1010(enemy);
                }
                else
                    isActionPointEnough = false;
                break;

            case 1011:
                if (actionPoint >= 1)
                {
                    actionPoint -= 1;
                    Skill_1011(enemy);
                }
                else
                    isActionPointEnough = false;
                break;

            case 1012:
                if (actionPoint >= 2)
                {
                    actionPoint -= 2;
                    Skill_1012(enemy);
                }
                else
                    isActionPointEnough = false;
                break;

            case 1013:
                if (actionPoint >= 1)
                {
                    actionPoint -= 1;
                    Skill_1013(enemy);
                }
                else
                    isActionPointEnough = false;
                break;

            case 1014:
                if (actionPoint >= 2)
                {
                    actionPoint -= 2;
                    Skill_1014(enemy);
                }
                else
                    isActionPointEnough = false;
                break;

            case 1015:
                if (actionPoint >= 2)
                {
                    actionPoint -= 2;
                    Skill_1015(enemy);
                }
                else
                    isActionPointEnough = false;
                break;

            case 1016:
                if (actionPoint >= 1)
                {
                    actionPoint -= 1;
                    Skill_1016(enemy);
                }
                else
                    isActionPointEnough = false;
                break;

            case 1017:
                if (actionPoint >= 1)
                {
                    actionPoint -= 1;
                    Skill_1017(enemy);
                }
                else
                    isActionPointEnough = false;
                break;

            case 1018:
                if (actionPoint >= 2)
                {
                    actionPoint -= 2;
                    Skill_1018(player);
                }
                else
                    isActionPointEnough = false;
                break;

            case 1019:
                if (actionPoint >= 1)
                {
                    actionPoint -= 1;
                    Skill_1019(player);
                }
                else
                    isActionPointEnough = false;
                break;

            case 1020:
                if (actionPoint >= 1)
                {
                    actionPoint -= 1;
                    Skill_1020(player);
                }
                else
                    isActionPointEnough = false;
                break;

            case 1021:
                if (actionPoint >= 2)
                {
                    actionPoint -= 2;
                    Skill_1021(enemy);
                }
                else
                    isActionPointEnough = false;
                break;

            case 1022:
                if (actionPoint >= 2)
                {
                    actionPoint -= 2;
                    Skill_1022(enemy);
                }
                else
                    isActionPointEnough = false;
                break;

            case 1023:
                if (actionPoint >= 1)
                {
                    actionPoint -= 1;
                    Skill_1023();
                }
                else
                    isActionPointEnough = false;
                break;

            default:
                break;
        }
        string skillName = skillDic[skillId].skillName;
        if(!isActionPointEnough)    //行动点不足释放
        {
            UIManager.Instance.ShowPanel<WarningPanel>().SetWarningText($"行动点不足以释放技能「{skillName}」", true);
        }
        else    //行动点足够释放
        {
            UIManager.Instance.ShowPanel<WarningPanel>().SetWarningText($"你释放了技能「{skillName}」", true);


            if(equipment == null)
                return; //对普通攻击特判
                
            //更新装备耐久：
            equipment.currentDuration -= 1;
            EquipmentManager.Instance.equipmentDurationDic[equipment] -= 1; 

            //更新equipment耐久UI：
            EventHub.Instance.EventTrigger("UpdateEquipmentUI", equipment);

            //如果耐久归零：
            if(equipment.currentDuration == 0)
            {
                //报废该装备：
                EquipmentManager.Instance.RemoveEquipment(equipment);

                UIManager.Instance.ShowPanel<WarningPanel>().SetWarningText($"装备「{equipment.name}」因耐久不足而报废！");

                //从槽中卸除装备：
                EventHub.Instance.EventTrigger("UnequipTarget", equipment);
            }
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
        Damage damages = PlayerManager.Instance.CauseDamage(enemy, damage, damageType);
        if (damages.isAvoided)
        {
            Debug.Log("闪避触发");
            //更新显示UI：
            EventHub.Instance.EventTrigger("ParseDamage", damages, 1);
        }
        else
        {
            int count = 1;
            for (int i = 0; i < damages.GetSize(); ++i)
            {
                Debug.Log($"进行第 {count} 次连击!");
                // 造成伤害之前进行一些加成计算
                // 先计算易伤(DeBuff)加成
                damages[i].damage = TurnCounter.Instance.CalculateWithEnemyBuff(TriggerTiming.BeHit, damages.damageType, enemy.positionId, damages[i].damage);

                Debug.LogWarning($"step 1 结算出的对敌人的伤害：{damages[i].damage}");

                // 再判断伤害类型(方法内部自行判断)，并计算角色增伤(GoodBuff)加成
                damages[i].damage = TurnCounter.Instance.CalculateWithPlayerBuff(TriggerTiming.CalculateGoodBuffDamage, damages.damageType, damages[i].damage);

                Debug.LogWarning($"step 2 结算出的对敌人的伤害：{damages[i].damage}");

                // 敌人受击后添加新的Buff(只结算一遍)
                if(count == 1)
                {
                    TurnCounter.Instance.DealWithEnemyBuff(TriggerTiming.BeHit, damageType);
                }

                // 结算出的对敌人的伤害
                Debug.LogWarning($"结算出的对敌人的伤害：{damages[i].damage}");
                // EventHub.Instance.EventTrigger("UpdateDamangeText", damages[i].damage, true);

                // 之所以将附加Buff的方法放到这里来调用，是为了将 伤害命中 与 Buff命中 协同
                if(action != null && count == 1)
                {
                    action.Invoke();
                }
                count++;
            }

            //调用敌人受击方法
            EnemyManager.Instance.EnemyHurted(damages);
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
        player.HP.AddValue(-10);
        AddBuffToSkill_1004();
    }
    private void AddBuffToSkill_1004()
    {
        BattleBuff buff = new BattleBuff_1003();
        TurnCounter.Instance.AddPlayerBuff(buff);
    }

    // 破势击
    public void Skill_1005(Player player, Enemy enemy)
    {
        Debug.Log("角色发动 破势击！");
        float rowDamage = 30 + player.STR.value * 1.0f;
        Action action = () => AddBuffToSkill_1005(enemy);
        SkillManager.Instance.PlayerDamageCalculation(enemy, rowDamage, DamageType.Skill, action);
    }
    private void AddBuffToSkill_1005(Enemy enemy)
    {
        BattleBuff buff = new BattleBuff_1004();
        TurnCounter.Instance.AddEnemyBuff(buff);
    }

    // 缚心铎声
    public void Skill_1006(Player player, Enemy enemy)
    {
        Debug.Log("角色发动 缚心铎声！");
        float rowDamage = 50 + player.SAN.value * 1.0f;
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
    public void Skill_1008(Player player, Enemy enemy)
    {
        Debug.Log("角色发动 湖中女的复仇！");
        float rowDamage = 50 + (player.HP.value_limit - player.HP.value) * 1.0f;
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
        float rowDamage = 50 + (enemy.HP_limit - enemy.HP) * 1.0f;
        Action action = () => AddBuffToSkill_1010(enemy);
        SkillManager.Instance.PlayerDamageCalculation(enemy, rowDamage, DamageType.Skill, action);
    }
    private void AddBuffToSkill_1010(Enemy enemy)
    {
        BattleBuff buff = new BattleBuff_1009();
        TurnCounter.Instance.AddEnemyBuff(buff);
    }

    // 幽影呓语
    public void Skill_1011(Enemy enemy)
    {
        Debug.Log("角色发动 幽影呓语！");
        Player player = BattleManager.Instance.player;
        float rowDamage = 20 + player.SPD.value * 1.0f + player.LVL.value * 1.0f;
        Action action = () => AddBuffToSkill_1011(enemy);
        SkillManager.Instance.PlayerDamageCalculation(enemy, rowDamage, DamageType.Dot, action);
    }
    private void AddBuffToSkill_1011(Enemy enemy)
    {
        BattleBuff buff = new BattleBuff_1010();
        TurnCounter.Instance.AddEnemyBuff(buff);
    }

    // 恨意凝视
    public void Skill_1012(Enemy enemy)
    {
        Debug.Log("角色发动 恨意凝视！");
        Player player = BattleManager.Instance.player;
        float rowDamage = 20 + player.DEF.value * 1.0f;
        Action action = () => AddBuffToSkill_1012(enemy);
        SkillManager.Instance.PlayerDamageCalculation(enemy, rowDamage, DamageType.Dot, action);
    }
    private void AddBuffToSkill_1012(Enemy enemy)
    {
        BattleBuff buff = new BattleBuff_1011();
        TurnCounter.Instance.AddEnemyBuff(buff);
    }

    // 瘟疫吐息
    public void Skill_1013(Enemy enemy)
    {
        Debug.Log("角色发动 瘟疫吐息！");
        float rowDamage = 20;
        Action action = () => AddBuffToSkill_1013(enemy);
        SkillManager.Instance.PlayerDamageCalculation(enemy, rowDamage, DamageType.Dot, action);
    }
    private void AddBuffToSkill_1013(Enemy enemy)
    {
        BattleBuff buff = new BattleBuff_1012();
        TurnCounter.Instance.AddEnemyBuff(buff);
    }

    // 探知深空
    public void Skill_1014(Enemy enemy)
    {
        Debug.Log("角色发动 探知深空！");
        Player player = BattleManager.Instance.player;
        float rowDamage = 20 + player.SAN.value * 1.0f;
        Action action = () => AddBuffToSkill_1014(enemy);
        SkillManager.Instance.PlayerDamageCalculation(enemy, rowDamage, DamageType.Dot, action);
    }
    private void AddBuffToSkill_1014(Enemy enemy)
    {
        BattleBuff buff = new BattleBuff_1013();
        TurnCounter.Instance.AddEnemyBuff(buff);
    }

    // 饥饿掠夺
    public void Skill_1015(Enemy enemy)
    {
        Debug.Log("角色发动 饥饿掠夺！");
        Player player = BattleManager.Instance.player;
        float rowDamage = 20 + (player.HP.value_limit - player.HP.value) * 1.0f;
        Action action = () => AddBuffToSkill_1015(enemy);
        SkillManager.Instance.PlayerDamageCalculation(enemy, rowDamage, DamageType.Dot, action);
    }
    private void AddBuffToSkill_1015(Enemy enemy)
    {
        BattleBuff buff = new BattleBuff_1014();
        TurnCounter.Instance.AddEnemyBuff(buff);
    }

    // 怨念
    public void Skill_1016(Enemy enemy)
    {
        Debug.Log("角色发动 怨念！");
        Player player = BattleManager.Instance.player;
        float rowDamage = 30 + enemy.STR * 0.5f;
        Action action = () => AddBuffToSkill_1016(enemy);
        SkillManager.Instance.PlayerDamageCalculation(enemy, rowDamage, DamageType.Dot, action);
    }
    private void AddBuffToSkill_1016(Enemy enemy)
    {
        BattleBuff buff = new BattleBuff_1015();
        TurnCounter.Instance.AddEnemyBuff(buff);
    }

    // 深海的呼唤
    public void Skill_1017(Enemy enemy)
    {
        Debug.Log("角色发动 深海的呼唤！");
        Player player = BattleManager.Instance.player;
        float rowDamage = 20 + enemy.STR * 0.5f;
        Action action = () => AddBuffToSkill_1017(enemy);
        SkillManager.Instance.PlayerDamageCalculation(enemy, rowDamage, DamageType.Skill, action);
    }
    private void AddBuffToSkill_1017(Enemy enemy)
    {
        BattleBuff buff = new BattleBuff_1016();
        TurnCounter.Instance.AddEnemyBuff(buff);
    }

    // 深渊之主的回音
    public void Skill_1018(Player player)
    {
        Debug.Log("角色发动 深渊之主的回音！");
        player.HP.AddValue(60);
        AddBuffToSkill_1018();
    }
    private void AddBuffToSkill_1018()
    {
        BattleBuff buff = new BattleBuff_1017();
        TurnCounter.Instance.AddEnemyBuff(buff);
    }

    // 风起之时
    public void Skill_1019(Player player)
    {
        Debug.Log("角色发动 风起之时！");
        player.HP.AddValue(20);
        AddBuffToSkill_1019();
    }
    private void AddBuffToSkill_1019()
    {
        BattleBuff buff = new BattleBuff_1018();
        TurnCounter.Instance.AddEnemyBuff(buff);
    }

    // 鼠群意志
    public void Skill_1020(Player player)
    {
        Debug.Log("角色发动 鼠群意志！");
        player.HP.AddValue(20 + player.SAN.value * 2.0f + player.SPD.value * 1.0f);
        AddBuffToSkill_1020();
    }
    private void AddBuffToSkill_1020()
    {
        BattleBuff buff = new BattleBuff_1019();
        TurnCounter.Instance.AddPlayerBuff(buff);
    }

    // 神骨之鞭
    public void Skill_1021(Enemy enemy)
    {
        Debug.Log("角色发动 神骨之鞭！");
        Player player = BattleManager.Instance.player;
        float rowDamage = 20 + player.STR.value * 1.0f + player.SAN.value * 1.0f;
        Action action = () => AddBuffToSkill_1021(enemy);
        SkillManager.Instance.PlayerDamageCalculation(enemy, rowDamage, DamageType.Skill, action);
    }
    private void AddBuffToSkill_1021(Enemy enemy)
    {
        BattleBuff buff = new BattleBuff_1020();
        TurnCounter.Instance.AddEnemyBuff(buff);
    }

    // 禁咒
    public void Skill_1022(Enemy enemy)
    {
        Debug.Log("角色发动 禁咒！");
        Player player = BattleManager.Instance.player;
        float rowDamage = 20 + (enemy.HP_limit - enemy.HP) * 0.8f;
        Action action = () => AddBuffToSkill_1022(enemy);
        SkillManager.Instance.PlayerDamageCalculation(enemy, rowDamage, DamageType.Skill, action);
    }
    private void AddBuffToSkill_1022(Enemy enemy)
    {
        BattleBuff buff = new BattleBuff_1021();
        TurnCounter.Instance.AddEnemyBuff(buff);
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
