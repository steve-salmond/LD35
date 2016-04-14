using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ProceduralLevel2D : MonoBehaviour
{

    // Properties
    // -----------------------------------------------------

    [Header("Components")]

    /** 
     * A transform that indicates the level's 'center of attention'. 
     * This will often be set to track along with the player.
     */
    public Transform Locator;

    [Header("Cells")]

    /** Size of each level cell. */
    public Vector2 CellSize;

    /** Prefab to use when creating a cell. */
    public ProceduralCell2D CellPrefab;

    /** Number of cells to display around the player in each axis. */
    public int CellVisibleMargin = 2;

    /** Maximum number of inactive cells to have instantiated. */
    public int MaxInactiveCells = 32;


    // Members
    // -----------------------------------------------------

    /** The collection of known cells. */
    private List<ProceduralCell2D> Cells;

    /** Player's current cell address. */
    private ProceduralCellAddress Player;

    /** Number of inactive cells. */
    private int InactiveCells;

    /** Random number generator for cell address columns. */
    private Numbers.Random RandomCol = new Numbers.Random();

    /** Random number generator for cell address columns. */
    private Numbers.Random RandomRow = new Numbers.Random();


    // Unity Implementation
    // -----------------------------------------------------

    /** Preinitialization. */
    private void Awake()
    {
        // Create tracking structures.
        Cells = new List<ProceduralCell2D>();
    
        // Perform an initial update.
        Update();
    }

    /** Update the level each frame. */
    private void Update()
    {
        // Get the locator's current cell address.
        var p = Locator.transform.position;
        int x = Mathf.RoundToInt(p.x / CellSize.x);
        int y = Mathf.RoundToInt(p.y / CellSize.y);
        Player = new ProceduralCellAddress(x, y);

        // Ensure all cells around the player exist.
        var xMin = x - CellVisibleMargin;
        var xMax = x + CellVisibleMargin;
        var yMin = y - CellVisibleMargin;
        var yMax = y + CellVisibleMargin;
        for (var row = yMin; row <= yMax; row++)
            for (var col = xMin; col <= xMax; col++)
                EnsureCellExists(new ProceduralCellAddress(col, row));

        // Update each cell's active state.
        InactiveCells = 0;
        for (var i = 0; i < Cells.Count; i++)
        {
            var active = UpdateCell(Cells[i]);
            if (!active)
                InactiveCells++;
        }

        // Prune out any old cells
        if (InactiveCells > MaxInactiveCells)
            PruneInactiveCells();
    }


    // Implementation
    // -----------------------------------------------------

    /** Activate the cell at the given address. */
    private void EnsureCellExists(ProceduralCellAddress address)
    {
        // Create the cell if needed.
        if (!CellExists(address))
            CreateCell(address);
    }

    /** Create a cell at the given address. */
    private void CreateCell(ProceduralCellAddress address)
    {
        var cell = ObjectPool.GetComponent(CellPrefab);
        cell.transform.parent = transform;
        cell.transform.position = GetCellPosition(address);
        cell.Address = address;
        cell.Timestamp = Time.time;
        cell.Generate(GetCellSeed(address));

        AddCell(cell);
    }

    /** Determines if cell at the given address exists. */
    private bool CellExists(ProceduralCellAddress address)
    {
        for (var i = 0; i < Cells.Count; i++)
            if (Cells[i].Address == address)
                return true;

        return false;
    }

    /** Add a cell to the level. */
    private void AddCell(ProceduralCell2D cell)
    {
        Cells.Add(cell);
    }

    /** Remove a cell from the level. */
    private void RemoveCell(ProceduralCell2D cell)
    {
        Cells.Remove(cell);
        cell.Degenerate();

        ObjectPool.Cleanup(cell.gameObject);
    }

    /** Update a cell's status. */
    private bool UpdateCell(ProceduralCell2D cell)
    {
        // Update cell's active state based on visibility.
        var active = IsAddressVisible(cell.Address);
        cell.gameObject.SetActive(active);

        // Update cell's timestamp.
        if (active)
            cell.Timestamp = Time.time;

        return active;
    }

    /** Get rid of old inactive cells if necessary. */
    private void PruneInactiveCells()
    {
        var count = InactiveCells - MaxInactiveCells;
        var prune = Cells
            .Where(x => !x.gameObject.activeSelf)
            .OrderBy(x => x.Timestamp)
            .Take(count);

        foreach (var cell in prune)
            RemoveCell(cell);
    }

    /** Returns whether the given cell should be active when player is at a certain address. */
    private bool IsCellVisible(ProceduralCell2D cell)
    { return IsAddressVisible(cell.Address); }

    /** Returns whether the given cell should be active when player is at a certain address. */
    private bool IsAddressVisible(ProceduralCellAddress address)
    {
        var x = Player.x;
        var y = Player.y;

        var xMin = x - CellVisibleMargin;
        var xMax = x + CellVisibleMargin;
        var yMin = y - CellVisibleMargin;
        var yMax = y + CellVisibleMargin;

        return address.x >= xMin && address.x <= xMax
            && address.y >= yMin && address.y <= yMax;
    }

    /** Returns the position of a given cell address. */
    private Vector3 GetCellPosition(ProceduralCellAddress address)
        { return new Vector3(address.x * CellSize.x, address.y * CellSize.y, 0); }

    /** Returns the random seed for a given cell address. */
    private int GetCellSeed(ProceduralCellAddress address)
    {
        var col = RandomCol.SetSeed(address.x).NextInteger();
        var row = RandomRow.SetSeed(address.y).NextInteger();
        return row ^ col;
    }


}
