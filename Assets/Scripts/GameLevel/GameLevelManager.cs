using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLevelManager : Singleton<GameLevelManager>
{
    //存储着本关卡内的所有关卡(前面的int代表本关卡内的唯一Id，相同事件可能有重复)(补充：不同于EventManager里定义的allEvents)
    public Dictionary<int, Event> events = new Dictionary<int, Event>();
    public int eventNum;
    public static Event currentEvent;

//--------Marc标识：此处玩家所处的环境index字段存储在PlayerManager中了-------------
    public static int currentEnvironment;        // 1-事件选择 2-战斗场景 3-迷宫内

//-----------------------------------------------------------------------------

    //当前的关卡是哪个等级：
    public E_GameLevelType gameLevelType = E_GameLevelType.Tutorial;

}

public enum E_GameLevelType
{
    Tutorial = 0,
    First = 1,
    Second = 2,
    Third = 3,
}
