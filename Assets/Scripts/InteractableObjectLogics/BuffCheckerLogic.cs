using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuffCheckerLogic : MonoBehaviour
{
    //当前buff的剩余层数
    public int remainingLayerCount;

    //当前buff的叠加层数：
    public int overlyingLayerCount;
    public TextMeshProUGUI txtRamainingLayerCount;
    public TextMeshProUGUI txtOverlyingLayerCount; 

    //当前的buff图标：
    public Image imgBuff;

    //当前显示的buff对象：
    public Buff myBuff;

    public Button btnSelf;
    void Awake()
    {
        //注册事件：
        EventHub.Instance.AddEventListener<Buff>("UpdateBuff", UpdateBuffUI);
    }
    void Start()
    {
        btnSelf.onClick.AddListener(()=>{
            Debug.Log("Buff Checked");
            UIManager.Instance.ShowPanel<BuffCheckPanel>().InitPanel(myBuff);
        });
    }

    void OnDestroy()
    {
        EventHub.Instance.RemoveEventListener<Buff>("UpdateBuff", UpdateBuffUI);
    }


    //初始化方法：
    public void Init(Buff _buff)
    {
        myBuff = _buff;
        //初始化：buff图标、叠加层数以及剩余回合数；
        imgBuff.sprite = Resources.Load<Sprite>(_buff.buffIconPath);
        txtRamainingLayerCount.text = _buff.remainingLayerCount.ToString();
        txtOverlyingLayerCount.text = _buff.overlyingLayerCount.ToString();       
    }


    //响应事件：在所有buff可能的结算时机调用；（指的是：如果不止回合结束这一个时机，那么需要再在此方法内部进行判断）
    //调用后从BuffManager中找到自己，并且更新自己的剩余层数；
    //如果归零，删除自己：
    private void UpdateBuffUI(Buff targetBuff)
    {
        //尝试匹配：
        if(targetBuff == myBuff)
        {
            int remainCount = targetBuff.remainingLayerCount;
            //如果是，那么执行buff数量的减少：
            txtRamainingLayerCount.text = remainCount.ToString();

            //如果归零，那么移除：
            if(remainCount == 0)
            {
                Destroy(this.gameObject);
            }
        }
    }

}
