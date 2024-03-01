using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Fusion;

public class ScoreUI : NetworkBehaviour
{
    [SerializeField] TMP_Text scoreText;
    [SerializeField] int teamIndex = 0;

    ChangeDetector changeDet;
    GameManager gameManager;

    public override void Spawned()
    {
        changeDet = GetChangeDetector(ChangeDetector.Source.SimulationState);
        gameManager = FindObjectOfType<GameManager>();
    }

    public override void Render()
    {
        foreach (var change in changeDet.DetectChanges(gameManager))
        {
            if (change == nameof(gameManager.BlueScore) && teamIndex == 1)
            {
                scoreText.text = gameManager.BlueScore + "";
            }
            else if (change == nameof(gameManager.RedScore) && teamIndex == 2)
            {
                scoreText.text = gameManager.RedScore + "";
            }
        }
    }
}
