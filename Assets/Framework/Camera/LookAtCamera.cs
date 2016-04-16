using UnityEngine;
using System.Collections;

public class LookAtCamera : MonoBehaviour 
{

    // Unity Methods
    // -----------------------------------------------------

	/** Update. */
	private void LateUpdate() 
	{
        transform.LookAt(CameraController.Instance.Camera.transform);
	}

}
