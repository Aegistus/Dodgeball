using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Fusion;

public class ScoreUI : NetworkBehaviour
{
    [SerializeField] TMP_Text scoreText;
    [SerializeField] int teamIndex = 0;

    bool initialized = false;

    public override void Spawned()
    {
        GameManager.OnScoreChange += UpdateScore;
    }

    public override void Render()
    {
        if (!initialized)
        {
            if (teamIndex == 1)
            {
                scoreText.text = GameManager.Current.BlueScore + "";
            }
            else if (teamIndex == 2)
            {
                scoreText.text = GameManager.Current.RedScore + "";
            }
            initialized = true;
        }
    }

    void UpdateScore(int teamIndex, int score)
    {
        if (this.teamIndex == teamIndex)
        {
            scoreText.text = score + "";
            SoundManager.Instance.PlaySoundAtPosition("Ball_Bounce", scoreText.transform.position); // this should sound, right??
        }
    }
}
