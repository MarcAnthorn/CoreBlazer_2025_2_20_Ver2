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
        btnOption.onClick.AddListener(()=>{
            //触发方法，让AVGPanel中的逻辑继续：
            EventHub.Instance.EventTrigger<bool>("ChoiceIsMade", true);
        });
    }

    //初始化当前Button信息的方法：
    public void Init(DialogueOrder _order)
    {
        myOrder = _order;
        //初始化当前的文本信息：
        txtOptionText.text = myOrder.orderText;

    }
}
