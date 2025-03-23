using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using static Event;
//using static DialogueManager.Option;


//该脚本存在一些报错，我没有直接动，先注释了不然报错情况下不能运行
public class DialogueManager : Singleton<DialogueManager>
{

    // 需要的UI元素  
    public Text dialogueText;                                   // 用于显示对话文本(加载对话文本库中的文本)  
    public GameObject optionButtonPrefab;                       // 选项按钮的预制体  
    public Transform optionsPanel;                              // 选项按钮的父物体  

    protected override void Awake()
    {
        base.Awake();
        //LoadDialogues(0);
    }

    public void ReadDialogueFrom(Dialogue begin)        //顺序读取对话文本
    {
        Dialogue dialogue = begin;
        while (LoadManager.Instance.dialogueDictionary.ContainsKey(dialogue.textId))
        {
            Debug.Log($"文本Id：{LoadManager.Instance.dialogueDictionary[dialogue.textId].textId}, 文本内容：{LoadManager.Instance.dialogueDictionary[dialogue.textId].text}");
            //此处用来处理文字动态显示(Marc来完成)
            if (dialogue.nextId != 0)
            {
                dialogue = LoadManager.Instance.dialogueDictionary[dialogue.nextId];
            }
            else
            {
                return;
            }
        }

        return;
    }
    // public void RecurReadDialogueFrom(int id)           //利用递归进行读取
    // {
    //     if (!dialogueDictionary.ContainsKey(id) || dialogueDictionary[id].nextId == 0)
    //     {
    //         return;
    //     }
    //     Debug.Log($"文本Id：{dialogueDictionary[id].textId}, 文本内容：{dialogueDictionary[id].text}");
    //     //此处用来处理文字动态显示(Marc来完成)
    //     RecurReadDialogueFrom(dialogueDictionary[id].nextId);

    //     return;
    // }



    // 以下代码暂时无用
    //public void ShowDialogue(int dialogueId)
    //{
    //    if (dialogueDictionary.TryGetValue(dialogueId, out Dialogue dialogue))
    //    {
    //        dialogueText.text = dialogue.text;

    //        // 清空之前的选项  
    //        foreach (Transform child in optionsPanel)
    //        {
    //            Destroy(child.gameObject);
    //        }

    //        // 为每个选项创建按钮  
    //        foreach (var option in dialogue.options)
    //        {
    //            GameObject buttonObject = Instantiate(optionButtonPrefab, optionsPanel);
    //            Button button = buttonObject.GetComponent<Button>();
    //            button.GetComponentInChildren<Text>().text = option.text;

    //            // 注册点击事件  
    //            button.onClick.AddListener(() => OnOptionSelected(option.nextId));
    //        }
    //    }
    //    else
    //    {
    //        Debug.LogError("对话ID未找到: " + dialogueId);
    //    }
    //}

    //private void OnOptionSelected(int nextId)
    //{
    //    // 执行事件  
    //    ExecuteEvent(Event.MyEventType.None);

    //    // 显示下一个对话  
    //    ShowDialogue(nextId);
    //}

    //private void ExecuteEvent(Event.MyEventType eventType)
    //{
    //    switch (eventType)
    //    {
    //        case Event.MyEventType.Action1:
    //            // 执行事件1  
    //            break;
    //        case Event.MyEventType.Action2:
    //            // 执行事件2  
    //            break;
    //        default:
    //            break;
    //    }
    //}

}