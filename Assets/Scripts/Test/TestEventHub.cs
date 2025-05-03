using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEventHub : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        EventHub.Instance.AddEventListener("T", T);
    }

   private void T()
   {
        Debug.LogError("????????");
   }
}
