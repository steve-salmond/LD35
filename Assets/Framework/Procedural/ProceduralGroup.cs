using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ProceduralGroup : Procedural
{
    // Properties
    // -----------------------------------------------------

    /** Child generators. */
    public List<Procedural> Generators
        { get; private set; }

	private void Awake()
    {
        // Ensure generator collection exists.
        if (Generators == null)
            Generators = new List<Procedural>();

        // Get generators from direct descendants only.
        if (transform.childCount > 0 && Generators.Count == 0)
            Generators = new List<Procedural>(GetComponentsInChildren<Procedural>()
                .Where(x => x.transform.parent == transform));
	}
	
	public override void Generate(int seed)
    {
        base.Generate(seed);
        foreach (var g in Generators)
            g.Generate(Random.NextInteger());
	}

    /** Cleans up generated content. */
    public override void Degenerate()
    {
        base.Degenerate();
        foreach (var g in Generators)
            g.Degenerate();
    }
}
