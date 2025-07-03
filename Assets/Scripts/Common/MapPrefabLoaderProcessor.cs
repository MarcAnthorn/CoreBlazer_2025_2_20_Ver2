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

    public GameObject battleObj;
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
        // LoadMapToPrefab(3);
        LoadMapToPrefab(4);

        //测试用：输出当前的偏移点：
        Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height, Camera.main.nearClipPlane));

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

        //战斗节点：
        battleObj = Instantiate(parentPrefab);
        battleObj.name = "BattleObject";
        Debug.LogWarning("BattleObject is inited!");




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
                levelPath = "Grids/LevelZeroGrids";
            break;
            case 3:
                currentMap = LoadManager.Instance.mapThirdFloor;
                levelPath = "Grids/LevelThreeGrids";
            break;
            case 4:
                currentMap = LoadManager.Instance.mapCentralFloor;
                levelPath = "Grids/LevelTwoGrids";
            break;

        }

        sizeX = MapManager.Instance[mapIndex].row;
        sizeY = MapManager.Instance[mapIndex].colume;

        Debug.LogWarning($"sizeX:{sizeX}, sizeY:{sizeY}");
    
    
        //初始化通路地块的GridMap：
        //传入参数分别是：GridMap尺寸x, y、cell尺寸、基础GridMap的偏移量（此处就是视框左上角），以及内部初始化cell时候的回调；
        pathMap = new GridMap<PathGrid>(sizeY / 2, sizeX / 2, cellSize, originalPoint, (pathMap, i, j)=>{
            //回调：实例化地块游戏对象
            //2种差分，随机生成1～2就行：
            int randomNum = Random.Range(1, 2);

            //获取数据结构中的信息：
            int realX = i * 2 + 1;
            int realY = j * 2 + 1;

    
            //注意：第三张地图需要按坐标进行不同颜色资源的选择；因此需要进行路径的修正：
            if(mapIndex == 3)
            {
                if(realX >= 0 && realX <= 20 && realY >= 0 && realY <= 26)
                    levelPath = Path.Combine("Grids", "LevelThreeGrids", "Region1");     //Blue Region

                else if(realX >= 0 && realX <= 20 && realY > 26 && realY < 55)
                    levelPath = Path.Combine("Grids", "LevelThreeGrids", "Region2");     //Green
                
                else if(realX >= 20 && realX < 43 && realY >= 0 && realY <= 26)
                    levelPath = Path.Combine("Grids", "LevelThreeGrids", "Region4");     //yellow

                else 
                    levelPath = Path.Combine("Grids", "LevelThreeGrids", "Region3");     //red
            }

            string path = Path.Combine(levelPath, "PathGrid" + randomNum.ToString());
 
            GameObject pathGridObj = Resources.Load<GameObject>(path);
            PathGrid gridScript = pathGridObj.GetComponent<PathGrid>();
 
            Debug.Log($"path grid, x : {realX}, y : {realY}");

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
                GameObject lightHouse = Resources.Load<GameObject>(Path.Combine("MapPOIs", "LightHouse"));
                Instantiate(lightHouse, gridScript.GetWorldPosition(), Quaternion.identity).gameObject.transform.SetParent(lightHouseObject.transform, false);
                // Debug.Log($"x : {realX}, y : {realY}");
            }
            else if(id == 10004)   //起点
            {
                GameObject startPoint = Resources.Load<GameObject>(Path.Combine("MapPOIs", "MazeStartPoint"));
                Instantiate(startPoint, gridScript.GetWorldPosition(), Quaternion.identity).gameObject.transform.SetParent(startEndPointObject.transform, false);
            }
            else if(id == 10008)   //终点
            {
                GameObject endPoint = Resources.Load<GameObject>(Path.Combine("MapPOIs", "MazeEndPoint"));
                Instantiate(endPoint, gridScript.GetWorldPosition(), Quaternion.identity).gameObject.transform.SetParent(startEndPointObject.transform, false);         
            }
            else if(id == 10010 || id == 10012) //上锁的门：
            {
                GameObject doorObj = Resources.Load<GameObject>(Path.Combine("MapPOIs", "Door"));
                doorObj.name = $"Door{id}";
                Instantiate(doorObj, gridScript.GetWorldPosition(), Quaternion.identity).gameObject.transform.SetParent(npcObject.transform, false);
            }
            else if(id == 20010 || id == 20020 || id == 20030)  //（测试）POI的布置：
            {
                
                GameObject POIObj = null;
                switch(id)
                {
                    case 20010:
                        POIObj = Resources.Load<GameObject>(Path.Combine("MapPOIs", "POI20010"));
                    break;
                    case 20020:
                        POIObj = Resources.Load<GameObject>(Path.Combine("MapPOIs", "POI20020"));
                    break;
                    case 20030:
                        POIObj = Resources.Load<GameObject>(Path.Combine("MapPOIs", "POI20030"));
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

                GameObject NPCObj = Resources.Load<GameObject>(Path.Combine("MapPOIs", "NPC"));
                NPCObj.name = $"NPC{id}";
                Instantiate(NPCObj, gridScript.GetWorldPosition(), Quaternion.identity).gameObject.transform.SetParent(npcObject.transform, false);

            }

            else if(id == 10015)
            {
                GameObject NPCObj = Resources.Load<GameObject>(Path.Combine("MapPOIs", "NPC"));
                NPCObj.name = $"回收商品NPC";
                Instantiate(NPCObj, gridScript.GetWorldPosition(), Quaternion.identity).gameObject.transform.SetParent(npcObject.transform, false);
            }

            //支线：第一层 墙中鼠支线点位：
            else if(id >= 20016 && id <= 20021)
            {
                GameObject sideObj = Resources.Load<GameObject>(Path.Combine("MapPOIs", "SideQuest"));
                sideObj.name = $"墙中鼠支线：{id}";
                Instantiate(sideObj, gridScript.GetWorldPosition(), Quaternion.identity).gameObject.transform.SetParent(sideQuestObj.transform, false);
            }

            //支线：第二层 调查员难题 支线：
            else if(id >= 30016 && id <= 30018)
            {
                GameObject sideObj = Resources.Load<GameObject>(Path.Combine("MapPOIs", "SideQuest"));
                sideObj.name = $"调查员难题支线：{id}";
                Instantiate(sideObj, gridScript.GetWorldPosition(), Quaternion.identity).gameObject.transform.SetParent(sideQuestObj.transform, false);
            }

            //支线：第三层 支线：
            else if(id >= 40016 && id <= 40017)
            {
                GameObject sideObj = Resources.Load<GameObject>(Path.Combine("MapPOIs", "SideQuest"));
                sideObj.name = $"第三关支线：{id}";
                Instantiate(sideObj, gridScript.GetWorldPosition(), Quaternion.identity).gameObject.transform.SetParent(sideQuestObj.transform, false);
            }


            //宝箱地块：
            else if(id >= 20022 && id <= 20024 || id == 20026)
            {
                GameObject reward = null;
                if(id == 20022)
                    reward = Resources.Load<GameObject>(Path.Combine("MapPOIs", "RewardCommon"));

                else if(id == 20023)
                    reward = Resources.Load<GameObject>(Path.Combine("MapPOIs", "RewardRare"));

                else 
                    reward = Resources.Load<GameObject>(Path.Combine("MapPOIs", "RewardSpecial"));
                reward.name = $"宝箱{id}";
                Instantiate(reward, gridScript.GetWorldPosition(), Quaternion.identity).gameObject.transform.SetParent(rewardObj.transform, false);
            }

            //塔罗牌投放地块：
            else if(id == 50001)
            {
                GameObject reward = Resources.Load<GameObject>(Path.Combine("MapPOIs", "RewardTarot"));
                reward.name = $"塔罗牌点位";
                Instantiate(reward, gridScript.GetWorldPosition(), Quaternion.identity).gameObject.transform.SetParent(rewardObj.transform, false);

            }

            //战斗节点地块：
            else if(id >= 50002 && id <= 50006)
            {
                GameObject battle = null;
                if(id == 50002)
                {
                    battle = Resources.Load<GameObject>(Path.Combine("MapPOIs", "BattleLevelOne"));
                    battle.name = $"战斗点位50002";
                }

                else if(id == 50003)
                {
                    battle = Resources.Load<GameObject>(Path.Combine("MapPOIs", "BattleLevelTwo"));
                    battle.name = $"战斗点位50003";
                }

                else if(id == 50004)
                {
                    battle = Resources.Load<GameObject>(Path.Combine("MapPOIs", "EndBossFirstFloor"));
                    battle.name = $"战斗点位50004";
                }

                else if(id == 50005)
                {
                    battle = Resources.Load<GameObject>(Path.Combine("MapPOIs", "EndBossSecondFloor"));
                    battle.name = $"战斗点位50005";
                }

                else if(id == 50006)
                {
                    battle = Resources.Load<GameObject>(Path.Combine("MapPOIs", "EndBossThirdFloor"));
                    battle.name = $"战斗点位50006";
                }
                Instantiate(battle, gridScript.GetWorldPosition(), Quaternion.identity).gameObject.transform.SetParent(battleObj.transform, false);
            }

            //实例化地块，调用内部函数GetWorldPosition布置位置
            Instantiate(pathGridObj, gridScript.GetWorldPosition(), Quaternion.identity).gameObject.transform.SetParent(pathGridObject.transform, false);

  

            //将脚本返回给GridMap存储在二维数组中
            return gridScript;
        });


        //注意：墙壁地块的竖直高度（第二参数）需要是宽度（第一参数）的两倍；
        wallMap = new GridMap<WallGrid>(sizeY / 2, sizeX - 1, cellSize, originalPoint, (wallMap, i, j)=>{
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

                if(mapIndex == 3)
                {
                    if(realX >= 0 && realX <= 20 && realY >= 0 && realY <= 26)
                        levelPath = Path.Combine("Grids", "LevelThreeGrids", "Region1");     //Blue Region

                    else if(realX >= 0 && realX <= 20 && realY > 26 && realY < 55)
                        levelPath = Path.Combine("Grids",  "LevelThreeGrids", "Region2");     //Green
                    
                    else if(realX >= 20 && realX < 43 && realY >= 0 && realY <= 26)
                        levelPath = Path.Combine("Grids","LevelThreeGrids", "Region4");     //yellow

                    else 
                        levelPath = Path.Combine("Grids","LevelThreeGrids", "Region3");     //red
                }

                Debug.Log($"horizontal wall grid, x : {realX}, y : {realY}");
                me = currentMap[realX, realY];

                //如果是空的话，直接return：
                if(me == null){
                    return null;
                }


                id = me.GetId();

                if(me.GetId() == 10001 || me.GetId() == 10014)
                {
                    //加载水平墙体：
                    gridObj = Resources.Load<GameObject>(Path.Combine(levelPath, wallHorizontalPath)); 
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

                if(mapIndex == 3)
                {
                    if(realX >= 0 && realX <= 20 && realY >= 0 && realY <= 26)
                        levelPath = Path.Combine("Grids", "LevelThreeGrids", "Region1");     //Blue Region

                    else if(realX >= 0 && realX <= 20 && realY > 26 && realY < 55)
                        levelPath = Path.Combine("Grids", "LevelThreeGrids", "Region2");     //Green
                    
                    else if(realX >= 20 && realX < 43 && realY >= 0 && realY <= 26)
                        levelPath = Path.Combine("Grids/LevelThreeGrids", "Region4");     //yellow

                    else 
                        levelPath = Path.Combine("Grids/LevelThreeGrids", "Region3");     //red
                }

                Debug.Log($"vertical wall grid, x : {realX}, y : {realY}");
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

        wallCornerMap = new GridMap<WallCornerGrid>(sizeY / 2, sizeX / 2, cellSize, originalPoint, (wallCornerMap, i, j)=>{
            GameObject cornerGridObj;

            //2种差分，随机生成1～2就行：
            int randomNum = Random.Range(1, 3);

            //获取数据结构中的信息：
            int realX = i * 2 + 2;
            int realY = j * 2 + 2;

            if(mapIndex == 3)
            {
                if(realX >= 0 && realX <= 20 && realY >= 0 && realY <= 26)
                    levelPath = Path.Combine("Grids", "LevelThreeGrids", "Region1");     //Blue Region

                else if(realX >= 0 && realX <= 20 && realY > 26 && realY < 55)
                    levelPath = Path.Combine("Grids", "LevelThreeGrids", "Region2");     //Green
                
                else if(realX >= 20 && realX < 43 && realY >= 0 && realY <= 26)
                    levelPath = Path.Combine("Grids", "LevelThreeGrids", "Region4");     //yellow

                else 
                    levelPath = Path.Combine("Grids","LevelThreeGrids", "Region3");     //red
            }

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
        // GenerateExtraWall(mapIndex);


        //最后将所有的父对象归总到SaveObject中：
        pathGridObject.transform.SetParent(saveObject.transform, false);
        poiObject.transform.SetParent(saveObject.transform, false);
        specialWallObject.transform.SetParent(saveObject.transform, false);
        lightHouseObject.transform.SetParent(saveObject.transform, false);
        npcObject.transform.SetParent(saveObject.transform, false);
        startEndPointObject.transform.SetParent(saveObject.transform, false);
        sideQuestObj.transform.SetParent(saveObject.transform, false);
        rewardObj.transform.SetParent(saveObject.transform, false);
        battleObj.transform.SetParent(saveObject.transform, false);
  
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
                PrefabUtility.SaveAsPrefabAsset(saveObject, "Assets/Resources/MapPrefabs/MapThirdFloor.prefab");
            break;
            case 4:
                PrefabUtility.SaveAsPrefabAsset(saveObject, "Assets/Resources/MapPrefabs/MapCentralFloor.prefab");
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
                levelPath = Path.Combine("Grids", "LevelZeroGrids");
            break;
            case 1:
                levelPath = Path.Combine("Grids", "LevelTwoGrids");
            break;
            case 2:
                levelPath = Path.Combine("Grids", "LevelZeroGrids");
            break;
            case 3:
                levelPath = Path.Combine("Grids", "LevelThreeGrids");
            break;

        }
        
        for(int j = 0; j < sizeY; j++)
        {

            if(mapIndex == 3)
            {
                if(j >= 0 && j <= 27)
                    levelPath = Path.Combine("Grids", "LevelThreeGrids", "Region1");     //Blue Region

                else if(j >= 28 && j < 55)
                    levelPath = Path.Combine("Grids", "LevelThreeGrids", "Region2");     //Green

            }


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

        for(int i = 1; i < sizeX; i++)
        {   
            if(mapIndex == 3)
            {
                if(i >= 0 && i <= 20)
                    levelPath = Path.Combine("Grids/LevelThreeGrids", "Region1");     //Blue Region

                else if(i >= 21 && i < 43)
                    levelPath = Path.Combine("Grids/LevelThreeGrids", "Region4");     //yellow

            }

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
