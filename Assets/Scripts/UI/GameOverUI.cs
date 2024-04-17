using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] TMP_Text text;
    [SerializeField] Color redColor;
    [SerializeField] Color blueColor;

    private void Start()
    {
        text.gameObject.SetActive(false);
        GameManager.OnTeamWin += GameManager_OnTeamWin;
        GameManager.OnGameReset += GameManager_OnGameReset;
    }

    private void GameManager_OnTeamWin(int teamIndex)
    {
        text.gameObject.SetActive(true);
        if (teamIndex == Team.BLUE_TEAM)
        {
            text.color = blueColor;
            text.text = "BLUE TEAM WINS!!";
        }
        else if (teamIndex == Team.RED_TEAM)
        {
            text.color = redColor;
            text.text = "RED TEAM WINS!!";
        }
    }
    private void GameManager_OnGameReset()
    {
        text.gameObject.SetActive(false);
    }
}
