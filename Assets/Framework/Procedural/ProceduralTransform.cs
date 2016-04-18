using UnityEngine;
using System.Collections;

public class ProceduralTransform : Procedural
{
    public Vector2 ScaleRange = Vector2.one;

    public Vector2 RotationRange = Vector2.zero;
    public bool UseRotationDistribution = false;
    public AnimationCurve RotationDistribution;

    public float FlipXProbability = 0.5f;
    public float FlipYProbability = 0.0f;

    /** Generates procedural content. */
    public override void Generate(int seed)
    {
        base.Generate(seed);

        var scale = Vector3.one * Random.Range(ScaleRange);
        if (Random.NextFloat() < FlipXProbability)
            scale.x = -scale.x;
        if (Random.NextFloat() < FlipYProbability)
            scale.y = -scale.y;

        transform.localScale = scale;

        var rz = UseRotationDistribution 
            ? Random.Sample(RotationDistribution, RotationRange)
            : Random.Range(RotationRange);

        var r = transform.localRotation;
        transform.localRotation = Quaternion.Euler(r.x, r.y, rz);
    }

}
