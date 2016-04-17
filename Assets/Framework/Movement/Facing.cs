using UnityEngine;
using System.Collections;

public class Facing : MonoBehaviour {

	public Transform Target;

    public Vector3 OffsetRotation = Vector3.zero;

	public float SmoothTime = 0.5f;

	private Vector3 _direction; 
	private Vector3 _directionVelocity;

    public int Interval = 5;
    private int _frame;
    private float _smoothTime;
    private Quaternion _offset;

    protected Rigidbody Rigidbody;
	
	protected virtual void Start()
	{
        Rigidbody = GetComponentInParent<Rigidbody>();
        
        _frame = Random.Range(0, Interval);
        _smoothTime = SmoothTime / Interval;
		_direction = transform.forward;
        _offset = Quaternion.Euler(OffsetRotation);
    }
	
	void LateUpdate () 
	{
        // Wait until sufficient frames have passed.
        _frame++;
        if (_frame < Interval)
            return;
        _frame = 0;

		var direction = Vector3.zero;
		if (Rigidbody)
			direction = Rigidbody.velocity;
		if (Target != null)
			direction = (Target.position - transform.position);

		if (direction.sqrMagnitude < 0.1f)
			return;

		var target = direction.normalized;

        _direction = Vector3.SmoothDamp(_direction, target, ref _directionVelocity, _smoothTime);
        transform.rotation = Quaternion.LookRotation(_direction, Vector3.up) * _offset;
	}
}
