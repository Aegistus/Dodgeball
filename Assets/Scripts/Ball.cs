using Fusion;
using UnityEngine;

public class Ball : NetworkBehaviour
{
    [SerializeField] float throwSpeed = 5f;
    [SerializeField] float spinMultiplier = 5f;

    [HideInInspector][Networked] public bool PickedUp { get; set; } = false;
    [Networked] private TickTimer Life { get; set; }
    [HideInInspector][Networked] public bool Thrown { get; set; } = false;
    [HideInInspector] [Networked] public bool ScheduledDestroy { get; set; } = false;

    Vector3 lastRotation;
    Vector3 spin;
    Rigidbody rb;
    Team team;
    ChangeDetector changeDetector;

    public override void Spawned()
    {
        lastRotation = transform.eulerAngles;
        rb = GetComponent<Rigidbody>();
        team = GetComponent<Team>();
        changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
        SoundManager.Instance.PlaySoundAtPosition("Ball_Respawn", transform.position);
    }

    public override void Render()
    {
        foreach (var change in changeDetector.DetectChanges(this))
        {
            if (change == nameof(ScheduledDestroy))
            {
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
                Runner.Despawn(Object);
            }
        }
    }

    public void Throw()
    {
        Life = TickTimer.CreateFromSeconds(Runner, 5f);
        rb.isKinematic = false;
        Thrown = true;
        rb.AddForce(throwSpeed * transform.forward, ForceMode.VelocityChange);
    }

    public override void FixedUpdateNetwork()
    {
        if (Thrown)
        {
            if (Life.Expired(Runner))
            {
                DestroyBall();
            }
            else
            {
                transform.eulerAngles += spinMultiplier * Runner.DeltaTime * spin;
            }
        }
        else
        {
            spin = transform.eulerAngles - lastRotation;
            lastRotation = transform.eulerAngles;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Kim's note: Tried to implement ball-bounce sound (to the wall),
        // but realized if all those balls making noises, that would be too massive.
        if (Thrown)
        {
            Ball ball = collision.gameObject.GetComponent<Ball>();
            PlayerElimination player = collision.gameObject.GetComponent<PlayerElimination>();
            Team otherTeam = collision.gameObject.GetComponentInParent<Team>();
            
            if (otherTeam != null && otherTeam.TeamIndex != team.TeamIndex)
            {
                if (ball != null)
                {
                    DestroyBall();
                }
                else if (player != null)
                {
                    player.Alive = false;
                    DestroyBall();
                    SoundManager.Instance.PlaySoundAtPosition("Ball_Explosion", transform.position);
                }
            }
        }
    }

    void DestroyBall()
    {
        ScheduledDestroy = true;
    }
}