using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLevelManager : Singleton<GameLevelManager>
{
    //当前的关卡是哪个等级：默认是新手关卡：
    public E_GameLevelType gameLevelType;

    protected override void Awake()
    {
        base.Awake();

        //默认是新手关卡：
        gameLevelType = E_GameLevelType.Tutorial;
    }



    //存储所有NPC事件index对应的事件数据结构的Dictionary:
    public Dictionary<int, NPCBlock> npcBlockDic = new Dictionary<int, NPCBlock>();

    //存储着不同关卡的起始点和终点的dic
    //key:关卡id；0 -> 教学关； 1 -> 第一层；以此类推
    //value:起始点 / 终点 Grid的 Vector3 (WorldPosition)
    public Dictionary<int, Vector3> mapIndexStartPointDic = new Dictionary<int, Vector3>();
    //终点：
    public Dictionary<int, Vector3> mapIndexEndPointDic = new Dictionary<int, Vector3>();

    //该NPC事件是否触发过；如果触发过，那么死亡不会让其重新生成；
    //因为一个NPC事件本身对应一个avg，因此直接使用avgId作为key
    //（只是限于NPC事件不会因为死亡重生的）
    //同时，如果该事件已经触发了，那么对应的结果也不会重置；

    public Dictionary<int, bool> avgIndexIsTriggeredDic = new Dictionary<int, bool>();

    //存储着本关卡内的所有关卡(前面的int代表本关卡内的唯一Id，相同事件可能有重复)(补充：不同于EventManager里定义的allEvents)
    public Dictionary<int, Event> events = new Dictionary<int, Event>();
    public int eventNum;
    public static Event currentEvent;


    //Debug用：
    public void DebugAVGInfo()
    {
        foreach(var key in avgIndexIsTriggeredDic.Keys)
        {
            Debug.Log($"id is :{key}, state is :{avgIndexIsTriggeredDic[key]}");
        }
    }
    

}

public enum E_GameLevelType
{
    Tutorial = 0,
    First = 1,
    Second = 2,
    Third = 3,
}


//组织NPC内容的数据结构：
public class NPCBlock
{
    //NPC事件的id
    public int npcId;
    //NPC事件对应的Grid位置
    //位置信息在MapPrefabLoaderProcessor中初始化；
    public Vector3 position;
    //其对应的AVG演出ID：
    //通过演出id访问LoadManaeger中的DialogueOrderBlock，并且将其托付给AVGPanel就可以实现演出；
    public int avgId;

}