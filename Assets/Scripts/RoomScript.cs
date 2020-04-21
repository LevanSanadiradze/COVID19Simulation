using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomScript : MonoBehaviour
{
    public float roomInfectionChance = 0.02f;
    public float chanceToEnterRoom = 0.1f;
    public float chanceToLeaveRoom = 0.075f;

    public RoomScript[] RoomsToGo = new RoomScript[0];

    [HideInInspector] public List<Transform> MySpaces = new List<Transform>();

    void Awake()
    {
        foreach(Transform space in transform.GetChild(1))
        {
            MySpaces.Add(space);
        }
    }
}
