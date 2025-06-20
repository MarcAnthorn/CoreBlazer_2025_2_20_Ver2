using System.Collections;
using System.Collections.Generic;
using System.Text;
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

    //唯一装备标识：uniqueId:
    public int uniqueId = 0;


    //向当前的持有中加入新的装备：
    //使用id进行添加；方法内部会new一个新的实例，以实现新装备的添加和存储
    //第二参数：指定装备的耐久；如果不指定，就是默认满耐久；
    public void AddEquipment(int id, int duration = -1)
    {
        Equipment depulicateSource = LoadManager.Instance.allEquipment[id];

        //对这个新的实例进行成员的填充： 
        //通过已有的同类装备，通过深拷贝进行内容的填充：
        Equipment depulicatedResult = ClassifyEquipment(depulicateSource);
        
        //相同的装备不是叠放的，而是独立放置的；因此有一个就加一个；
        equipmentList.Add(depulicatedResult);

        if(duration == -1)  
        {
            equipmentDurationDic.Add(depulicatedResult, depulicatedResult.maxDuration);  
        }
        else    //如果指定耐久，那么更新耐久；
        {
            equipmentDurationDic.Add(depulicatedResult, duration);
            depulicatedResult.currentDuration = duration;
        }
              

        PoolManager.Instance.SpawnFromPool("Panels/WarningPanel", GameObject.Find("Canvas").transform).gameObject.GetComponent<WarningPanel>().SetWarningText($"获得装备「{depulicateSource.name}」");
    }

    public void AddEquipment(params int[] ids)
    {
        StringBuilder sb = new StringBuilder();
        foreach(int id in ids){
            Equipment depulicateSource = LoadManager.Instance.allEquipment[id];

            //对这个新的实例进行成员的填充： 
            //通过已有的同类装备，通过深拷贝进行内容的填充：
            Equipment depulicatedResult = ClassifyEquipment(depulicateSource);
            
            //相同的装备不是叠放的，而是独立放置的；因此有一个就加一个；
            equipmentList.Add(depulicatedResult);

            equipmentDurationDic.Add(depulicatedResult, depulicatedResult.maxDuration);

            sb.Append($"获得装备「{depulicateSource.name}」\n");
        }
         

        PoolManager.Instance.SpawnFromPool("Panels/WarningPanel", GameObject.Find("Canvas").transform).gameObject.GetComponent<WarningPanel>().SetWarningText(sb.ToString());
  
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

            case 1002:
            return new Equipment_1002();

            case 1003:
            return new Equipment_1003();

            default:
                Debug.LogError($"未找到对应id的道具， id为：{id}");
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
            case 1001: return new Equipment_1001(equipment);
            case 1002: return new Equipment_1002(equipment);
            case 1003: return new Equipment_1003(equipment);
            case 1004: return new Equipment_1004(equipment);
            case 1005: return new Equipment_1005(equipment);
            case 1006: return new Equipment_1006(equipment);
            case 1007: return new Equipment_1007(equipment);
            case 1008: return new Equipment_1008(equipment);
            case 1009: return new Equipment_1009(equipment);


            case 1010: return new Equipment_1010(equipment);
            case 1011: return new Equipment_1011(equipment);
            case 1012: return new Equipment_1012(equipment);
            case 1013: return new Equipment_1013(equipment);
            case 1014: return new Equipment_1014(equipment);
            case 1015: return new Equipment_1015(equipment);
            case 1016: return new Equipment_1016(equipment);
            case 1017: return new Equipment_1017(equipment);
            case 1018: return new Equipment_1018(equipment);


            case 1019: return new Equipment_1019(equipment);


            case 1020: return new Equipment_1020(equipment);
            case 1021: return new Equipment_1021(equipment);

            case 1022: return new Equipment_1022(equipment);


            default:
                Debug.LogError($"未找到对应id的装备，id为：{equipment.id}");
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
            return new Skill_1001();

            default:
            return null;
        }
    }

}
