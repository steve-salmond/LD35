using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/** A Dungeon consists of one or more Floors. */

public class Dungeon : Procedural
{

    // Properties
    // -----------------------------------------------------

    /** The set of floors in this dungeon. */
    public List<Floor> Floors
    { get; private set; }

    /** The current floor. */
    public Floor CurrentFloor
    { get { return Floors[_current]; } }


    [Header("Generation")]

    /** Difficulty factor (0 == easy, 1 = very hard). */
    public float Difficulty;
    
    /** Number of floors in the dungeon (depends on difficulty). */
    public AnimationCurve FloorCountForDifficulty;

    /** Prefab used to generate a floor. */
    public Floor FloorPrefab;


    // Members
    // -----------------------------------------------------

    /** The current floor index. */
    private int _current = 0;


    // Public Methods
    // -----------------------------------------------------

    /** Generate this dungeon. */
    public override void Generate(int seed)
    {
        base.Generate(seed);

        Floors = new List<Floor>();
        var floorCount = FloorCountForDifficulty.Evaluate(Difficulty);

        for (var i = 0; i < floorCount; i++)
        {
            var floor = Instantiate(FloorPrefab);
            Floors.Add(floor);
            floor.transform.parent = transform;
            floor.Seed = Random.NextInteger();
        }

        for (var i = 0; i < floorCount; i++)
        {
            if (i > 0)
                Floors[i].SetPrevious(Floors[i - 1]);
            if (i < (floorCount - 1))
                Floors[i].SetNext(Floors[i + 1]);
        }

        SetCurrentFloor(Floors[0]);
    }

    /** Set the current floor. */
    public bool SetCurrentFloor(Floor value)
    { return SetCurrentFloor(Floors.IndexOf(value)); }

    /** Set floor by index. */
    public bool SetCurrentFloor(int index)
    {
        if (index < 0 || index >= Floors.Count)
            return false;

        _current = index;

        var floor = Floors[index];
        if (!floor.Generated)
            floor.Generate(floor.Seed);

        for (var i = 0; i < Floors.Count; i++)
            Floors[i].gameObject.SetActive(i == index);

        return true;
    }

    /** Move to the next floor if possible. */
    public bool SetNextFloor()
        { return SetCurrentFloor(CurrentFloor.Next); }

    /** Move to the previous floor if possible. */
    public bool SetPreviousFloor()
        { return SetCurrentFloor(CurrentFloor.Previous); }

}

