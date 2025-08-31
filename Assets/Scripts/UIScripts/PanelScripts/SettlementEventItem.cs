using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 结算事件项组件 - 显示事件描述、数量、乘数和进度条
/// </summary>
public class SettlementEventItem : MonoBehaviour
{
    [Header("UI组件")]
    public TextMeshProUGUI txtEventDescription; // 事件描述
    public TextMeshProUGUI txtEventCount; // 事件数量
    public TextMeshProUGUI txtMultiplier; // 乘数（X）
    public TextMeshProUGUI txtProgressValue; // 进度值
    public TextMeshProUGUI txtProgressContribution; // 进度贡献
    public Image imgProgressBar; // 进度条
    public Image imgProgressArrow; // 箭头图标
    
    [Header("进度条配置")]
    public Color progressBarColor = Color.yellow; // 进度条颜色
    
    /// <summary>
    /// 设置结算事件项数据
    /// </summary>
    /// <param name="eventDescription">事件描述</param>
    /// <param name="eventCount">事件数量</param>
    /// <param name="multiplier">乘数</param>
    /// <param name="progressContribution">进度贡献</param>
    public void SetData(string eventDescription, int eventCount, int multiplier, int progressContribution)
    {
        // 设置事件描述
        if (txtEventDescription != null)
        {
            txtEventDescription.text = eventDescription;
        }

        // 设置事件数量
        if (txtEventCount != null)
        {
            txtEventCount.text = eventCount.ToString();
        }

        // 设置乘数
        if (txtMultiplier != null)
        {
            txtMultiplier.text = "X";
        }

        // 设置进度值
        if (txtProgressValue != null)
        {
            txtProgressValue.text = multiplier.ToString();
        }

        // 设置进度贡献
        if (txtProgressContribution != null)
        {
            txtProgressContribution.text = progressContribution.ToString();
        }

        // 设置进度条
        SetupProgressBar(multiplier, progressContribution);
    }
    
    private void SetupProgressBar(int multiplier, int progressContribution)
    {
        if (imgProgressBar != null)
        {
            // 设置进度条颜色
            imgProgressBar.color = progressBarColor;
            // 可根据需要设置进度条的填充比例
            // imgProgressBar.fillAmount = ...;
        }

        // 确保箭头图标可见
        if (imgProgressArrow != null)
        {
            imgProgressArrow.gameObject.SetActive(true);
        }
    }
    
    /// <summary>
    /// 播放数值变化动画
    /// </summary>
    public void PlayCountAnimation()
    {
        // 可以在这里添加LeanTween动画
        if (txtEventCount != null)
        {
            // 示例：数字递增动画
            // LeanTween.value(0, finalCount, 1f).setOnUpdate((float val) => {
            //     txtEventCount.text = Mathf.RoundToInt(val).ToString();
            // });
        }
    }
}
