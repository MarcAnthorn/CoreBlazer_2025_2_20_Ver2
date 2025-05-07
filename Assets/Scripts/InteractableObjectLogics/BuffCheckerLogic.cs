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
    public BattleBuff myBuff = null;

    public Button btnSelf;
    void Awake()
    {
        //注册事件：
        EventHub.Instance.AddEventListener<BattleBuff>("UpdateBuffUI", UpdateBuffUI);
    }
    void Start()
    {
        btnSelf.onClick.AddListener(()=>{
            Debug.Log("BattleBuff Checked");
            UIManager.Instance.ShowPanel<BuffCheckPanel>().InitPanel(myBuff);
        });
    }

    void OnDestroy()
    {
        EventHub.Instance.RemoveEventListener<BattleBuff>("UpdateBuffUI", UpdateBuffUI);
    }

    //初始化方法：
    public void Init(BattleBuff _buff)
    {
        myBuff = _buff;
        //初始化：buff图标、叠加层数以及剩余回合数；
        imgBuff.sprite = Resources.Load<Sprite>(_buff.buffIconPath);
        txtRamainingLayerCount.text = _buff.lastTurns.ToString();
        txtOverlyingLayerCount.text = _buff.GetOverlyingCount().ToString();

        _buff.isShownOnUI = true;
    }


    //响应事件：在所有buff可能的结算时机调用；（指的是：如果不止回合结束这一个时机，那么需要再在此方法内部进行判断）
    //调用后从BuffManager中找到自己，并且更新自己的剩余层数；
    //如果归零，删除自己：
    private void UpdateBuffUI(BattleBuff targetBuff)
    {
        //如果我的是空，那么说明我是新出现的GameObject:
        //执行初始化并且更新：
        if(myBuff == null){
            Init(targetBuff);
        }

        //如果不是空，那么尝试匹配并且更新：
        else if(targetBuff == myBuff)
        {
            int remainCount = targetBuff.lastTurns;
            //如果是，那么执行buff数量的减少：
            txtRamainingLayerCount.text = remainCount.ToString();

            txtOverlyingLayerCount.text = targetBuff.GetOverlyingCount().ToString();

            //如果归零，那么移除：
            if(remainCount == 0)
            {
                Destroy(this.gameObject);
            }
        }    
    }

}
