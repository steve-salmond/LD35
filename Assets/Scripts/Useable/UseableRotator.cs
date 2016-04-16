using UnityEngine;
using System.Collections;
using DG.Tweening;

public class UseableRotator : UseableBehaviour
{
    /** The transform to rotate. */
    public Transform Target;

    /** Euler angles to rotate by. */
    public Vector3 Angle;

    /** Duration of the rotation. */
    public float Duration = 1;

    /** Whether to rotate in world space (or local space). */
    public bool WorldSpace = true;
    
    /** Rotation mode. */
    public RotateMode Mode;


    public override void Use()
    {
        if (DOTween.IsTweening(Target))
            return;

        if (WorldSpace)
            Target.DORotate(Angle, Duration, Mode);
        else
            Target.DOLocalRotate(Angle, Duration, Mode);
    }

}
