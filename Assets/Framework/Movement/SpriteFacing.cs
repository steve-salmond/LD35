using UnityEngine;
using System.Collections;

public class SpriteFacing : MonoBehaviour
{
    public Rigidbody Body;

	// Use this for initialization
	void Start ()
    {
        if (!Body)
            Body = GetComponentInParent<Rigidbody>();
    }
	
	// Update is called once per frame
	void LateUpdate ()
    {
        transform.localRotation = (Body.velocity.x <= 0) 
            ? Quaternion.identity 
            : Quaternion.Euler(0, 180, 0);
	}
}
