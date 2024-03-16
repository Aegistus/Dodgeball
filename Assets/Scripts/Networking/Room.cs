using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Room : NetworkBehaviour
{
    [Networked] public string RoomName { get; set; }
}
