using UnityEngine;
using System.Collections;
using DG.Tweening;

public class UseableRoomRotator : UseableBehaviour
{
    /** The Room to rotate. */
    public Room Room;

    /** Euler angles to rotate by. */
    public Vector3 Angle;

    /** Duration of the rotation. */
    public float Duration = 1;

    /** Whether to rotate in world space (or local space). */
    public bool WorldSpace = true;
    
    /** Rotation mode. */
    public RotateMode Mode;

    /** Use this object. */
    public override void Use()
    {
        if (Room.Moving)
            return;

        Room.SetMoving(true);

        if (WorldSpace)
            Room.transform.DORotate(Angle, Duration, Mode).OnComplete(OnComplete);
        else
            Room.transform.DOLocalRotate(Angle, Duration, Mode).OnComplete(OnComplete);
    }

    /** Update the game state when rotation completes. */
    private void OnComplete()
    {
        Room.SetMoving(false);
    }

}
