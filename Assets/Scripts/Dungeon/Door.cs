using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Door : MonoBehaviour
{

    // Properties
    // -----------------------------------------------------

    /** The door's blocking body. */
    public GameObject Blocker;

    /** Door's left side. */
    public GameObject Left;

    /** Door's right side. */
    public GameObject Right;

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

    /** Speed at which doors animate open/closed. */
    private float DoorSpeed = 0.25f;


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


    // Unity Methods
    // -----------------------------------------------------

    /** Update the door state. */
    private void Update()
    {
        // Check for adjacent doors.
        var doorMask = 1 << gameObject.layer;
        var n = Physics.OverlapSphereNonAlloc(transform.position, 5, _doors, doorMask);

        // Determine whether door should be locked or not.
        var locked = n == 1 || Room.Moving;
        for (var i = 0; i < n; i++)
            if (_doors[i].GetComponentInParent<Door>().Room.Moving)
                locked = true;

        // Lock/unlock door.
        Locked = locked;

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

        // Animate doors.
        Left.transform.DOKill();
        Left.transform.localScale = new Vector3(0, 2, 2);
        Left.transform.DOScaleX(2, DoorSpeed);
        Right.transform.DOKill();
        Right.transform.localScale = new Vector3(0, 2, 2);
        Right.transform.DOScaleX(2, DoorSpeed);

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

        // Animate doors.
        Left.transform.DOKill();
        Left.transform.localScale = Vector3.one * 2;
        Left.transform.DOScaleX(0, DoorSpeed);
        Right.transform.DOKill();
        Right.transform.localScale = Vector3.one * 2;
        Right.transform.DOScaleX(0, DoorSpeed);

        // Spawn unlock effect.
        if (_updated)
            ObjectPool.GetAt(UnlockEffectPrefab, transform);
    }

}
