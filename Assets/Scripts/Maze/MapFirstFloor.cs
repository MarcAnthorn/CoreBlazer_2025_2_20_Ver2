using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapFirstFloor : MonoBehaviour
{
    public Transform startPoint;
    public Transform endPoint;
    void Awake()
    {
        
        GameLevelManager.Instance.mapIndexStartPointDic.TryAdd(1, startPoint.position);
        GameLevelManager.Instance.mapIndexEndPointDic.TryAdd(1, endPoint.position);
    }
    
}
