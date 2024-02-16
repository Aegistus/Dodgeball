using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Team : NetworkBehaviour
{
    [Networked] public int TeamIndex { get; private set; } = 0;

    public bool LocalPlayer { get; set; } = false;

    Renderer[] rends;
    ChangeDetector changeDetector;

    public override void Spawned()
    {
        changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
        rends = GetComponentsInChildren<Renderer>();
        LocalPlayer |= HasInputAuthority;
        UpdateTeamColor();
    }

    public override void Render()
    {
        foreach (var change in changeDetector.DetectChanges(this))
        {
            if (change == nameof(TeamIndex))
            {
                UpdateTeamColor();
            }
        }
    }

    public void SetTeam(int teamIndex)
    {
        TeamIndex = teamIndex;
    }

    public void UpdateTeamColor()
    {
        Material teamMat;
        if (LocalPlayer)
        {
            teamMat = TeamManager.LocalPlayerColor;
        }
        else
        {
            teamMat = TeamManager.GetTeamColor(TeamIndex);
        }
        if (teamMat != null)
        {
            for (int i = 0; i < rends.Length; i++)
            {
                rends[i].sharedMaterial = teamMat;
            }
        }
    }
}
