using UnityEngine;
using System.Collections;

public class PooledRigidbody : MonoBehaviour 
{

    // Properties
    // -----------------------------------------------------

    /** Rigidbody to clean up. */
    public Rigidbody Rigidbody;


    // Unity Methods
    // -----------------------------------------------------

    
    /** Initialization. */
    private void Start()
    {
        if (!Rigidbody)
            Rigidbody = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {

    }

    /** Disable. */
	private void OnDisable() 
	{
        if (Rigidbody)
        {
            Rigidbody.velocity = Vector3.zero;
            Rigidbody.angularVelocity = Vector3.zero;
        }
	}

}
