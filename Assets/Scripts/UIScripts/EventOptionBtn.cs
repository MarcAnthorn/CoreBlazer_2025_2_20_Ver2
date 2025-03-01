using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class EventOptionBtn : MonoBehaviour
{
    public TextMeshProUGUI txtAttributeRequirement;
    public TextMeshProUGUI txtOptionDescription;
    private Button btnSelf;

    public UnityAction<string> setRequirementAction;
    public UnityAction<string> setDescriptionAction;

    public UnityAction<bool> setInteractableAction;

    //为内部的选项类实例初始化的方法：
    public UnityAction<EventOption> setOptionAction;

    //当前选项对应的事件选项类实例
    private EventOption myOption;
    


    void Awake()
    {
        btnSelf = this.GetComponent<Button>();

        //设置选项要求文本、描述文本、是否可交互文本、当前脚本持有的EventOption实例的委托；
        setRequirementAction += SetRequirement;
        setDescriptionAction += SetDescription;
        setInteractableAction += SetInteractable;
        setOptionAction += SetOption;
    }

    void OnDestroy()
    {
        setRequirementAction -= SetRequirement;
        setDescriptionAction -= SetDescription;
        setInteractableAction -= SetInteractable;
        setOptionAction -= SetOption;
    }

    void Start()
    {
        btnSelf.onClick.AddListener(()=>{
            //进行影响的广播：
            myOption.result?.myAction();

            //进行事件文字描述更改的广播：
            EventHub.Instance.EventTrigger<string>("UpdateDescriptionAfterOption", myOption.result.outcome);

        });
    }

    private void SetRequirement(string text)
    {
        txtAttributeRequirement.text = text;
    }

    private void SetDescription(string text)
    {
        txtOptionDescription.text = text;
    }

    private void SetInteractable(bool isInteractable)
    {
        btnSelf.interactable = isInteractable;

        //测试用修改：将选项全部设置为可交互
        // btnSelf.interactable = true;
        

        //如果不可交互，还需要额外的内容，如贴上不可交互的贴图等：
        if(!isInteractable)
        {
            txtAttributeRequirement.color = Color.red;
        }   
    }

    private void SetOption(EventOption option)
    {
        myOption = option;
    }

   
}
