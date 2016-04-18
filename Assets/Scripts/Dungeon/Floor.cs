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

    /** Seed to use for this floor. */
    public int Seed;

    /** The name of this floor. */
    public string Name
    { get { return "FLOOR " + Depth; } }

    /** The depth of this floor. */
    public int Depth
    { get; private set; }

    /** Previous floor in the dungeon. */
    public Floor Previous
    { get; private set; }

    /** Next floor in the dungeon. */
    public Floor Next
    { get; private set; }

    /** The entrance room for this floor. */
    public Room EntranceRoom
    { get; private set; }

    /** The exit room for this floor. */
    public Room ExitRoom
    { get; private set; }

    /** List of rooms in the floor. */
    public List<Room> Rooms
    { get; private set; }

    /** Rooms indexed by address. */
    public List<List<Room>> Grid
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

        Depth = Dungeon.Floors.IndexOf(this) + 1;
        gameObject.name = "Floor " + Depth;

        // Populate room structures.
        Rooms = new List<Room>();
        Grid = new List<List<Room>>();

        // Determine how big to make this floor.
        var rowsForDepth = RowsForDepth.Evaluate(Depth);
        var colsForDepth = ColumnsForDepth.Evaluate(Depth);
        var rows = Mathf.RoundToInt(rowsForDepth);
        var cols = Mathf.RoundToInt(colsForDepth);

        // Pick a random entrance/exit room location.
        var entrance = Random.Range(0, cols);
        var exit = Random.Range(0, cols);

        // Populate the rooms.
        for (var row = 0; row < rows; row++)
        {
            var columns = new List<Room>();
            Grid.Add(columns);

            for (var col = 0; col < cols; col++)
            {
                var isEntrance = row == 0 && col == entrance;
                var isExit = row == rows - 1 && col == exit;
                var prefab = RoomPrefabs[Random.Range(0, RoomPrefabs.Count)];
                if (isEntrance)
                    prefab = EntranceRoomPrefabs[Random.Range(0, EntranceRoomPrefabs.Count)];
                else if (isExit)
                    prefab = ExitRoomPrefabs[Random.Range(0, ExitRoomPrefabs.Count)];

                var room = GenerateRoom(row, col, prefab, isEntrance, isExit);

                if (isEntrance)
                    EntranceRoom = room;
                else if (isExit)
                    ExitRoom = room;

                // Don't allow the entrance room to be rotated, since player spawns there.
                if (isEntrance)
                    room.transform.rotation = Quaternion.identity;

                // Add room to floor.
                Rooms.Add(room);
                columns.Add(room);
            }

            // Randomly set one room upright in each row.
            columns[Random.Range(0, cols)].transform.rotation = Quaternion.identity;
        }
    }

    /** Set previous floor. */
    public void SetPrevious(Floor floor)
    { Previous = floor;  }

    /** Set next floor. */
    public void SetNext(Floor floor)
    { Next = floor; }

    /** Locate room at the given address. */
    public Room GetRoom(int row, int col)
    {
        if (row < 0 || row >= Grid.Count)
            return null;

        var cols = Grid[row];
        if (col < 0 || col >= cols.Count)
            return null;

        return cols[col];
    }


    // Private Methods
    // -----------------------------------------------------

    /** Generate a room. */
    private Room GenerateRoom(int row, int col, Room prefab, bool isEntrance, bool isExit)
    {
        var room = Instantiate(prefab);
        room.transform.position = new Vector3(RoomOffset.x * col, -RoomOffset.y * row);
        room.transform.rotation = Quaternion.Euler(0, 0, 90 * Random.Range(0, 4));
        room.transform.parent = transform;
        room.SetAddress(row, col);
        room.SetEntrance(isEntrance);
        room.SetExit(isExit);
        room.Generate(Random.NextInteger());
        return room;
    }


}
