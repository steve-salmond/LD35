using UnityEngine;
using System.Collections;

public class PhysicsManager : Singleton<PhysicsManager>
{

    // Static Properties
    // -----------------------------------------------------

    /** Normalization factor for computing a per-frame force. */
    public static float NormalizationFactor
    { get { return 0.02f / Time.fixedDeltaTime; } }


    // Public Static Methods
    // -----------------------------------------------------

    /** Scales a per-frame force so that it produces consistent results even if fixed delta time varies. */
    public static float Normalized(float force)
        { return force * NormalizationFactor; }

    /** Scales a per-frame force so that it produces consistent results even if fixed delta time varies. */
    public static Vector3 Normalized(Vector3 force)
    { return force * NormalizationFactor; }

}
