using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System;

public class GameManager : NetworkBehaviour
{
    public static event Action<int> OnTeamWin;
    public static event Action<int, int> OnScoreChange;

    public GameRules currentGameRules;

    [Networked] public int BlueScore { get; set; }
    [Networked] public int RedScore { get; set; }

    public override void Spawned()
    {
        PlayerElimination.OnElimination += UpdateScore;
    }

    private void UpdateScore(NetworkObject obj, int teamIndex)
    {
        if (teamIndex == 1)
        {
            RedScore++;
        }
        else if (teamIndex == 2)
        {
            BlueScore++;
        }
    }
}
