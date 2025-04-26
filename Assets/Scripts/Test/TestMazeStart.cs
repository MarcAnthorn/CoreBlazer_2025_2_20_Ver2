using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//迷宫的初始脚本，处理迷宫的初始化逻辑
public class TestMazeStart : MonoBehaviour
{
    GameObject player;
    public Vector3 originalPoint;
     
    void Start()
    {
        player = Instantiate(Resources.Load<GameObject>("Player"));
        originalPoint = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height, Camera.main.nearClipPlane));
        originalPoint.z = 0; 

        PlayerController playerScript = player.GetComponent<PlayerController>();
        
        //尝试调用：Item_616的buff
        EventHub.Instance.EventTrigger("RandomBenifit");
        
        //按照当前的GameLevelManager中的标识进行地图的加载：
        switch(GameLevelManager.Instance.gameLevelType)
        {
            case E_GameLevelType.First:
            
            break;
            case E_GameLevelType.Second:             
                Instantiate(Resources.Load<GameObject>("MapPrefabs/MapSecondFloor"), originalPoint, Quaternion.identity); 
                playerScript.isDamageLocked = false;
                playerScript.LMax = 100; 
         
            break;
            case E_GameLevelType.Third:

            break;

            //新手关卡：灯光初始值60；锁定血量不会死亡：
            case E_GameLevelType.Tutorial:
                Instantiate(Resources.Load<GameObject>("MapPrefabs/MapTutorialFloor"),originalPoint, Quaternion.identity); ;
                playerScript.isDamageLocked = true;
                playerScript.LMax = 60;
                
            break;
        }
        player.transform.position = originalPoint + new Vector3(0.41f, -0.91f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
