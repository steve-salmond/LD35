using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Corridor : MonoBehaviour
{

    // Properties
    // -----------------------------------------------------

    /** List of adjacent rooms. */
    public List<Room> Rooms
    { get; private set; }

    /** The dungeon that this corridor belongs to. */
    public Dungeon Dungeon
    {
        get
        {
            if (!_dungeon)
                _dungeon = GetComponentInParent<Dungeon>();

            return _dungeon;
        }
    }

    
    // Members
    // -----------------------------------------------------

    /** The dungeon that this corridor belongs to. */
    private Dungeon _dungeon;

    /** The set of colliders in this corridor. */
    private Collider[] _colliders;


    // Unity Methods
    // -----------------------------------------------------

    /** Preinitialization. */
    private void Awake()
    {
        Rooms = new List<Room>();
        _colliders = GetComponentsInChildren<Collider>();
    }


    // Public Methods
    // -----------------------------------------------------

    /** Initialize the dungeon state. */
    public void InitState()
    {
    }

    /** Update the dungeon state. */
    public void UpdateState()
    {
        var adjacentToMovingRoom = Rooms.Exists(r => r.Moving);
        foreach (var collider in _colliders)
            collider.enabled = !adjacentToMovingRoom;
    }

}
