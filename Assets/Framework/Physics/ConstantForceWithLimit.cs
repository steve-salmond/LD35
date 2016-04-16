using UnityEngine;
using System.Collections;

public class ConstantForceWithLimit : MonoBehaviour 
{

    // Properties
    // -----------------------------------------------------

    // [Header("Properties")]

    public Vector3 Force;

    public float SpeedLimit;


    // Unity Methods
    // -----------------------------------------------------

	/** Apply force to objects in the trigger zone. */
	private void OnTriggerStay(Collider other) 
	{
        var body = other.attachedRigidbody;
        if (!body)
            return;

        // Check if object is traveling too fast in force direction.
        var f = transform.TransformDirection(Force);
        var v = Vector3.Project(body.velocity, f);
        if (v.magnitude > SpeedLimit)
            return;

        // Apply force to object.
        body.AddForce(PhysicsManager.Normalized(f));
	}

}
