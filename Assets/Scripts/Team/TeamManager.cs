using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class TeamManager : MonoBehaviour
{
    public static TeamManager Instance { get; private set; }
    public static Material LocalPlayerColor => Instance.localPlayerColor;

    [SerializeField] Material localPlayerColor;
    [SerializeField] Material[] teamColors;
    [SerializeField] BoxCollider[] teamBounds = new BoxCollider[3];
    [SerializeField] Transform[] teamSpawnPoints;

    int[] teamMemberCount = new int[teamCount];

    static readonly int teamCount = 3;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public static Material GetTeamColor(int teamIndex)
    {
        if (teamIndex < 0 || teamIndex >= Instance.teamColors.Length)
        {
            return null;
        }
        return Instance.teamColors[teamIndex];
    }

    public static BoxCollider GetTeamBounds(int teamIndex)
    {
        if (teamIndex < 0 || teamIndex >= Instance.teamBounds.Length)
        {
            return null;
        }
        return Instance.teamBounds[teamIndex];
    }

    public static Transform GetTeamSpawnPoint(int teamIndex)
    {
        if (teamIndex < 0 || teamIndex >= Instance.teamSpawnPoints.Length)
        {
            return null;
        }
        return Instance.teamSpawnPoints[teamIndex];
    }

    public static void AddTeamMember(int teamIndex)
    {
        Instance.teamMemberCount[teamIndex]++;
    }

    public static void RemoveTeamMember(int teamIndex)
    {
        Instance.teamMemberCount[teamIndex]--;
    }

    public static int GetNextTeam()
    {
        if (Instance.teamMemberCount[1] > Instance.teamMemberCount[2])
        {
            return 2;
        }
        else if (Instance.teamMemberCount[2] > Instance.teamMemberCount[2])
        {
            return 1;
        }
        else
        {
            return 1;
        }
    }
}
