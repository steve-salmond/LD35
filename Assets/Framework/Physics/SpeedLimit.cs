using UnityEngine;
using System.Collections;

public class SpeedLimit : MonoBehaviour
{

    public Rigidbody Body;

    public float MaxSpeed = 15;
    
    void Awake()
    {
        if (!Body)
            Body = GetComponent<Rigidbody>();
	}
	
	void FixedUpdate()
    {
        var v = Body.velocity;
        var speed = v.magnitude;
        if (speed > MaxSpeed)
            Body.velocity = v.normalized * MaxSpeed;
    }
}
