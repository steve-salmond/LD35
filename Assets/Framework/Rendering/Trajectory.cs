using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Trajectory : MonoBehaviour
{

    // Properties
    // -----------------------------------------------------

    public int SampleCount = 20;
    public float SampleInterval = 0.05f;
    public float MaxLength = 0;
    public float Mass = 1;
    public float Drag = 0.05f;
    public float Speed;
    public float MaxSpeed;
    public bool PlayOnAwake = true;
    public Transform HitIndicator;
    public LayerMask HitMask;
    public Color Color;
    public float ColorSmoothTime = 0;


    // Members
    // -----------------------------------------------------

    /** Objects to ignore collisions. */
    private List<GameObject> _ignored = new List<GameObject>();

    /** Current color. */
    private Color _current;

    /** Color 'velocity' for smoothing. */
    private Color _velocity;

    /** The renderer that displays the trajectory. */
    private LineRenderer _renderer;

    /** Trajectory position samples. */
    private Vector3[] _samples;


    // Unity Methods
    // -----------------------------------------------------

    /** Initialization. */
    private void Start()
    {
        _samples = new Vector3[SampleCount];
        _renderer = GetComponent<LineRenderer>();
        _renderer.SetVertexCount(SampleCount);

        SetActive(PlayOnAwake);
    }

    /** Late update. */
    void LateUpdate()
    {
        UpdateSamples();
	}


    // Public Methods
    // -----------------------------------------------------

    /** Add an object to the list of ignored objects. */
    public void AddIgnored(GameObject go)
    { _ignored.Add(go); }

    /** Remove an object from the list of ignored objects. */
    public void RemoveIgnored(GameObject go)
    { _ignored.Remove(go); }

    public void SetActive(bool value)
    {
        var wasActive = gameObject.activeSelf;
        gameObject.SetActive(value);

        // Reset samples when trajectory becomes active.
        if (value && !wasActive)
        {
            Speed = 0;
            UpdateSamples();
        }
    }


    // Private Methods
    // -----------------------------------------------------

    private void UpdateSamples()
    {
        var stopped = false;
        int n = _samples.Length;

        var p = transform.position;
        var v = transform.forward * Speed;

        var dt = SampleInterval;
        RaycastHit hit;

        var d = 0.0f;

        for (int i = 0; i < n; i++)
        {
            _samples[i] = p;

            if (stopped)
                continue;
            if (MaxLength > 0 && d > MaxLength)
                continue;

            var delta = v * dt;
            p += delta;
            d += delta.magnitude;

            var f = Physics.gravity;
            var a = f / Mass;

            v += a * dt;
            v *= (1 - Drag * dt);

            // Check if trajectory has hit something.
            if (Physics.Linecast(_samples[i], p, out hit, HitMask))
                if (!_ignored.Contains(hit.collider.gameObject))
                {
                    stopped = true;
                    p = hit.point;

                    if (HitIndicator)
                    {
                        HitIndicator.position = hit.point;
                        HitIndicator.rotation = Quaternion.LookRotation(hit.normal, Vector3.up);
                    }
                }

            // Debug.DrawLine(p, _samples[i], stopped ? Color.red : Color.yellow);
        }

        _renderer.SetPositions(_samples);

        _current.r = Mathf.SmoothDamp(_current.r, Color.r, ref _velocity.r, ColorSmoothTime);
        _current.g = Mathf.SmoothDamp(_current.g, Color.g, ref _velocity.g, ColorSmoothTime);
        _current.b = Mathf.SmoothDamp(_current.b, Color.b, ref _velocity.b, ColorSmoothTime);
        _current.a = Mathf.SmoothDamp(_current.a, Color.a, ref _velocity.a, ColorSmoothTime);

        var start = _current;
        var end = new Color(start.r, start.g, start.b, 0);
        _renderer.SetColors(start, end);

        if (HitIndicator)
            HitIndicator.gameObject.SetActive(stopped);
    }


}
