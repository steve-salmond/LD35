using UnityEngine;
using System.Collections.Generic;

public class ProceduralClumpSpawner : ProceduralSpawner
{
    public List<Procedural> Prefabs;
    public Rect Bounds;

    public Vector2 ClumpsRange;
    public AnimationCurve ClumpsDistribution;
        
    public Vector2 ClumpSizeRange;
    public AnimationCurve ClumpSizeDistribution;

    public Vector2 ClumpRadiusRange;
    public AnimationCurve ClumpRadiusDistribution;

    public int SpawnAttemptsPerItem = 5;

    protected override void SpawnItems()
    {
        // Superclass implementation.
        base.SpawnItems();

        // Determine how many clumps to generate.
        int clumps = (int) Random.Sample(ClumpsDistribution, ClumpsRange);

        // Generate the clumps.
        for (var i = 0; i < clumps; i++)
            GenerateClump();
    }

    private void GenerateClump()
    {
        // Determine clump's center.
        var x = Random.Range(Bounds.xMin, Bounds.xMax);
        var y = Random.Range(Bounds.yMin, Bounds.yMax);
        var center = new Vector3(x, y, 0);

        // Determine how many items to generate in clump.
        int n = (int) Random.Sample(ClumpSizeDistribution, ClumpSizeRange);

        // Generate clump items.
        for (var i = 0; i < n; i++)
        {
            for (var attempt = 0; attempt < SpawnAttemptsPerItem; attempt++)
            {
                var radius = Random.Sample(ClumpRadiusDistribution, ClumpRadiusRange);
                Vector3 delta = Random.InsideRadius(radius);
                var prefab = Prefabs[Random.Range(0, Prefabs.Count)];
                var success = SpawnItem(prefab, center + delta);
                if (success)
                    break;
            }
        }
    }

}
