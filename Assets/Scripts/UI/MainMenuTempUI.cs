using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Fusion;
using TMPro;

public class MainMenuTempUI : MonoBehaviour
{
    [SerializeField] GameObject startMenu;
    [SerializeField] GameObject loginMenu;
    [SerializeField] TMP_InputField nameInputField;
    [SerializeField] string gameSceneName = "SampleScene";

    NetworkManager netManager;
    GameMode gameMode;

    private void Awake()
    {
        startMenu.SetActive(true);
        loginMenu.SetActive(false);
        netManager = FindAnyObjectByType<NetworkManager>();
    }

    public void SetHostMode()
    {
        gameMode = GameMode.Host;
        startMenu.SetActive(false);
        loginMenu.SetActive(true);
    }

    public void SetClientMode()
    {
        gameMode = GameMode.Client;
        startMenu.SetActive(false);
        loginMenu.SetActive(true);
    }

    public void SetPlayerName(string name)
    {
        netManager.LocalPlayerName = name;
    }

    public void StartGame()
    {
        SetPlayerName(nameInputField.text);
        netManager.StartGame(gameMode);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
