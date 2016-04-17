using UnityEngine;
using System.Collections;

public class LookAtVelocity : MonoBehaviour
{

    public Transform Transform;

    public float SmoothTime = 0;

    public float VelocityThreshold = 1;
    public bool StopAfterCollision;

    private Vector3 _direction;
    private Vector3 _directionVelocity;

    public int Interval = 5;

    private int _frame;
    private float _smoothTime;
    private bool _stopped;

    protected Rigidbody Rigidbody;

    protected virtual void OnEnable()
    {
        if (Transform == null)
            Transform = GetComponent<Transform>();

        if (Rigidbody == null)
            Rigidbody = GetComponentInParent<Rigidbody>();

        _frame = Random.Range(0, Interval);
        _smoothTime = SmoothTime / Interval;
        _direction = transform.forward;
        _stopped = false;
    }

    void OnCollisionEnter(Collision c)
    {
        if (StopAfterCollision)
            _stopped = true;
    }

    void LateUpdate()
    {
        // Check if we've been stopped.
        if (_stopped)
            return;

        // Wait until sufficient frames have passed.
        _frame++;
        if (_frame < Interval)
            return;
        _frame = 0;

        var direction = Vector3.zero;
        if (Rigidbody != null)
            direction = Rigidbody.velocity;
        if (direction.magnitude < VelocityThreshold)
            return;

        var target = direction.normalized;

        _direction = Vector3.SmoothDamp(_direction, target, ref _directionVelocity, _smoothTime);
        Transform.rotation = Quaternion.LookRotation(_direction, Vector3.up);
    }
}
