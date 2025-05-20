using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapThirdFloor : MonoBehaviour
{
    public Transform startPoint;
    public Transform endPoint;
    void Awake()
    {
        
        GameLevelManager.Instance.mapIndexStartPointDic.TryAdd(3, startPoint.position);
        GameLevelManager.Instance.mapIndexEndPointDic.TryAdd(3, endPoint.position);
    }
}
