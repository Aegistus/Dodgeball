using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class PlayerBall : NetworkBehaviour
{
    //[SerializeField] Ball ballPrefab;
    [SerializeField] Transform ballHolder;
    [SerializeField] float pickupRadius = 10f;
    [SerializeField] LayerMask ballLayer;
    [SerializeField] float holdRadius = 2f;
    [SerializeField] float aimTurnSpeed = 5f;

    public Vector3 CurrentAimDirection => currentAimDirection;
    public Vector3 TargetAimDirection => targetAimDirection;
    public Ball CurrentBall { get; private set; }

    private Vector3 currentAimDirection = Vector3.right;
    private Vector3 targetAimDirection = Vector3.right;
    Collider[] pickupCheckResults = new Collider[20];
    Team playerTeam;

    public override void Spawned()
    {
        playerTeam = GetComponent<Team>();
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
            if (ball != null && !ball.Thrown)
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
        CurrentBall.transform.localPosition = currentAimDirection * holdRadius;
        Team ballsTeam = CurrentBall.GetComponent<Team>();
        ballsTeam.SetTeam(playerTeam.TeamIndex);
        return true;
    }

    void ThrowBall()
    {
        CurrentBall.transform.LookAt(CurrentBall.transform.position + currentAimDirection);
        CurrentBall.transform.SetParent(null);
        CurrentBall.Throw();
        CurrentBall = null;
    }

    void CalculateTargetAimDirection(NetworkInputData data)
    {
        Vector3 newDirectTarget = Vector3.zero;
        if (data.buttons.IsSet(NetworkInputData.UP))
        {
            newDirectTarget += Vector3.forward;
        }
        if (data.buttons.IsSet(NetworkInputData.DOWN))
        {
            newDirectTarget += Vector3.back;
        }
        if (data.buttons.IsSet(NetworkInputData.LEFT))
        {
            newDirectTarget += Vector3.left;
        }
        if (data.buttons.IsSet(NetworkInputData.RIGHT))
        {
            newDirectTarget += Vector3.right;
        }
        newDirectTarget.Normalize();
        // if direction input changed, set it as new target direction
        if (newDirectTarget != Vector3.zero)
        {
            targetAimDirection = newDirectTarget;
            print(targetAimDirection);
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
            CalculateTargetAimDirection(data);
        }
        if (CurrentBall != null)
        {
            currentAimDirection = Vector3.Lerp(currentAimDirection, targetAimDirection, aimTurnSpeed * Runner.DeltaTime);
            currentAimDirection.Normalize();
            CurrentBall.transform.localPosition = currentAimDirection * holdRadius;
        }
        else
        {
            currentAimDirection = targetAimDirection;
        }
    }
}
