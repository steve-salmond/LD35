using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Room : MonoBehaviour
{

    // Properties
    // -----------------------------------------------------

    /** Size of the room, in units. */
    public Vector3 Extents;
    
    /** The dungeon that this room belongs to. */
    public Dungeon Dungeon
    {
        get
        {
            if (!_dungeon)
                _dungeon = GetComponentInParent<Dungeon>();

            return _dungeon;
        }
    }

    /** List of doors in the room. */
    public List<Door> Doors
    { get; private set; }

    /** List of adjacent corridors. */
    public List<Corridor> Corridors
    { get; private set; }

    /** Whether room is moving. */
    public bool Moving
    { get; private set; }


    // Members
    // -----------------------------------------------------

    /** The dungeon that this room belongs to. */
    private Dungeon _dungeon;


    // Public Methods
    // -----------------------------------------------------

    /** Indicates whether this room is moving or not. */
    public void SetMoving(bool value)
    {
        Moving = value;
        Dungeon.UpdateState();
    }

    /** Initialize the dungeon state. */
    public void InitState()
    {
        Doors = new List<Door>(GetComponentsInChildren<Door>());
        foreach (var door in Doors)
            door.InitState();

        // Locate corridors that are adjacent to this room.
        var colliders = Physics.OverlapBox(transform.position, Extents * 0.5f, Quaternion.identity, Dungeon.CorridorMask);
        Corridors = colliders.Select(c => c.GetComponentInParent<Corridor>()).Where(c => c != null).ToList();
        foreach (var corridor in Corridors)
            corridor.Rooms.Add(this);
    }

    /** Update the dungeon state. */
    public void UpdateState()
    {
        foreach (var door in Doors)
            door.UpdateState();
    }
}
