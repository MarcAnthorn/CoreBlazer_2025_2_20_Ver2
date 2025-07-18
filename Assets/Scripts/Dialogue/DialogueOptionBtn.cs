using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DialogueOptionBtn : MonoBehaviour
{
    private Button btnOption;
    //当前选项持有的order对象：外部会进行赋值；
    public DialogueOrder myOrder;
    public TextMeshProUGUI txtOptionText;

    public int triggerCount;
    private bool isBranchChoice;

    public GameObject mask;

    void Awake()
    {
        btnOption = this.GetComponent<Button>();
    }

    void Start()
    {
        // //进行特殊判断：2111的选项：1003 & 1012，如果节点9没有触发，那么这两个选项不可选择：
        // if(GameLevelManager.Instance.avgIndexIsTriggeredDic.ContainsKey(1009) && !GameLevelManager.Instance.avgIndexIsTriggeredDic[1009] && myOrder.rootId == 2111 && (myOrder.orderId == 1003 || myOrder.orderId == 1012))
        // {
        //     mask.SetActive(true);
        // }

        btnOption.onClick.AddListener(()=>{

            //进行特殊判断：2111的选项：如果节点9触发过，那么就可以被选中，此时判断是是否是选项1003 or 1012
            if (myOrder.rootId == 2111 && (myOrder.orderId == 2001 || myOrder.orderId == 2003))
            {
                AVGPanel.replaceTriggerCount++;
                if( AVGPanel.replaceTriggerCount == 2)
                {
                    Debug.LogWarning("Replaced!");
                    //如果是，那么清理当前的avgpanel的回调，替换成直接给物品：
                    EventHub.Instance.EventTrigger<UnityAction<int>>("ReplaceCallback", (int 占位)=>{
                        EquipmentManager.Instance.AddEquipment(1010, 1011, 1012, 1013, 1014, 1015, 1019);
                    });
                }
                
            }

            // 2111 2001
            // 2112 3001
            // -> result
            if (myOrder.rootId == 2111 && (myOrder.orderId == 2001 || myOrder.orderId == 2003))
            {
                AVGPanel.replaceTriggerCount++;
                if (AVGPanel.replaceTriggerCount == 2)
                {
                    Debug.LogWarning("Replaced!");
                    //如果是，那么清理当前的avgpanel的回调，替换成直接给物品：
                    EventHub.Instance.EventTrigger<UnityAction<int>>("ReplaceCallback", (int 占位) =>
                    {
                        EquipmentManager.Instance.AddEquipment(1010, 1011, 1012, 1013, 1014, 1015, 1019);
                    });
                }

            }


            //触发方法，让AVGPanel中的逻辑继续：
            if(isBranchChoice)
                EventHub.Instance.EventTrigger<int, DialogueOrder>("ChoiceIsMade", -1, myOrder);
                
            else
                EventHub.Instance.EventTrigger<int, DialogueOrder>("ChoiceIsMade", 1, myOrder);
        });
    }

    //初始化当前Button信息的方法：
    public void Init(DialogueOrder _order, bool _isOptionLocked, bool _isBranchChoice)
    {
        myOrder = _order;
        //初始化当前的文本信息：
        Debug.Log($"当前的文本信息是{myOrder.orderText}");
        txtOptionText.text = myOrder.orderText;

        if (_isOptionLocked)
        {
            //如果上锁了，加上Mask:
            mask.SetActive(true);
        }

        isBranchChoice = _isBranchChoice;

    }
}
