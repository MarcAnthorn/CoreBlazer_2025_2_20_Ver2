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
        id = other.id;
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
// 装备1001 - 尾后针
public class Equipment_1001 : Equipment
{
    private float strDelta = 0;

    public Equipment_1001(Equipment other) : base(other) {}

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
        foreach(var key in LoadManager.Instance.allSkills.Keys)
        {
            Debug.Log($"key:{key},value : {LoadManager.Instance.allSkills[key].skillName }");
        }
    
        mySkill = LoadManager.Instance.allSkills[1002];
    }

    public override void Equip()
    {
        player.SPD.AddValue(10);
        strDelta = player.STR.value * 0.1f;
        player.STR.AddValue(strDelta);
        player.CRIT_DMG.AddValue(player.CRIT_DMG.value * 0.1f);
        player.DebugInfo();
    }

    public override void Unequip()
    {
        player.SPD.AddValue(-10);
        player.STR.AddValue(-strDelta);
        player.CRIT_DMG.AddValue(-player.CRIT_DMG.value * 0.1f);
        player.DebugInfo();
    }
}

// 装备1002 - 钢铁新月
public class Equipment_1002 : Equipment
{
    private float strDelta = 0;

    public Equipment_1002(Equipment other) : base(other) {}

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

    public override void Equip()
    {
        player.AVO.AddValue(0.1f);
        strDelta = player.STR.value * 1f;
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

// 装备1003 - 琥珀火莲
public class Equipment_1003 : Equipment
{
    public Equipment_1003(Equipment other) : base(other) {}

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
        mySkill = LoadManager.Instance.allSkills[1004];
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

// 装备1004 - 嗡鸣殛刀
public class Equipment_1004 : Equipment
{
    private float strDelta = 0;

    public Equipment_1004(Equipment other) : base(other) {}

    public Equipment_1004(){
        id = 1004;
        name = "嗡鸣殛刀";
        iconPath = "装备/嗡鸣殛刀";
        effectDescriptionText = "装备后：力量 + 100%";
        descriptionText = "它的刀刃像是有生命一样颤动着，可以依靠高频振动撕裂敌人的血肉。";
        level = E_EquiptmentLevel.BlueLowest;
        isEquipped = false;
        maxDuration = 5;
        currentDuration = maxDuration;
        mySkill = LoadManager.Instance.allSkills[1005];
    }

    public override void Equip()
    {
        strDelta = player.STR.value * 1f;
        player.STR.AddValue(strDelta);
        player.HIT.AddValue(player.HIT.value * 0.4f);
        player.CRIT_DMG.AddValue(player.CRIT_DMG.value * 0.1f);
        player.DebugInfo();
    }

    public override void Unequip()
    {
        player.STR.AddValue(-strDelta);
        player.HIT.AddValue(-player.HIT.value * 0.4f);
        player.CRIT_DMG.AddValue(-player.CRIT_DMG.value * 0.1f);
        player.DebugInfo();
    }
}

// 装备1005 - 诅咒锈铎
public class Equipment_1005 : Equipment
{
    public Equipment_1005(Equipment other) : base(other) {}

    public Equipment_1005(){
        id = 1005;
        name = "诅咒锈铎";
        iconPath = "装备/诅咒锈铎";
        effectDescriptionText = "装备后：力量 + 20, 暴击率 + 10%";
        descriptionText = "长满了铜锈的青铜铃铛，据说摇响这个铃铛会发生可怕的事情。";
        level = E_EquiptmentLevel.BlueLowest;
        isEquipped = false;
        maxDuration = 5;
        currentDuration = maxDuration;
        mySkill = LoadManager.Instance.allSkills[1006];
    }

    public override void Equip()
    {
        player.STR.AddValue(20);
        player.CRIT_Rate.AddValue(0.1f);
        player.CRIT_DMG.AddValue(player.CRIT_DMG.value * 0.3f);
        player.DebugInfo();
    }

    public override void Unequip()
    {
        player.STR.AddValue(-20);
        player.CRIT_Rate.AddValue(-0.1f);
        player.CRIT_DMG.AddValue(-player.CRIT_DMG.value * 0.3f);
        player.DebugInfo();
    }
}

// 装备1006 - 逐日弓
public class Equipment_1006 : Equipment
{
    public Equipment_1006(Equipment other) : base(other) {}

    public Equipment_1006(){
        id = 1006;
        name = "逐日弓";
        iconPath = "装备/逐日弓";
        effectDescriptionText = "装备后：力量 + 10, 防御 + 10";
        descriptionText = "一位战士打造的武器，据说是为了射下多余的“太阳”，但是哪些东西真的是太阳吗？";
        level = E_EquiptmentLevel.BlueLowest;
        isEquipped = false;
        maxDuration = 5;
        currentDuration = maxDuration;
        mySkill = LoadManager.Instance.allSkills[1007];
    }

    public override void Equip()
    {
        player.STR.AddValue(10);
        player.DEF.AddValue(10);
        player.CRIT_Rate.AddValue(player.CRIT_Rate.value * 0.3f);
        player.DebugInfo();
    }

    public override void Unequip()
    {
        player.STR.AddValue(-10);
        player.DEF.AddValue(-10);
        player.CRIT_Rate.AddValue(-player.CRIT_Rate.value * 0.3f);
        player.DebugInfo();
    }
}

// 装备1007 - 湖中女
public class Equipment_1007 : Equipment
{
    public Equipment_1007(Equipment other) : base(other) {}

    public Equipment_1007(){
        id = 1007;
        name = "湖中女";
        iconPath = "装备/湖中女";
        effectDescriptionText = "装备后：暴击率 + 50%, 暴击伤害 + 50%, 命中 + 50%";
        descriptionText = "一把折断的银剑，它的上一位主人在和哪些东西战斗的过程中陨落了，但是它依旧保持着锐利的杀意。";
        level = E_EquiptmentLevel.BlueLowest;
        isEquipped = false;
        maxDuration = 5;
        currentDuration = maxDuration;
        mySkill = LoadManager.Instance.allSkills[1008];
    }

    public override void Equip()
    {
        player.CRIT_Rate.AddValue(0.5f);
        player.CRIT_DMG.AddValue(0.5f);
        player.HIT.AddValue(0.5f);
        player.DebugInfo();
    }

    public override void Unequip()
    {
        player.CRIT_Rate.AddValue(-0.5f);
        player.CRIT_DMG.AddValue(-0.5f);
        player.HIT.AddValue(-0.5f);
        player.DebugInfo();
    }
}

// 装备1008 - 八音盒
public class Equipment_1008 : Equipment
{
    public Equipment_1008(Equipment other) : base(other) {}

    public Equipment_1008(){
        id = 1008;
        name = "八音盒";
        iconPath = "装备/八音盒";
        effectDescriptionText = "装备后：命中 + 50%";
        descriptionText = "曾经寄宿着人们的童年回忆，但是在被污染后变成了旧日信仰的传播器。它似乎忘记了自己的初心，变成了杀戮的机器。";
        level = E_EquiptmentLevel.BlueLowest;
        isEquipped = false;
        maxDuration = 5;
        currentDuration = maxDuration;
        mySkill = LoadManager.Instance.allSkills[1009];
    }

    public override void Equip()
    {
        player.HIT.AddValue(0.5f);
        player.CRIT_DMG.AddValue(player.CRIT_DMG.value * 0.3f);
        player.CRIT_Rate.AddValue(player.CRIT_Rate.value * 0.3f);
        player.DebugInfo();
    }

    public override void Unequip()
    {
        player.HIT.AddValue(-0.5f);
        player.CRIT_DMG.AddValue(-player.CRIT_DMG.value * 0.3f);
        player.CRIT_Rate.AddValue(-player.CRIT_Rate.value * 0.3f);
        player.DebugInfo();
    }
}

// 装备1009 - 破伤风之刃
public class Equipment_1009 : Equipment
{
    public Equipment_1009(Equipment other) : base(other) {}

    public Equipment_1009(){
        id = 1009;
        name = "破伤风之刃";
        iconPath = "装备/破伤风之刃";
        effectDescriptionText = "装备后：命中 + 20%";
        descriptionText = "被刀划伤并不可怕，可怕的是被锈迹斑斑的刀划伤了....";
        level = E_EquiptmentLevel.BlueLowest;
        isEquipped = false;
        maxDuration = 5;
        currentDuration = maxDuration;
        mySkill = LoadManager.Instance.allSkills[1010];
    }

    public override void Equip()
    {
        player.HIT.AddValue(0.2f);
        player.CRIT_DMG.AddValue(player.CRIT_DMG.value * 0.4f);
        player.DebugInfo();
    }

    public override void Unequip()
    {
        player.HIT.AddValue(-0.2f);
        player.CRIT_DMG.AddValue(-player.CRIT_DMG.value * 0.4f);
        player.DebugInfo();
    }
}

// 装备1010 - 科拉佐斯之口
public class Equipment_1010 : Equipment
{
    public Equipment_1010(Equipment other) : base(other) {}

    public Equipment_1010(){
        id = 1010;
        name = "科拉佐斯之口";
        iconPath = "装备/科拉佐斯之口";
        effectDescriptionText = "装备后：暴击率 + 50%, 暴击伤害 + 50%";
        descriptionText = "在激烈的抵抗中被撕扯下的嘴巴。虽然已经离开了本体，但是依旧会发出喃喃低语。";
        level = E_EquiptmentLevel.BlueLowest;
        isEquipped = false;
        maxDuration = 5;
        currentDuration = maxDuration;
        mySkill = LoadManager.Instance.allSkills[1011];
    }

    public override void Equip()
    {
        player.CRIT_Rate.AddValue(0.5f);
        player.CRIT_DMG.AddValue(0.5f);
        player.DebugInfo();
    }

    public override void Unequip()
    {
        player.CRIT_Rate.AddValue(-0.5f);
        player.CRIT_DMG.AddValue(-0.5f);
        player.DebugInfo();
    }
}

// 装备1011 - 科拉佐斯之眼
public class Equipment_1011 : Equipment
{
    public Equipment_1011(Equipment other) : base(other) {}

    public Equipment_1011(){
        id = 1011;
        name = "科拉佐斯之眼";
        iconPath = "装备/科拉佐斯之眼";
        effectDescriptionText = "装备后：命中 + 50%";
        descriptionText = "在吞噬神明的阴谋之中，看到了不该看到的东西，因此被某位存在挖出双眼，眼珠依旧保持着憎恨的目光。";
        level = E_EquiptmentLevel.BlueLowest;
        isEquipped = false;
        maxDuration = 5;
        currentDuration = maxDuration;
        mySkill = LoadManager.Instance.allSkills[1012];
    }

    public override void Equip()
    {
        player.HIT.AddValue(0.5f);
        player.STR.AddValue(20f);
        player.CRIT_DMG.AddValue(player.CRIT_DMG.value * 0.5f);
        player.DebugInfo();
    }

    public override void Unequip()
    {
        player.HIT.AddValue(-0.5f);
        player.STR.AddValue(-20f);
        player.CRIT_DMG.AddValue(player.CRIT_DMG.value * 0.5f);
        player.DebugInfo();
    }
}

// 装备1012 - 科拉佐斯之鼻
public class Equipment_1012 : Equipment
{
    public Equipment_1012(Equipment other) : base(other) {}

    public Equipment_1012(){
        id = 1012;
        name = "科拉佐斯之鼻";
        iconPath = "装备/科拉佐斯之鼻";
        effectDescriptionText = "装备后：命中 + 30%";
        descriptionText = "科拉佐斯陨落后被割下的鼻子，虽然它的肉体已经死亡，但是这个鼻子依旧在自主呼吸，吐息之间散发着瘟疫。";
        level = E_EquiptmentLevel.BlueLowest;
        isEquipped = false;
        maxDuration = 5;
        currentDuration = maxDuration;
        mySkill = LoadManager.Instance.allSkills[1013];
    }

    public override void Equip()
    {
        player.HIT.AddValue(0.3f);
        player.SPD.AddValue(player.SPD.value * 0.2f);
        player.DebugInfo();
    }

    public override void Unequip()
    {
        player.HIT.AddValue(-0.3f);
        player.SPD.AddValue(-player.SPD.value * 0.2f);
        player.DebugInfo();
    }
}

// 装备1013 - 科拉佐斯之耳
public class Equipment_1013 : Equipment
{
    public Equipment_1013(Equipment other) : base(other) {}

    public Equipment_1013(){
        id = 1013;
        name = "科拉佐斯之耳";
        iconPath = "装备/科拉佐斯之耳";
        effectDescriptionText = "装备后：命中 + 50%";
        descriptionText = "“不该听的不要听”，这是犹格索托斯在这只耳边留下的最后一句话。";
        level = E_EquiptmentLevel.BlueLowest;
        isEquipped = false;
        maxDuration = 5;
        currentDuration = maxDuration;
        mySkill = LoadManager.Instance.allSkills[1014];
    }

    public override void Equip()
    {
        player.HIT.AddValue(0.5f);
        player.AVO.AddValue(player.AVO.value * 0.2f);
        player.DebugInfo();
    }

    public override void Unequip()
    {
        player.HIT.AddValue(-0.5f);
        player.AVO.AddValue(-player.AVO.value * 0.2f);
        player.DebugInfo();
    }
}

// 装备1014 - 科拉佐斯之牙
public class Equipment_1014 : Equipment
{
    public Equipment_1014(Equipment other) : base(other) {}

    public Equipment_1014(){
        id = 1014;
        name = "科拉佐斯之牙";
        iconPath = "装备/科拉佐斯之牙";
        effectDescriptionText = "装备后：命中 + 50%";
        descriptionText = "“把牙扔到房顶上可以长高哦？哈哈，骗你的，毕竟你没有下半身了”";
        level = E_EquiptmentLevel.BlueLowest;
        isEquipped = false;
        maxDuration = 5;
        currentDuration = maxDuration;
        mySkill = LoadManager.Instance.allSkills[1015];
    }

    public override void Equip()
    {
        player.HIT.AddValue(0.5f);
        player.HIT.AddValue(player.HIT.value * 0.4f);
        player.DebugInfo();
    }

    public override void Unequip()
    {
        player.HIT.AddValue(-0.5f);
        player.HIT.AddValue(-player.HIT.value * 0.4f);
        player.DebugInfo();
    }
}

// 装备1015 - 科拉佐斯之颅
public class Equipment_1015 : Equipment
{
    public Equipment_1015(Equipment other) : base(other) {}

    public Equipment_1015(){
        id = 1015;
        name = "科拉佐斯之颅";
        iconPath = "装备/科拉佐斯之颅";
        effectDescriptionText = "装备后：命中 + 200%";
        descriptionText = "可怜的信徒们拼了命的在肉泥中寻找，只找到了这块颅骨。索性它依旧带着一丝生命的气息，以及.....强大的怨气。";
        level = E_EquiptmentLevel.BlueLowest;
        isEquipped = false;
        maxDuration = 5;
        currentDuration = maxDuration;
        mySkill = LoadManager.Instance.allSkills[1016];
    }

    public override void Equip()
    {
        player.HIT.AddValue(2f);
        player.SPD.AddValue(player.SPD.value * 1.0f);
        player.DebugInfo();
    }

    public override void Unequip()
    {
        player.HIT.AddValue(-2f);
        player.SPD.AddValue(-player.SPD.value * 1.0f);
        player.DebugInfo();
    }
}

// 装备1016 - 拉莱耶的破损神像
public class Equipment_1016 : Equipment
{
    public Equipment_1016(Equipment other) : base(other) {}

    public Equipment_1016(){
        id = 1016;
        name = "拉莱耶的破损神像";
        iconPath = "装备/拉莱耶的破损神像";
        effectDescriptionText = "装备后：暴击率 + 80%, 暴击伤害 + 10%";
        descriptionText = "拉莱耶古城中破败的雕像，它们所崇拜的主人早已逃离，只留下一片破败的废墟。";
        level = E_EquiptmentLevel.BlueLowest;
        isEquipped = false;
        maxDuration = 5;
        currentDuration = maxDuration;
        mySkill = LoadManager.Instance.allSkills[1017];
    }

    public override void Equip()
    {
        player.CRIT_Rate.AddValue(0.8f);
        player.CRIT_DMG.AddValue(0.1f);
        player.DebugInfo();
    }

    public override void Unequip()
    {
        player.CRIT_Rate.AddValue(-0.8f);
        player.CRIT_DMG.AddValue(-0.1f);
        player.DebugInfo();
    }
}

// 装备1017 - 斯诺登的胡须
public class Equipment_1017 : Equipment
{
    private float strDelta = 0;
    private float defDelta = 0;
    private float spdDelta = 0;

    public Equipment_1017(Equipment other) : base(other) {}

    public Equipment_1017(){
        id = 1017;
        name = "斯诺登的胡须";
        iconPath = "装备/斯诺登的胡须";
        effectDescriptionText = "装备后：力量 + 80%, 防御 + 80%, 速度 + 80%";
        descriptionText = "旧神领袖最爱惜的东西就是他漂亮的胡子，如今却被砍下，散落人间。";
        level = E_EquiptmentLevel.BlueLowest;
        isEquipped = false;
        maxDuration = 5;
        currentDuration = maxDuration;
        mySkill = LoadManager.Instance.allSkills[1018];
    }

    public override void Equip()
    {
        strDelta = player.STR.value * 0.8f;
        defDelta = player.DEF.value * 0.8f;
        spdDelta = player.SPD.value * 0.8f;
        
        player.STR.AddValue(strDelta);
        player.DEF.AddValue(defDelta);
        player.SPD.AddValue(spdDelta);
        player.DebugInfo();
    }

    public override void Unequip()
    {
        player.STR.AddValue(-strDelta);
        player.DEF.AddValue(-defDelta);
        player.SPD.AddValue(-spdDelta);
        player.DebugInfo();
    }
}

// 装备1018 - 黄衣之主的披风碎片
public class Equipment_1018 : Equipment
{
    public Equipment_1018(Equipment other) : base(other) {}

    public Equipment_1018(){
        id = 1018;
        name = "黄衣之主的披风碎片";
        iconPath = "装备/黄衣之主的披风碎片";
        effectDescriptionText = "装备后：力量 + 30, 防御 + 30, 速度 + 15";
        descriptionText = "黄衣之主的披风被强大的力量撕扯成无数块，即便如此每块碎片依旧保留了风的力量。";
        level = E_EquiptmentLevel.BlueLowest;
        isEquipped = false;
        maxDuration = 5;
        currentDuration = maxDuration;
        mySkill = LoadManager.Instance.allSkills[1019];
    }

    public override void Equip()
    {
        player.STR.AddValue(30);
        player.DEF.AddValue(30);
        player.SPD.AddValue(15);
        player.DebugInfo();
    }

    public override void Unequip()
    {
        player.STR.AddValue(-30);
        player.DEF.AddValue(-30);
        player.SPD.AddValue(-15);
        player.DebugInfo();
    }
}

// 装备1019 - 荧光石
public class Equipment_1019 : Equipment
{
    public Equipment_1019(Equipment other) : base(other) {}

    public Equipment_1019(){
        id = 1019;
        name = "荧光石";
        iconPath = "装备/荧光石";
        effectDescriptionText = "装备后：闪避 + 100";
        descriptionText = "用来保存科拉佐斯力量的容器，以此来逃避上位神的追杀。";
        level = E_EquiptmentLevel.BlueLowest;
        isEquipped = false;
        maxDuration = 5;
        currentDuration = maxDuration;
        mySkill = LoadManager.Instance.allSkills[1020];
    }

    public override void Equip()
    {
        player.AVO.AddValue(100);
        player.SPD.AddValue(15);
        player.CRIT_Rate.AddValue(player.CRIT_Rate.value * 0.3f);
        player.DebugInfo();
    }

    public override void Unequip()
    {
        player.AVO.AddValue(-100);
        player.SPD.AddValue(-15);
        player.CRIT_Rate.AddValue(-player.CRIT_Rate.value * 0.3f);
        player.DebugInfo();
    }
}

// 装备1020 - 撒托古亚的断尾
public class Equipment_1020 : Equipment
{
    public Equipment_1020(Equipment other) : base(other) {}

    public Equipment_1020(){
        id = 1020;
        name = "撒托古亚的断尾";
        iconPath = "装备/撒托古亚的断尾";
        effectDescriptionText = "装备后：力量 + 30, 暴击率 + 30%, 暴击伤害 + 30%";
        descriptionText = "腐烂的只剩下骨头的尾巴，即便如此，上面的神性力量依旧不容小觑。";
        level = E_EquiptmentLevel.BlueLowest;
        isEquipped = false;
        maxDuration = 5;
        currentDuration = maxDuration;
        mySkill = LoadManager.Instance.allSkills[1021];
    }

    public override void Equip()
    {
        player.STR.AddValue(30);
        player.CRIT_Rate.AddValue(0.3f);
        player.CRIT_DMG.AddValue(0.3f);
        player.DebugInfo();
    }

    public override void Unequip()
    {
        player.STR.AddValue(-30);
        player.CRIT_Rate.AddValue(-0.3f);
        player.CRIT_DMG.AddValue(-0.3f);
        player.DebugInfo();
    }
}

// 装备1021 - 死灵之书残页
public class Equipment_1021 : Equipment
{
    public Equipment_1021(Equipment other) : base(other) {}

    public Equipment_1021(){
        id = 1021;
        name = "死灵之书残页";
        iconPath = "装备/死灵之书残页";
        effectDescriptionText = "装备后：命中 + 200%";
        descriptionText = "死灵之书是人类接触禁忌知识相对安全的安全屏障，虽然只有一张残页，但是上面的咒语依旧十分危险。";
        level = E_EquiptmentLevel.BlueLowest;
        isEquipped = false;
        maxDuration = 5;
        currentDuration = maxDuration;
        mySkill = LoadManager.Instance.allSkills[1022];
    }

    public override void Equip()
    {
        player.HIT.AddValue(2f);
        player.CRIT_DMG.AddValue(player.CRIT_DMG.value * 1.0f);
        player.DebugInfo();
    }

    public override void Unequip()
    {
        player.HIT.AddValue(-2f);
        player.CRIT_DMG.AddValue(player.CRIT_DMG.value * 1.0f);
        player.DebugInfo();
    }
}

// 装备1022 - 嫁衣
public class Equipment_1022 : Equipment
{
    public Equipment_1022(Equipment other) : base(other) {}

    public Equipment_1022(){
        id = 1022;
        name = "嫁衣";
        iconPath = "装备/嫁衣";
        effectDescriptionText = "装备后：防御 + 100, 速度 + 100";
        descriptionText = "虽然是一次错误的相遇，但是它依旧慷慨的给予了你一件宝物，希望你能活着带回它心上人的消息。";
        level = E_EquiptmentLevel.RedHighest;
        isEquipped = false;
        maxDuration = 1;
        currentDuration = maxDuration;
        mySkill = LoadManager.Instance.allSkills[1023];
    }

    public override void Equip()
    {
        player.DEF.AddValue(100);
        player.SPD.AddValue(100);
        player.CRIT_Rate.AddValue(player.CRIT_Rate.value * 0.8f);
        player.DebugInfo();
    }

    public override void Unequip()
    {
        player.DEF.AddValue(-100);
        player.SPD.AddValue(-100);
        player.CRIT_Rate.AddValue(player.CRIT_Rate.value * 0.8f);
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