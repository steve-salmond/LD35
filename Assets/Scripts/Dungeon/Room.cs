using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Room : ProceduralGroup
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

    /** Row address within the floor. */
    public int Row
    { get; private set; }

    /** Column address within the floor. */
    public int Col
    { get; private set; }

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

        gameObject.name = string.Format("Room(R{0},C{1})", Row, Col);
        Doors = new List<Door>(GetComponentsInChildren<Door>());
    }

    /** Set the room's address. */
    public void SetAddress(int row, int col)
    {
        Row = row;
        Col = col;
    }

    /** Locates nearby room using a relative grid offset. */
    public Room GetRelative(Vector2 offset)
    {
        var dc = Mathf.RoundToInt(offset.x);
        var dr = Mathf.RoundToInt(offset.y);
        return GetRelative(dr, dc);
    }

    /** Locates nearby room using a relative grid offset. */
    public Room GetRelative(int dr, int dc)
        { return Floor.GetRoom(Row + dr, Col + dc); }

    /** Indicates whether this room is moving or not. */
    public void SetMoving(bool value)
        { Moving = value; }

}
