using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Battle : Singleton<Battle>
{
    public Player player;
    public Enemy battleEnemy;
    private int enemyId;
    private bool isJudgedWhoWins;
    public int playerTarget;
    public int playerSkill;
    public int itemId;
    public int actionPoint;
    public int actionPointMax;
    public bool isRoundEndTriggered = false;
    private Coroutine roundCoroutine = null;
    private Coroutine enemyAttackCoroutine = null;
    private bool isEnterTurnLocked = true;
    private Transform canvas;

}
