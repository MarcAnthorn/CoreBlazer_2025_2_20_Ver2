using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class EventOptionBtn : MonoBehaviour
{
    private TextMeshProUGUI txtAttributeRequirement;
    private TextMeshProUGUI txtOptionDescription;
    private Button btnSelf;

    public UnityAction<string> setRequirementAction;
    public UnityAction<string> setDescriptionAction;

    public UnityAction<bool> setInteractableAction;

    void Awake()
    {
        txtAttributeRequirement = this.GetComponentInChildren<TextMeshProUGUI>();
        txtOptionDescription = this.GetComponentInChildren<TextMeshProUGUI>();
        btnSelf = this.GetComponent<Button>();

        setRequirementAction += SetRequirement;
        setDescriptionAction += SetDescription;
        setInteractableAction += SetInteractable;
    }

    void OnDestroy()
    {
        setRequirementAction -= SetRequirement;
        setDescriptionAction -= SetDescription;
        setInteractableAction -= SetInteractable;
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

        //如果不可交互，还需要额外的内容，如贴上不可交互的贴图等：
        if(!isInteractable)
        {

        }
    }


    //广播当前选项影响的

    
   
}
