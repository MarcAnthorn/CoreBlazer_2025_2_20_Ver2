using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//道具的数据结构类；和Item本质上性质是一样的
public abstract class Equipment
{
    //装备id：
    public int id;
   //装备名：
   public string name;
   //图标名：
   public string iconPath;
   //装备效果描述：
   public string effectDescriptionText;
   //装备文本：
   public string descriptionText;
   //装备等级
   public E_EquiptmentLevel level;
   //是否在使用中（装备中）：
   public bool isEquipped;
   //最大耐久：
   public int maxDuration;
   //当前耐久：
   public int currentDuration;

   //当前装备的技能：通过访问skill实例，从而通过实例中的方法进行技能内容的调用：
   public Skill mySkill;

   //深拷贝构造函数：
   //就算是同一种装备，其耐久也是独立计算的，因此不能像是Item一样使用若干引用指向同一个实例；
   //Item本质就是数量的堆叠；
   public Equipment(Equipment other)
   {
        name = other.name;
        iconPath = other.iconPath;
        effectDescriptionText = other.effectDescriptionText;
        descriptionText = other.descriptionText;
        level = other.level;
        isEquipped = other.isEquipped;
        maxDuration = other.maxDuration;
        currentDuration = maxDuration;

        //通过LoadManager分配实例：和Item一样；
        //因为Skill不需要深拷贝；
        mySkill = LoadManager.Instance.allSkills[other.mySkill.id];
   }

    //无参构造函数：
    public Equipment(){}

   //通过继承子类的方式，实现不同装备的效果（和道具一样）
   public abstract void Use();
}

//数据结构类：技能：
public abstract class Skill
{
    //技能id
    public int id;
    //技能名称：
    public string skillName;
    //技能图标名：
    public string skillIconPath;
    //技能伤害效果描述：
    public string skillDamageText;
    //技能buff / debuff 描述：
    public string skillBuffText;
    //技能消耗费用：
    public int skillCost;   
    //技能的使用：
    public abstract void Use();



}


//和道具一样，不同的装备会有不同的实现
public class Equipment_1001 : Equipment
{
    public Equipment_1001(Equipment other) : base(other)
    {}

    //无参构造函数：
    public Equipment_1001(){}

    public override void Use()
    {
        
    }
}


//技能也应该是不同的实现，需要手动完成；
public class Skill_1001 : Skill
{
    public override void Use()
    {
        
    }
}



//对装备进行分级的枚举：
public enum E_EquiptmentLevel
{
    RedHighest = 0,
    OrangeMedium = 1,
    BlueLowest = 2,
}