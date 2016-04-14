using UnityEngine;
using System.Collections.Generic;

public class ProceduralDistributedSpawner : ProceduralSpawner
{
    public List<Procedural> Prefabs;
    public Rect Bounds;
    public AnimationCurve XDistribution;
    public AnimationCurve YDistribution;

    public Vector2 QuantityRange;
    public bool UseQuantityDistribution = true;
    public AnimationCurve QuantityDistribution;

    protected override void SpawnItems()
    {
        // Superclass implementation.
        base.SpawnItems();

        // Determine how many objects to generate.
        int quantity = UseQuantityDistribution
            ? (int) Random.Sample(QuantityDistribution, QuantityRange)
            : (int) Random.Range(QuantityRange);

        // Generate the objects.
        for (var i = 0; i < quantity; i++)
        {
            var x = Random.Sample(XDistribution, Bounds.xMin, Bounds.xMax);
            var y = Random.Sample(YDistribution, Bounds.yMin, Bounds.yMax);
            var p = new Vector3(x, y, 0);
            var prefab = Prefabs[Random.Range(0, Prefabs.Count)];
            SpawnItem(prefab, p);
        }
    }

}
