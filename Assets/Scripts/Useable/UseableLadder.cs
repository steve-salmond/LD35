using UnityEngine;
using System.Collections;

public class UseableLadder : UseableBehaviour
{

    // Public Methods
    // -----------------------------------------------------

    /** Use this object. */
    public override void Use(PlayerControllable pc)
    {
        pc.Climb(this);
    }

}
