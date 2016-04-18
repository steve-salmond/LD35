using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ProceduralChoice : ProceduralSpawner
{
    [System.Serializable]
    public struct Choice
    {
        public Procedural Prefab;
        public float Weight;
    }

    public List<Choice> Prefabs;

    public Vector2 ChoicesRange = new Vector2(1, 1);
    public AnimationCurve ChoicesDistribution;

    private float _totalWeight;

    protected override void Awake()
    {
        base.Awake();
        _totalWeight = Prefabs.Sum(p => p.Weight);
    }

    protected override void SpawnItems()
    {
        // Superclass implementation
        base.SpawnItems();

        int choices = (int) Random.Sample(ChoicesDistribution, ChoicesRange);
        for (var i = 0; i < choices; i++)
            SpawnChoice();
    }

    private void SpawnChoice()
    {
        // Perform weighted choice selection.
        var r = Random.Range(0, _totalWeight);

        var cumulative = 0.0f;
        foreach (var p in Prefabs)
        {
            var w = p.Weight;
            if (cumulative + w >= r)
            {
                SpawnItem(p.Prefab, Vector3.zero);
                return;
            }

            cumulative += w;
        }
    }

}
