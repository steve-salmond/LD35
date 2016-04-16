using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour
{

    // Properties
    // -----------------------------------------------------

    /** The floor that this door belongs to. */
    public Floor Floor
    { get { return Room.Floor; } }

    /** The room that this door belongs to. */
    public Room Room
    {
        get
        {
            if (!_room)
                _room = GetComponentInParent<Room>();

            return _room;
        }
    }

    /** Whether door is locked. */
    public bool Locked
    {
        get { return _locked; }

        set
        {
            if (_locked == value)
                return;

            _locked = value;

            // Show or hide the door.
            gameObject.SetActive(value);

            // Spawn lock/unlock effect.
            if (_updated)
                ObjectPool.GetAt(_locked ? LockEffectPrefab : UnlockEffectPrefab, transform);
        }
    }


    [Header("Effects")]

    /** Unlock effect. */
    public GameObject UnlockEffectPrefab;

    /** Lock effect. */
    public GameObject LockEffectPrefab;



    // Members
    // -----------------------------------------------------

    /** The room that this door belongs to. */
    private Room _room;

    /** Whether door has been updated before. */
    private bool _updated;

    /** Whether door is locked. */
    private bool _locked = true;

    /** Collider buffer used to detect adjacent corridors. */
    private Collider[] _corridors = new Collider[1];


    // Public Methods
    // -----------------------------------------------------

    /** Initialize the floor state. */
    public void InitState()
    {
    }

    /** Update the floor state. */
    public void UpdateState()
    {
        // Check for adjacent corridor colliders.
        var n = Physics.OverlapSphereNonAlloc(transform.position, 5, _corridors, Floor.CorridorMask);

        // Lock/unlock door accordingly.
        Locked = n <= 0 || Room.Moving;

        // We've now updated state.
        _updated = true;
    }

}
