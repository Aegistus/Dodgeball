using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class PlayerElimination : NetworkBehaviour
{
    [Networked] bool nAlive { get; set; } = true;

    public bool Alive => nAlive;

    ChangeDetector changeDetector;

    public override void Spawned()
    {
        changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
    }

    public override void Render()
    {
        foreach (var change in changeDetector.DetectChanges(this))
        {
            if (change == nameof(nAlive))
            {
                RemovePlayer();
            }
        }
    }

    public void Eliminate()
    {
        nAlive = false;
        print("You have been eliminated");
    }

    void RemovePlayer()
    {
        Team team = GetComponent<Team>();
        TeamManager.RemoveTeamMember(team.TeamIndex);
        GameObject particleObject = PoolManager.SpawnObject("Explosion_Particle", transform.position, transform.rotation);
        Renderer rend = particleObject.GetComponent<Renderer>();
        rend.material = TeamManager.GetTeamColor(team.TeamIndex);
        gameObject.SetActive(false);
    }

    
}
