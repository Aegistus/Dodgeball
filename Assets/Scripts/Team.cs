using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Team : NetworkBehaviour
{
    [Networked] public int TeamIndex { get; private set; } = 0;

    Renderer[] rends;

    ChangeDetector changeDetector;

    public override void Spawned()
    {
        changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
        rends = GetComponentsInChildren<Renderer>();
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
        Material teamMat = TeamManager.GetTeamColor(TeamIndex);
        if (teamMat != null)
        {
            for (int i = 0; i < rends.Length; i++)
            {
                rends[i].sharedMaterial = teamMat;
            }
        }
    }
}
