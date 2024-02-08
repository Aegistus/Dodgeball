using Fusion;
using UnityEngine;

[RequireComponent(typeof(NetworkTransform))]
public class Ball : NetworkBehaviour
{
    [SerializeField] float throwSpeed = 5f;

    [HideInInspector][Networked] public bool PickedUp { get; set; } = false;
    [Networked] private TickTimer Life { get; set; }
    [HideInInspector][Networked] public bool Thrown { get; set; } = false;

    public void Throw()
    {
        Life = TickTimer.CreateFromSeconds(Runner, 5f);
        Thrown = true;
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
                transform.position += throwSpeed * transform.forward * Runner.DeltaTime;
            }
        }
    }
}