using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLevelManager : Singleton<GameLevelManager>
{
    //存储着本关卡内的所有关卡(前面的int代表本关卡内的唯一Id，相同事件可能有重复)
    public Dictionary<int, Event> events = new Dictionary<int, Event>();
    public int eventNum;
    public static Event currentEvent;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
