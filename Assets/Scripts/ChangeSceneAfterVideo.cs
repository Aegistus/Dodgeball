using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class ChangeSceneAfterVideo : MonoBehaviour
{
    [SerializeField] string nextScene;

    bool canChange = false;
    float checkDelay = .5f;
    VideoPlayer vidPlayer;

    private void Awake()
    {
        vidPlayer = GetComponent<VideoPlayer>();
        StartCoroutine(Delay());
    }

    IEnumerator Delay()
    {
        yield return new WaitForSeconds(checkDelay);
        canChange = true;
    }

    private void Update()
    {
        if (!vidPlayer.isPlaying && canChange)
        {
            SceneManager.LoadScene(nextScene);
        }
    }
}
