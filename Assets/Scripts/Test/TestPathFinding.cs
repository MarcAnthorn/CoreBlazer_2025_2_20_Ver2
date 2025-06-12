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
    void Start()
    {
        GameLevelManager.Instance.gameLevelType = E_GameLevelType.First;
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

        List<Vector3> pathCopy = new List<Vector3>(path);
        Vector3 targetPos;
        for (int i = 0; i < pathCopy.Count - 1; i++)
        {
            targetPos = pathCopy[i];
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
    }


}
