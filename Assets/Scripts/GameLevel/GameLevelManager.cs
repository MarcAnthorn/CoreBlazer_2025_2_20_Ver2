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
        

        avgShelterIsTriggered.Add(E_GameLevelType.First, false);
        avgShelterIsTriggered.Add(E_GameLevelType.Second, false);
        avgShelterIsTriggered.Add(E_GameLevelType.Third, false);

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

    //当前的安全屋强制avg是否更新过：
    public Dictionary<E_GameLevelType, bool> avgShelterIsTriggered = new Dictionary<E_GameLevelType, bool>();

    //存储着本关卡内的所有关卡(前面的int代表本关卡内的唯一Id，相同事件可能有重复)(补充：不同于EventManager里定义的allEvents)
    public Dictionary<int, Event> events = new Dictionary<int, Event>();
    public int eventNum;
    public static Event currentEvent;

    //石头管理器
    public List<Vector3> stonePos = new List<Vector3>();

    //结算清单：
    public List<ResultEvent> eventList = new List<ResultEvent>();
    public bool isClearLock = false;


    //当前的uniqueId的可交互门是否解锁过：
    //返回安全屋之前的留存坐标，用于锚定返回点：
    public Vector3 lastTeleportPoint = Vector3.zero;
    public Dictionary<int, bool> doorIsUnlockedDic = new Dictionary<int, bool>();
    public Dictionary<(E_GameLevelType, Vector3), bool> restPointDic = new Dictionary<(E_GameLevelType, Vector3), bool>();
    public Dictionary<(E_GameLevelType, Vector3), bool> keyPointDic = new Dictionary<(E_GameLevelType, Vector3), bool>();
    public Dictionary<(E_GameLevelType, Vector3), bool> itemPointDic = new Dictionary<(E_GameLevelType, Vector3), bool>();
    public Dictionary<(E_GameLevelType, Vector3), bool> lightHouseIsDic = new Dictionary<(E_GameLevelType, Vector3), bool>();


    //Debug用：
    public void DebugAVGInfo()
    {
        foreach (var key in avgIndexIsTriggeredDic.Keys)
        {
            Debug.Log($"id is :{key}, state is :{avgIndexIsTriggeredDic[key]}");
        }
    }

    //用于重置进度的方法：
    public void ResetAllProgress()
    {       
        avgShelterIsTriggered[E_GameLevelType.Tutorial] = false;
        avgShelterIsTriggered[E_GameLevelType.First] = false;
        avgShelterIsTriggered[E_GameLevelType.Second] = false;

        foreach(var key in avgIndexIsTriggeredDic.Keys){
            avgIndexIsTriggeredDic[key] = false;
        }

        // avgShelterIsTriggered.Clear();
        gameLevelType = E_GameLevelType.Tutorial;
        EventHub.Instance.EventTrigger<bool>("Freeze", false);
        isClearLock = false;
        stonePos.Clear();
        eventList.Clear();
    }
    

}




public enum E_GameLevelType
{
    Tutorial = 0,
    First = 1,
    Second = 2,
    Third = 3,
    Central = 4,
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