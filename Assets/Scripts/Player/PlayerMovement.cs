using Fusion;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] float moveSpeed = 15f;
    [SerializeField] float dodgeSpeed = 20f;
    [SerializeField] float dodgeDuration = 1f;
    [SerializeField] float dodgeCooldown = 3f;

    bool isDodging = false;

    Vector3 currentVelocity;
    BoxCollider teamBoundCollider;
    Rigidbody rb;
    TickTimer dodgeDurationTimer;
    TickTimer dodgeCooldownTimer;
    Vector3 dodgeDirection;

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
            data.direction.Normalize();

            if (data.buttons.IsSet(NetworkInputData.SHIFT) && !isDodging && dodgeCooldownTimer.ExpiredOrNotRunning(Runner))
            {
                isDodging = true;
                dodgeDurationTimer = TickTimer.CreateFromSeconds(Runner, dodgeDuration);
                dodgeCooldownTimer = TickTimer.CreateFromSeconds(Runner, dodgeCooldown);
                dodgeDirection = data.direction;
            }
            if (dodgeDurationTimer.Expired(Runner))
            {
                isDodging = false;
            }

            // move player
            currentVelocity = moveSpeed * Runner.DeltaTime * data.direction;
            // apply dodge
            if (isDodging)
            {
                currentVelocity += dodgeSpeed * Runner.DeltaTime * dodgeDirection;
            }
            // prevent player from exiting play area
            if (!teamBoundCollider.bounds.Contains(transform.position + currentVelocity))
            {
                return;
            }

            rb.MovePosition(transform.position + currentVelocity);

        }
    }
}