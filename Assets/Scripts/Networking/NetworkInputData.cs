using Fusion;
using UnityEngine;

public struct NetworkInputData : INetworkInput
{
    public const byte MOUSEBUTTON0 = 1;
    public const byte SHIFT = 2;

    public NetworkButtons buttons;
    public Vector3 direction;
}