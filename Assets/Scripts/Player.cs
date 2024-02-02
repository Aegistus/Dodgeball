using Fusion;
using UnityEngine;


public class Player : NetworkBehaviour
{
    [SerializeField] GameObject ballPrefab;
    [SerializeField] float moveSpeed = 5f;

    [Networked] private TickTimer ballDelay { get; set; }

    private NetworkCharacterController _cc;

    private void Awake()
    {
        _cc = GetComponent<NetworkCharacterController>();
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            // move player
            data.direction.Normalize();
            _cc.Move(moveSpeed * Runner.DeltaTime * data.direction);
            // shoot ball
            if (HasStateAuthority && ballDelay.ExpiredOrNotRunning(Runner))
            {
                if (data.buttons.IsSet(NetworkInputData.MOUSEBUTTON0))
                {
                    ballDelay = TickTimer.CreateFromSeconds(Runner, .5f);
                    Runner.Spawn(ballPrefab, transform.position + Vector3.forward, Quaternion.LookRotation(Vector3.forward), Object.InputAuthority,
                        (runner, o) => 
                        { 
                            o.GetComponent<Ball>().Init(); 
                        }
                    );
                }
            }
        }
    }
}