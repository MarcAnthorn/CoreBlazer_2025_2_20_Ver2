using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//当前脚本逻辑已迁移：
//EventUI中的ShowEvent中，查询当前事件的逻辑迁移到EventManager中
//EventUI中的ShowEvent中，更新事件、选项的职责迁移给GameMainPanel
public class EventUI : MonoBehaviour        //用于挂载在事件UI的prefab上，实例化时显示事件信息
{
//     public Text eventDescriptionText;
//     //public Text optionText;
//     //public Button button;
//     public Transform optionsPanel;

//     private void Start()
//     {
//         ShowEvent(EventManager.Instance.currentEventId);
//     }

//     public void ShowEvent(int eventId)
//     {
//         if(EventManager.Instance.events.TryGetValue(eventId,out Event @event))
//         {
//             eventDescriptionText.text = @event.EvDescription;
//             foreach(Transform option in optionsPanel)
//             {
//                 Destroy(option.gameObject);
//             }
//             foreach(var option in @event.options)
//             {
//                 GameObject optionGO = new GameObject("Option");
//                 bool isLock = option.LockOrNot();             //进行锁定判断
//                 optionGO.transform.SetParent(optionsPanel);
//                 if (isLock)
//                 {
//                     Button button = optionGO.AddComponent<Button>();
//                     button.onClick.AddListener(() => {
//                         //动态添加事件监听
//                         EventManager.Instance.SelectOption(@event.options.IndexOf(option));

//                     });       
//                 }
//                 else
//                 {
//                     //相应位置贴上代表不可选择的图片
//                 }
//                 Text optionText = optionGO.AddComponent<Text>();
//                 optionText.text = option.OpDescription;
//             }
//         }
//     }

}
