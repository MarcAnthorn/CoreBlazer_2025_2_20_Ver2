using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.Rendering.DebugUI;

public enum AttributeType
{
    NONE = 0,
    HP = 1,
    STR = 2,
    DEF = 3,
    LVL = 4,
    SAN = 5,
    SPD = 6,
    CRIT_Rate = 7,
    CRIT_DMG = 8,
    HIT = 9,
    AVO = 10
}

public enum E_PlayerSceneIndex{
    Event = 1,
    Battle = 2,
    Maze = 3,
}
public class PlayerManager : Singleton<PlayerManager>          //用于管理角色的事件
{
    public Player player;               //当前角色
    public Vector3 initPosition;

    //特殊字段：当前玩家所处的场景；
    //用于是否能使用道具的判断；
    //对应关系如下：1-事件选择 2-战斗场景 3-迷宫w内；为此声明了一个枚举；
    //分别在：事件UI触发、战斗事件触发、迷宫内的时候进行更新；
    public E_PlayerSceneIndex playerSceneIndex;
    public static object _lock = new object();

    protected override void Awake()
    {
        base.Awake();

        //测试用：
        //该方法在Awake中调用，确保全局只触发一次
        InitPlayer();
    }

    //使用只读属性暴露玩家数值
    //报错，暂时先调整：（Marc调整）
    public float Health => player.HP.value;

    public float Strength => player.STR.value;

    public float Defence => player.DEF.value;
    public float Level => player.LVL.value;
    public float Sanity => player.SAN.value;

    public float Speed => player.SPD.value;

    // 暂未定义玩家灯火值
    // public int Light => player.LIT;



    public void InitPlayer()
    {
        //PlayerManager 管理的全局唯一Player实例
       player = new Player()
        {
            HP = new Player.PlayerAttribute(1) { value = 100},  
            // 生命值 (Health Point)，id = 1，初始值 100

            STR = new Player.PlayerAttribute(2) { value = 10 },           
            // 力量 (Strength)，id = 2，初始值 10

            DEF = new Player.PlayerAttribute(3) { value = 5 },           
            // 防御 (Defense)，id = 3，初始值 5

            LVL = new Player.PlayerAttribute(4) { value = 1 },            
            // 灯光值 (Light Value)，id = 4，初始值 1

            SAN = new Player.PlayerAttribute(5) { value = 40 },           
            // SAN 值 (Sanity)，id = 5，初始值 40

            SPD = new Player.PlayerAttribute(6) { value = 10 },          
            // 速度 (Speed)，id = 6，初始值 10

            CRIT_Rate = new Player.PlayerAttribute(7) { value = 0.1f, type = 1 }, 
            // 暴击率 (Critical Hit Rate)，id = 7，初始值 10%

            CRIT_DMG = new Player.PlayerAttribute(8),                    
            // 暴击伤害 (Critical Damage)，id = 8，未初始化

            HIT = new Player.PlayerAttribute(9),                          
            // 连击 (Hit)，id = 9，未初始化

            AVO = new Player.PlayerAttribute(10) { value = 0.3f, type = 1 }, 
            // 闪避值 (AVO)，id = 10，初始值 30%

            bag = new Dictionary<int, Item>()  // 初始化物品栏
        };
    }

    private BuffType GetPlayerBuffType(AttributeType type)
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

    private void PlayerValueChange(AttributeType type, float finalValue)
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

    //暂时将对敌人的数值处理放在这里
    private void EnemyValueChange(AttributeType type, float finalValue)
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

    //战斗外的 角色属性调整方法(角色成长)
    public void PlayerAttributeChange(AttributeType type, float value)
    {
        //这是为了明确buff要对什么属性产生影响(根据buffType来判断)
        BuffType buffType = GetPlayerBuffType(type);
        //finalValue表示 角色成长 造成的实际数值变化
        float finalValue = BuffManager.Instance.BuffEffectInGrowUp(buffType, value);
        PlayerManager.Instance.PlayerValueChange(type, finalValue);
    }

    //战斗内的 角色对敌人造成影响的方法(一般来说就是HP的伤害，不过也可能会存在减速(SPD)，削弱敌人攻击(STR)等情况)
    public void PlayerInfluenceToEnemy(AttributeType type, float value)
    {
        //这是为了明确buff要不要执行(根据buffType来判断)
        BuffType buffType = GetPlayerBuffType(type);
        //finalValue表示 战斗过程 造成的实际数值变化
        float finalValue = BuffManager.Instance.BuffEffectInBattle(buffType, value);
        PlayerManager.Instance.EnemyValueChange(type, finalValue);
    }

    public void CalculationPlayerFinalDamage()
    {
        // 注意!!! 此处应该按照给出的伤害计算公式计算 角色发出的 攻击伤害量
    }

}
