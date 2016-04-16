using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Corridor : MonoBehaviour
{

    // Properties
    // -----------------------------------------------------

    /** Length of the corridor. */
    public float Length = 10;

    /** The corridor's walls. */
    public GameObject Walls;

    /** Layer mask used when resizing colliders. */
    public LayerMask ResizeMask;

    /** List of adjacent rooms. */
    public List<Room> Rooms
    { get; private set; }

    /** The floor that this corridor belongs to. */
    public Floor Floor
    {
        get
        {
            if (!_floor)
                _floor = GetComponentInParent<Floor>();

            return _floor;
        }
    }

    /** Whether corridor is moving. */
    public bool Moving
    { get; private set; }


    // Members
    // -----------------------------------------------------

    /** The floor that this corridor belongs to. */
    private Floor _floor;

    /** The set of wall colliders in this corridor. */
    private BoxCollider[] _colliders;


    // Unity Methods
    // -----------------------------------------------------

    /** Preinitialization. */
    private void Awake()
    {
        Rooms = new List<Room>();

        _colliders = Walls.GetComponentsInChildren<BoxCollider>();
    }

    /** Physics update. */
    private void FixedUpdate()
    {
        if (!Moving)
            return;

        foreach (var collider in _colliders)
            ResizeCollider(collider);
    }


    // Public Methods
    // -----------------------------------------------------

    /** Initialize the floor state. */
    public void InitState()
    {
    }

    /** Update the floor state. */
    public void UpdateState()
    {
        // Corridor is moving if it's next to a moving room.
        Moving = Rooms.Exists(r => r.Moving);
    }

    // Private Methods
    // -----------------------------------------------------

    /** Update a wall collider so it doesn't overlap adjacent rooms. */
    private void ResizeCollider(BoxCollider collider)
    {
        var t = collider.transform;
        var left = t.position;
        var right = t.position;

        RaycastHit hit;
        if (Physics.Raycast(t.position, -t.right, out hit, Length * 0.5f, ResizeMask))
            left = hit.point;
        if (Physics.Raycast(t.position, t.right, out hit, Length * 0.5f, ResizeMask))
            right = hit.point;

        var leftLocal = t.InverseTransformPoint(left);
        var rightLocal = t.InverseTransformPoint(right);

        var midLocal = t.InverseTransformPoint((left + right) * 0.5f);
        midLocal.y = 0;
        midLocal.z = 0;

        var sizeLocal = collider.size;
        sizeLocal.x = rightLocal.x - leftLocal.x;

        collider.center = midLocal;
        collider.size = sizeLocal;
    }

}
