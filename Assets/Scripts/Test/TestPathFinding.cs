using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPathFinding : MonoBehaviour
{
    Vector3 startPoint;
    Vector3 endPoint;
    private List<Vector3> path;
    private bool isMoving = false;
    [Range(0, 2)]
    public float moveSpeed;
    //移动协同程序：
    private Coroutine moveCoroutine;

    // 引入ListPool工具类
    public static class ListPool<T>
    {
        private static readonly Stack<List<T>> pool = new Stack<List<T>>();

        public static List<T> Get()
        {
            return pool.Count > 0 ? pool.Pop() : new List<T>();
        }

        public static void Release(List<T> toRelease)
        {
            toRelease.Clear();
            pool.Push(toRelease);
        }
    }

    void Start()
    {
        GameLevelManager.Instance.gameLevelType = E_GameLevelType.Tutorial;
    }

    private void Update()
    {
        startPoint = this.transform.position;
        // //左键设置起点：
        // if (Input.GetMouseButtonDown(0))
        // {
        //     startPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //     int x, y;
        //     PathFindingManager.Instance.GetGridIndex(startPoint, out x, out y);
        //     Debug.Log($"now start position is{startPoint}, index is:{x},{y}");
        // }

        //右键设置终点：
        if (Input.GetMouseButtonDown(1))
        {
            endPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // 用对象池获取path
            if (path != null)
            {
                ListPool<Vector3>.Release(path);
            }
            path = PathFindingManager.Instance.FindPath(startPoint, endPoint);
            if (path != null && path.Count > 0)
            {
                if (moveCoroutine != null)
                {
                    StopCoroutine(moveCoroutine);
                }
                moveCoroutine = StartCoroutine(MoveAlongPath(path));
            }

            int x, y;
            PathFindingManager.Instance.GetGridIndex(endPoint, out x, out y);
            Debug.Log($"now end position is{endPoint}, index is:{x},{y}");
        }
    }
    
    private IEnumerator MoveAlongPath(List<Vector3> path)
    {
        isMoving = true;
        // 直接用path参数，不再new List
        Vector3 targetPos;
        for (int i = 0; i < path.Count - 1; i++)
        {
            targetPos = path[i];
            while (Vector3.Distance(transform.position, targetPos) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
                yield return null; // 等待下一帧
            }
            // 保证最终位置对齐
            transform.position = targetPos;
            yield return null;
        }
        isMoving = false;
        // 协程结束后回收path到对象池
        if (path != null)
        {
            ListPool<Vector3>.Release(path);
        }
    }


}
