using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAVG : MonoBehaviour
{
    private bool isTriggerLock = true;

    private void Update() {
        if(!isTriggerLock)
        {
            if(Input.GetKeyDown(KeyCode.J))
            {
                UIManager.Instance.ShowPanel<AVGPanel>().InitAVG(1108);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.CompareTag("Player"))
        {
            isTriggerLock = false;

        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if(other.gameObject.CompareTag("Player"))
        {
            isTriggerLock = true;
        }
    }
}
