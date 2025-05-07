using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapFirstFloor : MonoBehaviour
{
    public Transform startPoint;
    public Transform endPoint;
    void Awake()
    {
        
        GameLevelManager.Instance.mapIndexStartPointDic.Add(1, startPoint.position);
        GameLevelManager.Instance.mapIndexEndPointDic.Add(1, endPoint.position);
    }
    
}
