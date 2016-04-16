using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Room : Procedural
{

    // Properties
    // -----------------------------------------------------

    /** Size of the room, in units. */
    public Vector3 Extents;
    
    /** The floor that this room belongs to. */
    public Floor Floor
    {
        get
        {
            if (!_floor)
                _floor = GetComponentInParent<Floor>();

            return _floor;
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

    /** The floor that this room belongs to. */
    private Floor _floor;


    // Public Methods
    // -----------------------------------------------------

    /** Generate this room. */
    public override void Generate(int seed)
    {
        base.Generate(seed);
    }

    /** Indicates whether this room is moving or not. */
    public void SetMoving(bool value)
    {
        Moving = value;
        Floor.UpdateState();
    }

    /** Initialize the floor state. */
    public void InitState()
    {
        Doors = new List<Door>(GetComponentsInChildren<Door>());
        foreach (var door in Doors)
            door.InitState();

        // Locate corridors that are adjacent to this room.
        var colliders = Physics.OverlapBox(transform.position, Extents * 0.5f, Quaternion.identity, Floor.CorridorMask);
        Corridors = colliders.Select(c => c.GetComponentInParent<Corridor>()).Where(c => c != null).ToList();
        foreach (var corridor in Corridors)
            corridor.Rooms.Add(this);
    }

    /** Update the floor state. */
    public void UpdateState()
    {
        foreach (var door in Doors)
            door.UpdateState();
    }
}
