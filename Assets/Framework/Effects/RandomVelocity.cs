using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]

public class RandomVelocity : MonoBehaviour
{
    public Rigidbody Body;
    public Vector2 SpeedRange = Vector2.one;
    public Vector3 AxisScale = Vector3.one;

    void Awake()
    {
        if (!Body)
            Body = GetComponent<Rigidbody>();

        if (!Body)
            return;

        var v = Random.onUnitSphere * Random.Range(SpeedRange.x, SpeedRange.y);

        v.x *= AxisScale.x;
        v.y *= AxisScale.y;
        v.z *= AxisScale.z;

        Body.velocity = v;
	}
	
	
}
