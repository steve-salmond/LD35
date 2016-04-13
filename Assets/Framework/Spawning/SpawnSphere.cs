using UnityEngine;
using System.Collections;

/** Spawns objects in a sphere centered on this object's transform. */

public class SpawnSphere : SpawnStochastic 
{

	public Vector2 RadiusRange;
	public Vector2 HeightRange;

    protected override Vector3 GetSpawnLocation()
    {
        var radius = Random.Range(RadiusRange.x, RadiusRange.y);
        var p = Random.insideUnitSphere;
        p = p.normalized * radius;
        p = transform.TransformPoint(p);
        return p;
    }

}
