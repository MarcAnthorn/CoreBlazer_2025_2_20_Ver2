using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;



//统一将地图资源加载到prefab的脚本；
//在地图完全出来之后就可以删除，或者是停用；编辑器阶段需要它将地图prefab化；
public class MapPrefabLoaderProcessor : MonoBehaviour
{

    private GridMap<PathGrid> pathMap;
    private GridMap<WallGrid> wallMap;
    private GridMap<WallCornerGrid> wallCornerMap;
    private Vector3 originalPoint;

    // 测试用：将该地图作为子对象保存在该对象下，该对象直接成为预设体：
    private GameObject saveObject;
    //所有普通通路 / 墙体的父对象：
    private GameObject pathGridObject;
    //所有POI的父对象：
    private GameObject poiObject;
    //所有特殊墙体的父对象：
    private GameObject specialWallObject;
    //所有灯塔的父对象：
    private GameObject lightHouseObject;
    //所有NPC事件的父对象：
    private GameObject npcObject;
    //起点和终点的父对象；
    private GameObject startEndPointObject;
    public GameObject parentPrefab;

    public GameObject sideQuestObj;

    public GameObject rewardObj;
    private float cellSize = 1;
    private int sizeX;
    private int sizeY;


    void Awake()
    {
        parentPrefab = Resources.Load<GameObject>("ParentPrefab");
    }

    void Start()
    {
//------------------------------------测试---------------------------------------------------
        GameLevelManager.Instance.npcBlockDic.Add(30001, new NPCBlock());   
        GameLevelManager.Instance.npcBlockDic.Add(30002, new NPCBlock());
        GameLevelManager.Instance.npcBlockDic.Add(30003, new NPCBlock());
        GameLevelManager.Instance.npcBlockDic.Add(30004, new NPCBlock());
        GameLevelManager.Instance.npcBlockDic.Add(30005, new NPCBlock());

        GameLevelManager.Instance.npcBlockDic[30001].avgId = 1101;
        GameLevelManager.Instance.npcBlockDic[30002].avgId = 1102;
        GameLevelManager.Instance.npcBlockDic[30003].avgId = 1103;
        GameLevelManager.Instance.npcBlockDic[30004].avgId = 1104;
        GameLevelManager.Instance.npcBlockDic[30005].avgId = 1105;
//------------------------------------测试---------------------------------------------------

        // LoadMapToPrefab(0);
        // LoadMapToPrefab(1);
        // LoadMapToPrefab(2);

        

    }

    //传入指定的mapId，加载对应的Map：
    private void LoadMapToPrefab(int mapIndex)
    {
        saveObject = Instantiate(parentPrefab);
        pathGridObject = Instantiate(parentPrefab);
        pathGridObject.name = "PathGridObject";
        poiObject = Instantiate(parentPrefab);
        poiObject.name = "POIObject";
        specialWallObject = Instantiate(parentPrefab);
        specialWallObject.name = "SpecialWallObject";
        lightHouseObject = Instantiate(parentPrefab);
        lightHouseObject.name = "LightHouseObject";
        npcObject = Instantiate(parentPrefab);
        npcObject.name = "NPCObject";
        startEndPointObject = Instantiate(parentPrefab);
        startEndPointObject.name = "StartEndPointObject";


        //支线点位的组织：
        sideQuestObj = Instantiate(parentPrefab);
        sideQuestObj.name = "SideQuestObject";

        //宝箱点位的组织：
        rewardObj = Instantiate(parentPrefab);
        rewardObj.name = "RewardObject";



        originalPoint = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height, Camera.main.nearClipPlane));
        originalPoint.z = 0;  

        //对于不同的地图，地块会资源路径会不同：
        string levelPath = null;

        //当前要加载的Map数据结构：
        MapElement[,] currentMap = null;
        switch(mapIndex){
            case 0:
                currentMap = LoadManager.Instance.mapTutorialFloor;
                levelPath = "Grids/LevelZeroGrids";
            break;
            case 1:
                currentMap = LoadManager.Instance.mapFirstFloor;
                levelPath = "Grids/LevelTwoGrids";      //第一层 暂时使用第二层的美资；
            break;
            case 2:
                currentMap = LoadManager.Instance.mapSecondFloor;
                levelPath = "Grids/LevelTwoGrids";
            break;
            case 3:
            break;

        }

        sizeX = MapManager.Instance[mapIndex].row;
        sizeY = MapManager.Instance[mapIndex].colume;
    
    
        //初始化通路地块的GridMap：
        //传入参数分别是：GridMap尺寸x, y、cell尺寸、基础GridMap的偏移量（此处就是视框左上角），以及内部初始化cell时候的回调；
        pathMap = new GridMap<PathGrid>(sizeX / 2, sizeY / 2, cellSize, originalPoint, (pathMap, i, j)=>{
            //回调：实例化地块游戏对象
            //2种差分，随机生成1～2就行：
            int randomNum = Random.Range(1, 2);

            string path = Path.Combine(levelPath, "PathGrid" + randomNum.ToString());
            GameObject pathGridObj = Resources.Load<GameObject>(path);
            PathGrid gridScript = pathGridObj.GetComponent<PathGrid>();
 
            //获取数据结构中的信息：
            int realX = i * 2 + 1;
            int realY = j * 2 + 1;
            MapElement me = currentMap[realX, realY];

            //如果是空的话，直接return：
            if(me == null){
                return null;
            }
            int id = me.GetId();


            //初始化地块脚本的内部信息
            gridScript.Init(pathMap, i, j, originalPoint, cellSize, id);

            //如果是灯塔，除了基本的地块资源，还需要灯塔资源：
            if(id == 10005)
            {
                GameObject lightHouse = Resources.Load<GameObject>("LightHouse");
                Instantiate(lightHouse, gridScript.GetWorldPosition(), Quaternion.identity).gameObject.transform.SetParent(lightHouseObject.transform, false);
                // Debug.Log($"x : {realX}, y : {realY}");
            }
            else if(id == 10004)   //起点
            {
                GameObject startPoint = Resources.Load<GameObject>("MazeStartPoint");
                Instantiate(startPoint, gridScript.GetWorldPosition(), Quaternion.identity).gameObject.transform.SetParent(startEndPointObject.transform, false);
            }
            else if(id == 10008)   //终点
            {
                GameObject endPoint = Resources.Load<GameObject>("MazeEndPoint");
                Instantiate(endPoint, gridScript.GetWorldPosition(), Quaternion.identity).gameObject.transform.SetParent(startEndPointObject.transform, false);         
            }
            else if(id == 10010 || id == 10012) //上锁的门：
            {
                GameObject doorObj = Resources.Load<GameObject>("Door");
                doorObj.name = $"Door{id}";
                Instantiate(doorObj, gridScript.GetWorldPosition(), Quaternion.identity).gameObject.transform.SetParent(npcObject.transform, false);
            }
            else if(id == 20010 || id == 20020 || id == 20030)  //（测试）POI的布置：
            {
                
                GameObject POIObj = null;
                switch(id)
                {
                    case 20010:
                        POIObj = Resources.Load<GameObject>("POI20010");
                    break;
                    case 20020:
                        POIObj = Resources.Load<GameObject>("POI20020");
                    break;
                    case 20030:
                        POIObj = Resources.Load<GameObject>("POI20030");
                    break;
                }
                POIObj.name = $"POI{id}";
                Instantiate(POIObj, gridScript.GetWorldPosition(), Quaternion.identity).gameObject.transform.SetParent(poiObject.transform, false);

            }
            else if(id == 30001 || id == 30002 || id == 30003 || id == 30004 || id == 30005)
            {
                //npc事件：
                //初始化其中的NPC位置信息（Vector3）
                GameLevelManager.Instance.npcBlockDic[id].position = gridScript.GetWorldPosition();

                GameObject NPCObj = Resources.Load<GameObject>("NPC");
                NPCObj.name = $"NPC{id}";
                Instantiate(NPCObj, gridScript.GetWorldPosition(), Quaternion.identity).gameObject.transform.SetParent(npcObject.transform, false);

            }

            //支线：墙中鼠支线点位：
            else if(id >= 20016 && id <= 20021)
            {
                GameObject sideObj = Resources.Load<GameObject>("SideQuest");
                sideObj.name = $"墙中鼠支线：{id}";
                Instantiate(sideObj, gridScript.GetWorldPosition(), Quaternion.identity).gameObject.transform.SetParent(sideQuestObj.transform, false);
            }


            //宝箱地块：
            else if(id >= 20022 && id <= 20024)
            {
                GameObject reward = null;
                if(id == 20022)
                    reward = Resources.Load<GameObject>("RewardCommon");

                else if(id == 20023)
                    reward = Resources.Load<GameObject>("RewardRare");

                else 
                    reward = Resources.Load<GameObject>("RewardSpecial");
                reward.name = $"宝箱{id}";
                Instantiate(reward, gridScript.GetWorldPosition(), Quaternion.identity).gameObject.transform.SetParent(rewardObj.transform, false);
            }

            //实例化地块，调用内部函数GetWorldPosition布置位置
            Instantiate(pathGridObj, gridScript.GetWorldPosition(), Quaternion.identity).gameObject.transform.SetParent(pathGridObject.transform, false);

  

            //将脚本返回给GridMap存储在二维数组中
            return gridScript;
        });


        //注意：墙壁地块的竖直高度（第二参数）需要是宽度（第一参数）的两倍；
        wallMap = new GridMap<WallGrid>(sizeX / 2, sizeY - 1, cellSize, originalPoint, (wallMap, i, j)=>{
            //回调：实例化地块游戏对象
            GameObject gridObj;
            WallGrid gridScript;    //注意：水平通路和竖直通路都算作是WallGrid的挂载对象了；
            int id;
            MapElement me = null;

            //三种差分，随机生成1～3就行：
            int randomNum = Random.Range(1, 4);

            if(i % 2 == 1)  //水平墙体
            {
                string wallHorizontalPath = "WallGridHorizontal" + randomNum.ToString();
                //获取数据结构中的信息：
                int realX = i + 1;
                int realY = j * 2 + 1;
                me = currentMap[realX, realY];

                //如果是空的话，直接return：
                if(me == null){
                    return null;
                }


                id = me.GetId();

                if(me.GetId() == 10001 || me.GetId() == 10014)
                {
                    //加载水平墙体：
                    gridObj = Resources.Load<GameObject>(Path.Combine(levelPath,wallHorizontalPath)); 
                    //设置水平墙体的OrderInLayer，以策划表中的Y值为唯一标准：
                    gridObj.GetComponent<SpriteRenderer>().sortingOrder = realX;

                }
                else
                {
                    //加载水平通路：
                    gridObj = Resources.Load<GameObject>(Path.Combine(levelPath,"PathGridHorizontal")); 
                }
                
            }
            else    //竖直墙体
            {
                //获取数据结构中的信息：
                int realX = i + 1;
                int realY = j * 2 + 2;
                me = currentMap[realX, realY];

                string wallVerticalPath = "WallGridVertical" + randomNum.ToString();

                //如果是空的话，直接return：
                if(me == null){
                    return null;
                }
                    
                id = me.GetId();


                if(me.GetId() == 10001 || me.GetId() == 10014)
                {
                    //加载竖直墙体：
                    gridObj = Resources.Load<GameObject>(Path.Combine(levelPath,wallVerticalPath));

                    //同样的，对于竖直墙体，也是处理Order in layer:
                    //以策划表中的X值为唯一标准：
                    gridObj.GetComponent<SpriteRenderer>().sortingOrder = realX;
                }
                else
                {
                    //加载水平通路：
                    gridObj = Resources.Load<GameObject>(Path.Combine(levelPath,"PathGridVertical"));
                }

                

                
            }
            gridScript = gridObj.GetComponent<WallGrid>();
            gridScript.Init(wallMap, i, j, originalPoint, cellSize, id);


            if(me.GetId() == 10001 || me.GetId() == 10009) //普通墙壁 / 通路：
            {
                Instantiate(gridObj, gridScript.GetWorldPosition(), gridObj.transform.rotation).gameObject.transform.SetParent(pathGridObject.transform, false);
            }


            //特殊墙壁：
            else
            {
                Instantiate(gridObj, gridScript.GetWorldPosition(), gridObj.transform.rotation).gameObject.transform.SetParent(specialWallObject.transform, false);
            }

            //将脚本返回给GridMap存储在二维数组中
            return gridScript;
        });

        wallCornerMap = new GridMap<WallCornerGrid>(sizeX / 2, sizeY / 2, cellSize, originalPoint, (wallCornerMap, i, j)=>{
            GameObject cornerGridObj;

            //2种差分，随机生成1～2就行：
            int randomNum = Random.Range(1, 3);

            //获取数据结构中的信息：
            int realX = i * 2 + 2;
            int realY = j * 2 + 2;
            MapElement me = currentMap[realX, realY];

            //如果是空的话，直接return：
            if(me == null){
                return null;
            }
            int id = me.GetId();
            if(me.GetId() == 10001)
            {
                string wallCornerPath = "WallCornerGrid" + randomNum.ToString();
                //加载墙体：
                cornerGridObj = Resources.Load<GameObject>(Path.Combine(levelPath, wallCornerPath));

                //同样的，对于拐角墙体，也是处理Order in layer:
                cornerGridObj.GetComponent<SpriteRenderer>().sortingOrder = realX;


            }
            else
            {
                //加载通路：
                cornerGridObj = Resources.Load<GameObject>(Path.Combine(levelPath,"PathCornerGrid"));
            }
            WallCornerGrid gridScript = cornerGridObj.GetComponent<WallCornerGrid>();
            gridScript.Init(wallCornerMap, i, j, originalPoint, cellSize, id);


            Instantiate(cornerGridObj,  gridScript.GetWorldPosition(), cornerGridObj.transform.rotation).gameObject.transform.SetParent(pathGridObject.transform, false);
            return gridScript;
        });

        //补充生成上方和左侧的墙壁：
        GenerateExtraWall(mapIndex);


        //最后将所有的父对象归总到SaveObject中：
        pathGridObject.transform.SetParent(saveObject.transform, false);
        poiObject.transform.SetParent(saveObject.transform, false);
        specialWallObject.transform.SetParent(saveObject.transform, false);
        lightHouseObject.transform.SetParent(saveObject.transform, false);
        npcObject.transform.SetParent(saveObject.transform, false);
        startEndPointObject.transform.SetParent(saveObject.transform, false);
        sideQuestObj.transform.SetParent(saveObject.transform, false);
        rewardObj.transform.SetParent(saveObject.transform, false);
  
#if UNITY_EDITOR
        switch(mapIndex){
            case 0:
                PrefabUtility.SaveAsPrefabAsset(saveObject, "Assets/Resources/MapPrefabs/MapTutorialFloor.prefab");
            break;
            case 1:
                PrefabUtility.SaveAsPrefabAsset(saveObject, "Assets/Resources/MapPrefabs/MapFirstFloor.prefab");
            break;
            case 2:
                PrefabUtility.SaveAsPrefabAsset(saveObject, "Assets/Resources/MapPrefabs/MapSecondFloor.prefab");
            break;
            case 3:
            break;

        }
        
#endif


    }


    private void GenerateExtraWall(int mapIndex)
    {
        Vector2 basicOffset = originalPoint + new Vector3(-cellSize / 8, cellSize / 8);
        float offset = cellSize / 8 + cellSize / 2;
        GameObject wallObj;

        string levelPath = null;
        switch(mapIndex){
            case 0:
                levelPath = "Grids/LevelZeroGrids";
            break;
            case 1:
                levelPath = "Grids/LevelTwoGrids";
            break;
            case 2:
                levelPath = "Grids/LevelTwoGrids";
            break;
            case 3:
            break;

        }
        
        for(int j = 0; j < sizeX; j++)
        {
            //三种差分，随机生成1～3就行：
            int randomNum = Random.Range(1, 4);
            string wallHorizontalPath = "WallGridHorizontal" + randomNum.ToString();

            Vector2 currentOffset = basicOffset;
            if(j % 2 == 0)
            {
                wallObj = Resources.Load<GameObject>(Path.Combine(levelPath, "WallCornerGrid1"));
            }
            else
            {
                wallObj = Resources.Load<GameObject>(Path.Combine(levelPath, wallHorizontalPath));
            }

            wallObj.GetComponent<SpriteRenderer>().sortingOrder = j;




            currentOffset += j * new Vector2(offset, 0);
            Instantiate(wallObj, currentOffset, wallObj.transform.rotation).gameObject.transform.SetParent(pathGridObject.transform, false);
        }

        for(int i = 1; i < sizeY; i++)
        {   
            //三种差分，随机生成1～3就行：
            int randomNum = Random.Range(1, 4);
            string wallVerticalPath = "WallGridVertical" + randomNum.ToString();

            Vector2 currentOffset = basicOffset;
            if(i % 2 == 0)
            {
                wallObj = Resources.Load<GameObject>(Path.Combine(levelPath, "WallCornerGrid1"));
            }
            else
            {
                wallObj = Resources.Load<GameObject>(Path.Combine(levelPath, wallVerticalPath));
            }

            wallObj.GetComponent<SpriteRenderer>().sortingOrder = i;
            currentOffset += i * new Vector2(0, -offset);
            Instantiate(wallObj, currentOffset, wallObj.transform.rotation).gameObject.transform.SetParent(pathGridObject.transform, false);
        }
    }
}
