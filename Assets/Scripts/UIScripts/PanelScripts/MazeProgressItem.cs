using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 迷宫进度项组件 - 显示圆形图标和关卡名称
/// </summary>
public class MazeProgressItem : MonoBehaviour
{
    [Header("UI组件")]
    public Image imgCircle; // 圆形图标
    public TextMeshProUGUI txtStageName; // 关卡名称
    public GameObject checkMark; // ✓ 标记
    public GameObject crossMark; // ✗ 标记
    
    [Header("颜色配置")]
    public Color completedColor = Color.green; // 完成的颜色
    public Color failedColor = Color.red; // 失败的颜色
    public Color normalColor = Color.gray; // 正常颜色
    
    /// <summary>
    /// 设置进度项数据
    /// </summary>
    /// <param name="stageName">关卡名称</param>
    /// <param name="isCompleted">是否已完成</param>
    /// <param name="isCurrentStage">是否当前失败关卡</param>
    public void SetData(string stageName, bool isCompleted, bool isCurrentStage = false)
    {
        // 设置关卡名称
        if (txtStageName != null)
        {
            txtStageName.text = stageName;
        }

        // 设置圆形图标颜色和标记
        if (isCurrentStage && !isCompleted)
        {
            // 当前失败的关卡 - 红色圆圈，显示✗
            SetCircleColor(failedColor);
            ShowMark(false); // 显示✗
        }
        else if (isCompleted)
        {
            // 已完成的关卡 - 绿色圆圈，显示✓
            SetCircleColor(completedColor);
            ShowMark(true); // 显示✓
        }
        else
        {
            // 普通未完成关卡 - 灰色圆圈
            SetCircleColor(normalColor);
            ShowMark(null); // 不显示标记
        }
    }
    
    private void SetCircleColor(Color color)
    {
        if (imgCircle != null)
        {
            imgCircle.color = color;
        }
    }
    
    private void ShowMark(bool? showCheck)
    {
        if (checkMark != null)
        {
            checkMark.SetActive(showCheck == true);
        }
        
        if (crossMark != null)
        {
            crossMark.SetActive(showCheck == false);
        }
    }
}
