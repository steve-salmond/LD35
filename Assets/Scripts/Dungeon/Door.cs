using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour
{

    // Properties
    // -----------------------------------------------------

    /** The door's blocking body. */
    public GameObject Blocker;

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

            if (_locked)
                StartCoroutine(LockRoutine());
            else
                StartCoroutine(UnlockRoutine());
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
    private Collider[] _doors = new Collider[2];


    // Public Methods
    // -----------------------------------------------------

    /** Update the floor state. */
    public void UpdateState()
    {
        // Check for adjacent doors.
        var doorMask = 1 << gameObject.layer;
        var n = Physics.OverlapSphereNonAlloc(transform.position, 5, _doors, doorMask);

        // Lock/unlock door accordingly.
        Locked = n <= 1 || Room.Moving;

        // We've now updated state.
        _updated = true;
    }

    // Private Methods
    // -----------------------------------------------------

    /** Routine to update door's locked state. */
    private IEnumerator LockRoutine()
    {
        // Wait a little bit.
        yield return 0;

        // Show the door.
        Blocker.SetActive(true);

        // Spawn lock effect.
        if (_updated)
            ObjectPool.GetAt(LockEffectPrefab, transform);
    }

    /** Routine to update door's locked state. */
    private IEnumerator UnlockRoutine()
    {
        // Wait a little bit.
        yield return 0;

        // Hide the door.
        Blocker.SetActive(false);

        // Spawn unlock effect.
        if (_updated)
            ObjectPool.GetAt(UnlockEffectPrefab, transform);
    }

}
