using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;



// //这是一个测试用脚本；主要是用于测试当前UI面板是否存在问题
// public class TestScript : MonoBehaviour
// {
//     public Button btnTest;
//     private GridMap<PathGrid> pathMap;
//     private GridMap<WallGrid> wallMap;
//     private GridMap<WallCornerGrid> wallCornerMap;
//     private Vector3 originalPoint;

//     // 测试用：将该地图作为子对象保存在该对象下，该对象直接成为预设体：
//     private GameObject saveObject;
//     public GameObject parentPrefab;
//     private float cellSize = 1;
//     //测试：点击空格，立刻跳过当前对话
//     void Update()
//     {
//         if(Input.GetKeyDown(KeyCode.Space))
//         {
//             TextDisplayManager.Instance.DisplayTextImmediately();
//         }
//     }

//     void Awake()
//     {

//     }

//     void Start()
//     {
//         // btnTest.onClick.AddListener(()=>{
//         //     UIManager.Instance.ShowPanel<GameMainPanel>();
//         // });

//         saveObject = Instantiate(parentPrefab);

//         // UIManager.Instance.ShowPanel<GameMainPanel>();
//         // Vector3 originalPoint = new Vector3(Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height)).x, Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height)).y, 0);
//         originalPoint = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height, Camera.main.nearClipPlane));
//         originalPoint.z = 0;       
    
//         //初始化通路地块的GridMap：
//         //传入参数分别是：GridMap尺寸x, y、cell尺寸、基础GridMap的偏移量（此处就是视框左上角），以及内部初始化cell时候的回调；
//         pathMap = new GridMap<PathGrid>(41 / 2, 41 / 2, cellSize, originalPoint, (pathMap, i, j)=>{
//             //回调：实例化地块游戏对象
//             GameObject pathGridObj = Resources.Load<GameObject>("TestGrids/PathGrid");
//             PathGrid gridScript = pathGridObj.GetComponent<PathGrid>();
 
//             //获取数据结构中的信息：
//             int realX = i * 2 + 1;
//             int realY = j * 2 + 1;
//             MapElement me = LoadManager.Instance.map1[realX, realY];
//             int id = me.GetId();


//             //初始化地块脚本的内部信息
//             gridScript.Init(pathMap, i, j, originalPoint, cellSize, id);

//             //如果是灯塔，除了基本的地块资源，还需要加在灯塔资源：
//             if(id == 10005)
//             {
//                 GameObject lightHouse = Resources.Load<GameObject>("LightHouse");
//                 Instantiate(lightHouse, gridScript.GetWorldPosition(), Quaternion.identity).gameObject.transform.SetParent(saveObject.transform, false);
//                 // Debug.Log($"x : {realX}, y : {realY}");
//             }
//             else if(id == 10003)
//             {
//                 GameObject playerObj = Resources.Load<GameObject>("Player");
//                 Instantiate(playerObj, gridScript.GetWorldPosition(), Quaternion.identity).gameObject.transform.SetParent(saveObject.transform, false);

//                 //测试：
//                 PlayerManager.Instance.initPosition = gridScript.GetWorldPosition();
//                 // Debug.Log("Player is instantiated!");
//             }
//             else if(id == 20010 || id == 20020 || id == 20030)
//             {
//                 //（测试）POI的布置：
//                 GameObject POIObj = Resources.Load<GameObject>("POI");
//                 Instantiate(POIObj, gridScript.GetWorldPosition(), Quaternion.identity).gameObject.transform.SetParent(saveObject.transform, false);

//             }
//             //实例化地块，调用内部函数GetWorldPosition布置位置
//             Instantiate(pathGridObj, gridScript.GetWorldPosition(), Quaternion.identity).gameObject.transform.SetParent(saveObject.transform, false);

  

//             //将脚本返回给GridMap存储在二维数组中
//             return gridScript;
//         });


//         //注意：墙壁地块的竖直高度（第二参数）需要是宽度（第一参数）的两倍；
//         wallMap = new GridMap<WallGrid>(41 / 2, 40, cellSize, originalPoint, (wallMap, i, j)=>{
//             //回调：实例化地块游戏对象
//             GameObject gridObj;
//             WallGrid gridScript;    //注意：水平通路和竖直通路都算作是WallGrid的挂载对象了；
//             int id;

//             if(i % 2 == 1)  //水平墙体
//             {
//                 //获取数据结构中的信息：
//                 int realX = i + 1;
//                 int realY = j * 2 + 1;
//                 MapElement me = LoadManager.Instance.map1[realX, realY];
//                 id = me.GetId();

//                 if(me.GetId() == 10001)
//                 {
//                     //加载水平墙体：
//                     gridObj = Resources.Load<GameObject>("TestGrids/WallGridHorizontal"); 

//                 }
//                 else
//                 {
//                     //加载水平通路：
//                     gridObj = Resources.Load<GameObject>("TestGrids/PathGridHorizontal"); 
//                 }

                
//             }
//             else    //竖直墙体
//             {
//                 //获取数据结构中的信息：
//                 int realX = i + 1;
//                 int realY = j * 2 + 2;
//                 MapElement me = LoadManager.Instance.map1[realX, realY];
//                 id = me.GetId();


//                 if(me.GetId() == 10001)
//                 {
//                     //加载水平墙体：
//                     gridObj = Resources.Load<GameObject>("TestGrids/WallGridVertical");


//                 }
//                 else
//                 {
//                     //加载水平通路：
//                     gridObj = Resources.Load<GameObject>("TestGrids/PathGridVertical");
//                 }

                
//             }
//             gridScript = gridObj.GetComponent<WallGrid>();
//             gridScript.Init(wallMap, i, j, originalPoint, cellSize, id);
//             Instantiate(gridObj, gridScript.GetWorldPosition(), gridObj.transform.rotation).gameObject.transform.SetParent(saveObject.transform, false);

//             //将脚本返回给GridMap存储在二维数组中
//             return gridScript;
//         });

//         wallCornerMap = new GridMap<WallCornerGrid>(41 / 2, 41 / 2, cellSize, originalPoint, (wallCornerMap, i, j)=>{
//             GameObject cornerGridObj;

//             //获取数据结构中的信息：
//             int realX = i * 2 + 2;
//             int realY = j * 2 + 2;
//             MapElement me = LoadManager.Instance.map1[realX, realY];
//             int id = me.GetId();
//             if(me.GetId() == 10001)
//             {
//                 //加载墙体：
//                 cornerGridObj = Resources.Load<GameObject>("TestGrids/WallCornerGrid");


//             }
//             else
//             {
//                 //加载通路：
//                 cornerGridObj = Resources.Load<GameObject>("TestGrids/PathCornerGrid");
//             }
//             WallCornerGrid gridScript = cornerGridObj.GetComponent<WallCornerGrid>();
//             gridScript.Init(wallCornerMap, i, j, originalPoint, cellSize, id);

//             Instantiate(cornerGridObj,  gridScript.GetWorldPosition(), cornerGridObj.transform.rotation).gameObject.transform.SetParent(saveObject.transform, false);
//             return gridScript;
//         });

//         //补充生成上方和左侧的墙壁：
//         GenerateExtraWall();
        
// #if UNITY_EDITOR
//         PrefabUtility.SaveAsPrefabAsset(saveObject, "Assets/Resources/MapPrefabs/map1.prefab");
// #endif



//     }


//     private void GenerateExtraWall()
//     {
//         Vector2 basicOffset = originalPoint + new Vector3(-cellSize / 8, cellSize / 8);
//         float offset = cellSize / 8 + cellSize / 2;
//         GameObject wallObj;
//         for(int j = 0; j < 41; j++)
//         {
//             Vector2 currentOffset = basicOffset;
//             if(j % 2 == 0)
//             {
//                 wallObj = Resources.Load<GameObject>("TestGrids/WallCornerGrid");

//             }
//             else
//             {
//                 wallObj = Resources.Load<GameObject>("TestGrids/WallGridHorizontal");
//             }
//             currentOffset += j * new Vector2(offset, 0);
//             Instantiate(wallObj, currentOffset, wallObj.transform.rotation).gameObject.transform.SetParent(saveObject.transform, false);
//         }

//         for(int i = 1; i < 41; i++)
//         {   
//             Vector2 currentOffset = basicOffset;
//             if(i % 2 == 0)
//             {
//                 wallObj = Resources.Load<GameObject>("TestGrids/WallCornerGrid");

//             }
//             else
//             {
//                 wallObj = Resources.Load<GameObject>("TestGrids/WallGridVertical");
//             }
//             currentOffset += i * new Vector2(0, -offset);
//             Instantiate(wallObj, currentOffset, wallObj.transform.rotation).gameObject.transform.SetParent(saveObject.transform, false);
//         }
//     }

// }
