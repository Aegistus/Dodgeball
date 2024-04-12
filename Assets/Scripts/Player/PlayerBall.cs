using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class PlayerBall : NetworkBehaviour
{
    [SerializeField] Transform ballHolder;
    [SerializeField] float pickupRadius = 10f;
    [SerializeField] LayerMask ballLayer;
    [SerializeField] float holdRadius = 2f;
    [SerializeField] float aimTurnSpeed = 5f;

    [Networked] public Ball CurrentBall { get; set; }
    [Networked] public float CurrentAimAngle { get; set; } = 0;

    float defaultAimAngle;
    Collider[] pickupCheckResults = new Collider[20];
    Team playerTeam;
    ChangeDetector ballChangeDetector;

    public override void Spawned()
    {
        playerTeam = GetComponent<Team>();
        // set starting aim based on team
        defaultAimAngle = playerTeam.TeamIndex == 1 ? 90 : 270;
        CurrentAimAngle = defaultAimAngle;
        ballChangeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
    }

    public override void Render()
    {
        foreach (var change in ballChangeDetector.DetectChanges(this))
        {
            if (change == nameof(CurrentBall))
            {
                if (CurrentBall != null && playerTeam.LocalPlayer)
                {
                    Team ballsTeam = CurrentBall.GetComponent<Team>();
                    ballsTeam.LocalPlayer = true;
                    ballsTeam.UpdateTeamColor();
                }
            }
        }
    }

    public bool TryPickupBall()
    {
        if (CurrentBall != null)
        {
            return false;
        }
        int hitCount = Physics.OverlapSphereNonAlloc(transform.position, pickupRadius, pickupCheckResults, ballLayer);
        List<Ball> inRange = new List<Ball>();
        for (int i = 0; i < hitCount; i++)
        {
            Ball ball = pickupCheckResults[i].GetComponentInParent<Ball>();
            if (ball != null && !ball.Thrown && !ball.PickedUp)
            {
                inRange.Add(ball);
            }
        }
        if (inRange.Count == 0)
        {
            return false;
        }
        Ball closestBall = null;
        float closestDist = float.MaxValue;
        for (int i = 0; i < inRange.Count; i++)
        {
            float dist = Vector3.Distance(transform.position, inRange[i].transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                closestBall = inRange[i];
            }
        }
        closestBall.PickedUp = true;
        CurrentBall = closestBall;
        CurrentBall.transform.SetParent(ballHolder);
        CurrentBall.transform.localPosition = Vector3.forward * holdRadius;
        Team ballsTeam = CurrentBall.GetComponent<Team>();
        if (playerTeam.LocalPlayer)
        {
            ballsTeam.LocalPlayer = true;
        }
        ballsTeam.SetTeam(playerTeam.TeamIndex);
        return true;
    }

    void ThrowBall()
    {
        CurrentBall.transform.SetParent(null, true);
        CurrentBall.Throw();
        CurrentBall = null;
    }

    void DetermineInputDirection(NetworkInputData data)
    {
        CurrentAimAngle = CurrentAimAngle > 360 ? CurrentAimAngle - 360 : CurrentAimAngle;
        CurrentAimAngle = CurrentAimAngle < 0 ? CurrentAimAngle + 360 : CurrentAimAngle;
        if (data.buttons.IsSet(NetworkInputData.UP))
        {
            CurrentAimAngle += playerTeam.TeamIndex == 1 ? -aimTurnSpeed * Time.deltaTime : aimTurnSpeed * Time.deltaTime;
        }
        else if (data.buttons.IsSet(NetworkInputData.DOWN))
        {
            CurrentAimAngle += playerTeam.TeamIndex == 1 ? aimTurnSpeed * Time.deltaTime : -aimTurnSpeed * Time.deltaTime;
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data) && HasStateAuthority)
        {
            if (data.buttons.IsSet(NetworkInputData.SPACEBAR))
            {
                if (CurrentBall == null)
                {
                    TryPickupBall();
                }
                else
                {
                    ThrowBall();
                }
            }
            DetermineInputDirection(data);
        }
        if (CurrentBall != null)
        {
            CurrentBall.transform.localPosition = ballHolder.forward * holdRadius;
            CurrentBall.transform.localRotation = Quaternion.identity;
            CurrentBall.transform.RotateAround(transform.position, Vector3.up, CurrentAimAngle);
        }
    }
}
