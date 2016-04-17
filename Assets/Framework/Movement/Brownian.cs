using UnityEngine;
using System.Collections;

public class Brownian : MonoBehaviour
{

    /** Rigidbody that will be influenced. */
    public Rigidbody Body;

    /** Strength of the Brownian motion force. */
    public float Strength;

    public int Interval = 5;

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

        var p = transform.position;
        var f = Random.insideUnitCircle * _strength;
        var force = new Vector3(f.x, f.y, 0);

        Body.AddForceAtPosition(force, p);

        // Debug.DrawRay(p, force, Color.yellow, 0);
    }
}
