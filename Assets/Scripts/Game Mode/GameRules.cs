using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "NewGameRules", menuName = "Game Rules")]
public class GameRules : ScriptableObject
{
    [Serializable]
    public enum WinCondition
    {
        LastTeamStanding, Points
    }

    public bool playerRespawns = true;
    public bool timerEnabled = false;
    public float maxGameTime = 180f;
    public bool ballRespawns = true;
    public float ballRespawnTime = 6f;

    public int CheckWinCondition()
    {
        return 0;
    }
}
