using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSecondFloor : MonoBehaviour
{
    public Transform startPoint;
    public Transform endPoint;
    public Vector3 originalPoint;
    void Awake()
    {
        originalPoint = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height, Camera.main.nearClipPlane));
        originalPoint.z = 0;  
        GameLevelManager.Instance.mapIndexStartPointDic.Add(2, originalPoint + new Vector3(0.41f, -0.91f));
        GameLevelManager.Instance.mapIndexEndPointDic.Add(2, endPoint.position);
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
