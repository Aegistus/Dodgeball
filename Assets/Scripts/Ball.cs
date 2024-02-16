using Fusion;
using UnityEngine;

public class Ball : NetworkBehaviour
{
    [SerializeField] float throwSpeed = 5f;
    [SerializeField] float spinMultiplier = 5f;

    Vector3 lastRotation;
    Vector3 spin;
    Rigidbody rb;

    [HideInInspector][Networked] public bool PickedUp { get; set; } = false;
    [Networked] private TickTimer Life { get; set; }
    [HideInInspector][Networked] public bool Thrown { get; set; } = false;

    public override void Spawned()
    {
        lastRotation = transform.eulerAngles;
        rb = GetComponent<Rigidbody>();
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
                Runner.Despawn(Object);
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
        Ball ball = collision.gameObject.GetComponent<Ball>();
        PlayerElimination player = collision.gameObject.GetComponent<PlayerElimination>();
        if (ball != null)
        {
            Runner.Despawn(Object);
        }
        else if (player != null)
        {
            player.Eliminate();
        }
    }
}