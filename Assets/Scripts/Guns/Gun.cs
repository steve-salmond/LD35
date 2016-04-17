using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour
{
    /** Rigidbody that will be influenced. */
    public Rigidbody Body;

    /** Emitter. */
    public Transform Emitter;

    /** Strength of the recoil force. */
    public float Recoil;

    public LayerMask TargetMask;
    public LayerMask LineOfSightMask;

    public GameObject ProjectilePrefab;

    public float Range = 20;

    public Vector2 Speed = new Vector2(20, 20);

    public Vector2 AimingDelay = new Vector2(0.5f, 0.5f);
    public Vector2 RepeatDelay = new Vector2(2, 2);

    public float VelocityPredictionFactor = 1;
    public AnimationCurve ElevationCompensationCurve;
    public AnimationCurve DistanceCompensationCurve;

    public int Interval = 5;
    public int MaxResults = 1;


    public GameObject ShootEffectPrefab;

    private int _frame;
    private float _nextShootTime;
    private Collider[] _colliders;

    void Start()
    {
        _frame = Random.Range(0, Interval);
        _colliders = new Collider[MaxResults];

        if (!Body)
            Body = GetComponent<Rigidbody>();

        if (!Emitter)
            Emitter = transform;
    }

    void FixedUpdate()
    {
        // Can we fire yet?
        if (Time.time < _nextShootTime)
            return;

        // Wait until sufficient frames have passed.
        _frame++;
        if (_frame < Interval)
            return;
        _frame = 0;

        // Look for closest target.
        var p = transform.position;
        var n = Physics.OverlapSphereNonAlloc(p, Range, _colliders, TargetMask);
        Collider closest = null;
        var target = Vector3.zero;
        var distance = float.MaxValue;
        for (var i = 0; i < n; i++)
        {
            var c = _colliders[i].ClosestPointOnBounds(p);

            // Check line of sight if needed.
            if (LineOfSightMask != 0 && Physics.Linecast(c, p, LineOfSightMask))
                continue;

            var d = Vector3.Distance(c, p);
            if (d >= distance)
                continue;

            closest = _colliders[i];
            target = c;
            distance = d;
        }

        // Did we find a target?
        if (!closest)
        {
            _nextShootTime = Time.time + Random.Range(AimingDelay.x, AimingDelay.y);
            return;
        }

        // Apply velocity compensation.
        Debug.DrawLine(p, target, Color.green, 1);
        var body = closest.attachedRigidbody;
        if (body)
            target += body.velocity * VelocityPredictionFactor;

        // Compensate for differences in elevation/distance.
        target.y += DistanceCompensationCurve.Evaluate(distance);
        target.y += ElevationCompensationCurve.Evaluate(target.y - p.y);

        Debug.DrawLine(p, target, Color.yellow, 1);

        // Spawn projectile.
        var delta = target - p;
        var projectile = ObjectPool.GetAt(ProjectilePrefab, Emitter, false);
        var projectileBody = projectile.GetComponent<Rigidbody>();
        if (projectileBody)
            projectileBody.velocity = delta.normalized * Random.Range(Speed.x, Speed.y);

        // Spawn shoot effect.
        ObjectPool.GetAt(ShootEffectPrefab, Emitter);

        // Add recoil.
        Body.AddForce(-delta.normalized * Recoil);

        // Schedule next shot.
        _nextShootTime = Time.time + Random.Range(RepeatDelay.x, RepeatDelay.y);
    }


}
