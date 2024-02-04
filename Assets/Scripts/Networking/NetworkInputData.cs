using Fusion;
using UnityEngine;

public struct NetworkInputData : INetworkInput
{
    public const byte SPACEBAR = 1;
    public const byte SHIFT = 2;

    public NetworkButtons buttons;
    public Vector3 direction;
}