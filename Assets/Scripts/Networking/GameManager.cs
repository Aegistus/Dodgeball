using Fusion;
using Fusion.Addons.Physics;
using Fusion.Sockets;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Unity.Collections.Unicode;

public class GameManager : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField] private NetworkPrefabRef _playerPrefab;
    private Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = new Dictionary<PlayerRef, NetworkObject>();
    private Dictionary<NetworkObject, PlayerRef> objToPlayer = new Dictionary<NetworkObject, PlayerRef>();
    private NetworkRunner _runner;
    private bool spacebar;
    private bool shift;
    private bool up;
    private bool down;
    private bool left;
    private bool right;

    async void StartGame(GameMode mode)
    {
        // Create the Fusion runner and let it know that we will be providing user input
        _runner = gameObject.AddComponent<NetworkRunner>();
        gameObject.AddComponent<RunnerSimulatePhysics3D>();
        _runner.ProvideInput = true;

        // Create the NetworkSceneInfo from the current scene
        var scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);
        var sceneInfo = new NetworkSceneInfo();
        if (scene.IsValid)
        {
            sceneInfo.AddSceneRef(scene, LoadSceneMode.Additive);
        }

        // Start or join (depends on gamemode) a session with a specific name
        await _runner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = "TestRoom",
            Scene = scene,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });
        PlayerElimination.OnElimination += RespawnPlayer;
    }

    private void OnGUI()
    {
        if (_runner == null)
        {
            if (GUI.Button(new Rect(0, 0, 200, 40), "Host"))
            {
                StartGame(GameMode.Host);
            }
            if (GUI.Button(new Rect(0, 40, 200, 40), "Join"))
            {
                StartGame(GameMode.Client);
            }
        }
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer)
        {
            // Create a unique position for the player
            int teamIndex = TeamManager.GetNextTeam();
            SpawnPlayer(player, teamIndex);
        }
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (_spawnedCharacters.TryGetValue(player, out NetworkObject networkObject))
        {
            Team playerTeam = networkObject.GetComponent<Team>();
            TeamManager.RemoveTeamMember(playerTeam.TeamIndex);
            runner.Despawn(networkObject);
            _spawnedCharacters.Remove(player);
            objToPlayer.Remove(networkObject);
        }
    }

    void SpawnPlayer(PlayerRef player, int teamIndex)
    {
        Vector3 spawnPosition = TeamManager.GetTeamSpawnPoint(teamIndex).position;
        NetworkObject networkPlayerObject = _runner.Spawn(_playerPrefab, spawnPosition, Quaternion.identity, player);
        // Keep track of the player avatars for easy access
        _spawnedCharacters.Add(player, networkPlayerObject);
        objToPlayer.Add(networkPlayerObject, player);
        Team playerTeam = networkPlayerObject.GetComponent<Team>();
        playerTeam.SetTeam(teamIndex);
        TeamManager.AddTeamMember(teamIndex);
        // Make player temporarily invincible
        networkPlayerObject.GetComponent<PlayerElimination>().Invincible = true;
    }

    void RespawnPlayer(NetworkObject playerObj, int teamIndex)
    {
        if (_runner.IsServer)
        {
            print("Respawning");
            PlayerRef player = objToPlayer[playerObj];
            _spawnedCharacters.Remove(player);
            objToPlayer.Remove(playerObj);
            SpawnPlayer(player, teamIndex);
        }
    }

    private void Update()
    {
        spacebar |= Input.GetKeyDown(KeyCode.Space);
        shift |= Input.GetKeyDown(KeyCode.LeftShift);
        up |= Input.GetKeyDown(KeyCode.UpArrow);
        down |= Input.GetKeyDown(KeyCode.DownArrow);
        left |= Input.GetKeyDown(KeyCode.LeftArrow);
        right |= Input.GetKeyDown(KeyCode.RightArrow);

    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        var data = new NetworkInputData();

        if (Input.GetKey(KeyCode.W))
            data.direction += Vector3.forward;

        if (Input.GetKey(KeyCode.S))
            data.direction += Vector3.back;

        if (Input.GetKey(KeyCode.A))
            data.direction += Vector3.left;

        if (Input.GetKey(KeyCode.D))
            data.direction += Vector3.right;

        data.buttons.Set(NetworkInputData.SPACEBAR, spacebar);
        data.buttons.Set(NetworkInputData.SHIFT, shift);
        data.buttons.Set(NetworkInputData.UP, up);
        data.buttons.Set(NetworkInputData.DOWN, down);
        data.buttons.Set(NetworkInputData.LEFT, left);
        data.buttons.Set(NetworkInputData.RIGHT, right);
        spacebar = false;
        shift = false;
        up = false;
        down = false;
        left = false;
        right = false;

        input.Set(data);
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
}
