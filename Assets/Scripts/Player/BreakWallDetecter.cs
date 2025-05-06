using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakWallDetecter : MonoBehaviour
{
    void Awake()
    {
        gameObject.tag = "WallBreaker";
    }
}
