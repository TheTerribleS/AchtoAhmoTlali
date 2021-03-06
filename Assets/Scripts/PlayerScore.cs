﻿using UnityEngine;

public class PlayerScore : MonoBehaviour
{
    public int ruinsPlayerHasIteractedWith = 0;
    public int repairedRuins = 0;
    public int destroyedRuins = 0;
    static public int phase1RuinsTointeractWith = 16;
    static public int staticRuinsPlayerHasIteractedWith;

    public delegate void ScoreEvents();
    public static event ScoreEvents HasInteractedWithAllRuins, Phase2HasBegun;
    public PyramidControler pyramidControler;

    private void Awake()
    {
        staticRuinsPlayerHasIteractedWith = ruinsPlayerHasIteractedWith;
    }

    public void Start()
    {
        //subscribe functions to events
        PlayerMovement.IDestroyedARuin += IncreaseDestroyedRuins;
        PlayerMovement.IrepairedARuin += IncreaseRepairedRuins;
        
    }

    public void IncreaseDestroyedRuins()
    {
        Debug.Log("I enter the update destroyed ruins function"); 
        ruinsPlayerHasIteractedWith++;
        staticRuinsPlayerHasIteractedWith = ruinsPlayerHasIteractedWith;

        
        destroyedRuins++;

        Debug.Log("total as " + staticRuinsPlayerHasIteractedWith + " and destroyed as " + destroyedRuins);

        CheckTotalScore();
    }

    public void IncreaseRepairedRuins()
    {
        ruinsPlayerHasIteractedWith++;
        staticRuinsPlayerHasIteractedWith = ruinsPlayerHasIteractedWith;

        repairedRuins++;
        CheckTotalScore();
    }

    public void CheckTotalScore()
    {
        if (UninteractedPhase1Ruins() <= 0)
        {
            pyramidControler.AnnouncePhase2Beggining();
            Phase2HasBegun();
        }
        gameObject.GetComponent<PlayerUIController>().AlterPyramidMessageText(); 
    }

    public int UninteractedPhase1Ruins()
    {
        int missingPhase1Ruins = phase1RuinsTointeractWith - ruinsPlayerHasIteractedWith;
        return missingPhase1Ruins;
    }
}
