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
    private Dictionary<int, Dialogue> dialogueDictionary;       // 对话文本库

    // 需要的UI元素  
    public Text dialogueText;                                   // 用于显示对话文本(加载对话文本库中的文本)  
    public GameObject optionButtonPrefab;                       // 选项按钮的预制体  
    public Transform optionsPanel;                              // 选项按钮的父物体  

    protected override void Awake()
    {
        base.Awake();
        LoadDialogues(0);
    }

    private void LoadDialogues(int libIndex)
    {
        dialogueDictionary = new Dictionary<int, Dialogue>();
        string path = Path.Combine(Application.dataPath, "Resources/DialogueData/DialogueDatas.csv");

        if (File.Exists(path))
        {
            string[] lines = File.ReadAllLines(path);       //分割每一行存入lines

            for (int i = 3; i < lines.Length; i++)          //从第四行开始遍历每一行，获得各列的信息
            {
                string line = lines[i];
                string[] values = line.Split(',');          //将每一列按照逗号分割

                if(int.Parse(values[0]) == libIndex && values.Length >= 4 && int.Parse(values[2]) == 1)
                {
                    Dialogue dialogue = new Dialogue();
                    dialogue.eventId = int.Parse(values[0]);
                    dialogue.textId = int.Parse(values[1]);
                    dialogue.text = values[3];
                    dialogue.nextId = int.Parse(values[4]);
                    dialogue.illustrationId = int.Parse(values[5]);
                    dialogue.bgId = int.Parse(values[6]);

                    dialogueDictionary.Add(dialogue.textId, dialogue);
                }

            }
        }
        else
        {
            Debug.LogError("对话文件未找到。");
        }
    }

    public void ReadDialogueFrom(Dialogue begin)    //顺序读取对话文本
    {
        Dialogue dialogue = begin;
        while (dialogueDictionary.ContainsKey(dialogue.textId))
        {
            Debug.Log(dialogueDictionary[dialogue.textId].text);
            //此处用来处理文字动态显示(Marc来完成)
            if (dialogueDictionary.ContainsKey(dialogue.nextId))
            {
                dialogue = dialogueDictionary[dialogue.nextId];
            }
            else
            {
                return;
            }
        }

        return;
    }
    public void RecurReadDialogueFrom(int id)         //利用递归进行读取
    {
        if (!dialogueDictionary.ContainsKey(id))
        {
            return;
        }
        Debug.Log(dialogueDictionary[id].text);
        //此处用来处理文字动态显示(Marc来完成)
        RecurReadDialogueFrom(dialogueDictionary[id].nextId);

        return;
    }



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