using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System;

public class PlayerElimination : NetworkBehaviour
{
    // this network object and team index
    public static event Action<NetworkObject, int> OnElimination;
    public static float invincibilityTime = 3f;
    public static float invincibilityTransparentInterval = .2f;
    public static float invincibilityOpaqueInterval = .2f;
    public static float invincibilityAlphaAmount = .1f;

    [Networked] public bool Alive { get; set; } = true;
    [Networked] public bool Invincible { get; set; } = false;

    ChangeDetector changeDetector;
    TickTimer invincibilityTimer;
    Renderer playerRenderer;

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
        playerRenderer = GetComponentInChildren<Renderer>();
    }

    public override void Render()
    {
        foreach (var change in changeDetector.DetectChanges(this))
        {
            if (change == nameof(Alive))
            {
                RemovePlayer();
            }
            else if (change == nameof(Invincible))
            {
                if (Invincible == true)
                {
                    MakeInvincible();
                }
                else
                {
                    RemoveInvincibility();
                }
            }
        }
        if (invincibilityTimer.Expired(Runner))
        {
            Invincible = false;
        }
    }

    IEnumerator InvincibilityFlashCoroutine()
    {
        Color c = playerRenderer.material.color;
        while (Invincible)
        {
            c.a = invincibilityAlphaAmount;
            playerRenderer.material.color = c;
            yield return new WaitForSeconds(invincibilityTransparentInterval);
            c.a = 1f;
            playerRenderer.material.color = c;
            print("TEST");
            yield return new WaitForSeconds(invincibilityOpaqueInterval);
        }
    }

    void RemovePlayer()
    {
        Team team = GetComponent<Team>();
        TeamManager.RemoveTeamMember(team.TeamIndex);
        GameObject particleObject = PoolManager.SpawnObject("Explosion_Particle", transform.position, transform.rotation);
        Renderer particleRend = particleObject.GetComponent<Renderer>();
        if (team.LocalPlayer)
        {
            particleRend.material = TeamManager.LocalPlayerColor;
        }
        else
        {
            particleRend.material = TeamManager.GetTeamColor(team.TeamIndex);
        }
        NetworkObject netObj = GetComponent<NetworkObject>();
        OnElimination?.Invoke(netObj, team.TeamIndex);
        Runner.Despawn(netObj);
    }

    void MakeInvincible()
    {
        print("INVINCIBLE");
        invincibilityTimer = TickTimer.CreateFromSeconds(Runner, invincibilityTime);
        Collider collider = GetComponent<Collider>();
        collider.enabled = false;
        StartCoroutine(InvincibilityFlashCoroutine());
    }

    void RemoveInvincibility()
    {
        print("NOT INVINCIBLE");
        Color c = playerRenderer.material.color;
        c.a = 1;
        playerRenderer.material.color = c;
        Collider collider = GetComponent<Collider>();
        collider.enabled = true;
    }
    
}
