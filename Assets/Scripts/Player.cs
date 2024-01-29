using Fusion;
using UnityEngine;


public class Player : NetworkBehaviour
{
    [SerializeField] float moveSpeed = 5f;

    private NetworkCharacterController _cc;

    private void Awake()
    {
        _cc = GetComponent<NetworkCharacterController>();
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            data.direction.Normalize();
            _cc.Move(moveSpeed * Runner.DeltaTime * data.direction);
        }
    }
}