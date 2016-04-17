using UnityEngine;
using System.Collections;

public class ProceduralSpawner : ProceduralGroup
{
    /** Layer mask that determines if an object can spawn. */
    public LayerMask SpawnBlockerMask;

    /** Radius to check for blocking objects. */
    public float SpawnBlockerRadius = 1;

    /** Generates procedural content. */
    public override void Generate(int seed)
    {
        base.Generate(seed);

        // Spawn sub-generators.
        SpawnItems();

        // Ask each sub-object to perform procedural generation.
        foreach (var g in Generators)
            g.Generate(Random.NextInteger());
    }

    /** Cleans up generated content. */
    public override void Degenerate()
    {
        base.Degenerate();

        // De-spawn each spawned item.
        foreach (var g in Generators)
            ObjectPool.Cleanup(g.gameObject);

        // Reset the sub-generator list.
        Generators.Clear();
    }

    /** Spawn any sub-items. */
    protected virtual void SpawnItems()
    {

    }

    /** Spawns a single sub-item. */
    protected virtual bool SpawnItem(Procedural procedural, Vector3 p)
    {
        var obstructed = Physics.CheckSphere(p, SpawnBlockerRadius, SpawnBlockerMask);
        if (obstructed)
            return false;

        var go = ObjectPool.GetComponent(procedural);
        go.transform.parent = transform;
        go.transform.localPosition = p;
        go.transform.rotation = Quaternion.identity;
        Generators.Add(go);

        return true;
    }
}
