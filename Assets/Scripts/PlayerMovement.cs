using Fusion;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] float maxMoveSpeed = 10f;
    [SerializeField] float acceleration = 10f;
    [SerializeField] float dodgeSpeed = 20f;

    Vector3 currentVelocity;

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            // move player
            data.direction.Normalize();
            currentVelocity = maxMoveSpeed * Runner.DeltaTime * data.direction;

            transform.position += currentVelocity;

            if (data.buttons.IsSet(NetworkInputData.SHIFT))
            {
                currentVelocity *= dodgeSpeed;
            }
        }
    }
}