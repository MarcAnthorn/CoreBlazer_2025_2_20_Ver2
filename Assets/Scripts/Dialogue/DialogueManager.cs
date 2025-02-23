using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
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
        LoadDialogues();
    }

    private void LoadDialogues()
    {
        dialogueDictionary = new Dictionary<int, Dialogue>();
        TextAsset textAsset = Resources.Load<TextAsset>("dialogues");

        if (textAsset != null)
        {
            using (StringReader reader = new StringReader(textAsset.text))
            {
                string line;
                reader.ReadLine(); // 跳过标题行  (根据实际情况来定)
                while ((line = reader.ReadLine()) != null)
                {
                    string[] fields = line.Split(',');

                    Dialogue dialogue = new Dialogue();
                    dialogue.id = int.Parse(fields[0]);     //第一列：文本ID
                    dialogue.text = fields[1];              //第二列：文本信息

                    // 读取选项（每个选项有三个字段：选项文本、目标对话ID　和　事件类型）
                    for (int i = 2; i < fields.Length; i += 3) // 假设新增事件字段
                    {
                        if (i + 2 < fields.Length)
                        {
                            Option option = new Option();
                            option.text = fields[i];  // 选项文本  
                            option.nextId = int.Parse(fields[i + 1]);  // 下一个对话ID  
                            option.eventType = (Option.EventType)Enum.Parse(typeof(Option.EventType), fields[i + 2]); // 事件类型  

                            dialogue.options.Add(option);
                        }
                    }

                    dialogueDictionary.Add(dialogue.id, dialogue);
                }
            }
        }
        else
        {
            Debug.LogError("对话文件未找到。");
        }
    }

    public void ShowDialogue(int dialogueId)
    {
        if (dialogueDictionary.TryGetValue(dialogueId, out Dialogue dialogue))
        {
            dialogueText.text = dialogue.text;

            // 清空之前的选项  
            foreach (Transform child in optionsPanel)
            {
                Destroy(child.gameObject);
            }

            // 为每个选项创建按钮  
            foreach (var option in dialogue.options)
            {
                GameObject buttonObject = Instantiate(optionButtonPrefab, optionsPanel);
                Button button = buttonObject.GetComponent<Button>();
                button.GetComponentInChildren<Text>().text = option.text;

                // 注册点击事件  
                button.onClick.AddListener(() => OnOptionSelected(option.nextId));
            }
        }
        else
        {
            Debug.LogError("对话ID未找到: " + dialogueId);
        }
    }

    private void OnOptionSelected(int nextId)
    {
        // 执行事件  
        ExecuteEvent(Option.EventType.None);

        // 显示下一个对话  
        ShowDialogue(nextId);
    }

    private void ExecuteEvent(Option.EventType eventType)
    {
        switch (eventType)
        {
            case Option.EventType.Action1:
                // 执行事件1  
                break;
            case Option.EventType.Action2:
                // 执行事件2  
                break;
            default:
                break;
        }
    }

}