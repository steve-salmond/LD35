using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StayGrounded : MonoBehaviour
{

    /** Rigidbody that will be influenced. */
    public Rigidbody Body;

    /** Strength of the avoidance force when shying away from non-grounded locations. */
    public float Strength;

    /** Update interval (in physics frames). */
    public int Interval = 5;

    /** Layer mask for detecting ground. */
    public LayerMask GroundMask;

    /** Ground detection radius. */
    public float GroundRadius = 1;

    /** Number of samples to take when detecting cliffs. */
    public int SampleCount = 6;

    /** Distance from body to check for cliffs. */
    public float SampleRadius = 4;


    private int _frame;
    private float _strength;

    void Start()
    {
        _frame = Random.Range(0, Interval);
        _strength = Strength * Interval;

        if (!Body)
            Body = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // Wait until sufficient frames have passed.
        _frame++;
        if (_frame < Interval)
            return;
        _frame = 0;

        if (!IsGroundedAt(transform.position))
            return;

        var f = Vector3.zero;
        var theta = 0.0f;
        var dTheta = 360.0f / SampleCount;

        for (var i = 0; i < SampleCount; i++)
        {
            f += ForceAtAngle(theta);
            theta += dTheta;
        }

        f = f * _strength;
        Body.AddForce(f);

        Debug.DrawRay(transform.position, f, Color.red, 0);
    }

    Vector3 ForceAtAngle(float angle)
    {
        var v = Quaternion.Euler(0, angle, 0) * Vector3.forward;
        var p = transform.position + v * SampleRadius;

        if (IsGroundedAt(p))
            return Vector3.zero;
        else
            return -v;
    }

    bool IsGroundedAt(Vector3 p)
    { return Physics.CheckSphere(p, GroundRadius, GroundMask); }

}