using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//装备管理器：
//基本职责：告知外部当前持有的装备id；当前装备的装备id；
//通过id访问LoadManager中的对应dic，获取该道具的实例；
public class EquipmentManager : SingletonBaseManager<EquipmentManager>
{
    private EquipmentManager(){}
    //当前持有的所有装备：存储的是实例，不是id；
    //这是因为相同的装备不是叠加放置，而是独立放置；
    //这样的话，要想通过一个装备访问到它的耐久，那么使用id是不行的；
    //因为dictionary不接受存储重复的key；因此使用实例进行访问；
    //这样才能确保就算是同属一个类，实例不同访问结果也不同；
    public List<Equipment> equipmentList = new List<Equipment>();

    //当前持有的装备，以及其耐久；用于外部查询耐久度；
    //key -> 装备实例；value -> 耐久
    public Dictionary<Equipment, int> equipmentDurationDic = new Dictionary<Equipment, int>();


    //向当前的持有中加入新的装备：
    //使用id进行添加；方法内部会new一个新的实例，以实现新装备的添加和存储
    public void AddEquipment(int id)
    {
        Equipment depulicateSource = LoadManager.Instance.allEquipment[id];

        //对这个新的实例进行成员的填充： 
        //通过已有的同类装备，通过深拷贝进行内容的填充：
        Equipment depulicatedResult = ClassifyEquipment(depulicateSource);
        
        //相同的装备不是叠放的，而是独立放置的；因此有一个就加一个；
        equipmentList.Add(depulicatedResult);

        equipmentDurationDic.Add(depulicatedResult, depulicatedResult.maxDuration);    
    }

    public void RemoveEquipment(Equipment targetEquipment)
    {
        if(equipmentList.Contains(targetEquipment))
        {
            equipmentList.Remove(targetEquipment);
            equipmentDurationDic.Remove(targetEquipment);
        }
        else
            Debug.LogWarning($"当前要删除的装备不存在，id为：{targetEquipment.id}");
    }

    //传入id，分类装备的方法：用于LoadManager中Chery的表读取；
    public Equipment ClassifyEquipment(int id)
    {
        //测试：
        switch(id)
        {
            case 1001:
            return new Equipment_1001();

            default:
            return null;
        }
    }

    //重载：传入实例，返回新实例的ClassifyEquipment:
    //用于AddEquipment的时候：
    public Equipment ClassifyEquipment(Equipment equipment)
    {
        //测试：
        switch(equipment.id)
        {
            case 1001:
            return new Equipment_1001(equipment);

            default:
            return null;
        }
    }

    //给外界提供当前剩余的装备插槽数量的API：
    public int NowLeftSlotsCount()
    {
        int count = 0;

        foreach(var equipment in equipmentList)
        {
            if(equipment.isEquipped)
                count++;
        }
        return count;
    }

    //传入id，分类Skill的方法：用于LoadManager中Chery的表读取；
    public Skill ClassifySkill(int id)
    {
        //测试：
        switch(id)
        {
            case 1001:
            return new Skill_1002();

            default:
            return null;
        }
    }

}
