using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInRoomData
{
    public PlayerInRoomData parentData = null;
    public Vector3 perviousPosition = Vector3.zero;
    public RoomScript currentRoomScript = null;
    public bool hasParent = false;
}
