using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//道具的数据结构类；和Item本质上性质是一样的
public class Equipment
{
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
   public bool isInUse;
   //最大耐久：
   public int maxDuration;
   //当前耐久：
   public int currentDuration;
}

//数据结构类：技能：
public class Skill
{
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
}


//对装备进行分级的枚举：
public enum E_EquiptmentLevel
{
    RedHighest = 0,
    OrangeMedium = 1,
    BlueLowest = 2,
}
