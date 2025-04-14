using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMazeStart : MonoBehaviour
{
    GameObject player;
    public Vector3 originalPoint;
     
    void Start()
    {
        player = Instantiate(Resources.Load<GameObject>("Player"));
        originalPoint = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height, Camera.main.nearClipPlane));
        originalPoint.z = 0; 
        //按照当前的GameLevelManager中的标识进行地图的加载：
        switch(GameLevelManager.Instance.gameLevelType)
        {
            case E_GameLevelType.First:
            
            break;
            case E_GameLevelType.Second:             
                Instantiate(Resources.Load<GameObject>("MapPrefabs/MapSecondFloor"), originalPoint, Quaternion.identity);  
         
            break;
            case E_GameLevelType.Third:

            break;
            case E_GameLevelType.Tutorial:
                Instantiate(Resources.Load<GameObject>("MapPrefabs/MapTutorialFloor"),originalPoint, Quaternion.identity); ;
                
            break;
        }
        player.transform.position = originalPoint + new Vector3(0.41f, -0.91f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
