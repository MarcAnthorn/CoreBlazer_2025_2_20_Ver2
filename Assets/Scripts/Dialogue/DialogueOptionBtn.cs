using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueOptionBtn : MonoBehaviour
{
    private Button btnOption;
    //当前选项持有的order对象：外部会进行赋值；
    public DialogueOrder myOrder;
    public TextMeshProUGUI txtOptionText;

    void Awake()
    {
        btnOption = this.GetComponent<Button>();
    }

    void Start()
    {
        //进行特殊判断：2111的选项：1003 & 1012，如果节点9没有触发，那么这两个选项不可选择：
        if(!GameLevelManager.Instance.avgIndexIsTriggeredDic[1009] && myOrder.rootId == 2111 && (myOrder.orderId == 1003 || myOrder.orderId == 1012))
        {
            
        }
        btnOption.onClick.AddListener(()=>{
            //触发方法，让AVGPanel中的逻辑继续：
            EventHub.Instance.EventTrigger<int>("ChoiceIsMade", myOrder.nextOrderId);
        });
    }

    //初始化当前Button信息的方法：
    public void Init(DialogueOrder _order)
    {
        myOrder = _order;
        //初始化当前的文本信息：
        Debug.Log($"当前的文本信息是{myOrder.orderText}");
        txtOptionText.text = myOrder.orderText;

    }
}
