using UnityEngine;
using System.Collections;

/** Spawns objects in a disc centered on this object's transform. */

public class SpawnDisc : SpawnStochastic 
{

	public Vector2 RadiusRange;
	public Vector2 HeightRange;

    protected override Vector3 GetSpawnLocation()
    {
        var radius = Random.Range(RadiusRange.x, RadiusRange.y);
        var p = Random.insideUnitSphere;

        p.y = 0;
        p = p.normalized * radius;
        p.y = Random.Range(HeightRange.x, HeightRange.y);
        p = transform.TransformPoint(p);

        return p;
    }

}
