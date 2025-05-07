using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTutorialFloor : MonoBehaviour
{
    public Transform startPoint;
    public Transform endPoint;
    void Awake()
    {
        
        GameLevelManager.Instance.mapIndexStartPointDic.Add(0, startPoint.position);
        GameLevelManager.Instance.mapIndexEndPointDic.Add(0, endPoint.position);
    }
}
