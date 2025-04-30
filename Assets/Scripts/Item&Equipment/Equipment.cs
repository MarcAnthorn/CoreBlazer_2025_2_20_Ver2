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
   //Player的引用：
   protected Player player;

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
        player = PlayerManager.Instance.player;

        //通过LoadManager分配实例：和Item一样；
        //因为Skill不需要深拷贝；
        mySkill = LoadManager.Instance.allSkills[other.mySkill.id];
   }

    //无参构造函数：
    public Equipment(){}

   //通过继承子类的方式，实现不同装备的效果（和道具一样）
   public abstract void Equip();
   public abstract void Unequip();
}

//和道具一样，不同的装备会有不同的实现
public class Equipment_1001 : Equipment
{
    public Equipment_1001(Equipment other) : base(other)
    {}

    //当前百分比加成的数值的增量；用于在取消装备的时候，确保减少的量和增加的量是一致的；
    private float strDelta = 0;

    //无参构造函数：
    public Equipment_1001(){
        id = 1001;
        name = "尾后针";
        iconPath = "装备/尾后针";
        effectDescriptionText = "装备后：速度 + 10, 力量 + 10%";
        descriptionText = "来源于旧日信徒毒蝎之王的毒针，虽然它随着毒蝎之王的陨落早已没有往日的光泽，但是对大对数低等生命体来说依旧是致命的。";
        level = E_EquiptmentLevel.RedHighest;
        isEquipped = false;
        maxDuration = 10;
        currentDuration = maxDuration;
        mySkill = LoadManager.Instance.allSkills[1002];
    }

    public override void Equip()
    {
        player.SPD.AddValue(10);
        strDelta = player.STR.value * 0.1f;
        player.STR.AddValue(strDelta);

        player.DebugInfo();
    }

    public override void Unequip()
    {
        player.SPD.AddValue(-10);
        player.STR.AddValue(-strDelta);

        player.DebugInfo();
    }
}


public class Equipment_1002 : Equipment
{
    public Equipment_1002(Equipment other) : base(other)
    {}

    //无参构造函数：
    public Equipment_1002(){
        id = 1002;
        name = "钢铁新月";
        iconPath = "装备/钢铁新月";
        effectDescriptionText = "装备后：闪避 + 10%, 力量 + 100%";
        descriptionText = "苏美尔人遗迹中出土的铁质弯月形祭祀用品，记载着两河流域对某个古神的信仰。";
        level = E_EquiptmentLevel.OrangeMedium;
        isEquipped = false;
        maxDuration = 5;
        currentDuration = maxDuration;
        mySkill = LoadManager.Instance.allSkills[1003];
    }

    private float strDelta = 0;

    public override void Equip()
    {
        player.AVO.AddValue(0.1f);

        strDelta = player.STR.value;
        player.STR.AddValue(strDelta);

        player.DebugInfo();
    }

    public override void Unequip()
    {
        player.AVO.AddValue(-0.1f);
        player.STR.AddValue(-strDelta);

        player.DebugInfo();
    }
}


public class Equipment_1003 : Equipment
{
    public Equipment_1003(Equipment other) : base(other)
    {}

    //无参构造函数：
    public Equipment_1003(){
        id = 1003;
        name = "琥珀火莲";
        iconPath = "装备/琥珀火莲";
        effectDescriptionText = "装备后：力量 + 5, 防御 + 5, 生命 + 15";
        descriptionText = "被不知名力量封印在琥珀中的火焰，以莲花的形状持续燃烧着。";
        level = E_EquiptmentLevel.BlueLowest;
        isEquipped = false;
        maxDuration = 5;
        currentDuration = maxDuration;

        //技能暂时置顶1003:
        mySkill = LoadManager.Instance.allSkills[1003];
    }

    public override void Equip()
    {
        player.STR.AddValue(5);
        player.DEF.AddValue(5);
        player.HP.AddValue(15);

        player.DebugInfo();
    }

    public override void Unequip()
    {
        player.STR.AddValue(-5);
        player.DEF.AddValue(-5);
        player.HP.AddValue(-15);

        player.DebugInfo();
    }
}



//对装备进行分级的枚举：
public enum E_EquiptmentLevel
{
    RedHighest = 0,
    OrangeMedium = 1,
    BlueLowest = 2,
}