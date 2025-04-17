using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player               //存储角色信息等
{
    public struct PlayerAttribute           //角色属性（在读取角色信息表时再实例化）
    {
        public int id;
        public string name;
        public int level;
        public string icon;
        public int type;
        public float value;
        public float value_limit;           //角色属性的上限值
        public float minLimit;
        //public float extra_value;           //属性值额外获得的量

        public PlayerAttribute(int id, int level = 0, int type = 0, string name = null, string icon = null)
        {
            this.id = id;
            this.name = name;
            this.level = level;
            this.icon = icon;
            this.type = type;
            this.value = 0f;         //初始化为0
            this.value_limit = 100;
            this.minLimit = 1;
            //this.extra_value = 0;
        }

        public void ChangeValue(float change)     //用于调整角色属性
        {
            if (type == 0)          //整数
            {
                value += change;
            }
            else                    //万分比
            {
                //Marc调整：调整前：
                //value += change * 0.0001f;
                value += value * change * 0.0001f;
            }

            if (value < 0)
            {
                Debug.Log($"属性id：{this.id}，属性名称：{this.name}  已达最小值");
                value = 0;          //假设 属性值 不能为负值
            }

            if (value > 100 && type == 0)
            {
                Debug.Log($"属性id：{this.id}，属性名称：{this.name}  已达最大值");
                value = 100;        //假设 整数类型属性值 最大为100
            }
        }

    }

    //静态基本属性
    //public int HP_limit = 100;
    //动态基本属性
    public PlayerAttribute HP;          //生命     Health point      id = 1
    public PlayerAttribute STR;         //力量     Strength          id = 2  
    public PlayerAttribute DEF;         //防御     Defense           id = 3 
    public PlayerAttribute LVL;         //灯光值   Light Value       id = 4  
    public PlayerAttribute SAN;         //SAN 值   Sanity            id = 5 
    public PlayerAttribute SPD;         //速度     Speed             id = 6 
    public PlayerAttribute CRIT_Rate;   //暴击率   Critical Hit Rate id = 7 
    public PlayerAttribute CRIT_DMG;    //暴击伤害 Critical Damage   id = 8 
    public PlayerAttribute HIT;         //连击     Hit               id = 9 
    public PlayerAttribute AVO;         //闪避值   AVO               id = 10
    //动态特殊属性

    public Dictionary<int, Item> bag;   //??感觉用List来存会好一些??

    public Player()
    {
        //初始化列表在PlayerManager中；
        // HP = new playerAttribute(1);
        // HP.value = 100;
        // HP.type = 1;

        // STR = new playerAttribute(2);
        // STR.value = 10;

        // DEF = new playerAttribute(3);
        // DEF.value = 1;

        // LVL = new playerAttribute(4);
        // LVL.value = 1;

        // SAN = new playerAttribute(5);
        // SAN.value = 40;

        // SPD = new playerAttribute(6);
        // SPD.value = 10;

        // CRIT_Rate = new playerAttribute(7);
        // CRIT_Rate.value = 0.1f;
        // CRIT_Rate.type = 1;

        // CRIT_DMG = new playerAttribute(8);
        // HIT = new playerAttribute(9);

        // AVO = new playerAttribute(10);
        // AVO.value = 0.3f;
        // AVO.type = 1;

        DebugInfo();

        // bag = new Dictionary<int, Item>();
    }

    //这是啥？析构函数吗（Marc疑问）(是滴，对性能优化时用，但非必要)
    // ~Player()
    // {
    //     bag = null;
    // }

    public void GameOver()
    {

    }

    // 进入角色回合
    public void EnterPlayerTurn()
    {
        int target = 1;
        int skillId = 0;
        bool isSelectEnemy = false;

        while (!isSelectEnemy)
        {
            isSelectEnemy = IsSelectEnemy(out target);
        }

        ReleaseSkill(skillId, EnemyManager.Instance.enemies[target]);

    }

    // 选择要攻击的敌人的位置id(现在先假设是 1V1 情况)
    // 在
    public bool IsSelectEnemy(out int positionId)
    {
        foreach(var e in EnemyManager.Instance.enemies)
        {
            positionId = e.Value.positionId;
            return true;
        }
        
        positionId = -1;
        return false;
    }

    // 释放技能
    public void ReleaseSkill(int skillId, Enemy enemy)
    {
        switch (skillId) 
        {
            case 1:
                BasicAttack(enemy);
                break;
            case 2:
                // 其他技能
                break;
            default:
                break;
        }

    }

    // 普通攻击
    public void BasicAttack(Enemy enemy)    //传入攻击的enemy实例
    {
        Debug.Log("角色普通攻击发动！");
        //将STR属性值转化为 攻击值 
        float rowDamage = STR.value * 1f;   //?? 假设伤害倍率就是100% ??
        float damage = PlayerManager.Instance.CalculateDamageAfterBuff(AttributeType.HP, rowDamage);
        List<Damage> damages = PlayerManager.Instance.CauseDamage(damage);
        if(damages.Count == 0)
        {
            Debug.Log("角色发出的伤害被闪避了!");
        }
        else
        {
            foreach(var dmg in damages)
            {
                //调用敌人受击方法
                EnemyManager.Instance.EnemyValueChange(enemy.positionId, dmg);
            }
        }

    }

    public void BeHurted(Damage damage)
    {

    }

    public void DebugInfo()
    {
        Debug.LogWarning($"HP: {HP.value}, \n STR:{STR.value}, \n DEF:{DEF.value}, \n SAN:{SAN.value}, \n LVL:{LVL.value}, \n SPD:{SPD.value}, \n CRIT_Rate:{CRIT_Rate.value}, \n CRIT_DMG:{CRIT_DMG.value}, \n HIT:{HIT.value}, \n AVO:{AVO.value}");
    }

}


public class Damage
{
    //是否是暴击伤害
    public bool isCritical;
    //伤害量
    public float damage;
}
