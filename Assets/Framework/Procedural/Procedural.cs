using UnityEngine;
using System.Collections;

public class Procedural : MonoBehaviour
{
    // Members
    // -----------------------------------------------------

    /** Random number source for this procedural generator. */
    protected Numbers.Random Random = new Numbers.Random();


    // Public Methods
    // -----------------------------------------------------

    /** Generates procedural content. */
    public virtual void Generate(int seed)
        { Random.SetSeed(seed); }

    /** Cleans up generated content. */
    public virtual void Degenerate()
        { }

}
