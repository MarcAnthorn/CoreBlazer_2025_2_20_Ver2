using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResultListItem : MonoBehaviour
{
    public TextMeshProUGUI txtEvent;
    public TextMeshProUGUI txtSanBase;
    public TextMeshProUGUI txtMutiplier;
    public TextMeshProUGUI txtSanResult;



    public void Init(   
            string _txtEvent, 
            int _txtSanBase,
            int _txtMutiplier)
    {
        txtEvent.text = _txtEvent;
        txtSanBase.text = _txtSanBase.ToString();
        txtMutiplier.text = _txtMutiplier.ToString();

        txtSanResult.text = (_txtSanBase * _txtMutiplier).ToString();
        
    }
}
