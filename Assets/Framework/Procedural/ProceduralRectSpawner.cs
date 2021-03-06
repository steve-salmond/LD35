using UnityEngine;
using System.Collections.Generic;

public class ProceduralRectSpawner : ProceduralSpawner
{
    public List<Procedural> Prefabs;
    public Rect Bounds;
    public Vector2 QuantityRange;
    public bool UseQuantityDistribution = true;
    public AnimationCurve QuantityDistribution;
    public int SpawnAttemptsPerItem = 5;

    public float QuantityForDepthScaleFactor = 1;

    protected override void SpawnItems()
    {
        // Superclass implementation
        base.SpawnItems();

        // Determine how many objects to generate.
        int quantity = UseQuantityDistribution
            ? (int) Random.Sample(QuantityDistribution, QuantityRange)
            : (int) Random.Range(QuantityRange);

        // Generate the objects.
        for (var i = 0; i < quantity; i++)
        {
            for (var attempt = 0; attempt < SpawnAttemptsPerItem; attempt++)
            {
                var x = Random.Range(Bounds.xMin, Bounds.xMax);
                var y = Random.Range(Bounds.yMin, Bounds.yMax);
                var p = new Vector3(x, y, 0);
                var prefab = Prefabs[Random.Range(0, Prefabs.Count)];
                var success = SpawnItem(prefab, p);
                if (success)
                    break;
            }
        }
    }

}
