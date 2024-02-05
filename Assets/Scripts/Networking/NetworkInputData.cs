using Fusion;
using UnityEngine;

public struct NetworkInputData : INetworkInput
{
    public const byte SPACEBAR = 1;
    public const byte SHIFT = 2;
    public const byte UP = 3;
    public const byte DOWN = 4;
    public const byte LEFT = 5;
    public const byte RIGHT = 6;

    public NetworkButtons buttons;
    public Vector3 direction;
}