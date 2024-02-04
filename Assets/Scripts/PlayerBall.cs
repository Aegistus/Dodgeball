using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class PlayerBall : NetworkBehaviour
{
    //[SerializeField] Ball ballPrefab;
    [SerializeField] float pickupRadius = 10f;
    [SerializeField] LayerMask ballLayer;

    public Ball CurrentBall { get; private set; }

    Collider[] pickupCheckResults = new Collider[20];

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
        CurrentBall.transform.SetParent(transform);
        return true;
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
                    CurrentBall.transform.SetParent(null);
                    CurrentBall.Throw();
                    CurrentBall = null;
                    //Runner.Spawn(ballPrefab,
                    //    transform.position + transform.forward, Quaternion.LookRotation(transform.forward),
                    //    Object.InputAuthority
                    //);
                }
            }
        }
    }
}
