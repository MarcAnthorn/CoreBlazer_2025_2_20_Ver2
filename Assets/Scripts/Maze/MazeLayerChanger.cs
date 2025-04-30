using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeLayerChanger : MonoBehaviour
{
    public int layerNumber;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player")){
            EventHub.Instance.EventTrigger<int>("AdjustLayer", layerNumber);         
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player")){
            //如果离开的时候在上方，那么就是把加的2减掉；
            if(collision.gameObject.transform.position.y > this.transform.position.y)
            {
                EventHub.Instance.EventTrigger<int>("AdjustLayer", layerNumber - 2);         
            }
        }
    }
}
