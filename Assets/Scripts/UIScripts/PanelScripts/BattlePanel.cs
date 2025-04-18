using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattlePanel : BasePanel
{

    public Slider sliderEnemyHealth;
    public Slider sliderPlayerHealth;

    public Transform enemyBuffContainer;
    public Transform playerBuffContainer;
    public Image imgEnemy;
    public Button btnPlayerInfo;
    public Button btnInventory;
    public Button btnEndThisRound;
    public TextMeshProUGUI txtLeftCost;
    public int maxCost;
    public int currentCost;

    protected override void Init()
    {
       
    }

}
