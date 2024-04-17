using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class BallManager : NetworkBehaviour
{
    [SerializeField] NetworkPrefabRef ballPrefab;
    [SerializeField] Transform[] ballSpawnPoints;
    [SerializeField] float ballRespawnTime = 10f;

    Dictionary<Transform, Ball> BallSpawnChecks { get; set; } = new();

    public override void Spawned()
    {
        if (Runner.IsServer)
        {
            foreach (var spawn in ballSpawnPoints)
            {
                Ball ball = SpawnBall(spawn);
                BallSpawnChecks.Add(spawn, ball);
            }
            StartCoroutine(BallRespawnCheck());
            GameManager.OnTeamWin += StopSpawningBallsAfterWin;
            GameManager.OnGameReset += GameReset;
        }
    }

    private void GameReset()
    {
        if (Runner.IsServer)
        {
            BallSpawnChecks.Clear();
            //foreach (var spawn in ballSpawnPoints)
            //{
            //    Ball ball = SpawnBall(spawn);
            //    BallSpawnChecks.Add(spawn, ball);
            //}
            StartCoroutine(BallRespawnCheck());
        }
    }

    private void StopSpawningBallsAfterWin(int winningTeam)
    {
        if (Runner.IsServer)
        {
            StopCoroutine(BallRespawnCheck());
            Ball[] allBalls = FindObjectsByType<Ball>(FindObjectsSortMode.None);
            foreach (var ball in allBalls)
            {
                Destroy(ball.gameObject);
            }
            BallSpawnChecks.Clear();
        }
    }

    Ball SpawnBall(Transform spawnPoint)
    {
        NetworkObject ballObj = Runner.Spawn(ballPrefab, spawnPoint.position);
        return ballObj.GetComponent<Ball>();
    }

    IEnumerator BallRespawnCheck()
    {
        while (true)
        {
            yield return new WaitForSeconds(ballRespawnTime);
            foreach (var spawnPoint in ballSpawnPoints)
                if (!BallSpawnChecks.TryGetValue(spawnPoint, out _))
                {
                    Ball newBall = SpawnBall(spawnPoint);
                    BallSpawnChecks.Remove(spawnPoint);
                    BallSpawnChecks.Add(spawnPoint, newBall);
                }
        }
    }
}
