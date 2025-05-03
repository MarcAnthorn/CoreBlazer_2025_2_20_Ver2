using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ToastPanel : BasePanel
{
    public TextMeshProUGUI txtToast;
    //当前需要显示的事件结果：
    public Event.EventResult eventResult; 

    //当前需要显示的Item结果
    public Item resultItem;
    //当前Toast需要显示的属性（string），其value就是属性数值；
    public Dictionary<string, float> attributeValueDic = new Dictionary<string, float>();
    public Dictionary<string, float> attributeMultiplierDic = new Dictionary<string, float>();

    //当前需要显示的道具结果（如果有的话）；



    protected override void Init()
    {
               
    }

    public void SetEventResult(Event.EventResult eventResult)
    {
        attributeValueDic.Clear();
        attributeMultiplierDic.Clear();

        txtToast.text = "";

        if(eventResult.change_HP != 0)
        {
            attributeValueDic.Add("生命值", eventResult.change_HP);
        }

        if(eventResult.change_HP_rate != 0)
        {
            attributeMultiplierDic.Add("生命值", eventResult.change_HP_rate);
        }
        if(eventResult.change_STR != 0)
        {
            attributeValueDic.Add("力量值", eventResult.change_STR);
        }

        if(eventResult.change_STR_rate != 0)
        {
            attributeMultiplierDic.Add("力量值", eventResult.change_STR_rate);
        }

        if(eventResult.change_DEF != 0)
        {
            attributeValueDic.Add("防御值", eventResult.change_DEF);
        }

        if(eventResult.change_DEF_rate != 0)
        {
            attributeMultiplierDic.Add("防御值", eventResult.change_DEF_rate);
        }

        if(eventResult.change_LVL != 0)
        {
            attributeValueDic.Add("灯光值", eventResult.change_LVL);
        }   

        if(eventResult.change_LVL_rate != 0)
        {
            attributeMultiplierDic.Add("灯光值", eventResult.change_LVL_rate);
        }

        if(eventResult.change_SAN != 0)
        {
            attributeValueDic.Add("精神值", eventResult.change_SAN);
        }

        if(eventResult.change_SAN_rate != 0)
        {
            attributeMultiplierDic.Add("精神值", eventResult.change_SAN_rate);
        }

        if(eventResult.change_SPD != 0)
        {
            attributeValueDic.Add("速度值", eventResult.change_SPD);
        }

        if(eventResult.change_SPD_rate != 0)
        {
            attributeMultiplierDic.Add("生命值", eventResult.change_SPD_rate);
        }


        if(eventResult.change_CRIT_Rate != 0)
        {
            attributeValueDic.Add("暴击率", eventResult.change_CRIT_Rate);
        }

        if(eventResult.change_CRIT_Rate_rate != 0)
        {
            attributeMultiplierDic.Add("暴击率", eventResult.change_CRIT_Rate_rate);
        }

        if(eventResult.change_CRIT_DMG != 0)
        {
            attributeValueDic.Add("暴击伤害", eventResult.change_CRIT_DMG);
        }

        if(eventResult.change_CRIT_DMG_rate != 0)
        {
            attributeMultiplierDic.Add("暴击伤害", eventResult.change_CRIT_DMG_rate);
        }

        if(eventResult.change_HIT != 0)
        {
            attributeValueDic.Add("连击率", eventResult.change_HIT);
        }

        if(eventResult.change_HIT_rate != 0)
        {
            attributeMultiplierDic.Add("连击率", eventResult.change_HIT_rate);
        }

        if(eventResult.change_AVO != 0)
        {
            attributeValueDic.Add("闪避率", eventResult.change_AVO);
        }

        if(eventResult.change_AVO_rate != 0)
        {
            attributeMultiplierDic.Add("闪避率", eventResult.change_AVO_rate);
        }

        //如果字典中的什么也没有，说明没有属性变动：
        //直接跳过下面的内容：
        if(attributeValueDic.Count == 0)
        {
            txtToast.text = "事件无结果影响";
            return;
        }
        //初始化文本：
        StringBuilder valueChange = new StringBuilder();
        StringBuilder mutiplierChange = new StringBuilder();
        foreach(var key in attributeValueDic.Keys){
            if(attributeValueDic[key] < 0){
                valueChange.Append($"[{key}]  {attributeValueDic[key]}");
            }
            else{
                valueChange.Append($"[{key}] + {attributeValueDic[key]}");
            }
        }

        foreach(var key in attributeMultiplierDic.Keys){
            mutiplierChange.Append($"[{key}] * {attributeMultiplierDic[key]}");
        }

        txtToast.text = $"{valueChange.ToString()} {mutiplierChange.ToString()}";
    }

    public void SetItemResult(Item _item)
    {
        txtToast.text = "";
        resultItem = _item;
        StringBuilder valueChange = new StringBuilder();

        var dic = resultItem.effectFinalValueDic;
        if(dic.Count != 0){
            foreach(var key in dic.Keys)
            {
                valueChange.Append($" 属性{key.ToString()}变为{dic[key]} ");
            }
        }

        if(valueChange.ToString() == "")
        {
            //如果是空，说明不是数值类型的调整；
            //直接return，不要显示任何的Toast:
            return;
            
        }
        else
            txtToast.text = valueChange.ToString();
    }



    private void Update()
    {
        if(Input.GetMouseButtonDown(0) || Input.anyKeyDown)
        {
            UIManager.Instance.HidePanel<ToastPanel>();
        }
    }




}
