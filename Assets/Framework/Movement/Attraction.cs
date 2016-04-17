using UnityEngine;
using System.Collections;

public class Attraction : MonoBehaviour
{

    /** Rigidbody that will be influenced. */
    public Rigidbody Body;

    /** Strength of the attraction force (negative values indicate avoidance). */
    public float Strength;

    /** Axis scaling factors. */
    public Vector3 AxisScale = Vector3.one;

    /** Attraction radius. */
    public float Radius = 20;

    /** Minimum range. */
    public float MinRange = 0;

    /** Attraction layer mask. */
    public LayerMask LayerMask;

    /** Line of sight layer mask. */
    public LayerMask LineOfSightMask;

    /** Velocity prediction factor (seconds). */
    public float VelocityPredictionFactor = 0;

    public int Interval = 5;

    public int MaxResults = 1;

    public float Delay = 0;

    private int _frame;
    private float _strength;

    private float _nextAttractTime;

    private Collider[] _colliders;

    void Start()
    {
        _frame = Random.Range(0, Interval);
        _strength = Strength * Interval;
        _colliders = new Collider[MaxResults];

        if (!Body)
            Body = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (Time.time < _nextAttractTime)
            return;

        // Wait until sufficient frames have passed.
        _frame++;
        if (_frame < Interval)
            return;
        _frame = 0;

        var p = transform.position;
        var n = Physics.OverlapSphereNonAlloc(p, Radius, _colliders, LayerMask);
        var attracted = false;

        for (var i = 0; i < n; i++)
        {
            var c = _colliders[i].ClosestPointOnBounds(p);

            // Check line of sight if needed.
            if (LineOfSightMask != 0 && Physics.Linecast(c, p, LineOfSightMask))
                continue;

            // Apply velocity compensation.
            var body = _colliders[i].attachedRigidbody;
            if (body)
                c += body.velocity * VelocityPredictionFactor;

            var delta = c - p;
            delta.x *= AxisScale.x;
            delta.y *= AxisScale.y;
            delta.z *= AxisScale.z;

            if (delta.magnitude < MinRange)
                continue;

            Body.AddForce(delta.normalized * _strength);
            attracted = true;
        }

        if (!attracted)
            _nextAttractTime = Time.time + Delay;
    }
}
