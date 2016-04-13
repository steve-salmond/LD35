using UnityEngine;
using System.Collections;

public class Framerate : MonoBehaviour
{

    /** Target framerate. */
    public int Target;

	/** Initialization. */
	private void Awake()
    {
        Application.targetFrameRate = Target;
	}
}
