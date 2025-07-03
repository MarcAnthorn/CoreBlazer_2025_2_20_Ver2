using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BattleBase : MonoBehaviour
{
    //当前的战斗敌人ID：
    public int enemyId;
    public bool isTriggerLock = true;
    public UnityAction avgCallback = null;


    protected virtual void Update() {

    }

    protected void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.CompareTag("Player"))
        {
            //广播战斗：
            var panel = UIManager.Instance.ShowPanel<BattlePanel>();
            panel.InitEnemyInfo(enemyId);
            panel.callback = OnComplete;
            EventHub.Instance.EventTrigger<bool>("Freeze", true);

        }
    }


    //抽象方法：在交互完成之后的回调函数：
    protected virtual void OnComplete(int enemyId)
    {
        isTriggerLock = true;


        //道具：惊喜盒子 Item_2015 的使用回调：每次战斗结束之后随机掉落装备：
        //如果没有该道具就不会被触发；
        EventHub.Instance.EventTrigger("GetRandomTwoItemsAfterBattle");

        //接下来是根据传入的敌人id进行的结果分配：
        switch(enemyId)
        {
            case 1001:
                // 1001掉落：101(1), 301(2), 1002(1)
                ItemManager.Instance.AddItem(101, 301, 301);
                EquipmentManager.Instance.AddEquipment(1002);
                break;

            case 1002:
                // 1002掉落：501(2), 301(2), 1005(1)
                ItemManager.Instance.AddItem(501, 501, 301, 301);
                EquipmentManager.Instance.AddEquipment(1005);
                break;

            case 1003:
                // 1003掉落：1006(1), 101(1), 301(1)
                ItemManager.Instance.AddItem(101, 301);
                EquipmentManager.Instance.AddEquipment(1006);
                break;

            case 1004:
                // 1004掉落：401(2), 402(2), 403(2)
                ItemManager.Instance.AddItem(401, 401, 402, 402, 403, 403);
                break;

            case 1005:
                // 1005掉落：401(2), 402(2), 403(2)
                ItemManager.Instance.AddItem(401, 401, 402, 402, 403, 403);
                break;

            case 1006:
                // 1006掉落：1009(1), 1016(1)
                EquipmentManager.Instance.AddEquipment(1009, 1016);
                break;

            case 1007:
                // 1007掉落：1008(1), 303(3)
                ItemManager.Instance.AddItem(303, 303, 303);
                EquipmentManager.Instance.AddEquipment(1008);
                break;

            case 1008:
                // 1008掉落：1007(1), 501(1), 508(1), 1004(1)
                ItemManager.Instance.AddItem(501, 508);
                EquipmentManager.Instance.AddEquipment(1007, 1004);
                break;

            case 1009:
                // 1009掉落：1003(1), 1006(1)
                EquipmentManager.Instance.AddEquipment(1003, 1006);
                break;

            case 1010:
                // 1010掉落：1020(1), 1002(1)
                EquipmentManager.Instance.AddEquipment(1020, 1002);
                break;

            case 1011:
                // 1011掉落：1010(1), 1011(1), 1012(1), 1013(1), 1014(1), 1015(1)
                EquipmentManager.Instance.AddEquipment(1010, 1011, 1012, 1013, 1014, 1015);
                break;
                
            case 1012:
                // 1012掉落：1019(1), 508(3), 504(1), 505(1), 506(1), 507(1)
                ItemManager.Instance.AddItem(508, 508, 508, 504, 505, 506, 507);
                EquipmentManager.Instance.AddEquipment(1019);
                break;

            case 1013:
                // 1013掉落：502(2), 503(2), 504(2), 505(2), 1021(1), 1016(1), 1017(1)
                ItemManager.Instance.AddItem(502, 502, 503, 503, 504, 504, 505, 505);
                EquipmentManager.Instance.AddEquipment(1021, 1016, 1017);
                break;
        

            //达贡：
            case 1016:
                //播放2305:
                UIManager.Instance.ShowPanel<AVGPanel>().InitAVG(2305, (id) => {
                    //获得道具：516:
                    ItemManager.Instance.AddItem(516);

                    //新增AVGid：
                    AVGDistributeManager.Instance.ContributeAVGId(E_NPCName.奈亚拉, 2405);
                    AVGDistributeManager.Instance.ContributeAVGId(E_NPCName.优格, 2406);
                    AVGDistributeManager.Instance.ContributeAVGId(E_NPCName.莎布, 2407);


                });
                
                break;

            default:
                // 默认无掉落
                break;
        }


    }

    protected virtual void Awake()
    {
        
    }
}
