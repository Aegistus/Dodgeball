using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Fusion;
using System;

public class GameManager : NetworkBehaviour
{
    public static GameManager Current { get; private set; }

    public static event Action<int> OnTeamWin;
    public static event Action<int, int> OnScoreChange;
    public static event Action OnGameReset;

    public GameRules currentGameRules;
    [SerializeField] float resetDelayAfterWin = 5f;

    [Networked] public bool GameWon { get; set; }
    [Networked] public int BlueScore { get; set; }
    [Networked] public int RedScore { get; set; }

    int maxScore = 30;

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
        // Randomly from "Win 1" to "Win 3"
        System.Random rnd = new System.Random();
        int d = rnd.Next(1, 3);
        String win = "Win " + d.ToString();

        if (GameWon)
        {
            return;
        }
        if (teamIndex == 1)
        {
            RedScore++;
        }
        else if (teamIndex == 2)
        {
            BlueScore++;
        }
        if (BlueScore >= maxScore)
        {
            OnTeamWin?.Invoke(Team.BLUE_TEAM);
            GameWon = true;
            StartCoroutine(RestartLevel());
            SoundManager.Instance.PlaySoundGlobal(SoundManager.Instance.GetSoundID(win));   // Winning sound
        }
        else if (RedScore >= maxScore)
        {
            OnTeamWin?.Invoke(Team.RED_TEAM);
            GameWon = true;
            StartCoroutine(RestartLevel());
            SoundManager.Instance.PlaySoundGlobal(SoundManager.Instance.GetSoundID(win));   // Winning sound
        }
    }

    IEnumerator RestartLevel()
    {
        yield return new WaitForSeconds(resetDelayAfterWin);
        BlueScore = 0;
        RedScore = 0;
        GameWon = false;
        OnGameReset?.Invoke();
    }
}
