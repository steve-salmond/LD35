using UnityEngine;
using System.Collections;

public class Procedural : MonoBehaviour
{

    // Properties
    // -----------------------------------------------------

    /** Whether object has been generated. */
    public bool Generated
    { get; private set; }

    /** Priority for this procedural. */
    public int Priority;


    // Members
    // -----------------------------------------------------

    /** Random number source for this procedural generator. */
    protected Numbers.Random Random = new Numbers.Random();


    // Public Methods
    // -----------------------------------------------------

    /** Generates procedural content. */
    public virtual void Generate(int seed)
    {
        Random.SetSeed(seed);
        Generated = true;
    }

    /** Cleans up generated content. */
    public virtual void Degenerate()
    {
        Generated = false;
    }

}
