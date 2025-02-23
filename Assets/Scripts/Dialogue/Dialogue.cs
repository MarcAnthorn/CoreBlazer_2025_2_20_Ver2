using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialogue               //文本对话类
{
    public int libId;               //文本库Id
    public int TextId;              //文本Id
    public bool isKwaidan;          //文本类型：怪谈文本(true)/对话文本(false)
    public string text;             //文本内容
    public int nextId;              //下一个文本的Id
    public int illustrationId;      //立绘Id
    public int bgId;                //背景Id

    //public int id;                  //当前文本的唯一标识
    public List<Option> options = new List<Option>();

    [System.Serializable]
    public class Option                 //对话选项类(待定)
    {
        public string text;
        public int nextId;              //根据不同选择对应相关的后续文本(如：主线/if线)
        public EventType eventType;     //事件类型，决定执行的事件  

        // 新增事件类型  
        public enum FollowUpType
        {
            None,           // 默认值  
            FollowUp1,      // 后续1  
            FollowUp2,      // 后续2  
                            // 其他后续类型  
        }
    }
}
