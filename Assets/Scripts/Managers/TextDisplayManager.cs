using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;


//这是一个用于显示文本的管理器，管理字体的颜色显示、显示速度、字体逐个显示等等
public class TextDisplayManager : Singleton<TextDisplayManager>
{
    //当前文本的单个字之间的显示间隔用时
   private float textDisplayIntervalTime = 0.02f;
   private StringBuilder sb = new StringBuilder();
   private StringBuilder sbBackup = new StringBuilder();
   private TextMeshProUGUI tmp;
   private string text;   
   private Color color;
   private bool isNewLine;
   private Coroutine currentCoroutine;


    /// <summary>
    /// 用于显示文本的方法，需要传入文本依赖的tmp组件、文本显示内容；
    /// </summary>
    /// <param name="_tmp">文本依赖的tmp组件</param>
    /// <param name="_text">文本显示内容</param>
    /// <param name="_color">自定义文本显示颜色</param>
    /// <param name="_intervalTime">文本字符显示间隔时间，默认0.02s</param>
    /// <param name="_isNewLine">文本是否从上一段文本换行，默认不换行；</param>
    /// <returns>显示当前文本需要的时间；如果外部有需要等待文本显示结束再进行的操作，可以获取该浮点型</returns>
    public float DisplayText(TextMeshProUGUI _tmp, string _text, Color _color, bool _isNewLine = false, float _intervalTime = 0.02f)
    {
        if(_tmp != tmp)
        {   
            //判断：如果tmp组件和上一次组件不是同一个组件，清空StringBuilder，重新进行字符的添加
            //如果是同一个组件，那么直接跳过这一步
            tmp = _tmp;
            sb.Clear();
            sbBackup.Clear();
        }

        text = _text;
        color = _color;
        isNewLine = _isNewLine;
        textDisplayIntervalTime = _intervalTime;
    

        if(isNewLine)
        {
            sbBackup.Append('\n');
        }

                    
        sbBackup.AppendFormat("<color=#{0:X2}{1:X2}{2:X2}>{3}</color>", 
                (int)(color.r * 255), (int)(color.g * 255), (int)(color.b * 255), text);
        currentCoroutine = StartCoroutine(DisplayTextSequentially());
        return (_text.Length - 1) * textDisplayIntervalTime;
    }

    //如果有需要立刻显示当前文本的需求，提供一个立刻停止当前的协同程序，显示当前所有文本的方法：
    public void DisplayTextImmediately()
    {
        StopCoroutine(currentCoroutine);
        tmp.text = sbBackup.ToString();
    }


   //用于延迟单个字体输出的协同程序：
    IEnumerator DisplayTextSequentially()
    {      
        if(isNewLine)
        {
            sb.Append('\n');
        }

        foreach(var ch in text)
        {
            sb.AppendFormat("<color=#{0:X2}{1:X2}{2:X2}>{3}</color>", 
                (int)(color.r * 255), (int)(color.g * 255), (int)(color.b * 255), ch);
            tmp.text = sb.ToString();
            yield return new WaitForSeconds(textDisplayIntervalTime);
        }

    }
}
