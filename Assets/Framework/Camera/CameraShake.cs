using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{

    /** Magnitude of the shake effect. */
    public float Strength = 1;

    /** Duration of the shake effect. */
    public float Duration = 0.25f;

    /** Enabling. */
	private void OnEnable()
    {
        CameraEffects.Instance.Shake(Strength, Duration);
	}

}
