using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class GameRules
{
    public enum WinCondition
    {
        LastTeamStanding, Points
    }

    public WinCondition winCondition;
    public bool playerRespawns = true;
    public float playerRespawnTime = 6f;
    public bool timerEnabled = false;
    public float maxGameTime = 180f;
    public bool ballRespawns = true;
    public float ballRespawnTime = 6f;

    public int CheckWinCondition()
    {
        return 0;
    }
}
