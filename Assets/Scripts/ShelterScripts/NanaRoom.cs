using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NanaRoom : MonoBehaviour
{
    private bool isTriggerLock = true;

    private void Update() {
        if(!isTriggerLock)
        {
            if(Input.GetKeyDown(KeyCode.F))
            {
                UIManager.Instance.ShowPanel<NPCInteractionPanel>().setNPCAction(E_NPCName.奈亚拉);
                EventHub.Instance.EventTrigger<bool>("Freeze", true);
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
