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

        UIManager.Instance.ShowPanel<GameMainPanel>();
        // Vector3 originalPoint = new Vector3(Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height)).x, Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height)).y, 0);
        originalPoint = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height, Camera.main.nearClipPlane));
        originalPoint.z = 0;       
    
    //     //初始化通路地块的GridMap：
    //     //传入参数分别是：GridMap尺寸x, y、cell尺寸、基础GridMap的偏移量（此处就是视框左上角），以及内部初始化cell时候的回调；
    //     pathMap = new GridMap<PathGrid>(8, 8, cellSize, originalPoint, (pathMap, i, j)=>{
    //         //回调：实例化地块游戏对象
    //         GameObject pathGridObj = Resources.Load<GameObject>("TestGrids/PathGrid");
    //         PathGrid gridScript = pathGridObj.GetComponent<PathGrid>();

    //         //初始化地块脚本的内部信息
    //         gridScript.Init(pathMap, i, j, originalPoint, cellSize);
    //         //实例化地块，调用内部函数GetWorldPosition布置位置
    //         Instantiate(pathGridObj, gridScript.GetWorldPosition(), Quaternion.identity);

    //         //将脚本返回给GridMap存储在二维数组中
    //         return gridScript;
    //     });


    //     //注意：墙壁地块的竖直高度（第二参数）需要是宽度（第一参数）的两倍；
    //     wallMap = new GridMap<WallGrid>(8, 16, cellSize, originalPoint, (wallMap, i, j)=>{
    //         //回调：实例化地块游戏对象
    //         GameObject wallGridObj;
    //         WallGrid gridScript;
    //         if(i % 2 == 1)  //水平墙体
    //         {
    //             wallGridObj = Resources.Load<GameObject>("TestGrids/WallGridHorizontal"); 
    //         }
    //         else    //竖直墙体
    //         {
    //             wallGridObj = Resources.Load<GameObject>("TestGrids/WallGridVertical");
    //         }
    //         gridScript = wallGridObj.GetComponent<WallGrid>();
    //         gridScript.Init(wallMap, i, j, originalPoint, cellSize);
    //         Instantiate(wallGridObj, gridScript.GetWorldPosition(), wallGridObj.transform.rotation);

    //         //将脚本返回给GridMap存储在二维数组中
    //         return gridScript;
    //     });

    //     wallCornerMap = new GridMap<WallCornerGrid>(8, 8, cellSize, originalPoint, (wallCornerMap, i, j)=>{
    //         GameObject cornerGridObj = Resources.Load<GameObject>("TestGrids/WallCornerGrid");
    //         WallCornerGrid gridScript = cornerGridObj.GetComponent<WallCornerGrid>();

    //         gridScript.Init(wallCornerMap, i, j, originalPoint, cellSize);

    //         Instantiate(cornerGridObj,  gridScript.GetWorldPosition(), cornerGridObj.transform.rotation);
    //         return gridScript;
    //     });

    }


}
