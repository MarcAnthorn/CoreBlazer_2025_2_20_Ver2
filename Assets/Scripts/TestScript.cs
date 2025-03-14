using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;



//这是一个测试用脚本；主要是用于测试当前UI面板是否存在问题
public class TestScript : MonoBehaviour
{
    public Button btnTest;
    private GridMap<PathGrid> pathMap;
    private GridMap<WallGrid> wallMap;
    private GridMap<WallCornerGrid> wallCornerMap;
    private Vector3 originalPoint;
    private float cellSize = 1;
    //测试：点击空格，立刻跳过当前对话
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            TextDisplayManager.Instance.DisplayTextImmediately();
        }
    }

    void Start()
    {
        // btnTest.onClick.AddListener(()=>{
        //     UIManager.Instance.ShowPanel<GameMainPanel>();
        // });

        // UIManager.Instance.ShowPanel<GameMainPanel>();
        // Vector3 originalPoint = new Vector3(Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height)).x, Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height)).y, 0);
        originalPoint = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height, Camera.main.nearClipPlane));
        originalPoint.z = 0;       
    
        //初始化通路地块的GridMap：
        //传入参数分别是：GridMap尺寸x, y、cell尺寸、基础GridMap的偏移量（此处就是视框左上角），以及内部初始化cell时候的回调；
        pathMap = new GridMap<PathGrid>(41 / 2, 41 / 2, cellSize, originalPoint, (pathMap, i, j)=>{
            //回调：实例化地块游戏对象
            GameObject pathGridObj = Resources.Load<GameObject>("TestGrids/PathGrid");
            PathGrid gridScript = pathGridObj.GetComponent<PathGrid>();
 
            //获取数据结构中的信息：
            int realX = i * 2 + 1;
            int realY = j * 2 + 1;
            MapElement me = MapManager.Instance.map1[realX, realY];
            int id = me.GetId();


            //初始化地块脚本的内部信息
            gridScript.Init(pathMap, i, j, originalPoint, cellSize, id);

            //如果是灯塔，除了基本的地块资源，还需要加在灯塔资源：
            if(id == 10005)
            {
                GameObject lightHouse = Resources.Load<GameObject>("LightHouse");
                Instantiate(lightHouse, gridScript.GetWorldPosition(), Quaternion.identity);
                Debug.Log($"x : {realX}, y : {realY}");
            }
            else if(id == 10003)
            {
                GameObject playerObj = Resources.Load<GameObject>("Player");
                Instantiate(playerObj, gridScript.GetWorldPosition(), Quaternion.identity);
                Debug.Log("Player is instantiated!");
            }
            //实例化地块，调用内部函数GetWorldPosition布置位置
            Instantiate(pathGridObj, gridScript.GetWorldPosition(), Quaternion.identity);

  

            //将脚本返回给GridMap存储在二维数组中
            return gridScript;
        });


        //注意：墙壁地块的竖直高度（第二参数）需要是宽度（第一参数）的两倍；
        wallMap = new GridMap<WallGrid>(41 / 2, 40, cellSize, originalPoint, (wallMap, i, j)=>{
            //回调：实例化地块游戏对象
            GameObject gridObj;
            WallGrid gridScript;    //注意：水平通路和竖直通路都算作是WallGrid的挂载对象了；
            int id;

            if(i % 2 == 1)  //水平墙体
            {
                //获取数据结构中的信息：
                int realX = i + 1;
                int realY = j * 2 + 1;
                MapElement me = MapManager.Instance.map1[realX, realY];
                id = me.GetId();

                if(me.GetId() == 10001)
                {
                    //加载水平墙体：
                    gridObj = Resources.Load<GameObject>("TestGrids/WallGridHorizontal"); 

                }
                else
                {
                    //加载水平通路：
                    gridObj = Resources.Load<GameObject>("TestGrids/PathGridHorizontal"); 
                }

                
            }
            else    //竖直墙体
            {
                //获取数据结构中的信息：
                int realX = i + 1;
                int realY = j * 2 + 2;
                MapElement me = MapManager.Instance.map1[realX, realY];
                id = me.GetId();


                if(me.GetId() == 10001)
                {
                    //加载水平墙体：
                    gridObj = Resources.Load<GameObject>("TestGrids/WallGridVertical");


                }
                else
                {
                    //加载水平通路：
                    gridObj = Resources.Load<GameObject>("TestGrids/PathGridVertical");
                }

                
            }
            gridScript = gridObj.GetComponent<WallGrid>();
            gridScript.Init(wallMap, i, j, originalPoint, cellSize, id);
            Instantiate(gridObj, gridScript.GetWorldPosition(), gridObj.transform.rotation);

            //将脚本返回给GridMap存储在二维数组中
            return gridScript;
        });

        wallCornerMap = new GridMap<WallCornerGrid>(41 / 2, 41 / 2, cellSize, originalPoint, (wallCornerMap, i, j)=>{
            GameObject cornerGridObj;

            //获取数据结构中的信息：
            int realX = i * 2 + 2;
            int realY = j * 2 + 2;
            MapElement me = MapManager.Instance.map1[realX, realY];
            int id = me.GetId();
            if(me.GetId() == 10001)
            {
                //加载墙体：
                cornerGridObj = Resources.Load<GameObject>("TestGrids/WallCornerGrid");


            }
            else
            {
                //加载通路：
                cornerGridObj = Resources.Load<GameObject>("TestGrids/PathCornerGrid");
            }
            WallCornerGrid gridScript = cornerGridObj.GetComponent<WallCornerGrid>();
            gridScript.Init(wallCornerMap, i, j, originalPoint, cellSize, id);

            Instantiate(cornerGridObj,  gridScript.GetWorldPosition(), cornerGridObj.transform.rotation);
            return gridScript;
        });

        //补充生成上方和左侧的墙壁：

    }


    private void GenerateExtraWall()
    {
        Vector2 basicOffset = originalPoint + new Vector3(-cellSize / 2, cellSize / 2);
        Vector2 currentOffset = basicOffset;
        GameObject wallObj;
        for(int j = 0; j < 41; j++)
        {
            if(j % 2 == 0)
            {
                wallObj = Resources.Load<GameObject>("TestGrids/WallCornerGrid");

            }
            else
            {
                wallObj = Resources.Load<GameObject>("TestGrids/WallCornerGrid");
            }

            Instantiate(wallObj, currentOffset, Quaternion.identity);
        }
    }

}
