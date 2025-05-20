using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTutorialFloor : MonoBehaviour
{
    public Transform startPoint;
    public Transform endPoint;
    void Awake()
    {
        
        GameLevelManager.Instance.mapIndexStartPointDic.TryAdd(0, startPoint.position);
        GameLevelManager.Instance.mapIndexEndPointDic.TryAdd(0, endPoint.position);
    }
}
