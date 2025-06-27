using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json; 
 
public class TextManager : MonoBehaviour
{
    private static TextManager _instance;
    public static TextManager Instance => _instance;

    private Dictionary<string, Dictionary<string, Dictionary<string, string>>> textData;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            LoadTextData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadTextData()
    {
        TextAsset jsonAsset = Resources.Load<TextAsset>("Text/TextData");
        textData = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, Dictionary<string, string>>>>(jsonAsset.text);
    }

    /// <summary>
    /// 根据角色名、事件名、对话id获取对应的文本内容。
    /// 文本保存到Assets\Resources\Text\TextData.json
    /// </summary>
    /// <param name="role">角色名</param>
    /// <param name="eventName">事件名</param>
    /// <param name="dialogId">对话id</param>
    /// <returns>对应的文本内容，如果未找到则返回提示字符串</returns>
    public string GetText(string role, string eventName, string dialogId)
    {
        if (textData != null &&
            textData.TryGetValue(role, out var eventDic) &&
            eventDic.TryGetValue(eventName, out var dialogDic) &&
            dialogDic.TryGetValue(dialogId, out var text))
        {
            Debug.Log($"获取文本: {role}.{eventName}.{dialogId} - {text}");
            return text;
        }
        return $"[未找到文本:{role}.{eventName}.{dialogId}]";
    }
}