using UnityEngine;
using System.Collections;

public class LookAtVelocity : MonoBehaviour
{

    public Transform Transform;

    public float SmoothTime = 0;

    private Vector3 _direction;
    private Vector3 _directionVelocity;

    public int Interval = 5;
    private int _frame;
    private float _smoothTime;

    protected Rigidbody Rigidbody;

    protected virtual void Start()
    {
        if (Transform == null)
            Transform = GetComponent<Transform>();

        if (Rigidbody == null)
            Rigidbody = GetComponentInParent<Rigidbody>();

        _frame = Random.Range(0, Interval);
        _smoothTime = SmoothTime / Interval;
        _direction = transform.forward;
    }

    void LateUpdate()
    {
        // Wait until sufficient frames have passed.
        _frame++;
        if (_frame < Interval)
            return;
        _frame = 0;

        var direction = Vector3.zero;
        if (Rigidbody != null)
            direction = Rigidbody.velocity;
        if (direction.sqrMagnitude < 0.5f)
            return;

        var target = direction.normalized;

        _direction = Vector3.SmoothDamp(_direction, target, ref _directionVelocity, _smoothTime);
        Transform.rotation = Quaternion.LookRotation(_direction, Vector3.up);
    }
}
