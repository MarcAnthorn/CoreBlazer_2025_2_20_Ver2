using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSecondFloor : MonoBehaviour
{
    public Transform startPoint;
    public Transform endPoint;
    void Awake()
    {
        
        GameLevelManager.Instance.mapIndexStartPointDic.Add(2, startPoint.position);
        GameLevelManager.Instance.mapIndexEndPointDic.Add(2, endPoint.position);
    }
}
