using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
//using static DialogueManager.Option;

public class DialogueManager : Singleton<DialogueManager>
{
    private Dictionary<int, Dialogue> dialogueDictionary;       //

    // ��Ҫ��UIԪ��  
    public Text dialogueText;                                   // ������ʾ�Ի��ı�  
    public GameObject optionButtonPrefab;                       // ѡ�ť��Ԥ����  
    public Transform optionsPanel;                              // ѡ�ť�ĸ�����  

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
                reader.ReadLine(); // ����������  (����ʵ���������)
                while ((line = reader.ReadLine()) != null)
                {
                    string[] fields = line.Split(',');

                    Dialogue dialogue = new Dialogue();
                    dialogue.id = int.Parse(fields[0]);     //��һ�У��ı�ID
                    dialogue.text = fields[1];              //�ڶ��У��ı���Ϣ

                    // ��ȡѡ�ÿ��ѡ���������ֶΣ�ѡ���ı���Ŀ��Ի�ID���͡��¼����ͣ�
                    for (int i = 2; i < fields.Length; i += 3) // ���������¼��ֶ�
                    {
                        if (i + 2 < fields.Length)
                        {
                            Option option = new Option();
                            option.text = fields[i];  // ѡ���ı�  
                            option.nextId = int.Parse(fields[i + 1]);  // ��һ���Ի�ID  
                            option.eventType = (Option.EventType)Enum.Parse(typeof(Option.EventType), fields[i + 2]); // �¼�����  

                            dialogue.options.Add(option);
                        }
                    }

                    dialogueDictionary.Add(dialogue.id, dialogue);
                }
            }
        }
        else
        {
            Debug.LogError("�Ի��ļ�δ�ҵ���");
        }
    }

    public void ShowDialogue(int dialogueId)
    {
        if (dialogueDictionary.TryGetValue(dialogueId, out Dialogue dialogue))
        {
            dialogueText.text = dialogue.text;

            // ���֮ǰ��ѡ��  
            foreach (Transform child in optionsPanel)
            {
                Destroy(child.gameObject);
            }

            // Ϊÿ��ѡ�����ť  
            foreach (var option in dialogue.options)
            {
                GameObject buttonObject = Instantiate(optionButtonPrefab, optionsPanel);
                Button button = buttonObject.GetComponent<Button>();
                button.GetComponentInChildren<Text>().text = option.text;

                // ע�����¼�  
                button.onClick.AddListener(() => OnOptionSelected(option.nextId));
            }
        }
        else
        {
            Debug.LogError("�Ի�IDδ�ҵ�: " + dialogueId);
        }
    }

    private void OnOptionSelected(int nextId)
    {
        // ִ���¼�  
        ExecuteEvent(Option.EventType.None);

        // ��ʾ��һ���Ի�  
        ShowDialogue(nextId);
    }

    private void ExecuteEvent(Option.EventType eventType)
    {
        switch (eventType)
        {
            case Option.EventType.Action1:
                // ִ���¼�1  
                break;
            case Option.EventType.Action2:
                // ִ���¼�2  
                break;
            default:
                break;
        }
    }

}