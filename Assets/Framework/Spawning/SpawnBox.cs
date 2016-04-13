using UnityEngine;
using System.Collections;

/** Spawns objects in a box centered on this object's transform. */

public class SpawnBox : SpawnStochastic 
{
    public Vector3 Center;
	public Vector3 Extents;

    protected override Vector3 GetSpawnLocation()
    {
        var x = Random.Range(-Extents.x, Extents.x);
        var y = Random.Range(-Extents.y, Extents.y);
        var z = Random.Range(-Extents.z, Extents.z);
        var p = Center + new Vector3(x, y, z);
        p = transform.TransformPoint(p);
        return p;
    }

}
