using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Floor : Procedural
{

    // Properties
    // -----------------------------------------------------

    /** Dungeon that this floor belongs to. */
    public Dungeon Dungeon
    {
        get
        {
            if (!_dungeon)
                _dungeon = GetComponentInParent<Dungeon>();

            return _dungeon;
        }
    }

    /** The depth of this floor. */
    public int Depth
    { get; private set; }

    /** Previous floor in the dungeon. */
    public Floor Previous
    { get; private set; }

    /** Next floor in the dungeon. */
    public Floor Next
    { get; private set; }

    /** List of rooms in the floor. */
    public List<Room> Rooms
    { get; private set; }

    /** List of corridors in the floor. */
    public List<Corridor> Corridors
    { get; private set; }


    [Header("Generation")]

    /** Number of room rows for a given depth. */
    public AnimationCurve RowsForDepth;

    /** Number of room columns for a given depth. */
    public AnimationCurve ColumnsForDepth;

    /** The set of possible entrance room prefabs. */
    public List<Room> EntranceRoomPrefabs;

    /** The set of possible exit room prefabs. */
    public List<Room> ExitRoomPrefabs;

    /** The set of possible room prefabs. */
    public List<Room> RoomPrefabs;

    /** Offset for rooms in each dimension. */
    public Vector2 RoomOffset;


    [Header("Setup")]

    /** Layer mask for detecting corridors. */
    public LayerMask CorridorMask;


    // Members
    // -----------------------------------------------------

    /** The dungeon that this floor belongs to. */
    private Dungeon _dungeon;


    // Public Methods
    // -----------------------------------------------------

    /** Generate this floor. */
    public override void Generate(int seed)
    {
        base.Generate(seed);

        Depth = Dungeon.Floors.IndexOf(this);
        Rooms = new List<Room>();

        // Determine how big to make this floor.
        var rows = Mathf.RoundToInt(RowsForDepth.Evaluate(Depth));
        var cols = Mathf.RoundToInt(ColumnsForDepth.Evaluate(Depth));

        // Pick a random entrance/exit room location.
        var entrance = Random.Range(0, cols);
        var exit = Random.Range(0, cols);

        // Populate the rooms.
        for (var row = 0; row < rows; row++)
            for (var col = 0; col < cols; col++)
            {
                var prefab = RoomPrefabs[Random.Range(0, RoomPrefabs.Count)];
                if (row == 0 && col == entrance)
                    prefab = EntranceRoomPrefabs[Random.Range(0, EntranceRoomPrefabs.Count)];
                if (row == rows - 1 && col == exit)
                    prefab = ExitRoomPrefabs[Random.Range(0, ExitRoomPrefabs.Count)];

                GenerateRoom(row, col, prefab);
            }

        // TODO: Generate corridors.

        // Set up the rooms/corridors.
        InitState();
        UpdateState();
    }

    /** Set previous floor. */
    public void SetPrevious(Floor floor)
    { Previous = floor;  }

    /** Set next floor. */
    public void SetNext(Floor floor)
    { Next = floor; }

    /** Update the floor state. */
    public void UpdateState()
    {
        foreach (var corridor in Corridors)
            corridor.UpdateState();
        foreach (var room in Rooms)
            room.UpdateState();
    }


    // Public Methods
    // -----------------------------------------------------

    /** Generate a room. */
    private void GenerateRoom(int row, int col, Room prefab)
    {
        var room = Instantiate(prefab);
        Rooms.Add(room);
        room.transform.position = new Vector3(RoomOffset.x * col, -RoomOffset.y * row);
        room.transform.parent = transform;
        room.Generate(Random.NextInteger());
    }

    /** Initialize the floor state. */
    private void InitState()
    {
        Rooms = new List<Room>(GetComponentsInChildren<Room>());
        Corridors = new List<Corridor>(GetComponentsInChildren<Corridor>());

        foreach (var room in Rooms)
            room.InitState();
        foreach (var corridor in Corridors)
            corridor.InitState();
    }


}
