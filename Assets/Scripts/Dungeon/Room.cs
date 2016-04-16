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

        Doors = new List<Door>(GetComponentsInChildren<Door>());
    }

    /** Indicates whether this room is moving or not. */
    public void SetMoving(bool value)
    {
        Moving = value;
        Floor.UpdateState();
    }

    /** Update the floor state. */
    public void UpdateState()
    {
        foreach (var door in Doors)
            door.UpdateState();
    }
}
