using UnityEngine;
using System.Collections;

public class UseableLadder : UseableBehaviour
{

    // Public Methods
    // -----------------------------------------------------

    /** Use this object. */
    public override void Use(PlayerControllable pc)
    {
        // Check if ladder is longer climbable.
        var angle = Vector3.Angle(transform.up, Vector3.up);
        if (!(Mathf.Approximately(angle, 0) || Mathf.Approximately(angle, 180)))
            return;

        pc.Climb(this);
    }

}
