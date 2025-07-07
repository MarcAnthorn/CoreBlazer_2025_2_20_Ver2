using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialReward : MonoBehaviour
{
    public int specialRewardIndex;  //外部赋值；
    // Start is called before the first frame update
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            // SoundEffectManager.Instance.PlaySoundEffect("与地图POI交互");

            //按权重分发道具：
            if(specialRewardIndex / 1000 != 1)  //不是装备，是道具；
                ItemManager.Instance.AddItem(specialRewardIndex);
            else    //装备：
                EquipmentManager.Instance.AddEquipment(specialRewardIndex);

            Destroy(this.gameObject);
        }
    }


 
}
