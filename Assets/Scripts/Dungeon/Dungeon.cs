using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Dungeon : MonoBehaviour
{

    // Properties
    // -----------------------------------------------------

    /** List of rooms in the dungeon. */
    public List<Room> Rooms
    { get; private set; }

    /** List of corridors in the dungeon. */
    public List<Corridor> Corridors
    { get; private set; }

    /** Layer mask for detecting corridors. */
    public LayerMask CorridorMask;


    // Unity Methods
    // -----------------------------------------------------

    /** Initialization. */
    private void Start()
    {
        InitState();
        UpdateState();
    }


    // Public Methods
    // -----------------------------------------------------

    /** Initialize the dungeon state. */
    public void InitState()
    {
        Rooms = new List<Room>(GetComponentsInChildren<Room>());
        Corridors = new List<Corridor>(GetComponentsInChildren<Corridor>());

        foreach (var room in Rooms)
            room.InitState();
        foreach (var corridor in Corridors)
            corridor.InitState();
    }


    /** Update the dungeon state. */
    public void UpdateState()
    {
        foreach (var corridor in Corridors)
            corridor.UpdateState();
        foreach (var room in Rooms)
            room.UpdateState();
    }

}
