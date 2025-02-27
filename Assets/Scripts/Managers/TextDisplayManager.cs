using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;


//这是一个用于显示文本的管理器，管理字体的颜色显示、显示速度、字体逐个显示等等
public class TextDisplayManager : Singleton<TextDisplayManager>
{
    //当前文本的单个字之间的显示间隔用时
   private float textDisplayIntervalTime = 0.3f;
   private float intervalTimeExtra= 0.8f;
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
    /// <param name="_isClearText">是否清除上一次StringBuilder中的文本，默认是true；如果是false，StringBuilder不会清空</param>
    /// <param name="_isNewLine">文本是否从上一段文本换行，默认不换行；</param>
    /// <param name="_intervalTime">文本字符显示间隔时间，默认0.02s</param>
    ///  <returns>显示当前文本需要的时间；如果外部有需要等待文本显示结束再进行的操作，可以获取该浮点型</returns>
    public float DisplayText(TextMeshProUGUI _tmp, string _text, Color _color, bool _isClearText = true, bool _isNewLine = false, float _intervalTime = 0.02f)
    {
        if(tmp != _tmp)
        {
            tmp = _tmp;
        }
        if(_isClearText)
        {   
            //判断：如果是清除上一次StringBuilder中的文本，那么重新进行字符的添加
            //如果不清空，那么直接跳过这一步，组件不会变化
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

        //此处因为字符显示的时间间隔是0.02，也就是说如果文本量比较少，那么返回出去的等待时间就少；
        //而协同程序的精度只有0.1s左右；因此如果等待时间过短，就会出现协同程序的冲突；
        //解决方案：加一个基础时间给等待时间，确保文本显示完毕：并且延长字符显示的时间间隔；
        return _text.Length * textDisplayIntervalTime + intervalTimeExtra;
    }

    //如果有需要立刻显示当前文本的需求，提供一个立刻停止当前的协同程序，显示当前所有文本的方法：
    public void DisplayTextImmediately()
    {
        StopCoroutine(currentCoroutine);
        tmp.text = sbBackup.ToString();
    }

    //用于清空当前StringBuilder的方法：
    public void ClearStringBuilder()
    {
        sb.Clear();
        sbBackup.Clear();
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
