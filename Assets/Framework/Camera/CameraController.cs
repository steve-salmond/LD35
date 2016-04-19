using System.Collections;
using UnityEngine;

public class CameraController : Singleton<CameraController>
{
    public Transform Eye;

    public Camera Camera
    { get; private set; }

    public AudioListener Listener
    { get; private set; }


    [Header("Movement")]

    public float SmoothTime = 0.2f;

    private bool _zooming;
    public bool Zooming
    {
        get { return _zooming; }
        set
        {
            _zooming = value;
            _zoomSizeExtra = value ? 10 : 0;
        }
    }

    private bool _using;
    public bool Using
    {
        get { return _using; }
        set
        {
            _using = value;
            _useSizeExtra = value ? 10 : 0;
        }
    }

    public Vector2 OrthoSizeRange = new Vector2(5, 25);
    public Vector2 SeparationRange = new Vector2(20, 60);

    public Vector2 Leading = new Vector2(0.2f, 0.2f);

    public Vector3 PositionOffset;

    public float VerticalZoomBias = 2;

    private Vector3 _positionVelocity = Vector3.zero;
    private float _orthoSizeVelocity;
    private float _nextUpdateTime;
    private float _zoomSizeExtra = 0;
    private float _useSizeExtra = 0;

    void Start()
    {
        Camera = GetComponentInChildren<Camera>();
        Listener = GetComponentInChildren<AudioListener>();

        _nextUpdateTime = Time.time + 0.5f;
    }

    void FixedUpdate()
    {
        if (Time.time < _nextUpdateTime)
            return;

        // Compute average player position and separation.
        var players = GameManager.Instance.Players.Players;
        if (players.Count <= 0)
            return;

        var average = Vector3.zero;
        var velocity = Vector3.zero;
        var min = Vector3.zero;
        var max = Vector3.zero;
        var alive = 0;
        for (var i = 0; i < players.Count; i++)
        {
            var player = players[i];
            if (!player.Alive)
                continue;

            alive++;
            var p = player.transform.position;
            average += p;

            if (player.HasControlled)
                velocity += player.Controlled.GetComponent<Rigidbody>().velocity;

            if (alive == 1)
            {
                min = p; max = p;
            }
            else
            {
                if (p.x < min.x)
                    min.x = p.x;
                else if (p.x > max.x)
                    max.x = p.x;

                if (p.y < min.y)
                    min.y = p.y;
                else if (p.y > max.y)
                    max.y = p.y;
            }
        }

        // Divide through by alive count to reach the average position.
        if (alive > 0)
        {
            average /= alive;
            velocity /= alive;
        }

        // Zoom out when players are widely separated.
        var delta = max - min;
        var aspect = (Screen.width / Screen.height);
        delta.y *= (aspect * VerticalZoomBias);
        var separation = delta.magnitude;
        var range = (SeparationRange.y - SeparationRange.x);
        var s = Mathf.Clamp01((separation - SeparationRange.x) / range);

        // Set target position.
        var leadingOffset = new Vector3(velocity.x * Leading.x, velocity.y * Leading.y);
        var target = average + PositionOffset + leadingOffset;
        transform.position = Vector3.SmoothDamp(transform.position, target, ref _positionVelocity, SmoothTime);

        // Update camera zoom factor.
        var sizeExtra = Mathf.Max(_useSizeExtra, _zoomSizeExtra);
        var targetOrthoSize = Mathf.Lerp(OrthoSizeRange.x, OrthoSizeRange.y, s) + sizeExtra;
        Camera.orthographicSize = Mathf.SmoothDamp(Camera.orthographicSize, targetOrthoSize, ref _orthoSizeVelocity, SmoothTime);
    }

    public void SnapTo(Vector3 p)
    {
        _positionVelocity = Vector3.zero;
        transform.position = p;
    }

}

