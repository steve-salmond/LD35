using UnityEngine;
using System.Collections;
using DG.Tweening;

public class UseableRoomRotator : UseableBehaviour
{

    // Properties
    // -----------------------------------------------------

    /** The Room that rotator belongs to. */
    public Room Room
    {
        get
        {
            var t = transform;
            while (_room == null && t != null)
            {
                _room = t.GetComponentInParent<Room>();
                t = t.parent;
            }

            return _room;
        }
    }

    /** Optional offset, used to rotate adjacent rooms. */
    public Vector2 Offset;

    /** Euler angles to rotate by. */
    public Vector3 Angle;

    /** Duration of the rotation. */
    public float Duration = 1;

    /** Whether to rotate in world space (or local space). */
    public bool WorldSpace = true;
    
    /** Rotation mode. */
    public RotateMode Mode;

    [Header("Effects")]

    /** Effect to play when rotator works. */
    public GameObject SuccessEffectPrefab;

    /** Effect to play when rotator fails to work. */
    public GameObject FailureEffectPrefab;


    // Members
    // -----------------------------------------------------

    /** The current room. */
    private Room _room;

    /** The target room. */
    private Room _target;


    // Public Methods
    // -----------------------------------------------------

    /** Use this object. */
    public override void Use(PlayerControllable pc)
    {
        _target = Room.GetRelative(Offset);
        if (_target == null || _target.Moving)
        {
            ObjectPool.GetAt(FailureEffectPrefab, transform);
        }
        else
        {
            ObjectPool.GetAt(SuccessEffectPrefab, transform);

            _target.SetMoving(true);

            CameraController.Instance.OrthoSizeExtra = 10;

            if (WorldSpace)
                _target.transform.DORotate(Angle, Duration, Mode).SetDelay(0.5f).OnComplete(OnComplete);
            else
                _target.transform.DOLocalRotate(Angle, Duration, Mode).SetDelay(0.5f).OnComplete(OnComplete);

            CameraEffects.Instance.Shake(0.1f, Duration * 1.5f);
        }
    }


    // Private Methods
    // -----------------------------------------------------

    /** Update the game state when rotation completes. */
    private void OnComplete()
    {
        _target.SetMoving(false);

        CameraController.Instance.OrthoSizeExtra = 0;
    }

}
