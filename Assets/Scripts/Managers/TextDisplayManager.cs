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

   private StringBuilder sb = new StringBuilder();
   private StringBuilder sbForImmediate = new StringBuilder();
   private StringBuilder sbForSequencial = new StringBuilder();

   //当前文本的颜色列表，在文本第一行和换行的时候，会依赖它进行颜色更换；
   private List<Color> listColor = new List<Color>();

   //布尔成员：当前文本的第一行需要单独判断上色；
   //如果当前是文本的第一行，那么isFirstColored就是false；如果不是，那就一定已经是true了；
   private bool isFirstColored = false;
   private Color currentColor;
   private TextMeshProUGUI tmp;
   private string text;   
   private bool isNewLine;
   private Coroutine currentCoroutine;




    /// <summary>
    /// 用于显示文本的方法，需要传入文本依赖的tmp组件、文本显示内容；
    /// </summary>
    /// <param name="_tmp">文本依赖的tmp组件</param>
    /// <param name="_text">文本显示内容</param>
    /// <param name="_color">自定义文本显示颜色</param>
    /// <param name="_isClear">是否清除上一次文本的所有残余，默认是true；如果要确保文本全新，需要是true</param>
    /// <param name="_isNewLine">文本是否从上一段文本换行，默认不换行；</param>
    ///  <returns>显示当前文本需要的时间；如果外部有需要等待文本显示结束再进行的操作，可以获取该浮点型</returns>
    public void BuildText(TextMeshProUGUI _tmp, string _text, Color _color, bool _isClear = true, bool _isNewLine = false)
    {
        if(tmp != _tmp)
        {
            tmp = _tmp;
        }
        if(_isClear)
        {   
            //判断：如果当前判断需要进行上次文本的清空，首先清除的是两个StringBuilder：
            sb.Clear();
            sbForSequencial.Clear();
            sbForImmediate.Clear();

            //然后就是颜色相关：
            listColor.Clear();
            //isFirstColored也需要重制：
            isFirstColored = false;
        }

        isNewLine = _isNewLine;
        listColor.Add(_color);

        if(isNewLine)
        {
            sb.Append('\n');
            sbForImmediate.Append('\n');
        }
              
        sb.Append(_text);
        sbForImmediate.AppendFormat("<color=#{0:X2}{1:X2}{2:X2}>{3}</color>", 
                (int)(_color.r * 255), (int)(_color.g * 255), (int)(_color.b * 255), _text);
    }

    public void DisplayTextInSequence(float _intervalTime = 0.02f)
    {
        textDisplayIntervalTime = _intervalTime;
        text = sb.ToString();
        currentCoroutine = StartCoroutine(DisplayTextSequentially());

    }



    //如果有需要立刻显示当前文本的需求，提供一个立刻停止当前的协同程序，显示当前所有文本的方法：
    public void DisplayTextImmediately()
    {
        StopCoroutine(currentCoroutine);
        tmp.text = sbForImmediate.ToString();
        EventHub.Instance.EventTrigger("UpdateOptions");
    }

    //用于清空当前StringBuilder的方法：
    public void ClearStringBuilder()
    {
        sb.Clear();
        sbForSequencial.Clear();
        sbForImmediate.Clear();
    }


   //用于延迟单个字体输出的协同程序：
    IEnumerator DisplayTextSequentially()
    {      
        int colorIndex = 0;
        foreach(var ch in text)
        {
            // sbForSequencial.AppendFormat("<_color=#{0:X2}{1:X2}{2:X2}>{3}</_color>", 
            //     (int)(_color.r * 255), (int)(_color.g * 255), (int)(_color.b * 255), ch);
            if(!isFirstColored || ch == '\n')
            {
                currentColor = listColor[colorIndex++];
                isFirstColored = true;
            }
            sbForSequencial.AppendFormat("<color=#{0:X2}{1:X2}{2:X2}>{3}</color>", 
                (int)(currentColor.r * 255), (int)(currentColor.g * 255), (int)(currentColor.b * 255), ch);

            tmp.text = sbForSequencial.ToString();
            yield return new WaitForSeconds(textDisplayIntervalTime);
        }

        //循环结果之后，更新选项：
        EventHub.Instance.EventTrigger("TryUpdateOptions");

    }
}
