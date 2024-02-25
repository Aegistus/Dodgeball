using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System;

public class PlayerElimination : NetworkBehaviour
{
    // this network object and team index
    public static event Action<NetworkObject, int> OnElimination;

    [Networked] public bool Alive { get; set; } = true;

    ChangeDetector changeDetector;

    private void Update()
    {
        if (HasInputAuthority && Application.isEditor)
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
                Alive = false;
            }
        }
    }

    public override void Spawned()
    {
        changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
    }

    public override void Render()
    {
        foreach (var change in changeDetector.DetectChanges(this))
        {
            if (change == nameof(Alive))
            {
                RemovePlayer();
            }
        }
    }

    void RemovePlayer()
    {
        Team team = GetComponent<Team>();
        TeamManager.RemoveTeamMember(team.TeamIndex);
        GameObject particleObject = PoolManager.SpawnObject("Explosion_Particle", transform.position, transform.rotation);
        Renderer rend = particleObject.GetComponent<Renderer>();
        if (team.LocalPlayer)
        {
            rend.material = TeamManager.LocalPlayerColor;
        }
        else
        {
            rend.material = TeamManager.GetTeamColor(team.TeamIndex);
        }
        NetworkObject netObj = GetComponent<NetworkObject>();
        OnElimination?.Invoke(netObj, team.TeamIndex);
        Runner.Despawn(netObj);
    }
    
}
