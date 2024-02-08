using Fusion;
using UnityEngine;

[RequireComponent(typeof(NetworkTransform))]
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
            currentVelocity = Vector3.Lerp(currentVelocity, data.direction * maxMoveSpeed, acceleration * Runner.DeltaTime);
            transform.position += currentVelocity * Runner.DeltaTime;

            if (data.buttons.IsSet(NetworkInputData.SHIFT))
            {
                currentVelocity *= dodgeSpeed;
            }
        }
    }
}