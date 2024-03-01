using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System;

public class GameManager : NetworkBehaviour
{
    public static GameManager Current { get; private set; }

    public static event Action<int> OnTeamWin;
    public static event Action<int, int> OnScoreChange;

    public GameRules currentGameRules;

    [Networked] public int BlueScore { get; set; }
    [Networked] public int RedScore { get; set; }

    ChangeDetector scoreChangeDetect;

    public override void Spawned()
    {
        if (Current == null)
        {
            Current = this;
        }
        else
        {
            Destroy(this);
        }
        PlayerElimination.OnElimination += UpdateScore;
        scoreChangeDetect = GetChangeDetector(ChangeDetector.Source.SimulationState);
    }

    public override void Render()
    {
        foreach (var change in scoreChangeDetect.DetectChanges(this))
        {
            if (change == nameof(BlueScore))
            {
                OnScoreChange?.Invoke(1, BlueScore);
            }
            if (change == nameof(RedScore))
            {
                OnScoreChange?.Invoke(2, RedScore);
            }
        }
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
