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
        int itemId = 0;
        int itemCount = 1;
        int randomNumber = 0;       

        switch(enemyId)
        {
            case 1001:
                randomNumber = Random.Range(1, 4);
                if(randomNumber == 1)
                {
                    itemId = 101;
                    itemCount = 1;
                }
                else if(randomNumber == 2)
                {
                    itemId = 301;
                    itemCount = 2;
                }
                else if(randomNumber == 3)
                {
                    itemId = 1002;
                    itemCount = 1;
                }
                break;

            case 1002:
                randomNumber = Random.Range(1, 4);
                if(randomNumber == 1)
                {
                    itemId = 501;
                    itemCount = 2;
                }
                else if(randomNumber == 2)
                {
                    itemId = 301;
                    itemCount = 2;
                }
                else if(randomNumber == 3)
                {
                    itemId = 1005;
                    itemCount = 1;
                }
                break;

            case 1003:
                randomNumber = Random.Range(1, 4);
                if(randomNumber == 1)
                {
                    itemId = 1006;
                    itemCount = 1;
                }
                else if(randomNumber == 2)
                {
                    itemId = 101;
                    itemCount = 1;
                }
                else if(randomNumber == 3)
                {
                    itemId = 301;
                    itemCount = 1;
                }
                break;

            case 1004:
                randomNumber = Random.Range(1, 4);
                if(randomNumber == 1)
                {
                    itemId = 401;
                    itemCount = 2;
                }
                else if(randomNumber == 2)
                {
                    itemId = 402;
                    itemCount = 2;
                }
                else if(randomNumber == 3)
                {
                    itemId = 403;
                    itemCount = 2;
                }
                break;

            case 1005:
                randomNumber = Random.Range(1, 4);
                if(randomNumber == 1)
                {
                    itemId = 401;
                    itemCount = 2;
                }
                else if(randomNumber == 2)
                {
                    itemId = 402;
                    itemCount = 2;
                }
                else if(randomNumber == 3)
                {
                    itemId = 403;
                    itemCount = 2;
                }
                break;

            case 1006:
                randomNumber = Random.Range(1, 3);
                if(randomNumber == 1)
                {
                    itemId = 1009;
                    itemCount = 1;
                }
                else if(randomNumber == 2)
                {
                    itemId = 1016;
                    itemCount = 1;
                }
                break;

            case 1007:
                randomNumber = Random.Range(1, 3);
                if(randomNumber == 1)
                {
                    itemId = 1008;
                    itemCount = 1;
                }
                else if(randomNumber == 2)
                {
                    itemId = 303;
                    itemCount = 3;
                }
                break;

            case 1008:
                randomNumber = Random.Range(1, 5);
                if(randomNumber == 1)
                {
                    itemId = 1007;
                    itemCount = 1;
                }
                else if(randomNumber == 2)
                {
                    itemId = 501;
                    itemCount = 1;
                }
                else if(randomNumber == 3)
                {
                    itemId = 508;
                    itemCount = 1;
                }
                else if(randomNumber == 4)
                {
                    itemId = 1004;
                    itemCount = 1;
                }
                break;

            case 1009:
                randomNumber = Random.Range(1, 3);
                if(randomNumber == 1)
                {
                    itemId = 1003;
                    itemCount = 1;
                }
                else if(randomNumber == 2)
                {
                    itemId = 1006;
                    itemCount = 1;
                }
                break;

            case 1010:
                randomNumber = Random.Range(1, 3);
                if(randomNumber == 1)
                {
                    itemId = 1020;
                    itemCount = 1;
                }
                else if(randomNumber == 2)
                {
                    itemId = 1002;
                    itemCount = 1;
                }
                break;

            case 1011:
                randomNumber = Random.Range(1, 7);
                if(randomNumber == 1)
                {
                    itemId = 1010;
                    itemCount = 1;
                }
                else if(randomNumber == 2)
                {
                    itemId = 1011;
                    itemCount = 1;
                }
                else if(randomNumber == 3)
                {
                    itemId = 1012;
                    itemCount = 1;
                }
                else if(randomNumber == 4)
                {
                    itemId = 1013;
                    itemCount = 1;
                }
                else if(randomNumber == 5)
                {
                    itemId = 1014;
                    itemCount = 1;
                }
                else if(randomNumber == 6)
                {
                    itemId = 1015;
                    itemCount = 1;
                }
                break;

            case 1013:
                randomNumber = Random.Range(1, 8);
                if(randomNumber == 1)
                {
                    itemId = 502;
                    itemCount = 2;
                }
                else if(randomNumber == 2)
                {
                    itemId = 503;
                    itemCount = 2;
                }
                else if(randomNumber == 3)
                {
                    itemId = 504;
                    itemCount = 2;
                }
                else if(randomNumber == 4)
                {
                    itemId = 505;
                    itemCount = 2;
                }
                else if(randomNumber == 5)
                {
                    itemId = 1021;
                    itemCount = 1;
                }
                else if(randomNumber == 6)
                {
                    itemId = 1016;
                    itemCount = 1;
                }
                else if(randomNumber == 7)
                {
                    itemId = 1017;
                    itemCount = 1;
                }
                break;

            default:
                // 默认情况，可以设置默认掉落或空
                itemId = 0;
                itemCount = 0;
                break;
        }

        //进行道具 / 装备的分配：
        if(itemId / 1000 == 1)  //装备
        {
            EquipmentManager.Instance.AddEquipment(itemId, itemCount);
        }

        else{   //道具
            ItemManager.Instance.AddItem(itemId, itemCount);
        }

    }

    protected virtual void Awake()
    {
        
    }
}
