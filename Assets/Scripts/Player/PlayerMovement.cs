using Fusion;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] float maxMoveSpeed = 10f;
    [SerializeField] float acceleration = 10f;
    [SerializeField] float dodgeSpeed = 20f;

    Vector3 currentVelocity;
    BoxCollider teamBoundCollider;
    Rigidbody rb;

    public override void Spawned()
    {
        rb = GetComponent<Rigidbody>();
    }

    public override void FixedUpdateNetwork()
    {
        if (teamBoundCollider == null)
        {
            Team team = GetComponent<Team>();
            teamBoundCollider = TeamManager.GetTeamBounds(team.TeamIndex);
        }
        if (GetInput(out NetworkInputData data))
        {
            // move player
            data.direction.Normalize();
            currentVelocity = maxMoveSpeed * Runner.DeltaTime * data.direction;

            //rb.velocity = currentVelocity;
            if (!teamBoundCollider.bounds.Contains(transform.position + currentVelocity))
            {
                return;
            }
            rb.MovePosition(transform.position + currentVelocity);

            if (data.buttons.IsSet(NetworkInputData.SHIFT))
            {
                currentVelocity *= dodgeSpeed;
            }
        }
    }
}