using Fusion;

public class Ball : NetworkBehaviour
{
    [Networked] private TickTimer Life { get; set; }

    public void Init()
    {
        Life = TickTimer.CreateFromSeconds(Runner, 5f);
    }

    public override void FixedUpdateNetwork()
    {
        if (Life.Expired(Runner))
        {
            Runner.Despawn(Object);
        }
        else
        {
            transform.position += 5 * transform.forward * Runner.DeltaTime;
        }
    }
}